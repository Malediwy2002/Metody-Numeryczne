using System;
using System.Windows.Forms;
using BibKlas.AlgebraLiniowa;

namespace RownanieLiniowe {
    public partial class Form1 : Form {
        int N = 0;
        double[,] A;
        double[] B, X;

        public Form1() {
            InitializeComponent();
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            UstawRozmiarMacierzy();
        }

        private void UstawRozmiarMacierzy() {
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
            dataGridView3.ColumnCount = 1;
            dataGridView2.ColumnCount = 1;

            dataGridView1.ColumnHeadersHeight = 50;
            dataGridView2.ColumnHeadersHeight = 50;
            dataGridView3.ColumnHeadersHeight = 50;
            dataGridView3.Columns[0].Width = 60;
            dataGridView2.Columns[0].Width = 104;

            for (int i = 0; i < N; i++) {
                dataGridView1.Columns[i].Width = 45;
                dataGridView1.Columns[i].HeaderText = (i + 1).ToString();
                dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
                dataGridView2.Rows[i].HeaderCell.Value = (i + 1).ToString();
                dataGridView3.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            UstawRozmiarMacierzy();
        }

        private void generujBtn_Click(object sender, EventArgs e) {

            double suma = 0;
            Random random = new Random();

            for (int i = 0; i < N; i++) {
                suma = 0;
                for (int j = 0; j < N; j++) {
                    A[i, j] = random.NextDouble() * 100 - 50;
                    dataGridView1[j, i].Value = A[i, j].ToString("0.00");

                    suma += A[i, j];
                }
                B[i] = suma;
                dataGridView3[0, i].Value = suma.ToString("0.00");
            }
        }

        private void testBtn_Click(object sender, EventArgs e) {
            int suma = 0;
            Random random = new Random();

            for (int i = 0; i < N; i++) {
                suma = 0;
                for (int j = 0; j < N; j++) {
                    A[i, j] = random.Next(-10, 10);
                    dataGridView1[j, i].Value = A[i, j].ToString();
                    suma += (int)A[i, j];
                }
                B[i] = suma;
                dataGridView3[0, i].Value = suma.ToString();
            }
        }

        private void obliczBtn_Click(object sender, EventArgs e) {
            int blad = MetodaGaussa.RozRowMacGaussa(A, B, X, 1e-30);
            if (blad == 0) {
                for (int i = 0; i < N; i++) {
                    dataGridView2[0, i].Value = X[i].ToString("0.000000000000");
                }
            }
            else {
                MetodaGaussa.PiszKomunikat(blad);
            }
        }

    }
}
