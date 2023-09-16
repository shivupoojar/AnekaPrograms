using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Aneka;
using Aneka.Threading;
using Aneka.Entity;
using System.Xml.Linq;
using ConsoleApp2;
using static System.Net.Mime.MediaTypeNames;

namespace SentimentAnalyzer
{
    class Program
    {
        
        [Serializable]
        public class HelloWorld
        {
            public string result;
            public string message;
            public int i;
            public MLModel1.ModelInput sampledata;

            public void HelloPrint()
            {
                result = "Hello Aneka...";
                Console.WriteLine("Hi");
                // Load model and predict output of sample data
                var result2 = MLModel1.Predict(sampledata);

                // If Prediction is 1, sentiment is "Positive"; otherwise, sentiment is "Negative"
                message = result2.PredictedLabel == 1 ? "Positive" : "Negative";
                Console.WriteLine($"Text: {sampledata.Col0}\nSentiment: {message}");
                //  message = "Hello Aneka Task Example";

            }

        }

        static void Main(string[] args)
        {

            AnekaApplication<AnekaThread, ThreadManager> app = null;
            try
            {
                var sampleData = new MLModel1.ModelInput()
                {
                    Col0 = "This restaurant was wonderful."
                };

                Logger.Start();
                // Configuration file
                Configuration conf = Configuration.GetConfiguration(@"G:\My Drive\ANEKA\AnekaPrograms\conf.xml");
                //Attch the conf file to Anek aapplication
                app = new AnekaApplication<AnekaThread, ThreadManager>("Simple-Thread-Application", conf);
                // Create a object to class
                HelloWorld hw = new HelloWorld();
                hw.sampledata = sampleData;
                //Create a Aneka Thread
                AnekaThread anekaTh = new AnekaThread(hw.HelloPrint, app);
                //Start Aneka Thread
                anekaTh.Start();
                //Get the output from the thread execution
                anekaTh.Join();
                hw = (HelloWorld)anekaTh.Target;
                //Print the output in console
                Console.WriteLine("-|Thread output: {0} |Application Name: {1} |Node Id: {2} |Submission Time: {3} | Completion Time:{4} | Maximum Exec. Time:{5} |", hw.result, anekaTh.ApplicationId, anekaTh.NodeId, anekaTh.SubmissionTime, anekaTh.CompletionTime, anekaTh.MaximumExecutionTime);
            }
            finally
            {

                Logger.Stop();
                app.StopExecution();

            }
        }
    }
}
