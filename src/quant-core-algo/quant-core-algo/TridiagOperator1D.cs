using System;

namespace quant_core_algo
{
    public class TridiagOperator1D : IDiscreteOperator<double[]>
    {
        #region private fields
        private readonly int size;
        private readonly double[] lower;
        private readonly double[] diag;
        private readonly double[] upper;
        private double infBoundaryShift;
        private double supBoundaryShift;
        #endregion
        public TridiagOperator1D(double[] lower, double[] diag, double[] upper, 
            double infBoundaryShift = 0.0 , double supBoundaryShift = 0.0)
        {
            size = lower.Length;
            ArrayCheck.EqualSize(size, lower, diag, upper);
            this.lower = lower;
            this.diag = diag;
            this.upper = upper;
            this.infBoundaryShift = infBoundaryShift;
            this.supBoundaryShift = supBoundaryShift;
        }
        
        public void Apply(ref double[] x)
        {
            x.MultTridiagonal(lower, diag, upper);
            x[0] += infBoundaryShift;
            x[size - 1] += supBoundaryShift;
        }
        public void Solve(ref double[] y)
        {
            y[0] -= infBoundaryShift;
            y[size - 1] -= supBoundaryShift;
            y.SolveTridiagonal(lower, diag, upper);
        }

        public void Adjoint()
        {
            if (infBoundaryShift != 0.0 || supBoundaryShift != 0.0)
                throw new Exception("Transpose is not defined with non zero boundary shift");

            var buffer = new double[lower.Length];
            buffer.FillValue(lower);
            lower.FillValue(upper);
            upper.FillValue(buffer);
        }
        public void ScalePlusIdentity(double a)
        {
            lower.Mult(a);
            upper.Mult(a);
            diag.AxPlusB(a, 1.0);
            infBoundaryShift *= a;
            supBoundaryShift *= a;
        }
        public IDiscreteOperator<double[]> Copy()
        {
            return new TridiagOperator1D(lower.Copy(), diag.Copy(), upper.Copy(), infBoundaryShift, supBoundaryShift);
        }
        
        /// <summary>
        /// Apply to operator an affine boundary constraint
        ///     V[-1] = a  * V[0] + b * V[1] + c 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public void SetInfBoundaryCondition(double a, double b, double c)
        {
            // Tridiag(V)[0] = (diag[0] + lower[0] * a) * V[0] + (upper[0] + lower[0] * b) * V[1] + lower[0] * c
            var l_0 = lower[0];
            lower[0] = 0.0;
            diag[0] += l_0 * a;
            upper[0] += l_0 * b;
            infBoundaryShift += l_0 * c;
        }
        
        /// <summary>
        /// Apply to operator an affine boundary constraint
        ///     V[N + 1] = a  * V[N] + b * V[N - 1] + c 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        public void SetSupBoundaryCondition(double a, double b, double c)
        {
            // Tridiag(V)[N] = (diag[N] + upper[N] * a) * V[N] + (lower[N] + upper[N] * b) * V[N - 1] + upper[N] * c
            var n = upper.Length - 1;
            var u_n = upper[n];
            upper[n] = 0.0;
            diag[n] += u_n * a;
            lower[n] += u_n * b;
            supBoundaryShift += u_n * c;
        }
    }

    public abstract class BoundaryCondition1D
    {
        public abstract void SetInf(TridiagOperator1D tridiaOp, double dx);
        public abstract void SetSup(TridiagOperator1D tridiaOp, double dx);

        /// <summary>
        /// Boundary condition : d^2V/dx^2 = 0
        /// </summary>
        /// <returns></returns>
        public static BoundaryCondition1D NoConvexity()
        {
            return new NoConvexityBoundary();
        }

        /// <summary>
        /// Boundary condition : d^2V/dx^2 = dV/dx
        /// </summary>
        /// <returns></returns>
        public static BoundaryCondition1D Exponential()
        {
            return new ExponentialBoundary();
        }

        #region private class
        private class NoConvexityBoundary : BoundaryCondition1D
        {
            public override void SetInf(TridiagOperator1D tridiaOp, double dx)
            {
                // V[-1] =  2 * V[0] - V[1] 
                tridiaOp.SetInfBoundaryCondition(2.0, -1.0, 0.0);
            }
            public override void SetSup(TridiagOperator1D tridiaOp, double dx)
            {
                // V[N+1] = 2 * V[N] - V[N-1]
                tridiaOp.SetSupBoundaryCondition(2.0, -1.0, 0.0);
            }
        }

        private class ExponentialBoundary : BoundaryCondition1D
        {
            public override void SetInf(TridiagOperator1D tridiaOp, double dx)
            {
                tridiaOp.SetInfBoundaryCondition((2.0 + dx) / (1.0 + dx), -1.0 / (1.0 + dx), 0.0);
            }
            public override void SetSup(TridiagOperator1D tridiaOp, double dx)
            {
                tridiaOp.SetSupBoundaryCondition((dx - 2.0) / (dx - 1.0), 1.0 / (dx - 1.0), 0.0);
            }
        }
        #endregion

    }


}
