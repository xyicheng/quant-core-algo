using System;

namespace quant_core_algo
{
    public static class ArrayCheck
    {
        public static void EqualSize(int size, params double[][] arrays)
        {
            if (arrays.Length == 0)
                throw new Exception("Incompatible array size !");

            for (int i = 0; i < arrays.Length; i++)
            {
                if (arrays[i].Length != size)
                    throw new Exception("Incompatible array size !");
            }
        }
        public static void EqualSize(int nbRows, int nbCols, params double[][,] arrays)
        {
            throw new NotImplementedException();
        }
    }
}