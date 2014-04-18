using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTest.MainLogic
{
    [Serializable]
    public class Record
    {
        public Guid Id { get; private set; }
        public string Type { get; set; }
        public string Value { get; set; }

        public Record(string type, string value)
        {
            Id = Guid.NewGuid();
            Type = type;
            Value = value;
        }
    }
}
