using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using quant_core_algo;

namespace quant_core_algoTest
{
    [TestClass]
    public class TridiagonalMatrixTest
    {
        [TestMethod]
        public void ZeroDimMult()
        {
            var x = new double[0];
            x.MultTridiagonal(new double[0], new double[0], new double[0]);
            Assert.IsTrue(x.Length == 0);
        }

        [TestMethod]
        public void OneDimMult()
        {
            var x = new[] {3.14159265359};
            x.MultTridiagonal(new[] {double.NaN}, new[] {18.5284515}, new[] {double.NaN});
            Assert.IsTrue(x.Length == 1);
            Assert.AreEqual(x[0], 3.14159265359 * 18.5284515);
        }

        [TestMethod]
        public void TwoDimMult()
        {
            var x = new[] {3.14159265359, 2.71828182846};
            x.MultTridiagonal(new[] { double.NaN, 1.0 }, new[] { 18.5284515, 1.5186165 }, new[] { 1.0, double.NaN });
            Assert.IsTrue(x.Length == 2);
            Assert.AreEqual(x[0], 3.14159265359 * 18.5284515 + 2.71828182846 * 1.0);
            Assert.AreEqual(x[1], 3.14159265359 * 1.0 + 2.71828182846 * 1.5186165);
        }

        [TestMethod]
        public void ThreeDimMult()
        {
            var x = new[] { 3.14159265359, 2.71828182846, 1.0 };
            x.MultTridiagonal(new[] {double.NaN, 1.0, 0.22}, new[] {18.5284515, 1.5186165, -5.164}, new[] {1.0, 3.648, double.NaN});
            Assert.IsTrue(x.Length == 3);
            Assert.AreEqual(x[0], 3.14159265359 * 18.5284515 + 2.71828182846 * 1.0);
            Assert.AreEqual(x[1], 3.14159265359 * 1.0 + 2.71828182846 * 1.5186165 + 1.0 * 3.648);
            Assert.AreEqual(x[2], 2.71828182846 * 0.22 + 1.0 * (-5.164));
        }

        [TestMethod]
        public void OneDimInverse()
        {
            var x = new[] { 3.14159265359 };
            x.SolveTridiagonal(new[] { double.NaN }, new[] { 18.5284515 }, new[] { double.NaN });
            Assert.IsTrue(x.Length == 1);
            Assert.AreEqual(x[0], 3.14159265359 / 18.5284515);
        }

        [TestMethod]
        public void InverseVsMultCheck()
        {
            var rand = new Random(4653);
            for (int size = 1; size < 1000; size += 50)
            {
                var x = new double[size];
                var l = new double[size];
                var d = new double[size];
                var u = new double[size];

                for (int i = 0; i < size; i++)
                {
                    l[i] = -1.0 - 0.25 + 0.5 * rand.NextDouble();
                    d[i] = 2.0 - 0.25 + 0.5 * rand.NextDouble();
                    u[i] = -1.0 - 0.25 + 0.5 * rand.NextDouble();
                    x[i] = rand.NextDouble();
                }

                var xRef = new double[size];
                xRef.FillValue(x);

                //Multiplication first and inverse
                x.MultTridiagonal(l, d, u);
                x.SolveTridiagonal(l, d, u);

                Assert.AreEqual(xRef.Length, x.Length);
                for (int i = 0; i < x.Length; i++)
                {
                    double err = Math.Abs(x[i] - xRef[i]);
                    Assert.IsTrue(err <= 1.0e-10 * Math.Abs(xRef[i]));
                }

                //Inverse first first and multiplication
                x.FillValue(xRef);
                x.SolveTridiagonal(l, d, u);
                x.MultTridiagonal(l, d, u);

                Assert.AreEqual(xRef.Length, x.Length);
                for (int i = 0; i < x.Length; i++)
                {
                    double err = Math.Abs(x[i] - xRef[i]);
                    Assert.IsTrue(err <= 1.0e-10 * Math.Abs(xRef[i]));
                }
            }
        }

    } 
}
