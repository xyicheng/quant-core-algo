using System;

namespace quant_core_algo
{
    public static partial class Vector
    {
        #region private method
        private static unsafe void FillValue(double* x, int size, double value)
        {
            for (int i = 0; i < size; i++)
                x[i] = value;
        }
        private static unsafe void FillValue(double* x, int size, double* value)
        {
            for (int i = 0; i < size; i++)
                x[i] = value[i];
        }
        private static unsafe void ApplyExp(double* x, int size)
        {
            for (int i = 0; i < size; i++)
                x[i] = Math.Exp(x[i]);
        }
        #endregion

        public static unsafe void FillValue(this double[] x, double val)
        {
            fixed (double* p_x = &x[0])
            {
                FillValue(p_x, x.Length, val);
            }
        }
        public static unsafe void FillValue(this double[,] x, double val)
        {
            fixed (double* p_x = &x[0, 0])
            {
                FillValue(p_x, x.Length, val);
            }
        }

        public static unsafe void FillValue(this double[] x, double[] val)
        {
            int n = x.Length;
            ArrayCheck.EqualSize(n, x, val);
            fixed (double* px = &x[0], pv = &val[0])
            {
                FillValue(px, n, pv);
            }
        }
        public static unsafe void FillValue(this double[,] x, double[,] val)
        {
            throw new NotImplementedException();
        }

        public static unsafe void ApplyExp(this double[] x)
        {
            fixed (double* p_x = &x[0])
            {
                ApplyExp(p_x, x.Length);
            }
        }
        public static unsafe void ApplyExp(this double[,] x)
        {
            fixed (double* p_x = &x[0, 0])
            {
                ApplyExp(p_x, x.Length);
            }
        }

    }
}