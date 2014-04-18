using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppTest.MainLogic
{
    class Listener
    {
        private string _type;
        private List<Func<Record, bool>> _subscribers = new List<Func<Record, bool>>();
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

        public void Subscribe(Func<Record, bool> action)
        {
            lock (_locker)
            {
                _subscribers.Add(action);
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
                        
                        List<Record> processedRecords = new List<Record>();
                        foreach (Record r in _buffer)
                        {
                            while (_subscribers.Count > 0)
                            {
                                _currentSubscriber = _currentSubscriber % _subscribers.Count;

                                if (_subscribers[_currentSubscriber](r))
                                {
                                    processedRecords.Add(r);
                                    break;
                                }
                                else
                                {
                                    _subscribers.RemoveAt(_currentSubscriber);
                                }
                            }
                            if (_subscribers.Count == 0)
                            {
                                break;
                            }
                            _currentSubscriber = (_currentSubscriber + 1) % _subscribers.Count;
                        }
                        foreach (Record r in processedRecords)
                        {
                            _buffer.Remove(r);
                        }
                    }
                }
            }
        }
    }
}
