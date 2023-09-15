using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Aneka;
using Aneka.Threading;
using Aneka.Entity;
using System.Xml.Linq;

namespace SimpleThread
{
    class Program
    {
        [Serializable]
        public class HelloWorld
        {
            public string result;
            public int i;

            public void HelloPrint()
            {
                result = "Hello Aneka...";
            }

        }

        static void Main(string[] args)
        {

            AnekaApplication<AnekaThread, ThreadManager> app = null;
            try
            {
                Logger.Start();
                // Configuration file
                Configuration conf = Configuration.GetConfiguration(@"G:\My Drive\ANEKA\AnekaPrograms\conf.xml");
                //Attch the conf file to Anek aapplication
                app = new AnekaApplication<AnekaThread, ThreadManager>("Simple-Thread-Application",conf);
                // Create a object to class
                HelloWorld hw = new HelloWorld();
                //Create a Aneka Thread
                AnekaThread anekaTh = new AnekaThread(hw.HelloPrint, app);
                //Start Aneka Thread
                anekaTh.Start();
                //Get the output from the thread execution
                anekaTh.Join();
                hw = (HelloWorld)anekaTh.Target;
                //Print the output in console
                Console.WriteLine("-|Thread output: {0} |Application Name: {1} |Node Id: {2} |Submission Time: {3} | Completion Time:{4} | Maximum Exec. Time:{5} |", hw.result,anekaTh.ApplicationId,anekaTh.NodeId,anekaTh.SubmissionTime,anekaTh.CompletionTime,anekaTh.MaximumExecutionTime);
               // Console.WriteLine("Output of Hello Print" + hw.result + "val: " + hw.i + " " + anekaTh.NodeId + "submisstion time:" + anekaTh.SubmissionTime + "Completion Time" + anekaTh.CompletionTime);
               // System.Diagnostics.Debug.Write("Output of Hello Print" + hw.result + "val: " + hw.i + " " + anekaTh.NodeId + "submisstion time:" + anekaTh.SubmissionTime + "Completion Time" + anekaTh.CompletionTime);
            }
            finally
            {

                Logger.Stop();
                app.StopExecution();

            }
        }
    }
}
