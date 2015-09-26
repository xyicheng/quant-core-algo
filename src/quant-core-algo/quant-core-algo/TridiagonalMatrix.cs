namespace quant_core_algo
{
    public static class TridiagonalMatrix
    {
        #region private fields
        private static unsafe void SolveTridiagonal(double* x, int size, double* a, double* b, double* c)
        {
            //TODO faire le cas n=1 !!!
            if (size == 1)
            {
                x[0] /= b[0];
                return;
            }

            double[] cprime = new double[size];

            cprime[0] = c[0] / b[0];
            x[0] = x[0] / b[0];

            // loop from 1 to N - 1 
            for (int i = 1; i < size; i++)
            {
                double m = 1.0 / (b[i] - a[i] * cprime[i - 1]);
                cprime[i] = c[i] * m;
                x[i] = (x[i] - a[i] * x[i - 1]) * m;
            }

            // loop from N - 2 to 0 inclusive, safely testing loop end condition 
            for (int i = size - 1; i-- > 0;)
                x[i] = x[i] - cprime[i] * x[i + 1];
        }
        private static unsafe void MultTridiagonal(double* x, int size, double* a, double* b, double* c)
        {
            if (size == 1)
            {
                x[0] *= b[0];
                return;
            }

            double xPrevious = x[0];
            x[0] = b[0] * x[0] + c[0] * x[1];

            for (int i = 1; i < size - 1; ++i)
            {
                double temp = x[i];
                x[i] = a[i] * xPrevious + b[i] * x[i] + c[i] * x[i + 1];
                xPrevious = temp;
            }
            x[size - 1] = a[size - 1] * xPrevious + b[size - 1] * x[size - 1];
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