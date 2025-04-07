using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BibKlas.AlgebraLiniowa;
using BibKlas;

namespace BibKlas.RownaniaNieliniowe
{
    public abstract class RowNieLinAbstract
    {
        /// <summary>
        ///Zwraca zestaw błędów jakie mogą zaistnieć przy uruchomieniami programu 
        /// </summary>
        public string[] Komunikat =
        { /*0*/  "Brak błędu",
          /*1 */ "Błąd w wyznaczeniu macierzy Jacobiego ",
          /*2 */ "Przekroczono zadaną liczbę iteracji w procesie iteracyjnym Newtona ",
          /*3 */ "Błąd rozwiązywaniu układu równań liniowych metodą Gaussa przy obliczaniu "+
                 "poprawki rozwiązania w procesie iteracyjnym Newtona ",
          /*4 */ "Błąd w procesie iteracyjnym Newtona" 
        };

        /// <summary>
        /// Funkcja publiczna do wyświetlania komunikatów błędów w procesie kompilacji
        /// </summary>
        /// <param name="k">Numer błędu od 0 do 4</param>
        public void PiszKomunikat(int k)
        {
            MessageBox.Show(Komunikat[k]);
        }

        /// <summary>
        /// Pola pomocnicze 
        /// </summary>
        public double[] X;//Wektor pomocniczy do śledzenia procesu 
                          //iteracyjnego i do poboru wektora rozwiązań  
        protected int N, //Liczba zmiennych niezależnych
                      M, //Liczba równań
                      maxit;//Maksymalna ilość iteracji po której konczy się obliczenia
        protected double eps, //Dokladość iteracji
                         epsr,//Względny krok różniczkowania przy wyznaczeniu macierzt Jacobiego 
                         epsg;//Parametr dla metody Gaussa 
        public int Nite;

        /// <summary>
        /// Abstrakcyjna funkcja wektorowa zmiennej wektorowej do zadawania układu
        /// równań nieliniowych
        /// </summary>
        /// <param name="X">Wektor zmiennych niezależnych</param>
        /// <returns>Zwraca wektor </returns>
        public abstract double[] FunWektorWektor(double[] X);

        /// <summary>
        /// Konstruktor klasy RowNieLinAvstract
        /// </summary>
        /// <param name="N1">Liczba rownań </param>
        /// <param name="X0">Warunek początkowy iteracji</param>
        /// <param name="eps">Dokladość iteracji</param>
        /// <param name="epsr">Względny krok różniczkowania</param>
        /// <param name="epsg">Dla metody Gaussa</param>
        /// <param name="maxit1">Maksymalna ilość iteracji</param>
        public RowNieLinAbstract(int N1, double[] X0, double eps, 
                                 double epsr, double epsg, int maxit1)
        {
            N = N1;
            M = N1;
            this.eps = eps;
            this.epsr = epsr;
            this.epsg = epsg;
            maxit = maxit1;
            X =(double[])X0.Clone();
            //Inicjalizacja tablicy dynamicznej do zapisu procesu iteracyjnego
            XX = new List<double[]>();
            //Zapisywanie warunku początkowego iteracji
            XX.Add(X0); 
        }

