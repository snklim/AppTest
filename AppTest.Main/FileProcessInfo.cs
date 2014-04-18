using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppTest.MainLogic
{
    public class FileProcessInfo
    {
        public string FileTarget { get; set; }
        public Action OnFileProcessed { get; set; }

        public FileProcessInfo(string fileTarget, Action onFileProcessed)
        {
            FileTarget = fileTarget;
            OnFileProcessed = onFileProcessed;
        }
    }
}
