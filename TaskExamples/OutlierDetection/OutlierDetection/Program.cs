using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OutlierDetection
{
    public class Features
    {
        [LoadColumn(0)]
        public float Xposition { get; set; }
        [LoadColumn(1)]
        public float Yposition { get; set; }
        [LoadColumn(2)]
        public float Zposition { get; set; }
        [LoadColumn(3)]
        public float FirstSensorActivity { get; set; }
        [LoadColumn(4)]
        public float SecondSensorActivity { get; set; }
        [LoadColumn(5)]
        public float ThirdSensorActivity { get; set; }
        [LoadColumn(6)]
        public float FourthSensorActivity { get; set; }
        [LoadColumn(7)]
        public float Anomaly { get; set; }
    }

    public class Result
    {
        public bool PredictedLabel { get; set; }
        public float Score { get; set; }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
