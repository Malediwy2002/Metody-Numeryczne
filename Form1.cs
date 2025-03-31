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
            A = new double[N, N];
            B = new double[N];
            X = new double[N];

            A_zesp = new Complex[N, N];
            B_zesp = new Complex[N];
            X_zesp = new Complex[N];


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
            dataGridView3.Columns[0].Width = 90;
            dataGridView2.Columns[0].Width = 104;

            for (int i = 0; i < N; i++) {
                if (rzeczywisteRBtn.Checked) {
                    dataGridView1.Columns[i].Width = 90;
                }
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
            if (rzeczywisteRBtn.Checked) {
                double Ax, suma = 0;
                Random R = new Random(); //Generator liczb losowych
                N = (int)numericUpDown1.Value;//Odczyt liczby równań liniowych
                UstawTablice();
                //Inicjalizacja macierzy A oraz wektorów B,X
                A = new double[N + 1, N + 1];
                B = new double[N + 1];
                X = new double[N + 1];
                //Generacja macierzy A oraz wektora wyrazów wolnych B
                for (int i = 1; i <= N; i++) {
                    suma = 0;
                    for (int j = 1; j <= N; j++) {
                        Ax = R.Next(1, 100); //losowanie liczb z przedziału od 1 do 100
                        dataGridView1[j - 1, i - 1].Value = Ax.ToString();
                        A[i, j] = Ax; //i-ty wiersz , j-ta kolumna
                        suma += Ax;
                    }
                    B[i] = suma;//Jako suma elementow i-tego wiersza macierzy A[i,j]
                    dataGridView3[0, i - 1].Value = suma.ToString();
                }


            }
            else if (zespoloneRBtn.Checked) {
                Complex suma;
                Random random = new Random();

                for (int i = 0; i < N; i++) {
                    suma = new Complex(0, 0); // Poprawna inicjalizacja liczby zespolonej
                    for (int j = 0; j < N; j++) {
                        double realPart = random.NextDouble() * 100 - 50;  // Losowa część rzeczywista
                        double imagPart = random.NextDouble() * 100 - 50;  // Losowa część urojona
                        A_zesp[i, j] = new Complex(realPart, imagPart);

                        // Wyświetlanie liczby zespolonej w DataGridView
                        dataGridView1[j, i].Value = $"{A_zesp[i, j].Real:0.00} + {A_zesp[i, j].Imaginary:0.00}i";

                        suma += A_zesp[i, j];
                    }
                    B_zesp[i] = suma;
                    dataGridView3[0, i].Value = $"{suma.Real:0.00} + {suma.Imaginary:0.00}i"; // Wyświetlanie sumy zespolonej
                }
            }
        }

        private void testBtn_Click(object sender, EventArgs e) {
            if (rzeczywisteRBtn.Checked) {
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
            else if (zespoloneRBtn.Checked) {
                Complex suma;
                Random random = new Random();

                for (int i = 0; i < N; i++) {
                    suma = new Complex(0, 0); // Poprawna inicjalizacja liczby zespolonej
                    for (int j = 0; j < N; j++) {
                        int realPart = random.Next(-10,10);  // Losowa część rzeczywista
                        int imagPart = random.Next(-10,10);  // Losowa część urojona
                        A_zesp[i, j] = new Complex(realPart, imagPart);

                        // Wyświetlanie liczby zespolonej w DataGridView
                        dataGridView1[j, i].Value = $"{A_zesp[i, j].Real} + {A_zesp[i, j].Imaginary}i";

                        suma += A_zesp[i, j];
                    }
                    B_zesp[i] = suma;
                    dataGridView3[0, i].Value = $"{suma.Real} + {suma.Imaginary}i"; // Wyświetlanie sumy zespolonej
                }
            }

        }

        private void obliczBtn_Click(object sender, EventArgs e) {
            if (rzeczywisteRBtn.Checked) {
                {
                    int blad;
                    //Metoda statyczna RozRowMacGaussa nie wymaga inicjalizacji obiektu klasy
                    //MetodaGaussa lecz bezpośrednio odwołuje się do definicji klasy
                    blad = MetodaGaussa.RozRowMacGaussa(A, B, X, 1e-30);
                    if (blad == 0)
                        for (int i = 1; i <= N; i++)
                            dataGridView2[0, i - 1].Value = X[i].ToString("0.000000000000");
                    else MetodaGaussa.PiszKomunikat(blad);
                }
                }
            else if (zespoloneRBtn.Checked) {
                int blad = MetodaGaussa.RozRowMacGaussa(A_zesp, B_zesp, X_zesp, 1e-30);
                if (blad == 0) {
                    for (int i = 0; i < N; i++) {
                        dataGridView2[0, i].Value = X_zesp[i].ToString("0.000000000000");
                    }
                }
                else {
                    MetodaGaussa.PiszKomunikat(blad);
                }
            }

        }

    }
}
