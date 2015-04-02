using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using quant_core_algo;

namespace quant_core_algoTest
{
    [TestClass]
    public class VectorOpTest
    {
        [TestMethod]
        public void TestAdd()
        {
            var chrono = new Stopwatch();
            chrono.Start();

            for (int i = 0; i < 10000; i++)
            {
                var x = new double[100, 100];
                Vector.Add(ref x, Vector.Constant(1.0, x.GetLength(0), x.GetLength(1)));
            }

            chrono.Stop();
            Console.WriteLine("Elapsed : {0}", chrono.Elapsed);
        }
    }
}
