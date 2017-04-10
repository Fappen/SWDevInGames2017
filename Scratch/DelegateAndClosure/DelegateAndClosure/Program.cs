using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DelegateAndClosure
{

    public delegate void ProgressReporter(double percentDone);

    public class Calculator
    {
        public static void CalculateSomething(int param, ProgressReporter pr)
        {
            for (int i = 0; i < param; i++)
            {
                Thread.Sleep(1000);
                pr(100.0 * i / param);
                // Console.WriteLine("Calculating... " + 100.0 * i / param + " % done.");
            }
        }
    }



    //   ^
    //   |   Implementor of Calculator 
    //================================================================================================
    //   |   User of Calculator
    //   V


    public class ProgressReporterClass
    {
        public string AmountUnit; 
        public void MyProgressReporter(double percentDone)
        {
            Console.WriteLine(100 - percentDone + "% of " + AmountUnit + " to go...");
        }
    }



    class Program
    {
        static void Main(string[] args)
        {
            int amount = 10;
            string unit = "meters";

            // meters
            Calculator.CalculateSomething(10, delegate(double percent)
            {
                Console.WriteLine(100 - percent + "% of " + amount++ + " " + unit + " to go...");
            });

            Console.WriteLine("Amount "+ amount);

            //amount = 20;
            //unit = "kilograms";
            //// kilograms
            //Calculator.CalculateSomething(20, delegate (double percent)
            //{
            //    Console.WriteLine(100 - percent + "% of " + amount + " " + unit + " to go...");
            //});


            //// meters
            //var prMeter = new ProgressReporterClass { AmountUnit = "10 meters" };
            //Calculator.CalculateSomething(10, prMeter.MyProgressReporter);

            //// kilograms
            //var prKg = new ProgressReporterClass { AmountUnit = "20 kilograms" };
            //Calculator.CalculateSomething(20, prKg.MyProgressReporter);



            // Calculator.CalculateSomething(20, percentDone => Console.WriteLine(100 - percentDone + "% to go..."));

            Console.ReadKey();
        }
    }
}
