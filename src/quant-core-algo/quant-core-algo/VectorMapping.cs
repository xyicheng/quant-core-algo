using System;

namespace quant_core_algo
{
    public static partial class Vector
    {
        #region private method
        private static unsafe void SetValue(double* p_x, int size, double value)
        {
            for (int i = 0; i < size; i++)
                p_x[i] = value;
        }
        private static unsafe void ApplyExp(double* p_x, int size)
        {
            for (int i = 0; i < size; i++)
                p_x[i] = Math.Exp(p_x[i]);
        }
        #endregion

        public static unsafe void SetValue(ref double[] x, double val)
        {
            fixed (double* p_x = &x[0])
            {
                SetValue(p_x, x.Length, val);
            }
        }
        public static unsafe void SetValue(ref double[,] x, double val)
        {
            fixed (double* p_x = &x[0, 0])
            {
                SetValue(p_x, x.Length, val);
            }
        }

        public static unsafe void ApplyExp(ref double[] x)
        {
            fixed (double* p_x = &x[0])
            {
                ApplyExp(p_x, x.Length);
            }
        }
        public static unsafe void ApplyExp(ref double[,] x)
        {
            fixed (double* p_x = &x[0, 0])
            {
                ApplyExp(p_x, x.Length);
            }
        }
    }
}