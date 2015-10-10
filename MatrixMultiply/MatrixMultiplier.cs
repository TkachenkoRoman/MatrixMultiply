using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MatrixMultiply
{
    class MatrixMultiplier
    {
        public int Matr1RowsAmount { get; set; }
        public int Matr1ColsAmount { get; set; }
        public int Matr2RowsAmount { get; set; }
        public int Matr2ColsAmount { get; set; }

        private Random random;
        private int[,] matr1;
        private int[,] matr2;

        public MatrixMultiplier()
        {
            random = new Random();
        }

        public bool InitRandomMatrix()
        {
            if (Matr1RowsAmount > 0 && Matr2RowsAmount > 0 
                && Matr1ColsAmount > 0 && Matr2ColsAmount > 0)
            {
                matr1 = new int[Matr1RowsAmount, Matr1ColsAmount];
                matr2 = new int[Matr2RowsAmount, Matr2ColsAmount];

                for (int i = 0; i < Matr1RowsAmount; i++)
                {
                    for (int j = 0; j < Matr1ColsAmount; j++)
                    {
                        matr1[i, j] = random.Next(0, 100); /// TODO create min max random props in interface
                    }
                }

                for (int i = 0; i < Matr2RowsAmount; i++)
                {
                    for (int j = 0; j < Matr2ColsAmount; j++)
                    {
                        matr2[i, j] = random.Next(0, 100); 
                    }
                }
                return true;
            }
            return false;
        }

        public async Task<int[,]> MultiplyAsync()
        {
            int[,] res = await Task<int[,]>.Factory.StartNew(
                () => Multiply(), 
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.Default);
            return res;               
        }

        public int[,] Multiply()
        {
            if (!InitRandomMatrix())
            {
                throw new Exception("Specify valid matrix sizes");
            }
            if (matr1 != null && matr2 != null && matr1.GetLength(1) != matr2.GetLength(0))
            {
                throw new Exception("Matrix1 cols amount must be equal to matrix2 rows amount");
            }
            int[,] res = new int[matr1.GetLength(0), matr2.GetLength(1)];
            for (int i = 0; i < matr1.GetLength(0); i++)
            {
                for (int j = 0; j < matr2.GetLength(1); j++)
                {
                    for (int k = 0; k < matr2.GetLength(0); k++)
                    {
                        res[i, j] += matr1[i, k] * matr2[k, j];
                    }
                }
            }
            return res;
        }
    }
}
