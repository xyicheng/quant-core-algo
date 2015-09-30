namespace quant_core_algo
{
    public static class TridiagonalMatrix
    {
        #region private fields
        private static unsafe void SolveTridiagonal(double* y, int size, double* lower, double* diag, double* upper)
        {
            if (size == 1)
            {
                y[0] /= diag[0];
                return;
            }

            double[] buffer = new double[size];
            fixed (double* pBuffer = &buffer[0])
            {
                pBuffer[0] = upper[0] / diag[0];
                y[0] = y[0] / diag[0];

                for (int i = 1; i < size; i++)
                {
                    double m = 1.0 / (diag[i] - lower[i] * pBuffer[i - 1]);
                    pBuffer[i] = upper[i] * m;
                    y[i] = (y[i] - lower[i] * y[i - 1]) * m;
                }

                for (int i = size - 1; i-- > 0;)
                    y[i] = y[i] - pBuffer[i] * y[i + 1];
            }
        }
        private static unsafe void MultTridiagonal(double* x, int size, double* lower, double* diag, double* upper)
        {
            if (size == 1)
            {
                x[0] *= diag[0];
                return;
            }
            
            double prev = x[0];
            x[0] = diag[0] * x[0] + upper[0] * x[1];

            for (int i = 1; i < size - 1; ++i)
            {
                double temp = x[i];
                x[i] = lower[i] * prev + diag[i] * x[i] + upper[i] * x[i + 1];
                prev = temp;
            }
            x[size - 1] = lower[size - 1] * prev + diag[size - 1] * x[size - 1];
        }
        #endregion

        /// <summary>
        /// Solve tridiagonal system (in place) M * x = y
        /// M = ( diag_0  upper_0   0         0    )
        ///     ( lower_1 diag_1  upper_1     0    )
        ///     (   0     lower_2 diag_2   upper_2 )
        ///     (   0       0     lower_3  diag_3  )  
        /// </summary>
        /// <param name="y"></param>
        /// <param name="lower"></param>
        /// <param name="diag"></param>
        /// <param name="upper"></param>
        public static unsafe void SolveTridiagonal(this double[] y, double[] lower, double[] diag, double[] upper)
        {
            int n = y.Length;
            if (n == 0) return;
            ArrayCheck.EqualSize(n, y, lower, diag, upper);
            fixed (double* py = &y[0], plower = &lower[0], pdiag = &diag[0], pupper = &upper[0])
            {
                SolveTridiagonal(py, n, plower, pdiag, pupper);
            }
        }
        
        /// <summary>
        /// Tridiagonal matrix multiplication (in place) M * x
        /// M = ( diag_0  upper_0   0         0    )
        ///     ( lower_1 diag_1  upper_1     0    )
        ///     (   0     lower_2 diag_2   upper_2 )
        ///     (   0       0     lower_3  diag_3  )  
        /// </summary>
        /// <param name="x"></param>
        /// <param name="lower"></param>
        /// <param name="diag"></param>
        /// <param name="upper"></param>
        public static unsafe void MultTridiagonal(this double[] x, double[] lower, double[] diag, double[] upper)
        {
            int n = x.Length;
            if (n == 0) return;
            ArrayCheck.EqualSize(n, x, lower, diag, upper);
            fixed (double* px = &x[0], plower = &lower[0], pdiag = &diag[0], pupper = &upper[0])
            {
                MultTridiagonal(px, n, plower, pdiag, pupper);
            }
        }
    }
}