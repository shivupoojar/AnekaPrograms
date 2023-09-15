using System;
using System.Threading;
using System.Collections.Generic;
using Aneka.Entity;
using Aneka.Tasks;
using Aneka.Security;
using Aneka.Data.Entity;
using Aneka.Security.Windows;
using System.Net;


namespace SimpleTask
{
    [Serializable]
    public class Test : ITask
    {
        public string message;
        public Test() { }
        public void Execute()
        {
            
            
            message = "Hello Aneka Task Example";
  
            
        }
        public string Getmessage()
        {
            return message;
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
            
            app = new AnekaApplication<AnekaTask, TaskManager>("Test", conf);
            app.WorkUnitFinished += new EventHandler<WorkUnitEventArgs<AnekaTask>> (OnWorkUnitFinished);
            app.WorkUnitFailed += new EventHandler<WorkUnitEventArgs<AnekaTask>> (OnWorkUnitFailed);
            app.ApplicationFinished += new EventHandler<ApplicationEventArgs>(OnApplicationFinished);
            
            semaphore = new AutoResetEvent(false);
            Test test = new Test();
            AnekaTask anekaTask = new AnekaTask(test);
            app.ExecuteWorkUnit(anekaTask);
            semaphore.WaitOne();
 
            Aneka.Logger.Stop();
            app.StopExecution();
        }
        public static void OnWorkUnitFailed
            (object sender, WorkUnitEventArgs<AnekaTask> args)
        {

        }
        public static void OnWorkUnitFinished
            (object sender, WorkUnitEventArgs<AnekaTask> args)
        {
            Test task = args.WorkUnit.UserTask as Test;
            Console.WriteLine("-|Task output: {0} |Application Name: {1} |Node Id: {2} |Submission Time: {3} | Completion Time:{4} | Maximum Exec. Time:{5} |", task.Getmessage(), args.WorkUnit.Name, args.WorkUnit.NodeId, args.WorkUnit.SubmissionTime, args.WorkUnit.CompletionTime, args.WorkUnit.MaximumExecutionTime);
        }

        public static void
               OnApplicationFinished(object sender, ApplicationEventArgs args)
        {
            semaphore.Set();
            Console.WriteLine("Application Finished");

        }

    }
}