        /// <summary>
        /// Generacja macierzy Jacobiego
        /// Metoda jest wirtualna, można więc w klasach potomnych generować
        /// macierz Jacobiego analitycznie o ile jest to możliwe
        /// </summary>
        /// <param name="MacJac">Macierz Jacobiego</param>
        /// <param name="X">Punkt w którym oblicza się macierz Jacobiego</param>
        /// <param name="M">Liczba funkcji nieliniowych</param>
        /// <param name="eps2">Względny krok różniczkowania </param>
        /// <returns>return 0 brak błędu, różny od zera numer błędu</returns>
        public virtual int GeneracjaMacierzyJacobiego(double[,] MacJac,
                               double[] X, int M, double eps2)
        {
            int i, j;
            double h, r;
            double[] X1 = new double[N + 1];
            double[] Y1 ;
            double[] Y ;
            //Obliczenie wartości Y funkcji wektorowej w punkcie wejściowy X
            Y=FunWektorWektor(X);
            try
            {
                //Generacja macierzy Jacobiego (3.9)
                for (i = 1; i <= N; i++)
                {
                    //Ustalenie kroku różniczkowania h
                    r = Math.Abs(X[i]);
                    if (r > eps2) h = eps2 * r;
                    else h = eps2;
                    //Podstawieni tablicy wejściowej X do X1 
                    X1 = (double[]) X.Clone();
                    X1[i] = X[i] + h;  //Zmian i-tej składowej X1 o wartość h
                    //Obliczenie wartości funkcji wektorowej w punkcie X1
                    Y1 = FunWektorWektor(X1);
                    //Generacja i-tej kolumny macierzy Jacobiego w postaci ilorazów różnicowych (3.16)
                    for (j = 1; j <= M; j++) MacJac[j, i] = (Y1[j] - Y[j]) / h;
                }//i
                return 0;
            }
            catch
            {
                return 1;
            }
        }//GeneracjaMacierzyJacobiego

        public abstract int Rozwiaz();// Metoda abstrakcyjna realizująca proces obliczeniowy 
                                      //implementowana w klasach pochodnych

        /// <summary>
        /// Tablica dynamiczna do zapamiętania procesu iteracyjnego
        /// XX[NrIteracji][NrZmiennej] - element wektora stanu NrZmiennej
        /// iteracji numer NrIteracji
        /// </summary>
        public List<double[]> XX;

        /// <summary>
        /// Metoda protected klasy dopisująca wektor stanu X w danej
        /// chwili całkowania do tablicy dynamicznej List<double[]> FMS.
        /// Skladowa X[0] jest tą chwilą 
        /// </summary>
        /// <param name="X">Zapisywany wektor stanu</param>
        protected void Dopisz(double[] X)
        {
            double[] P = (double[])X.Clone();
            XX.Add(P);
            Nite++;
        }

    }//Koniec RowNieLinAbstract


    /// <summary>
    /// Klas pochodna względem klasy RowNieLinAbstract i abstrakcyjna 
    ///  zawierająca metodę Rozwiaz() implementującą metodę Newtona
    /// </summary>
    public abstract class RowNieLinMetodaNewtonaAbstr : RowNieLinAbstract
    {
        /// <summary>
        /// Konstruktor metody Newtona
        /// </summary>
        /// <param name="N1">Liczba równań</param>
        /// <param name="X0">Wektor warunków początkowych iteracji</param>
        /// <param name="eps">Dokładność obliczeń</param>
        /// <param name="epsr">Względny krok różniczkowania przy obliczaniu 
        /// numerycznym Jacobiego </param>
        /// <param name="epsg">Dokładność rozróżnienia maksymalnego elementu
        /// głównego w metodzie eliminacji Gauus 
        ///  w metodzie Newtona</param>
        /// <param name="maxit1">Maksymalna liczba iteracji</param>
        public RowNieLinMetodaNewtonaAbstr(int N1, double[] X0, double eps, 
            double epsr, double epsg, int maxit1) :
            base(N1, X0, eps, epsr, epsg, maxit1)
        { }
       
