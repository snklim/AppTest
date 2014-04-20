using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace AppTest.MainLogic
{
    public class FileProcessor : MarshalByRefObject
    {
        Dictionary<string, Listener> _listeners = new Dictionary<string, Listener>();
        object _locker = new object();
        FileWatcher _fileWacher;

        public FileProcessor()
        {
            _fileWacher = new FileWatcher(this);
        }

        public void Subscribe(string type, ClientChannel clientChannel)
        {
            Console.WriteLine("New subscription type: {0}", type);
            CreateListenerIfNeeded(type);
            _listeners[type].Subscribe(clientChannel);
        }

        internal void Process(FileProcessInfo fileProcessInfo)
        {
            new Thread(Worker).Start(fileProcessInfo);
        }

        private void Worker(object data)
        {
            FileProcessInfo fileProcessInfo = data as FileProcessInfo;

            using (XmlTextReader reader = new XmlTextReader(fileProcessInfo.FileTarget))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && string.Equals(reader.Name, "record", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string value = null;
                        string type = null;

                        while (reader.MoveToNextAttribute())
                        {
                            if (string.Equals(reader.Name, "value", StringComparison.CurrentCultureIgnoreCase))
                            {
                                value = reader.Value;
                            }
                            else if (string.Equals(reader.Name, "type", StringComparison.CurrentCultureIgnoreCase))
                            {
                                type = reader.Value;
                            }
                        }

                        if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(type))
                        {
                            NotifySubscribers(new Record(type, value));
                        }
                    }
                }
            }

            fileProcessInfo.OnFileProcessed();
        }

        private void CreateListenerIfNeeded(string type)
        {
            if (!_listeners.ContainsKey(type))
            {
                lock (_locker)
                {
                    if (!_listeners.ContainsKey(type))
                    {
                        Listener listener = new Listener(type);
                        //listener.Subscribe((v, t) => { Console.WriteLine("value: {0}, type: {1}", v, t); });
                        _listeners.Add(type, listener);
                    }
                }
            }
        }

        private void NotifySubscribers(Record record)
        {
            CreateListenerIfNeeded(record.Type);
            _listeners[record.Type].Notify(record);
            Console.WriteLine("Record {0} queued, type: {1}, value: {2}", record.Id, record.Type, record.Value);
        }
    }
}
