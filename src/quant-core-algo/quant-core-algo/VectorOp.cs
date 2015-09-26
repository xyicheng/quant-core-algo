using System;

namespace quant_core_algo
{
    public static partial class Vector
    {
        #region private methods
        private static unsafe void Add(double* x, double* y, int size)
        {
            for (int i = 0; i < size; i++)
                x[i] += y[i];
        }
        private static unsafe void Sub(double* x, double* y, int size)
        {
            for (int i = 0; i < size; i++)
                x[i] -= y[i];
        }
        private static unsafe void Mult(double* x, double a, int size)
        {
            for (int i = 0; i < size; i++)
                x[i] *= a;
        }
        private static unsafe void AxPlusBy(double* x, double a, double* y, double b, int size)
        {
            for (int i = 0; i < size; i++)
            {
                x[i] = a * x[i] + b * y[i];
            }
        }
        #endregion

        public static unsafe void Add(this double[] x, double[] y)
        {
            int size = Math.Min(x.Length, y.Length);
            fixed (double* p_x = &x[0], p_y = &y[0])
            {
                Add(p_x, p_y, size);
            }
        }
        public static unsafe void Add(this double[,] x, double[,] y)
        {
            if (x.GetLength(0) != y.GetLength(0) ||
                x.GetLength(1) != y.GetLength(1))
                throw new Exception("Incompatible size");

            fixed (double* p_x = &x[0, 0], p_y = &y[0, 0])
            {
                Add(p_x, p_y, x.Length);
            }
        }

        public static unsafe void Sub(this double[] x, double[] y)
        {
            int size = Math.Min(x.Length, y.Length);
            fixed (double* p_x = &x[0], p_y = &y[0])
            {
                Sub(p_x, p_y, size);
            }
        }
        public static unsafe void Sub(this double[,] x, double[,] y)
        {
            if (x.GetLength(0) != y.GetLength(0) ||
                x.GetLength(1) != y.GetLength(1))
                throw new Exception("Incompatible size");

            fixed (double* p_x = &x[0, 0], p_y = &y[0, 0])
            {
                Sub(p_x, p_y, x.Length);
            }
        }

        public static unsafe void Mult(this double[] x, double a)
        {
            fixed (double* p_x = &x[0])
            {
                Mult(p_x, a, x.Length);
            }
        }
        public static unsafe void Mult(this double[,] x, double a)
        {
            fixed (double* p_x = &x[0, 0])
            {
                Mult(p_x, a, x.Length);
            }
        }

        public static unsafe void AxPlusBy(this double[] x, double a, double[] y, double b)
        {
            int n = x.Length;
            ArrayCheck.CheckSize(n, x, y);
            fixed (double* p_x = &x[0], p_y = &y[0])
            {
                AxPlusBy(p_x, a, p_y, b, n);
            }
        }
        public static unsafe void AxPlusBy(this double[,] x, double a, double[,] y, double b)
        {
            if (x.GetLength(0) != y.GetLength(0) ||
                x.GetLength(1) != y.GetLength(1)) 
                throw new Exception("Incompatible size");

            fixed (double* p_x = &x[0, 0], p_y = &y[0, 0])
            {
                AxPlusBy(p_x, a, p_y, b, x.Length);
            }
        }
    }
}
