using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppTest.MainLogic
{
    class Listener
    {
        private string _type;
        private List<ClientWrapper> _subscribers = new List<ClientWrapper>();
        private List<Record> _buffer = new List<Record>();
        private int _currentSubscriber = 0;
        private Thread _workerThread = null;
        private object _locker = new object();
        private AutoResetEvent _waiter = new AutoResetEvent(false);

        public Listener(string type)
        {
            _type = type;
            _workerThread = new Thread(Worker);
            _workerThread.IsBackground = true;
            _workerThread.Start();
        }

        public void Notify(Record record)
        {
            lock (_locker)
            {
                _buffer.Add(record);
                _waiter.Set();
            }
        }

        public void Subscribe(ClientChannel channel)
        {
            lock (_locker)
            {
                _subscribers.Add(new ClientWrapper(channel, this));
                _waiter.Set();
            }
        }

        private void Worker()
        {
            _currentSubscriber = 0;
            while (true)
            {
                _waiter.WaitOne();
                if (_buffer.Count > 0 && _subscribers.Count > 0)
                {
                    lock (_locker)
                    {
                        List<Record> recordsToRemove = new List<Record>();
                        foreach (Record r in _buffer)
                        {
                            if (_subscribers.Count == 0)
                            {
                                break;
                            }

                            _currentSubscriber = _currentSubscriber % _subscribers.Count;

                            var clientWrapper = _subscribers[_currentSubscriber];
                            clientWrapper.Notify(r);

                            _subscribers.RemoveAt(_currentSubscriber);

                            recordsToRemove.Add(r);

                            _currentSubscriber++;
                        }
                        foreach (Record r in recordsToRemove)
                        {
                            _buffer.Remove(r);
                        }
                    }
                }
            }
        }
    }
}