        /// <summary>
        /// Implementacja metod Newtona 
        /// </summary>
        /// <returns>Zwraca zero brak błędu, różny od zera zwraca numer błędu </returns>
        public override int Rozwiaz()
        {
            int i, k, blad1 = 0, blad2 = 0;
            //Tablica do obliczeń poprawki Newtona wzór (3.13)
            double[] dX = new double[N + 1];
            //Tablica do zapisu prawej strony układu (3.13)
            double[] XX = new double[N + 1];
            //Wektor do zapisu wartości funkcji wektorowej
            // FunWektorWektor z klasy bazowej
            double[] Y;
            //Macierz jakobiego generowana przez metodę klasy bazowej
            double[,] MacJac = new double[N + 1, N + 1];
            double ni;
            k = 0;
            Nite = 0;
            do
            {   //Konstrukcja procesu iteracyjnego (3.11) (3.13)
                ni = 0;
                k++;  //Liczni iteracji
                //Konstrukcja macierzy Jacobiego MacJac -metoda klasy bazowej
                blad1 = GeneracjaMacierzyJacobiego(MacJac, X, N, epsr);
                if (blad1 == 0)
                {
                    Y = FunWektorWektor(X); //Y-wartość funkcji wektorowej w k-tej iteracji
                    XX = (double[])Y.Clone();  //Kopiowanie Y do Tablicy XX
                    //Rozwiązanie układu liniowych w postaci tablicy dX(3.13)
                    blad2 = MetodaGaussa.RozRowMacGaussa(MacJac, XX, dX, epsg);
                    if (blad2 == 0)
                    {
                        for (i = 1; i <= N; i++)
                        {
                            //ni += Math.Abs(Y[i]); //Wyznaczanie normy wektorowej 
                                                    //wektora Y jako kryterium stopu pętli do{}while
                                                    //lub wektora dX
                            ni += Math.Abs(dX[i]);
                            X[i] -= dX[i];//Poprawianie rozwiązania (3.11)
                        }
                        Dopisz(X);
                    }
                    else return 3;
                }
                else return 1;
            }
            while (!(ni < eps || k > maxit || blad1 != 0 || blad2 != 0));
            if (blad1 == 0 && blad2==0)
            {
                if (k > maxit) return 2;
                else return 0;
            }
            else  return 4;
        }//Koniec Rozwiaz()
  

    }//Koniec RowNieLinMetodaNewtonaAbstr

    /// <summary>
    ///Klas pochodna względem klasy RowNieLinMetodaNewtonaAbstr
    ///której konstruktor przekazuje egzemplarz delegata FunNieLinDelegate funkcji 
    ///definiującej układ równań nieliniowych
    /// </summary>
    public class RowNieLinMetodaNewtona : RowNieLinMetodaNewtonaAbstr
    {
        /// <summary>
        /// Egzemplarz FF delegata jako pole klasy
        /// </summary>
        FunWektorWektorDelegate FF;
        
        /// <summary>
        /// Konstruktor metody Newtona
        /// </summary>
        ///  <param name="FNLD">Egzemparz delegata funkcji definiującej układ
        ///  równań nieliniowych </param>
        /// <param name="N1">Liczba równań</param>
        /// <param name="X0">Wektor warunków początkowych iteracji</param>
        /// <param name="eps">Dokładność obliczeń</param>
        /// <param name="epsr">Względny krok różniczkowania przy obliczaniu
        /// numerycznym Jacobiego </param>
        /// <param name="epsg">Dokładność rozróżnienia maksymalnego elementu 
        /// głównego w metodzie eliminacji Gauus 
        ///  w metodzie Newtona</param>
        /// <param name="maxit1">Maksymalna liczba iteracji</param>
        public RowNieLinMetodaNewtona(FunWektorWektorDelegate FNLD, int N1, double[] X0,
             double eps, double epsr, double epsg, int maxit1)
            : base(N1, X0, eps, epsr, epsg, maxit1)
        {
            FF = FNLD;
            //Przekazanie egzemplarza delegata FNLD do klasy pochodnej 
            //RowNieLinMetodaNewtona po adres FF
        }

        /// <summary>
        /// Predefiniowanie funkcji zgodnie definicją w klasie 
        /// bazowej RowNieLinAbstract
        /// </summary>
        /// <param name="X">Argument funkcji wektorowej 
        /// jako wektor double[] X</param>
        /// <returns>Zwracawektor jako tablica double[] </returns>
        public override double[] FunWektorWektor(double[] X)
        {
            return FF(X); 
            //Zwraca egzemplarz delegata przekazany prze konstruktora klasy
        }

    }//Koniec RowNieLinMetodaNewtona
    //---------------------------------------------------------------------------

}
