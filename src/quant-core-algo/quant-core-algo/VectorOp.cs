using System;

namespace quant_core_algo
{
    public static partial class Vector
    {
        #region private methods
        private static unsafe void Add(double* p_x, double* p_y, int size)
        {
            for (int i = 0; i < size; i++)
                p_x[i] += p_y[i];
        }
        private static unsafe void Sub(double* p_x, double* p_y, int size)
        {
            for (int i = 0; i < size; i++)
                p_x[i] -= p_y[i];
        }
        private static unsafe void Mult(double* p_x, double a, int size)
        {
            for (int i = 0; i < size; i++)
                p_x[i] *= a;
        }
        private static unsafe void AxPlusBy(double* p_x, double a, double* p_y, double b, int size)
        {
            for (int i = 0; i < size; i++)
            {
                p_x[i] = a * p_x[i] + b * p_y[i];
            }
        }
        #endregion

        public static unsafe void Add(ref double[] x, double[] y)
        {
            int size = Math.Min(x.Length, y.Length);
            fixed (double* p_x = &x[0], p_y = &y[0])
            {
                Add(p_x, p_y, size);
            }
        }
        public static unsafe void Add(ref double[,] x, double[,] y)
        {
            if (x.GetLength(0) != y.GetLength(0) ||
                x.GetLength(1) != y.GetLength(1))
                throw new Exception("Incompatible size");

            fixed (double* p_x = &x[0, 0], p_y = &y[0, 0])
            {
                Add(p_x, p_y, x.Length);
            }
        }

        public static unsafe void Sub(ref double[] x, double[] y)
        {
            int size = Math.Min(x.Length, y.Length);
            fixed (double* p_x = &x[0], p_y = &y[0])
            {
                Sub(p_x, p_y, size);
            }
        }
        public static unsafe void Sub(ref double[,] x, double[,] y)
        {
            if (x.GetLength(0) != y.GetLength(0) ||
                x.GetLength(1) != y.GetLength(1))
                throw new Exception("Incompatible size");

            fixed (double* p_x = &x[0, 0], p_y = &y[0, 0])
            {
                Sub(p_x, p_y, x.Length);
            }
        }

        public static unsafe void Mult(ref double[] x, double a)
        {
            fixed (double* p_x = &x[0])
            {
                Mult(p_x, a, x.Length);
            }
        }
        public static unsafe void Mult(ref double[,] x, double a)
        {
            fixed (double* p_x = &x[0, 0])
            {
                Mult(p_x, a, x.Length);
            }
        }

        public static unsafe void AxPlusBy(ref double[] x, double a, double[] y, double b)
        {
            if (x.Length != y.Length)
                throw new Exception("Incompatible size");

            fixed (double* p_x = &x[0], p_y = &y[0])
            {
                AxPlusBy(p_x, a, p_y, b, x.Length);
            }
        }
        public static unsafe void AxPlusBy(ref double[,] x, double a, double[,] y, double b)
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
