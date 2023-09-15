using System;
using System.Collections.Generic;
using System.Text;
using Aneka;
using Aneka.Threading;
using Aneka.Entity;

namespace AnekaThreadTrigonometric
{
    [Serializable]
    public class Sine
    {
        double x;
        public double result;
        public Sine(double x)
        {
            this.x = x;
        }
        public void sineCompute()
        {
            result = System.Math.Sin(x * System.Math.PI / 180);
        }
    }
    [Serializable]
    public class Cosine
    {
        double x;
        public double result;
        public Cosine(double x)
        {
            this.x = x;
        }
        public void cosineCompute()
        {
            result = System.Math.Cos(x * System.Math.PI / 180);
        }
    }
    [Serializable]
    public class Tangent
    {
        double x;
        public double result;
        public Tangent(double x)
        {
            this.x = x;
        }
        public void tangentCompute()
        {
            result = System.Math.Tan(x * System.Math.PI / 180);
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
                Configuration conf = Configuration.GetConfiguration(@"G:\My Drive\ANEKA\AnekaPrograms\conf.xml");
                app = new AnekaApplication<AnekaThread, ThreadManager>(conf);

                Sine sine = new Sine(10);
                AnekaThread thsine = new AnekaThread(sine.sineCompute, app);
                thsine.Start();
                thsine.Join();
                sine = (Sine)thsine.Target;
                Console.WriteLine("Sine Thread Node ID:" + thsine.NodeId);

                Cosine cosine = new Cosine(20);
                AnekaThread thcosine = new AnekaThread(cosine.cosineCompute, app);
                thcosine.Start();
                thcosine.Join();
                cosine = (Cosine)thcosine.Target;
                Console.WriteLine("CoSine Thread Node ID:" + thcosine.NodeId);

                Tangent tanget = new Tangent(20);
                AnekaThread thtangent = new AnekaThread(tanget.tangentCompute, app);
                thtangent.Start();
                thtangent.Join();
                tanget = (Tangent)thtangent.Target;
                Console.WriteLine("Tagent Thread Node ID:" + thtangent.NodeId);

                Console.WriteLine("P = sin(10)+cos(20)+tan(20) = {0}", sine.result + cosine.result + tanget.result);


            }
            finally
            {
                app.StopExecution();
                Logger.Stop();
            }


        }
    }
}
