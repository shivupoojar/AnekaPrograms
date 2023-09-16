using System;
using System.Collections.Generic;
using System.Text;
using Aneka;
using Aneka.Threading;
using Aneka.Entity;
using System.Threading;

namespace AnekaThreadPractise1
{
    [Serializable]
    public class HelloWorld
    {
        public string result;
        public HelloWorld(){}

        public void PrintHello()
        {
            result = "HelloWorld";
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            AnekaApplication<AnekaThread, ThreadManager> app = null;
            try
            {
                Logger.Start();
                Configuration conf = Configuration.GetConfiguration("C:/Aneka/conf.xml");
            
                app = new AnekaApplication<AnekaThread,ThreadManager>(conf);
                HelloWorld hw = new HelloWorld();
                AnekaThread th = new AnekaThread(hw.PrintHello, app);
                th.Start();
                th.Join();
                hw = (HelloWorld)th.Target;
                Console.WriteLine("Value: "+hw.result);
            }
            finally
            {
                app.StopExecution();
                Logger.Stop();
            }
        }
    }
}