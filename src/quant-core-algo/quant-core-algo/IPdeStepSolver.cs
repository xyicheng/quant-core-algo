using System;
using System.Diagnostics.Contracts;

namespace quant_core_algo
{
    public interface IPdeStepSolver<TData>
    {
        void Backward(ref TData slice, double start, double end);
        void Forward(ref TData slice, double start, double end);
    }

    /// <summary>
    /// Scheme for solving pde : dV/dt + L(V) = 0, with L a linear operator.
    /// 
    /// Pde is approximated on a discrete time step by :
    /// (V(end) - V(start))/dt + theta * L(V)(start) + (1-theta) * L(V)(end) = 0
    /// 
    /// theta=0 => explicit scheme
    /// theta=1 => implicit scheme
    /// theta=0.5 => crank-nicholson scheme
    ///  
    /// </summary>
    public class ThetaScheme1D : IPdeStepSolver<double[]>
    {
        #region private fields
        private readonly IOperatorDiscretizer<double[]> discretizer;
        #endregion
        #region private methods
        private void BuildParts(double start, double end,
                                out IDiscreteOperator<double[]> implicitPart,
                                out IDiscreteOperator<double[]> explicitPart)
        {
            double dt = end - start;
            implicitPart = discretizer.Discretize(start, end);
            explicitPart = implicitPart.Copy();
            implicitPart.ScalePlusIdentity(Theta * dt);
            explicitPart.ScalePlusIdentity(-(1.0 - Theta) * dt);
        }
        #endregion
        public ThetaScheme1D(IOperatorDiscretizer<double[]> discretizer, double theta)
        {
            this.discretizer = discretizer;
            Theta = theta;
        }
        public void Backward(ref double[] slice, double start, double end)
        {
            IDiscreteOperator<double[]> implicitPart, explicitPart;
            BuildParts(start, end, out implicitPart, out explicitPart);

            explicitPart.Apply(ref slice);
            slice.Add(discretizer.SourceTerm(start, end));
            implicitPart.Solve(ref slice);
        }
        public void Forward(ref double[] slice, double start, double end)
        {
            IDiscreteOperator<double[]> implicitPart, explicitPart;
            BuildParts(start, end, out implicitPart, out explicitPart);

            explicitPart.Adjoint();
            implicitPart.Adjoint();

            implicitPart.Solve(ref slice);
            explicitPart.Apply(ref slice);
        }

        public double Theta { get; private set; }
    }
    
    public interface IDiscreteOperator<TData>
    {
        void Apply(ref TData x);
        void Solve(ref TData y);

        /// <summary>
        /// transpose operator
        /// </summary>
        /// <returns></returns>
        void Adjoint();

        /// <summary>
        /// M = a * M + Id
        /// </summary>
        /// <param name="a"></param>
        void ScalePlusIdentity(double a);

        IDiscreteOperator<TData> Copy();
    }

    public interface IOperatorDiscretizer<TData>
    {
        IDiscreteOperator<TData> Discretize(double start, double end);
        TData SourceTerm(double start, double end);
    }

    public class FiniteDiffDiscretizer1D : IOperatorDiscretizer<double[]>
    {
        #region private fields
        private readonly PdeCoeffSampler1D pdeCoeffs;
        private readonly BoundaryCondition1D infBoundaryCond;
        private readonly BoundaryCondition1D supBoundaryCond;
        #endregion
        private void FiniteDifference(double[] d2X, double[] dX, double[] I,
                                      ref double[] lower, ref double[] diag, ref double[] upper)
        {
            double step = pdeCoeffs.Grid.Step;
            for (int i = 0; i < lower.Length; i++)
            {
                double drift = dX[i] * step;
                double diffusion = 2.0 * d2X[i];

                if (drift < -diffusion)
                {
                    diag[i] = (drift - diffusion) / (step * step) + I[i];
                    upper[i] = 0.5 * diffusion / (step * step);
                    lower[i] = (-drift + 0.5 * diffusion) / (step * step);
                }
                else if (drift > diffusion)
                {
                    diag[i] = (-drift - diffusion) / (step * step) + I[i];
                    upper[i] = (drift + 0.5 * diffusion) / (step * step);
                    lower[i] = 0.5 * diffusion / (step * step);
                }
                else
                {
                    diag[i] = -diffusion / (step * step) + I[i];
                    upper[i] = 0.5 * (drift + diffusion) / (step * step);
                    lower[i] = 0.5 * (-drift + diffusion) / (step * step);
                }
            }
        }

        public FiniteDiffDiscretizer1D(PdeCoeffSampler1D pdeCoeffs)
        {
            this.pdeCoeffs = pdeCoeffs;
        }
        public IDiscreteOperator<double[]> Discretize(double start, double end)
        {
            var gridSize = pdeCoeffs.Grid.Size;
            double[] d2X = new double[gridSize];
            double[] dX = new double[gridSize];
            double[] I = new double[gridSize];
            pdeCoeffs.FillCoeff(start, end, ref d2X, ref dX, ref I);

            double[] lower = new double[gridSize];
            double[] diag = new double[gridSize];
            double[] upper = new double[gridSize];
            FiniteDifference(d2X, dX, I, ref lower, ref diag, ref upper);

            //TODO BoundaryCondition

            return new TridiagOperator1D(lower, diag, upper);
        }
        public double[] SourceTerm(double start, double end)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class PdeCoeffSampler1D
    {
        protected PdeCoeffSampler1D(RegularGrid1D grid)
        {
            Grid = grid;
        }

        public abstract void FillCoeff(double start, double end,
                                       ref double[] d2X, ref double[] dX, ref double[] I);

        public RegularGrid1D Grid { get; private set; }
    }

    public class RegularGrid1D
    {
        public RegularGrid1D(int size, double boundaryInf, double boundarySup)
        {
            Contract.Requires(boundarySup >= boundaryInf);
            Contract.Requires(size > 1);

            Size = size;
            BoundaryInf = boundaryInf;
            BoundarySup = boundarySup;
            Step = (boundarySup - boundaryInf) / (size - 1.0);
        }
        public int Size { get; private set; }
        public double BoundaryInf { get; private set; }
        public double BoundarySup { get; private set; }
        public double Step { get; private set; }
    }
}