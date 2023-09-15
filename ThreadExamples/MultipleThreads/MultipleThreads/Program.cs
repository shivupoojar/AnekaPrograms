using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Aneka;
using Aneka.Threading;
using Aneka.Entity;
namespace MultipleThreads
{
    class Program
    {
        [Serializable]
        public class HelloWorld
        {
            public String result;
            public void printHello()
            {
                result = "Hello!! Multiple Threads";
            }

        }


        static void Main(string[] args)
        {

            AnekaApplication<AnekaThread, ThreadManager> app = null;
            try
            {
                Logger.Start();

                Configuration conf = Configuration.GetConfiguration(@"G:\My Drive\ANEKA\AnekaPrograms\conf.xml");
                app = new AnekaApplication<AnekaThread, ThreadManager>(conf);
                HelloWorld hw = new HelloWorld();

                // Run for five threads:
                AnekaThread[] Five = new AnekaThread[50];
                for (int i = 0; i < 50; i++)
                {
                    Five[i] = new AnekaThread(hw.printHello, app);
                    Five[i].Start();

                }
                for (int i = 0; i < 50; i++)
                {
                    Five[i].Join();
                    hw = (HelloWorld)Five[i].Target;
                    Console.WriteLine("-|Thread output: {0} |Application Name: {1} |Node Id: {2} |Submission Time: {3} | Completion Time:{4} | Maximum Exec. Time:{5} |", hw.result, Five[i].Name, Five[i].NodeId, Five[i].SubmissionTime, Five[i].CompletionTime, Five[i].MaximumExecutionTime);

                }


            }
            finally
            {

                Logger.Stop();
                app.StopExecution();


            }
        }
    }
}
