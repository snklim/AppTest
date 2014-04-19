using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AppTest.MainLogic
{
    public class ClientChannel : MarshalByRefObject
    {
        public bool IsSlowMode { get; set; }

        public void Notify(Record record)
        {
            if (IsSlowMode)
            {
                Thread.Sleep(10000);
            }
            Console.WriteLine("Record received at {3}, id: {0}, type: {1}, value: {2}", record.Id, record.Type, record.Value, DateTime.Now);
        }
    }
}
