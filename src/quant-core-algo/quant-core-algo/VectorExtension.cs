namespace quant_core_algo
{
    public static partial class Vector
    {
        public static double[] Constant(double value, int size)
        {
            var x = new double[size];
            SetValue(ref x, value);
            return x;
        }
        public static double[,] Constant(double value, int nbRows, int nbCols)
        {
            var x = new double[nbRows, nbCols];
            SetValue(ref x, value);
            return x;
        }
    }
}
