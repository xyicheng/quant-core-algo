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
        private readonly double infBoundaryShift;
        private readonly double supBoundaryShift;
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
        public IDiscreteOperator<double[]> Adjoint()
        {
            if (infBoundaryShift!=0.0 || supBoundaryShift!=0.0)
                throw new Exception("Transpose is not defined with non zero boundary shift");

            return new TridiagOperator1D(upper, diag, lower);
        }
    }

    public interface IDiscreteOperator<TData> 
    {
        void Apply(ref TData x);
        void Solve(ref TData y);
        IDiscreteOperator<TData> Adjoint();
    }





}
