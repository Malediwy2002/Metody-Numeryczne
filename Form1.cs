using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RownanieLiniowe
{
    public partial class Form1: Form
    {
        // N - ilość równań
        int N = 0;

        // Macierz rzeczywista (tablica dwuwymiarowa)
        double[,] A;

        // Wektory wyrazów wolnych B układu równań. Rozwiązanie X
        double[] B, X;

        public Form1()
        {
            InitializeComponent();
            N = (int)numericUpDown1.Value;

            A = new double[N, N];
            B = new double[N];
            X = new double[N];

            UstawTablice();

        }

        private void UstawTablice() {
            dataGridView1.ColumnCount = N;
            dataGridView1.RowCount = N;
            dataGridView2.RowCount = N;
            dataGridView3.RowCount = N;

            dataGridView1.ColumnHeadersHeight = 50;
            dataGridView2.ColumnHeadersHeight = 50;
            dataGridView3.ColumnHeadersHeight = 50;
            dataGridView3.Columns[0].Width = 55;

            for (int i = 0; i < N; i++) {
                dataGridView1.Columns[i].Width = 45;
                dataGridView1.Columns[i].HeaderText = (i + 1).ToString();
                dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
                dataGridView2.Rows[i].HeaderCell.Value = (i + 1).ToString();
                dataGridView3.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }


        }



        private void numericUpDown1_Click(object sender, EventArgs e) {
            N = (int)numericUpDown1.Value;

            A = new double[N, N];
            B = new double[N];
            X = new double[N];

            UstawTablice();

        }

        private void button2_Click(object sender, EventArgs e) {
            int suma = 0;
            Random random = new Random();

            for (int i = 0; i < N; i++) {
                suma = 0;
                for (int j = 0; j < N; j++) {
                    A[i, j] = random.Next(-10, 20);
                    dataGridView1[i, j].Value = A[i, j].ToString();

                    suma += (int)A[j, i];
                }
                B[i] = suma;
                dataGridView3[0, i].Value = suma.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e) {

            int suma = 0;
            Random random = new Random();

            for (int i = 0; i < N; i++) {
                suma = 0;
                for (int j = 0; j < N; j++) {
                    A[i, j] = random.NextDouble() * (50 - (-50))+(-50);
                    dataGridView1[i, j].Value = A[i,j].ToString();

                    suma += (int)A[i, j];
                }
                B[i] = suma;
                dataGridView3[0, i].Value = suma.ToString();
            }
        }
    }
}
