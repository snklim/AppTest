using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppTest.MainLogic
{
    class ClientWrapper
    {
        private ClientChannel _channel;
        private Thread _workerThread;
        private AutoResetEvent _waiter = new AutoResetEvent(false);
        private Listener _lisener;
        private Record _record = null;

        public ClientWrapper(ClientChannel channel, Listener listener)
        {
            _channel = channel;
            _workerThread = new Thread(Worker);
            _workerThread.IsBackground = true;
            _workerThread.Start();
            _lisener = listener;
        }

        public void Notify(Record record)
        {
            if (_record != null)
            {
                throw new Exception("Sorry, I'm busy");
            }
            _record = record;
            _waiter.Set();
        }

        private void Worker()
        {
            while (true)
            {
                _waiter.WaitOne();
                Console.WriteLine("Record sent at {3}, id: {0}, type: {1}, value: {2}", _record.Id, _record.Type, _record.Value, DateTime.Now);
                try
                {
                    _channel.Notify(_record);
                    _record = null;
                    _lisener.Subscribe(_channel);
                }
                catch (WebException ex)
                {
                    Console.WriteLine("{0}, id: {1}, type: {2}, value: {3}", ex.Message, _record.Id, _record.Type, _record.Value);
                    _lisener.Notify(_record);
                    break;
                }
            }
        }
    }
}
