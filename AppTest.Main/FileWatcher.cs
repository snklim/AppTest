using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace AppTest.MainLogic
{
    public class FileWatcher
    {
        Thread _workerThread;
        FileProcessor _fileProcessor;

        public FileWatcher(FileProcessor fileProcessor)
        {
            _fileProcessor = fileProcessor;

            _workerThread = new Thread(Worker);
            _workerThread.IsBackground = true;
            _workerThread.Start();
        }

        private void Worker()
        {
            while (true)
            {
                foreach (FileInfo file in new DirectoryInfo(@".\Income").GetFiles("*.xml"))
                {
                    string fileOriginalName = file.Name;
                    string filePathProcessing = @".\Income\Processing\" + Guid.NewGuid().ToString() + ".xml";
                    string filePathProcessed = @".\Income\Processed\" + fileOriginalName;

                    Console.WriteLine("Start processing {0} file", fileOriginalName);

                    file.MoveTo(filePathProcessing);

                    FileProcessInfo fileProcessInfo = new FileProcessInfo(filePathProcessing, () =>
                    {
                        if (File.Exists(filePathProcessed))
                        {
                            File.Delete(filePathProcessed);
                        }

                        FileInfo fileProcessing = new FileInfo(filePathProcessing);

                        fileProcessing.MoveTo(filePathProcessed);

                        if (File.Exists(filePathProcessing))
                        {
                            File.Delete(filePathProcessing);
                        }

                        Console.WriteLine("File {0} processed", fileOriginalName);
                    });

                    _fileProcessor.Process(fileProcessInfo);
                }
                Thread.Sleep(1000);
            }
        }
    }
}
