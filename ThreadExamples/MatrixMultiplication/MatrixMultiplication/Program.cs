using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;



using Aneka;
using Aneka.Entity;
using Aneka.Threading;
using Aneka.Security;
using System.Runtime.CompilerServices;

namespace MatrixMultiplication
{

    [Serializable]
    public class ScalarProduct : ISerializable
    {
        private double result;
        public double Result { get { return this.result; } }
        private double[] row, column;
        public ScalarProduct(double[] row, double[] column)
        {
            this.row = row;
            this.column = column;
        }
        public ScalarProduct(SerializationInfo info, StreamingContext context)
        {
            this.result = info.GetDouble("result");
            this.row = info.GetValue("row", typeof(double[])) as double[];
            this.column = info.GetValue("column", typeof(double[])) as double[];
        }

       
        public void Multiply()
        {
            this.result = 0;
            for (int i = 0; i < this.row.Length; i++)
            {
                this.result += this.row[i] * this.column[i];
            }
           // Console.WriteLine("Result is {0}", result);
        }

        
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("result", this.result);
            info.AddValue("row", this.row, typeof(double[]));
            info.AddValue("column", this.column, typeof(double[]));
        }
    }

    public class MatrixProduct
    {
        private static double[,] a;        
        private static double[,] b;
        private static double[,] c;
        private static IDictionary<AnekaThread, ScalarProduct> workers;
        private static AnekaApplication<AnekaThread, ThreadManager> app;



        static void Main(string[] args)
        {
            try
            {
                a = new double[3, 3] {
   {0, 1, 2} ,
   {3, 4, 5} ,
   {6, 7, 8} 
};
                b = new double[3, 3] {
   {9, 10, 11} ,
   {12, 13, 14} ,
   {15, 16, 17}
};

                Logger.Start();
                Configuration conf = new Configuration();
                conf.SchedulerUri = new Uri("tcp://20.107.246.61:9090/Aneka");
                conf.UserCredential = new UserCredentials("Administrator", string.Empty);
                conf.UseFileTransfer = false;
                 app = new AnekaApplication<AnekaThread, ThreadManager>(conf);
                Console.WriteLine("Inside Main try");
              //  MatrixProduct.ReadMatrices();
                MatrixProduct.ExecuteThreads();
                Console.WriteLine("I am out");
                MatrixProduct.ComposeResult();
            }

            catch (Exception ex)
            {
                IOUtil.DumpErrorReport(ex, "Matrix Multiplication –Error executing" + "the application");
            }
            finally
            {
                try
                {
                    if (MatrixProduct.app != null)
                    {
                        MatrixProduct.app.StopExecution();
                    }
                }
                catch (Exception ex)
                {
                    IOUtil.DumpErrorReport(ex, "Matrix Multiplication –Error stopping " + "the application");

                }
                Logger.Stop();

            }

        }
        private static void ExecuteThreads()
        {
            Console.WriteLine("Inside Execute Threads");
            MatrixProduct.workers = new Dictionary<AnekaThread, ScalarProduct>();
            int rows = MatrixProduct.a.Length;
            double[] row = new double[a.GetLength(0)];
            int columns = b.GetLength(0);
           
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < columns; j++)
                {
                    
                    for (int row1 = 0; row1 < a.GetLength(0); ++row1)
                    {
                        row[row1]= a[i, row1];
                        Console.WriteLine("Row value {0}",row.GetValue(row1));

                    }
                    // double[] row = (double[])a.GetValue(0);
                   // Console.WriteLine(row);
                    double[] column = new double[columns];
                    
                    for (int k = 0; k < columns; k++)
                    {
                     
                        column[j] = MatrixProduct.b[k,j];
                        Console.WriteLine("Column Value {0}",column[j]);

                    }
                  //  Console.WriteLine(column);
                    ScalarProduct scalar = new ScalarProduct(row, column);
                    try
                    {


                    AnekaThread worker = new AnekaThread(scalar.Multiply, app);
                    
                    
                    worker.Name = string.Format("{0}.{1}", row.GetValue(i), column.GetValue(j));
                        Console.WriteLine(worker.Name);
                    worker.Start();
                    MatrixProduct.workers.Add(worker, scalar);
                        Console.WriteLine(MatrixProduct.workers.Count);
                    }
                    catch (Exception ex)
                    {
                        IOUtil.DumpErrorReport(ex, "Matrix Multiplication –Error executing" + "the application");
                        Console.WriteLine(ex);
                    }
                }
            Console.WriteLine("I am out");
        }
        private static void ComposeResult()
        {
            MatrixProduct.c = new double[a.GetLength(0), a.GetLength(1)];
            foreach (KeyValuePair<AnekaThread, ScalarProduct> pair in MatrixProduct.workers)
            {
                AnekaThread worker = pair.Key;
                string workername = worker.Name;
                string[] indices = workername.Split(new char[] { '.' });
                int i = int.Parse(indices[0]);
                int j = int.Parse(indices[1]);
                worker.Join();
                MatrixProduct.c[i, j] = ((ScalarProduct)worker.Target).Result;


            }
            MatrixProduct.PrintMatrices(MatrixProduct.c);

        }
        private static void ReadMatrices()
        {
            // code for reading the matrices a and b
        }


        private static void PrintMatrices(double[,] matrix)
        {
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    Console.WriteLine("a[{0},{1}] = {2}", i, j, matrix[i, j]);
                }
            }
        }

        
            
            


    

    }
}
