using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Numerics;

namespace BibKlas.AlgebraLiniowa
{
  
    
    public static  class MetodaRozkladuLU
    {

        public static string[] Komunikat = 
        { /*0*/ "Brak błędu",
            /*1 */ "Odwracana macierz w metodzie OdwMacCroutLU"+
                    "macierz musi być kwadratowa ",
            /*2 */ "Warunek przerwania poszukiwania elementu"+
                    " głównego metody RozRowMacCroutLU ",
            /*3 */ "Niezgodność wymiarów macierzy w metodzie RozRowMacCroutLU ",
            /*4 */ "Niezgodność wymiarów macierzy w metodzie RozRowMacDoolittleLU ",
            /*5 */ "Warunek przerwania poszukiwania elementu głównego"+
                    " metody RozRowMacDoolittleLU  "
        };

        public static void PiszKomunikat(int k)
        {
            MessageBox.Show(Komunikat[k]);
        }

        /// <summary>
        ///Metoda do rozwiązywania równań macierzowych A*X=B
        ///o elementach typu double dla standardowej postaci tablic
        /// metodą rozkładu LU wg algorytmu Crouta 
        /// </summary>
        /// <param name="A">Macierz kwadratowa 
        /// rzędu N=A.GetLength(0)-1</param>
        /// <param name="B">Macierz wyrazów wolnych o 
        /// N=X.A.GetLength(0)-1 elementach</param>
        /// <param name="X">Macierz rozwiązania o 
        /// N=X.A.GetLength(0)-1  elementach</param>
        /// <param name="eps">dokładność rozróżnienia 
        /// zerowej kolumny (np. eps=1E-25)</param>
        /// <returns> return 0 gdy brak błędu, 
        /// inna wartość = numer błędu</returns>
        public static int RozRowMacCroutLU(double[,] A, double[] B,
                                        double[] X, double eps)
        {
            int i, j, k, k1, N, M, R, blad;
            double S;
            double T, T1;
            double W;
            N = A.GetLength(0) - 1; M = A.GetLength(1) - 1;
            R = B.GetLength(0) - 1;
            if (R == N && M == N)
            {
                for (i = 1; i <= N; i++)
                {
                    T = Math.Abs(A[i, i]); k = i;
                    if (T < eps)
                    {
                        for (j = i + 1; j <= N; j++)
                        {
                            T1 = Math.Abs(A[j, i]);
                            if (T1 > T) { T = T1; k = j; }
                        }
                        if (T < eps)
                        { blad = 2; return blad; }
                        // Zamiana wiersza k-tego z i-tym
                        if (i != k)
                        {
                            W = B[k]; B[k] = B[i]; B[i] = W;
                            for (j = 1; j <= N; j++)
                            {
                                W = A[k, j]; A[k, j] = A[i, j];
                                A[i, j] = W;
                            }
                        }
                    }
                    //Rozkład LU macierzy A wg algorytmu 
                    //Crouta  A=L*U 
                    k1 = i + 1;
                    for (k = k1; k <= N; k++) A[i, k] /= A[i, i];
                    for (k = k1; k <= N; k++)
                        for (j = k1; j <= N; j++)
                            A[k, j] -= A[k, i] * A[i, j];
                }//i
                //Rozwiązanie układu trójkątnego dolnego
                //L*Y=B metodą postępowania w przód
                B[1] /= A[1, 1];
                for (i = 2; i <= N; i++)
                {
                    S = B[i];
                    for (j = 1; j <= i - 1; j++) S -= A[i, j] * B[j];
                    B[i] = S / A[i, i];
                }//i
                //Rozwiązanie układu trójkątnego górnego
                //UX=Y metodą postępowania wstecz
                for (i = N; i >= 1; i--)
                {
                    for (j = i + 1; j <= N; j++) B[i] -= A[i, j] * X[j];
                    X[i] = B[i];
                }//i
                return 0;
            }
            else return 3;
        }
        //---------------------------------------------------------------------------

        /// <summary>
        ///Metoda do rozwiązywania równań macierzowych A*X=B
        ///o elementach typu Complex dla standardowej postaci tablic
        /// metodą rozkładu LU wg algorytmu Crouta 
        /// </summary>
        /// <param name="A">Macierz kwadratowa 
        /// rzędu N=A.GetLength(0)-1</param>
        /// <param name="B">Macierz wyrazów wolnych o 
        /// N=X.A.GetLength(0)-1 elementach</param>
        /// <param name="X">Macierz rozwiązania o 
        /// N=X.A.GetLength(0)-1  elementach</param>
        /// <param name="eps">dokładność rozróżnienia 
        /// zerowej kolumny (np. eps=1E-25)</param>
        /// <returns> return 0 gdy brak błędu, 
        /// inna wartość = numer błędu</returns>
        public static int RozRowMacCroutLU(Complex[,] A, Complex[] B,
                                        Complex[] X, double eps)
        {
            int i, j, k, k1, N, M, R, blad;
            double T, T1;
            Complex W,S;
            N = A.GetLength(0) - 1; M = A.GetLength(1) - 1;
            R = B.GetLength(0) - 1;
            if (R == N && M == N)
            {
                for (i = 1; i <= N; i++)
                {
                    T = A[i, i].Magnitude; k = i;
                    if (T < eps)
                    {
                        for (j = i + 1; j <= N; j++)
                        {
                            T1 = A[j, i].Magnitude;
                            if (T1 > T) { T = T1; k = j; }
                        }
                        if (T < eps)
                        { blad = 2; return blad; }
                        // Zamiana wiersza k-tego z i-tym
                        if (i != k)
                        {
                            W = B[k]; B[k] = B[i]; B[i] = W;
                            for (j = 1; j <= N; j++)
                            {
                                W = A[k, j]; A[k, j] = A[i, j];
                                A[i, j] = W;
                            }
                        }
                    }
                    //Rozkład LU macierzy A wg algorytmu 
                    //Crouta  A=L*U 
                    k1 = i + 1;
                    for (k = k1; k <= N; k++) A[i, k] /= A[i, i];
                    for (k = k1; k <= N; k++)
                        for (j = k1; j <= N; j++)
                            A[k, j] -= A[k, i] * A[i, j];
                }//i
                //Rozwiązanie układu trójkątnego dolnego
                //L*Y=B metodą postępowania w przód
                B[1] /= A[1, 1];
                for (i = 2; i <= N; i++)
                {
                    S = B[i];
                    for (j = 1; j <= i - 1; j++) S -= A[i, j] * B[j];
                    B[i] = S / A[i, i];
                }//i
                //Rozwiązanie układu trójkątnego górnego
                //UX=Y metodą postępowania wstecz
                for (i = N; i >= 1; i--)
                {
                    for (j = i + 1; j <= N; j++) B[i] -= A[i, j] * X[j];
                    X[i] = B[i];
                }//i
                return 0;
            }
            else return 3;
        }
        //---------------------------------------------------------------------------

    
    }
}
