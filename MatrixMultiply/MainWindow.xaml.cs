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

        public MainWindow()
        {
            InitMatrixMultiplier();
            InitializeComponent();        
        }

        private void InitMatrixMultiplier()
        {
            matrixMultiplier = new MatrixMultiplier();
        }

        private async void ButtonCalculate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var watch = Stopwatch.StartNew();

                int[,] res = await matrixMultiplier.MultiplyAsync();

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                LabelDefaultTime.Content = elapsedMs.ToString();
                //
                // Print res matrix in Output
                //
                //for (int i = 0; i < res.GetLength(0); i++)
                //{
                //    for (int j = 0; j < res.GetLength(1); j++)
                //    {
                //        Debug.Write(res[i, j].ToString() + " ");
                //    }
                //    Debug.WriteLine("");
                //}
            }
            catch (AggregateException ae)
            {
                MessageBox.Show("Wrong matrix sizes");
                //Debug.WriteLine("*** ERROR *** " + ex.Message);
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
            matrixMultiplier.Matr1RowsAmount = amount;
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
            matrixMultiplier.Matr1ColsAmount = amount;
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
            matrixMultiplier.Matr2RowsAmount = amount;
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
            matrixMultiplier.Matr2ColsAmount = amount;
        }
        #endregion
    }
}
