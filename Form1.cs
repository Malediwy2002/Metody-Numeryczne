using System;
using System.Windows.Forms;
using BibKlas.AlgebraLiniowa;
using System.Numerics;
using System.ComponentModel;

namespace RownanieLiniowe {
    public partial class Form1 : Form {
        int N = 0;
        double[,] A;
        double[] B, X;

        Complex[,] A_zesp;
        Complex[] B_zesp, X_zesp;

        public Form1() {
            InitializeComponent();
            numericUpDown1.ValueChanged += numericUpDown1_ValueChanged;
            UstawRozmiarMacierzy();
        }

        private void UstawRozmiarMacierzy() {
            N = (int)numericUpDown1.Value;
            A = new double[N + 1, N + 1];
            B = new double[N + 1];
            X = new double[N + 1];

            A_zesp = new Complex[N + 1, N + 1];
            B_zesp = new Complex[N + 1];
            X_zesp = new Complex[N + 1];


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
            dataGridView1.RowHeadersWidth = 50;
            dataGridView3.Columns[0].Width = 90;
            dataGridView2.Columns[0].Width = 104;

            for (int i = 0; i < N; i++) {
                if (rzeczywisteRBtn.Checked) {
                    dataGridView1.Columns[i].Width = 75;
                }
                dataGridView1.Columns[i].HeaderText = (i + 1).ToString();
                dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
                dataGridView2.Rows[i].HeaderCell.Value = (i + 1).ToString();
                dataGridView3.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
            dataGridView2.ReadOnly = true;
            obliczBtn.Enabled = false;
        }

        // NUMERIC
        private void numericUpDown1_ValueChanged(object sender, EventArgs e) {
            UstawRozmiarMacierzy();
        }

        // GENERUJ
        private void generujBtn_Click(object sender, EventArgs e) {
            obliczBtn.Enabled = true;
            if (rzeczywisteRBtn.Checked) {
                Random random = new Random();

                for (int i = 1; i <= N; i++) {
                    for (int j = 1; j <= N; j++) {
                        dataGridView1[j - 1, i - 1].Value = (random.NextDouble() * 100 - 50).ToString("0.00");

                    }
                    dataGridView3[0, i - 1].Value = (random.NextDouble() * 100 - 50).ToString("0.00");
                    dataGridView2[0, i - 1].Value = "";
                }


            }
            else if (zespoloneRBtn.Checked) {
                Random random = new Random();

                for (int i = 1; i <= N; i++) {
                    for (int j = 1; j <= N; j++) {
                        A_zesp[i, j] = new Complex(random.NextDouble() * 100 - 50, random.NextDouble() * 100 - 50);

                        if (A_zesp[i, j].Imaginary >= 0)
                            dataGridView1[j - 1, i - 1].Value = $"{A_zesp[i, j].Real:0.00} +{A_zesp[i, j].Imaginary:0.00}i";
                        else
                            dataGridView1[j - 1, i - 1].Value = $"{A_zesp[i, j].Real:0.00} {A_zesp[i, j].Imaginary:0.00}i";
                    }

                    B_zesp[i] = new Complex(random.NextDouble() * 100 - 50, random.NextDouble() * 100 - 50);

                    if (B_zesp[i].Imaginary >= 0)
                        dataGridView3[0, i - 1].Value = $"{B_zesp[i].Real:0.00} +{B_zesp[i].Imaginary:0.00}i";
                    else
                        dataGridView3[0, i - 1].Value = $"{B_zesp[i].Real:0.00} {B_zesp[i].Imaginary:0.00}i";
                    dataGridView2[0, i - 1].Value = "";
                }
            }
        }

        private void CellValueChanged1(object sender, DataGridViewCellEventArgs e) {
            obliczBtn.Enabled = true;
        }

        // TEST
        private void testBtn_Click(object sender, EventArgs e) {
            obliczBtn.Enabled = true;
            if (rzeczywisteRBtn.Checked) {
                int suma = 0;
                Random random = new Random();

                for (int i = 1; i <= N; i++) {
                    suma = 0;
                    for (int j = 1; j <= N; j++) {
                        A[i, j] = random.Next(-10, 10);
                        dataGridView1[j - 1, i - 1].Value = A[i, j].ToString();

                        suma += (int)A[i, j];
                    }
                    B[i] = suma;
                    dataGridView3[0, i - 1].Value = suma.ToString();
                    dataGridView2[0, i - 1].Value = "";
                }
            }
            else if (zespoloneRBtn.Checked) {
                Complex suma;
                Random random = new Random();

                for (int i = 1; i <= N; i++) {
                    suma = new Complex(0, 0);
                    for (int j = 1; j <= N; j++) {

                        A_zesp[i, j] = new Complex(random.Next(-10, 10), random.Next(-10, 10));

                        if (A_zesp[i, j].Imaginary >= 0)
                            dataGridView1[j - 1, i - 1].Value = $"{A_zesp[i, j].Real} +{A_zesp[i, j].Imaginary}i";
                        else
                            dataGridView1[j - 1, i - 1].Value = $"{A_zesp[i, j].Real} {A_zesp[i, j].Imaginary}i";

                        suma += A_zesp[i, j];
                    }
                    B_zesp[i] = suma;

                    if (suma.Imaginary >= 0)
                        dataGridView3[0, i - 1].Value = $"{suma.Real} +{suma.Imaginary}i";
                    else
                        dataGridView3[0, i - 1].Value = $"{suma.Real} {suma.Imaginary}i";
                    dataGridView2[0, i - 1].Value = "";
                }
            }

        }

        // OBLICZ
        private void obliczBtn_Click(object sender, EventArgs e) {
            obliczBtn.Enabled = false;

            int err = 0;

            for (int i = 1; i <= N; i++) {
                for (int j = 1; j <= N; j++) {
                    if (dataGridView1[j - 1, i - 1].Value == null) {
                        err = 1;
                    }
                }
                if (dataGridView3[0, i - 1].Value == null) {
                    err = 1;
                }
            }


            if (err == 0) {
                if (rzeczywisteRBtn.Checked) {
                    {
                        for (int i = 1; i <= N; i++) {
                            for (int j = 1; j <= N; j++) {
                                A[i, j] = double.Parse(dataGridView1[j - 1, i - 1].Value.ToString());
                            }
                            B[i] = double.Parse(dataGridView3[0, i - 1].Value.ToString());
                            X[i] = 0.0;
                        }

                        int blad;
                        blad = MetodaGaussa.RozRowMacGaussa(A, B, X, 1e-30);
                        if (blad == 0)
                            for (int i = 1; i <= N; i++)
                                dataGridView2[0, i - 1].Value = X[i].ToString("0.0000");
                        else MetodaGaussa.PiszKomunikat(blad);
                    }
                }
                else if (zespoloneRBtn.Checked) {
                    {
                        for (int i = 1; i <= N; i++) {
                            for (int j = 1; j <= N; j++) {
                                string[] liczba = dataGridView1[j - 1, i - 1].Value.ToString().Split(new char[] { ' ', '+', 'i' }, StringSplitOptions.RemoveEmptyEntries);
                                A_zesp[i, j] = new Complex(double.Parse(liczba[0]), double.Parse(liczba[1]));
                            }
                            string[] liczba2 = dataGridView3[0, i - 1].Value.ToString().Split(new char[] { ' ', '+', 'i' }, StringSplitOptions.RemoveEmptyEntries);
                            B_zesp[i] = new Complex(double.Parse(liczba2[0]), double.Parse(liczba2[1]));
                            X_zesp[i] = new Complex(0, 0);
                        }
                        int blad;
                        blad = MetodaGaussa.RozRowMacGaussa(A_zesp, B_zesp, X_zesp, 1e-30);
                        if (blad == 0)
                            for (int i = 1; i <= N; i++)
                                dataGridView2[0, i - 1].Value = X_zesp[i].ToString("0.0000");
                        else MetodaGaussa.PiszKomunikat(blad);
                    }

                }
            }
            else {
                MessageBox.Show("Nie można wykonać obliczeń. Upewnij się, że wszystkie pola są wypełnione.");
            }
        }
    }
}
