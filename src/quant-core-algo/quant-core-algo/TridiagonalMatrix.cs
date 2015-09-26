namespace quant_core_algo
{
    public static class TridiagonalMatrix
    {
        #region private fields
        private static unsafe void SolveTridiagonal(double* x, int size, double* a, double* b, double* c)
        {
            if (size == 1)
            {
                x[0] /= b[0];
                return;
            }

            double[] buffer = new double[size];
            fixed (double* pBuffer = &buffer[0])
            {
                pBuffer[0] = c[0] / b[0];
                x[0] = x[0] / b[0];

                for (int i = 1; i < size; i++)
                {
                    double m = 1.0 / (b[i] - a[i] * pBuffer[i - 1]);
                    pBuffer[i] = c[i] * m;
                    x[i] = (x[i] - a[i] * x[i - 1]) * m;
                }

                for (int i = size - 1; i-- > 0;)
                    x[i] = x[i] - pBuffer[i] * x[i + 1];
            }
        }
        private static unsafe void MultTridiagonal(double* x, int size, double* a, double* b, double* c)
        {
            if (size == 1)
            {
                x[0] *= b[0];
                return;
            }
            
            double prev = x[0];
            x[0] = b[0] * x[0] + c[0] * x[1];

            for (int i = 1; i < size - 1; ++i)
            {
                double temp = x[i];
                x[i] = a[i] * prev + b[i] * x[i] + c[i] * x[i + 1];
                prev = temp;
            }
            x[size - 1] = a[size - 1] * prev + b[size - 1] * x[size - 1];
        }
        #endregion

        /// <summary>
        /// Solve tridiagonal system (in place) X = M^(-1) * X
        /// M = ( b_0 c_0  0    0  )
        ///     ( a_1 b_1 c_1   0  )
        ///     (  0  a_2 b_2  c_2 )
        ///     (  0   0  a_3  b_3 )  
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public static unsafe void SolveTridiagonal(this double[] x, double[] a, double[] b, double[] c)
        {
            int n = x.Length;
            if (n == 0) return;
            ArrayCheck.CheckSize(n, x, a, b, c);
            fixed (double* px = &x[0], pa = &a[0], pb = &b[0], pc = &c[0])
            {
                SolveTridiagonal(px, n, pa, pb, pc);
            }
        }
        
        /// <summary>
        /// Tridiagonal matrix multiplication (in place) X = M * X
        /// M = ( b_0 c_0  0    0  )
        ///     ( a_1 b_1 c_1   0  )
        ///     (  0  a_2 b_2  c_2 )
        ///     (  0   0  a_3  b_3 )  
        /// </summary>
        /// <param name="x"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public static unsafe void MultTridiagonal(this double[] x, double[] a, double[] b, double[] c)
        {
            int n = x.Length;
            if (n == 0) return;
            ArrayCheck.CheckSize(n, x, a, b, c);
            fixed (double* px = &x[0], pa = &a[0], pb = &b[0], pc = &c[0])
            {
                MultTridiagonal(px, n, pa, pb, pc);
            }
        }
    }
}