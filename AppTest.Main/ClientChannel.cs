using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTest.MainLogic
{
    public class ClientChannel : MarshalByRefObject
    {
        public void Notify(Record record)
        {
            Console.WriteLine("Record received, id: {0}, type: {1}, value: {2}", record.Id, record.Type, record.Value);
        }
    }
}
