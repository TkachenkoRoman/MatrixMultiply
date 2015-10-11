using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MatrixMultiply
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MatrixMultiplier matrixMultiplier;
        private int Matr1RowsAmount;
        private int Matr1ColsAmount;
        private int Matr2RowsAmount;
        private int Matr2ColsAmount;
        private int[,] matr1;
        private int[,] matr2;

        public MainWindow()
        {
            InitMatrixMultiplier();
            InitializeComponent();
            InitDefaultValues();
        }

        private void InitDefaultValues()
        {
            Matr1RowsAmount = 500;
            Matr1ColsAmount = 500;
            Matr2RowsAmount = 500;
            Matr2ColsAmount = 500;
            TextBoxMatr1Rows.Text = Matr1RowsAmount.ToString();
            TextBoxMatr1Cols.Text = Matr1ColsAmount.ToString();
            TextBoxMatr2Rows.Text = Matr2RowsAmount.ToString();
            TextBoxMatr2Cols.Text = Matr2ColsAmount.ToString();
        }

        private void InitMatrixMultiplier()
        {
            matrixMultiplier = new MatrixMultiplier();
        }

        private async void ButtonCalculate_Click(object sender, RoutedEventArgs e)
        {

            LabelStatus.Content = "calculating...";
            var watch = Stopwatch.StartNew();

            try
            {
                matr1 = matrixMultiplier.InitRandomMatrix(Matr1RowsAmount, Matr1ColsAmount);
                matr2 = matrixMultiplier.InitRandomMatrix(Matr2RowsAmount, Matr2ColsAmount);
                int[,] res = await matrixMultiplier.MultiplyAsync(matr1, matr2);
                //Debug.WriteLine("Matrix default method: ");
                //PrintMatrix(res);
            }
            catch (AggregateException ae)
            {
                MessageBox.Show("Wrong matrix sizes");
                LabelStatus.Content = "free";
                return;
            }
                
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            LabelDefaultTime.Content = elapsedMs.ToString();

            try
            {
                watch = Stopwatch.StartNew();

                var matr1SplitList = MatrixMultiplier.Split2DArrayIntoRows(matr1, 3);
                var matr2SplitList = MatrixMultiplier.Split2DArrayIntoCols(matr2, 3);


                IEnumerable<Task<MatrixWithId>> multiplyMatrixTasksQuery = from m1 in matr1SplitList
                                                                     //join m2 in matr2SplitList on matr1SplitList.IndexOf(m1) equals matr2SplitList.IndexOf(m2)
                                                                           from m2 in matr2SplitList
                                                                     select matrixMultiplier.MultiplyAsync(m1, m2);
                Task<MatrixWithId>[] multiplyMatrixTasks = multiplyMatrixTasksQuery.ToArray();
                MatrixWithId[] resultChunks = await Task.WhenAll(multiplyMatrixTasks);
                resultChunks.OrderBy(x => x.id1).ThenBy(x => x.id2);
                int[,] res = new int[Matr1RowsAmount, Matr1ColsAmount];
                int currRowIndex = 0;
                int currColIndex = 0;
                int rowOffset = 0;
                int colOffset = 0;
                int currMatrId = resultChunks[0].id1;

                for (int i = 0; i < resultChunks.Length; i++)
                {
                    if (currMatrId == resultChunks[i].id1)
                    {
                        currColIndex += colOffset;
                    }
                    else
                    {
                        currColIndex = 0;
                        currRowIndex += rowOffset;
                    }

                    currMatrId = resultChunks[i].id1;

                    rowOffset = 0;
                    for (int j = 0; j < resultChunks[i].matr.GetLength(0); j++)
                    {
                        colOffset = 0;
                        for (int k = 0; k < resultChunks[i].matr.GetLength(1); k++)
                        {
                            res[j + currRowIndex, k + currColIndex] = resultChunks[i].matr[j, k];
                            colOffset++;
                        }
                        rowOffset++;
                    }
                }

                //Debug.WriteLine("Matrix async method: ");
                //PrintMatrix(res);

                watch.Stop();
                elapsedMs = watch.ElapsedMilliseconds;
                LabelAsyncTime.Content = elapsedMs.ToString();

            }
            catch (AggregateException ae)
            {
                Debug.WriteLine("*** ERROR *** " + ae.Message);
                return;
            }
            
             
            LabelStatus.Content = "free";
        }

        private void PrintMatrix(int[,] matr)
        {
            //
            // Print res matrix in Output
            //
            Debug.WriteLine("");
            for (int i = 0; i < matr.GetLength(0); i++)
            {
                for (int j = 0; j < matr.GetLength(1); j++)
                {
                    Debug.Write(matr[i, j].ToString() + " ");
                }
                Debug.WriteLine("");
            }
        }

        #region TextBoxes TextChanging handling
        private void TextBoxMatr1Rows_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amount = 0;
            try
            {
                amount = Int32.Parse(TextBoxMatr1Rows.Text);
            }
            catch
            {
                MessageBox.Show("This parameter must be positive integer");
                return;
            }
            Matr1RowsAmount = amount;
        }

        private void TextBoxMatr1Cols_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amount = 0;
            try
            {
                amount = Int32.Parse(TextBoxMatr1Cols.Text);
            }
            catch
            {
                MessageBox.Show("This parameter must be positive integer");
                return;
            }
            Matr1ColsAmount = amount;
        }

        private void TextBoxMatr2Rows_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amount = 0;
            try
            {
                amount = Int32.Parse(TextBoxMatr2Rows.Text);
            }
            catch
            {
                // skipping this message cause TextBoxMatr2Rows.Text is bind to TextBoxMatr1Cols.Text
                //MessageBox.Show("This parameter must be integer"); 
                return;
            }
            Matr2RowsAmount = amount;
        }

        private void TextBoxMatr2Cols_TextChanged(object sender, TextChangedEventArgs e)
        {
            int amount = 0;
            try
            {
                amount = Int32.Parse(TextBoxMatr2Cols.Text);
            }
            catch
            {
                MessageBox.Show("This parameter must be positive integer");
                return;
            }
            Matr2ColsAmount = amount;
        }
        #endregion
    }
}
