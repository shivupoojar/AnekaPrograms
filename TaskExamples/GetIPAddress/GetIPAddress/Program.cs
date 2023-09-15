using System;
using System.Threading;
using System.Collections.Generic;
using Aneka.Entity;
using Aneka.Tasks;
using Aneka.Security;
using Aneka.Data.Entity;
using Aneka.Security.Windows;
using System.Net;


namespace GetIPAddress
{
    [Serializable]
    public class Test : ITask
    {
        public string executor;
        int a = 20, b = 30;
        public string result;
        public Test() { }
        public void Execute()
        {

            string hostname = Dns.GetHostName();
            IPHostEntry host = Dns.GetHostEntry(hostname);
            executor = hostname;
            IPAddress[] ip1 = Dns.GetHostAddresses(hostname);
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    
                    result = ip.ToString();
                }
            }
        }

        public string GetExecutorNode()
        {
            return executor;
        }
        public string GetIPNode()
        {
            return result;
        }
    }

    class MyTaskDemo
    {

        private static AutoResetEvent semaphore;

        private static AnekaApplication<AnekaTask, TaskManager> app;


        public static void Main(string[] args)
        {

            Aneka.Logger.Start();
            Configuration conf = Configuration.GetConfiguration(@"G:\My Drive\ANEKA\AnekaPrograms\conf.xml");
            conf.SingleSubmission = false;
            conf.ResubmitMode = ResubmitMode.MANUAL;
            conf.UseFileTransfer = false;
            app = new AnekaApplication<AnekaTask, TaskManager>
                        ("Test", conf);
            app.WorkUnitFinished +=
            new EventHandler<WorkUnitEventArgs<AnekaTask>>
                (OnWorkUnitFinished);
            app.WorkUnitFailed +=
                new EventHandler<WorkUnitEventArgs<AnekaTask>>
                    (OnWorkUnitFailed);
            app.ApplicationFinished +=
                new EventHandler<ApplicationEventArgs>(OnApplicationFinished);
            semaphore = new AutoResetEvent(false);
            Test t = new Test();

            AnekaTask gt = new AnekaTask(t);
            app.ExecuteWorkUnit(gt);

            semaphore.WaitOne();
       
            Aneka.Logger.Stop();
        }
        public static void OnWorkUnitFailed
            (object sender, WorkUnitEventArgs<AnekaTask> args)
        {
            //your own logic
        }

        public static void OnWorkUnitFinished
            (object sender, WorkUnitEventArgs<AnekaTask> args)
        {
            Test task = args.WorkUnit.UserTask as Test;
            Console.WriteLine("-|Task output: {0} |Application Name: {1} |Node Id: {2} |Submission Time: {3} | Completion Time:{4} | Maximum Exec. Time:{5} |", "Hostname: "+task.GetExecutorNode()+ " "+"IP:"+task.GetIPNode(), args.WorkUnit.ApplicationId, args.WorkUnit.NodeId, args.WorkUnit.SubmissionTime, args.WorkUnit.CompletionTime, args.WorkUnit.MaximumExecutionTime);
        
        app.StopExecution();

        }
        public static void
               OnApplicationFinished(object sender, ApplicationEventArgs args)
        {
            semaphore.Set();
            Console.WriteLine("Application Finished");

        }

    }
}
