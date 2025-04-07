using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSBibliotekaStudent.RownaniaRozniczkowe
{
    public delegate double[] FunNielinRowRozniczkoweDelegate(double[] X, double t);


    public abstract class RownaniaRozniczkoweBaza
    {
        /// <summary>
        /// Tablica dynamiczna do zapamiętania rozwiązania
        /// FMS[NrIteracji][NrZmiennej] - element wektora stanu NrZmiennej
        /// iteracji numer NrIteracji
        /// </summary>
        public List<double[]> FMS;

        /// <summary>
        /// FN-liczba równań
        /// </summary>
        public int FN;

        /// <summary>
        /// Rząd metody jednokrokowej
        /// </summary>
        public int FRzad;

        /// <summary>
        /// FNetap-liczba etapów metody całkowania jednokrokowego
        /// </summary>
        public int FNetap;

        public int FNEror;
        /// <summary>
        /// FLW-liczba iteracji rozwiązania
        /// </summary>
        public int FLW;

        /// <summary>
        /// FTp-czas początkowy
        /// </summary>
        public double FTp;

        /// <summary>
        /// FTk-czas końcowy
        /// </summary>
        public double FTk;

        /// <summary>
        /// FStKr=true - stały krok całkowania
        ///  FStKr=false- automatyczny dobór kroku całkowania
        /// </summary>
        public bool FStKr;

        /// <summary>
        /// FH0- wartość stałego kroku całkowania
        /// </summary>
        public double FH0;

        /// <summary>
        /// FEpsW - tolerancja błędu względnego
        /// </summary>
        public double FEpsW;

        /// <summary>
        ///FEpsWM- Minimalna tolerancja błędu względnego
        /// </summary>
        public double FEpsWM;

        /// <summary>
        /// FEpsA - błąd bezwzględny 
        /// </summary>
        public double FEpsA;

        /// <summary>
        /// Błąd komputera dla danej dokładności obliczeń
        /// </summary>
        public double Fepsm;

        /// <summary>
        /// FA26=Fepsm*26
        /// </summary>
        public double FA26;

        /// <summary>
        /// Przyjmuje wartość true, jeżeli wywołano metodę Obliczaj
        /// </summary>
        protected bool JestRozwiazanie;

        /// <summary>
        ///  Przyjmuje wartość true, jeżeli wywołano metodę Kontynuuj
        /// </summary>
        protected bool WywolanoKontynuuj;

        /// <summary>
        ///  Przyjmuje wartość true, jeżeli wywołano metodę ZerujIKontynuuj
        /// </summary>
        protected bool WywolanoZerujIKontynuuj;

        
        /// <summary>
        /// 
        /// </summary>
        public bool ZapiszBlad;

        protected bool WywolanoObliczaj;
        /// <summary>
        /// Metoda protected klasy dopisująca wektor stanu X w danej
        /// chwili całkowania do tablicy dynamicznej List<double[]> FMS.
        /// Skladowa X[0] jest tą chwilą 
        /// </summary>
        /// <param name="X">Zapisywany wektor stanu</param>
        protected void Dopisz(double[] X)
        {
            double[] P = (double[])X.Clone();
            FMS.Add(P);
            FLW++;
        }

        /// <summary>
        /// Metoda prywatna klasy RownaniaRozniczkoweBaza zerująca tablicę dynamiczną
        /// do zapisywania wektorów stanu , ustalająca licznik wektorów
        /// stanu na zerze FLW=0 oraz zapisująca do tej tablicy warunek początkowy
        /// </summary>
        protected void Zeruj()
        {
            double[] X;
            X =(double[])FMS[0].Clone();
            FMS.Clear();
            FLW = 0;
            Dopisz(X);
            JestRozwiazanie = false;
        }
        //Koniec przepisywania
 
        /// <summary>
        /// Metoda abstrakcyjna generowana w klasach pochodnych, narzucająca zapis 
        /// układu równań różniczkowych w postaci funkcji, która wektorowi 
        /// stanu X w chwili t zwraca wektor odpowiadający prawej stronie równania (3) 
        /// tj. wektor pochodnych wektora stanu 
        /// </summary>
        /// <param name="X">Wektor stanu</param>
        /// <param name="t">Chwila czasu dla wektora stanu</param>
        /// <returns>Zwraca wektor pochodnych wektora stanu tj. prawą stronę równania (3)</returns>
        public abstract double[]  FunkcjeNielinioweRownaniaRozniczkowego(double[] X, double t);

        /// <summary>
        /// Metodę Obliczaj należy zdefiniować w obiektach potomnych
        /// dla implementowanej metody
        /// </summary>
        /// <returns>Numer błędu</returns>
        protected abstract int Obliczaj();

        /// <summary>
        /// Pobiera warunek początkowy dla ewentualnych
        /// dalszych kroków całkowania
        /// </summary>
        /// <returns>Zwracany wektor jako warunek początkowy</returns>
        public double[] PobierzWP()
        {
            return FMS[0];

        }

        /// <summary>
        /// Metoda publiczna dla właściwości WarunekPoczatkowy do zadawania
        ///z zewnątrz warunku początkowego
        /// </summary>
        /// <param name="WP">Ustawiany wektor początkowy w tablicy dyamicznej FMS</param>
        public void UstawWP(double[] WP)
        {
            if (FMS != null) FMS.Clear();
            FLW = 0;
            Dopisz(WP);
            JestRozwiazanie = false;
        }

        /// <summary>
        /// Konstruktor klasy bez parametrów
        /// </summary>
        public RownaniaRozniczkoweBaza()
        {
            WywolanoKontynuuj = false;
            WywolanoZerujIKontynuuj = false;
        }

        /// <summary>
        /// Konstruktor klasy z parametrami
        /// </summary>
        /// <param name="N"> liczba równań</param>
        /// <param name="Tp"> czas początkowy obliczeń</param>
        /// <param name="Tk"> czas końcowy obliczeń</param>
        /// <param name="X0"> wektor zawierający warunek początkowy</param>
        public RownaniaRozniczkoweBaza(int N, double Tp, double Tk, double[] X0)
        {
            FN = N; FTp = Tp; FTk = Tk; FLW = 0;
            FStKr = false; FH0 = 1e-3; FEpsW = 1e-6;
            FEpsWM = 1e-8; FEpsA = 1e-6;
            JestRozwiazanie = false;
            ZapiszBlad = false;
            WywolanoKontynuuj = false; WywolanoZerujIKontynuuj = false;
            FMS = new List<double[]>();
            FMS.Add(X0);  //Zapisywanie warunku początkowego
            //Wy znaczanie błędu maszynowego Fepsm dla wybranej precyzji obliczeń
            double x; Fepsm = 1;
            do
            {
                Fepsm = Fepsm / 2.0; x = Fepsm + 1.0;
            } while (x > 1);
            FA26 = 26.0 * Fepsm;
        }
        /// <summary>
        /// Wyznaczanie błędu komputera 
        /// Fepsm dla wybranej precyzji obliczeń
        /// </summary>
        protected void BladMaszynowy()
        {
            double x; Fepsm = 1;
            do
            {
                Fepsm = Fepsm / 2.0;
                x = Fepsm + 1.0;
            }
            while (x > 1);
        }

        /// <summary>
        /// Metoda publiczna sprawdzająca poprawność podstawienia względnej
        ///dokładności obliczeń  i zadanego przedziału
        ///całkowania oraz inicjująca przedefiniowaną metodę Obliczaj
        /// </summary>
        /// <returns>Numer błędu obliczeń, zwraca zero brak błędu</returns>
        public int Rozwiaz()
        {
            int Blad; double dt;
            Zeruj();
            BladMaszynowy();
            if (FEpsW < 2 * Fepsm + FEpsWM) FEpsW = 2 * Fepsm + FEpsWM;
            dt = FTk - FTp;
            dt = Math.Abs(dt);
            if (dt < FA26) return 6;
            else
            {
                Blad = Obliczaj();
                if (Blad == 0)
                {
                    FTp = Czas(0);
                    FTk = Czas(FLW - 1);
                    JestRozwiazanie = true;
                }
                return Blad;
            }
        }

        /// <summary>
        /// Kontynuacja obliczeń przez czas DeltaT. Dotychczasowe wektory stanu są zachowane
        /// Metodę Kontynuuj można wywołać po metodzie Rozwiaz lub zamiast niej
        /// </summary>
        /// <param name="DeltaT">Czas dalszego całkowania</param>
        /// <returns>Numer błędu obliczeń, zwraca zero brak błędu</returns>
        public int Kontynuuj(double DeltaT)
        {
            int Blad;
            WywolanoKontynuuj = true;
            WywolanoZerujIKontynuuj = false;
            if (FEpsW < 2 * Fepsm + FEpsWM)
                FEpsW = 2 * Fepsm + FEpsWM;
            if (Math.Abs(FTk - FTp) < FA26) return 6;
            else
            {
                FTp = Czas(0);
                FTk = Czas(FLW - 1);
                FTk += DeltaT;
                Blad = Obliczaj();
                FTp = Czas(0);
                FTk = Czas(FLW - 1);
                JestRozwiazanie = true;
                return Blad;
            }
        }

        /// <summary>
        /// Kontynuacja obliczeń przez czas DeltaT. Dotychczasowe wektory stanu są wyzerowane,
        /// przy czym ostatni z nich staje sie warunkiem początkowycm.
        /// Metodę ZerujIKontynuuj można wywołać po metodzie Rozwiaz lub zamiast niej
        /// </summary>
        /// <param name="DeltaT"></param>
        /// <returns>Numer błędu obliczeń, zwraca zero brak błędu</returns>
        public virtual int ZerujIKontynuuj(double DeltaT)
        {
            int Blad;
            double[] WP = new double[FN + 1];  //FMS[FLW-1]
            WP =(double[])FMS[FLW - 1].Clone();
            WP[0] = 0;
            WywolanoZerujIKontynuuj = true;
            FMS.Clear();
            FLW = 0;
            Dopisz(WP);
            FTp = 0;
            FTk = DeltaT;
            Blad = Obliczaj();
            FTp = Czas(0);
            FTk = Czas(FLW - 1);
            return Blad;
        }

        /// <summary>
        /// Funkcja publiczna klasy do pobrania chwili czasowej
        /// wektora stanu o numerze NrWek
        /// </summary>
        /// <param name="NrWek">numer wektora stanu</param>
        /// <returns>Pobrana chwila czasu</returns>
        public double Czas(int NrWek)
        {
            return FMS[NrWek][0];
        }

        /// <summary>
        ///  Funkcja publiczna klasy do pobrania składowej Nr
        ///  wektora stanu o numerze NrWek
        /// </summary>
        /// <param name="NrWek">numer wektora stanu</param>
        /// <param name="Nr">numer skladowej wektora stanu</param>
        /// <returns></returns>
        public double X(int NrWek, int Nr)
        {
            if (Nr > 0 && Nr <= FN) return FMS[NrWek][Nr];
            else return FMS[0][0];
        }

       
        /// <summary>
        ///  Metoda publiczna klasy do pobrania wektora
        ///  stanu o numerze NrWek
        /// </summary>
        /// <param name="NrWek">numer pobieranego wektora stanu</param>
        /// <returns>Pobrany wektor stanu. Jego skladowa zerowa jest 
        /// chwilą czasu, której odpowiada pobrany wektor</returns>
        public double[] WektorStanu(int NrWek)
        {
            return FMS[NrWek];
        }

        public double[] KK;

        //c- wektor odpowiadający pierwszej kolumnie tablicy 2 Butchera
        //w1- wektor odpowiadający przedostatniemu  wierszowi tablicy 2 Butchera
        //w2 wektor odpowiadający ostatniemu wierszowi tablicy 2 Butchera
        //e - wektor błędów w metodach włożonych
        //a - macierz odpowiadająca tablicy 2 Butchera
        
        public double[] c, w1, w2, e;
        public double[,] a;
     
        //KX - macierz (20) do realizacji obliczeń w metodach Rungego-Kutta
        public double[,] KX;
      
        /// <summary>
        /// Metoda ogólna Rungego-Kutty par włożonych realizująca krok całkowania h 
        /// dla parametrów zadanch w postaci macierzy Butchera tabela 2
        /// </summary>
        /// <param name="Netap">Ilość etapów metody</param>
        /// <param name="h">Krok całkowania względem podstawy t0</param>
        /// <param name="t0">Podstawa czasu dla całkowania</param>
        /// <param name="X0">Punkt początkowy całkowania</param>
        /// <param name="X">Wynik całkowania względem podstawy X0</param>
        /// <param name="E">Oszacowany wektor błędu całkowania</param>
        /// <param name="a">Wewnętrzna część macierzy Butchera stosowana 
        /// do wykonywania obliczeń wg. wzorów (8) </param>
        /// <param name="c">Pierwsza kolumna macierzy Butchera 
        /// jako węzły czasowe całkowana (wzory (8)</param>
        /// <param name="w">Ostatni wiersz macierzy Butchera tabela 2
        /// stosowanu do wykonania obliczeń wektora X wg. wzoru (10)</param>
        /// <param name="e">Róznica pomiędzy ostatnim i przedostatnim 
        /// wierszem macierzy Butchera stosowana do wykonania obliczeń 
        /// wektora błędu E zgodnie z wzorem (16)</param>
        public virtual void MetodaRungeKuttaPairs(int Netap, double h, double t0,
                double[] X0, double[] X, double[] E)
        {
            int i, j, l, k;
            double t;
            double[] X1 = new double[FN + 1];
            double[] KK = new double[FN + 1]; ;
            KK = FunkcjeNielinioweRownaniaRozniczkowego(X0, t0);
            //Zapisywanie pierwszego wiersza macierzy (20) przy pomocy wzoru (8a)
            for (k = 1; k <= FN; k++) KX[1, k] = KK[k] * h;
            //Zapisywanie kolejnych wiersza macierzy (20) przy pomocy wzoru (8b)
            for (i = 2; i <= Netap; i++)
            {
                t = t0 + c[i] * h;
                for (l = 1; l <= FN; l++) X1[l] = X0[l];
                for (j = 1; j <= i - 1; j++)
                    for (k = 1; k <= FN; k++) X1[k] += KX[j, k] * a[i, j];
                KK = FunkcjeNielinioweRownaniaRozniczkowego(X1, t);
                for (k = 1; k <= FN; k++) KX[i, k] = KK[k] * h;
            }
            //Formowanie wektora błędu wg. wzoru (16) 
            for (i = 1; i <= FN; i++) E[i] = 0;
            for (i = 1; i <= Netap; i++)
                for (k = 1; k <= FN; k++) E[k] += KX[i, k] * e[i];
            //Formowanie rozwiązania wg. wzoru (10)
            for (l = 1; l <= FN; l++) X[l] = X0[l];
            for (j = 1; j <= Netap; j++)
                for (k = 1; k <= FN; k++) X[k] += KX[j, k] * w2[j];
        }
    }//Koniec RownaniaRozniczkoweBaza


    /// <summary>
    /// Klasa pochodna względem klasy bazowej RownaniaRozniczkoweBaza
    /// zawierająca metody typu par włożonych Rungego-Kutty 
    /// implementujące rozwiązywanie ukladów równań różniczkowych
    /// </summary>
    public abstract class RownanieRozniczkoweTypRKParyWlozoneAbstract : RownaniaRozniczkoweBaza //Metody jawne
    {

        public RownanieRozniczkoweTypRKParyWlozoneAbstract()
            : base()
        { }

        /// <summary>
        /// Konstruktor klasy z parametrami
        /// </summary>
        /// <param name="N"> liczba równań</param>
        /// <param name="Tp"> czas początkowy obliczeń</param>
        /// <param name="Tk"> czas końcowy obliczeń</param>
        /// <param name="X0"> wektor zawierający warunek początkowy</param>
        public RownanieRozniczkoweTypRKParyWlozoneAbstract(int N, double Tp1, double Tk1, double[] X0)
            : base(N, Tp1, Tk1, X0)
        { }


        private double[] X1;

        private double FH;  //Krok całkowania po ostatnim wywołaniu Obliczaj

        /// <summary>
        /// Metoda ogólna Rungego-Kutty realizująca krok całkowania h 
        /// dla parametrów zadanch w postaci macierzy Butchera tabela 2
        /// </summary>
        /// <param name="Netap">Ilość etapów metody</param>
        /// <param name="h">Krok całkowania względem podstawy t0</param>
        /// <param name="t0">Podstawa czasu dla całkowania</param>
        /// <param name="X0">Punkt początkowy całkowania</param>
        /// <param name="X">Wynik całkowania względem podstawy X0</param>
        /// <param name="E">Oszacowany wektor błędu całkowania</param>
        /// <param name="a">Wewnętrzna część macierzy Butchera stosowana 
        /// do wykonywania obliczeń wg. wzorów (8) </param>
        /// <param name="c">Pierwsza kolumna macierzy Butchera 
        /// jako węzły czasowe całkowana (wzory (8)</param>
        /// <param name="w">Ostatni wiersz macierzy Butchera tabela 2
        /// stosowanu do wykonania obliczeń wektora X wg. wzoru (10)</param>
        /// <param name="e">Róznica pomiędzy ostatnim i przedostatnim 
        /// wierszem macierzy Butchera stosowana do wykonania obliczeń 
        /// wektora błędu E zgodnie z wzorem (16)</param>
        public void MetodaRungeKuttaPairs(int Netap, double h, double t0,
                double[] X0, double[] X, double[] E)
        {
            int i, j, l, k;
            double t;
            KK = FunkcjeNielinioweRownaniaRozniczkowego(X0, t0);
            //Zapisywanie pierwszego wiersza macierzy (20) przy pomocy wzoru (8a)
            for (i = 1; i <= FN; i++) KX[1, i] = KK[i] * h;
            //KX[1] = KK * h;
            //Zapisywanie kolejnych wiersza macierzy (20) przy pomocy wzoru (8b)
            for (i = 2; i <= Netap; i++)
            {
                t = t0 + c[i] * h;
                //for (l = 1; l <= FN; l++) X1[l] = X0[l];
                X1 =(double[])X0.Clone();
                for (j = 1; j <= i - 1; j++)
                    for (k = 1; k <= FN; k++) X1[k] += KX[j, k] * a[i, j];
                KK = FunkcjeNielinioweRownaniaRozniczkowego(X1, t);
                //KX[i] = KK * h;
                for (j=1; j <= FN; j++) KX[i, j] = KK[j] * h;
            }
            //Formowanie wektora błędu wg. wzoru (16) 
            for (i = 1; i <= FN; i++) E[i] = 0;
            //E.Zero();
            for (i = 1; i <= Netap; i++)
                for (k = 1; k <= FN; k++) E[k] += KX[i, k] * e[i];
            //Formowanie rozwiązania wg. wzoru (10)
            for (l = 1; l <= FN; l++) X[l] = X0[l];
            for (j = 1; j <= Netap; j++)
                for (k = 1; k <= FN; k++) X[k] += KX[j, k] * w1[j];
        }

        /// <summary>
        /// Metoda abstrakcyjna, która implementowana jest w klasach pochodnych 
        /// musi przekazać macierz Butchera dla algorytmów typu Runge-Kutta
        /// par włożonych z podaniem rzędu FRzad i ilości etapów FNetap metody
        /// </summary>
        public abstract void TabelaButchera();

        /// <summary>
        /// Implementacja metody typu Runge-Kutta par włożonych
        /// zadanej w postaci tablicy Butchera 3.10
        /// </summary>
        /// <returns>Numer błędu obliczeń - zero brak błędu</returns>
        protected override int Obliczaj()
        {
            int KKX, j, Blad = 0;
            double[] Er, XX1, Y;
            Er = new double[FN + 1];
            XX1 = new double[FN + 1];
            Y = new double[FN + 1];
            X1 = new double[FN + 1];
            KK = new double[FN + 1];
            double alfa, t, e, xt, mxt, H, Hmin, ee, Hph, h1, maxE;
            //Wywołanie metody TabelaButchera() która jest implementowana
            //w klasach pochodnych tj. pobór elementów macierzy a, wektorów c oraz w1 i w2
            //oraz ustalenie rzędu metody FRzad i ilości etapów FNetap
            if (!WywolanoKontynuuj && !WywolanoZerujIKontynuuj) TabelaButchera();
            KKX = 0;
            //Pobór warunku początkowego 
            Y =(double[])FMS[FLW - 1].Clone();
            t = Y[0];
            //Wstępne ustalenie kroku całkowania H
            if (WywolanoKontynuuj) H = FH;
            else
            {
                if (FStKr) H = FH0;
                else H = (FTk - t) / 1000.0;
            }
            do
            {
                //Wyznaczanie normy wektora błędu Er oraz wektora XX1 wzór (3.46)i(3.40)
                MetodaRungeKuttaPairs(FNetap, H, t, Y, XX1, Er);
                ee = 0; mxt = 0;
                for (j = 1; j <= FN; j++)
                {
                    e = Math.Abs(Er[j]);
                    if (ee < e) ee = e;
                    xt = 0.5 * (Math.Abs(Y[j]) + Math.Abs(XX1[j]));
                    if (mxt < xt) mxt = xt;
                }
                //Badanie konieczności zmiany kroku całkowania wg. wzoru (29)
                ee /= mxt * FEpsW + FEpsA;
                if (ee == 0) Hph = 0.9;
                else Hph = Math.Exp(Math.Log(ee) / (FRzad + 1));
                h1 = H / Hph;
                alfa = 0.9 / Hph;
                Hmin = FA26 * Math.Abs(t);
                if (Math.Abs(h1) < Hmin && !FStKr)
                    throw new Exception("Krok całkowania przekroczył dopuszczalną wartość Hmin="
                        + Hmin.ToString() + "w metodzie Obliczaj()"); //Blad = 3;
                else
                {
                    //Warunek akceptujący zapis rozwiązania do tablicy dynamicznej FMS
                    if (Hph <= 1 || FStKr)
                    {
                        //for (i = 0; i <= FN; i++) Y[i] = XX1[i];
                        Y = (double[])XX1.Clone();
                        t += H;
                        Y[0] = t; XX1[0] = t;
                        //Zapis wektora Er błędu do tablicy dynamicznej
                        Dopisz(Y);
                        FH = H;
                        //Jeżeli w poprzedniej iteracji było spełnione kryterium Hph <= 1
                        //tj KKX==0 to pozwala się wydłużać krok całkowania 
                        if (KKX == 0 && !FStKr)
                        {
                            if (alfa < 5 && alfa >= 0.9) H *= alfa;
                            else if (alfa >= 5) H *= 5.0;
                        }
                        //w przeciwnym razie jeżeli alfa < 1 można tylko zmnijszać krok całkowania
                        else if (KKX > 0 && alfa < 1 && !FStKr) H *= alfa;
                        KKX = 0;
                    }
                    else
                    {
                        KKX++;
                        if (!FStKr) H *= alfa;
                        //else throw new Exception("Przy stałym kroku całkowania H=" +
                        //    H.ToString() + " przekroczono zadaną dokładność obliczeń");//Blad = 4;
                    }
                    if (Blad == 0)
                    {
                        h1 = FTk - t;
                        if (h1 < H && H > 0 && !FStKr) H = h1;
                        else if (h1 > H && H < 0 && !FStKr) H = h1;
                    }
                }
            }
            while (!(t >= FTk || Blad != 0));
            if (Blad == 0)
            {
                WywolanoObliczaj = true; WywolanoZerujIKontynuuj = false;
                WywolanoKontynuuj = false;
            }
            return Blad;
        }
    }//Koniec RownanieRozniczkoweTypRKParyWlozoneAbstract

    
    /// <summary>
    /// Klasa zawierająca implementację metody 
    /// par włożonych 5 i 4 rzędu Dormanda Prince'a 9 etapowa
    /// </summary>
    public abstract class MetodaRKDP54Abstract : RownanieRozniczkoweTypRKParyWlozoneAbstract
    {
        /// <summary>
        /// Konstruktor klasy z parametrami
        /// </summary>
        /// <param name="N"> liczba równań</param>
        /// <param name="Tp"> czas początkowy obliczeń</param>
        /// <param name="Tk"> czas końcowy obliczeń</param>
        /// <param name="X0"> wektor zawierający warunek początkowy</param>
        public MetodaRKDP54Abstract(int N, double Tp1, double Tk1, double[] X0)
            : base(N, Tp1, Tk1, X0)
        { }
        public MetodaRKDP54Abstract()
        { }

        /// <summary>
        /// Metoda par włożonych 5 i 4 rzędu Dormanda Prince'a 9 etapowa
        /// </summary>
        public override void TabelaButchera()
        {
            a = new double[10, 10];
            //b = new double[10, 5];
            c = new double[10];
            w1 = new double[10];
            w2 = new double[10];
            e = new double[10];
            c[1] = 0; a[1, 1] = 0;
            c[2] = 1.0 / 5.0; a[2, 1] = 1.0 / 5.0;
            c[3] = 3.0 / 10.0; a[3, 1] = 3.0 / 40.0; a[3, 2] = 9.0 / 40.0;
            c[4] = 4.0 / 5.0; a[4, 1] = 44.0 / 45.0; a[4, 2] = -56.0 / 15.0;
            a[4, 3] = 32.0 / 9.0;
            c[5] = 8.0 / 9.0; a[5, 1] = 19372.0 / 6561.0; a[5, 2] = -25360.0 / 2187.0;
            a[5, 3] = 64448.0 / 6561.0; a[5, 4] = -212.0 / 729.0;
            c[6] = 1.0; a[6, 1] = 9017.0 / 3168.0; a[6, 2] = -355.0 / 33.0;
            a[6, 3] = 46732.0 / 5247.0;
            a[6, 4] = 49.0 / 176.0; a[6, 5] = -5103.0 / 18656.0;
            c[7] = 1.0; a[7, 1] = 35.0 / 384.0; a[7, 2] = 0; a[7, 3] = 500.0 / 1113.0;
            a[7, 4] = 125.0 / 192.0; a[7, 5] = -2187.0 / 6784.0;
            a[7, 6] = 11.0 / 84.0;
            c[8] = 1.0 / 6.0; a[8, 1] = 6245.0 / 62208.0; a[8, 2] = 0;
            a[8, 3] = 8875.0 / 103032.0; a[8, 4] = -125.0 / 1728.0;
            a[8, 5] = 801.0 / 13568.0; a[8, 6] = -13519.0 / 368064.0;
            a[8, 7] = 11105.0 / 368064.0;
            c[9] = 5.0 / 6.0; a[9, 1] = 632855.0 / 4478976.0; a[9, 2] = 0;
            a[9, 3] = 4146875.0 / 6491016.0; a[9, 4] = 5490625.0 / 14183424.0;
            a[9, 5] = -15975.0 / 108544.0; a[9, 6] = 8295925.0 / 220286304.0;
            a[9, 7] = -1779595.0 / 62938944.0; a[9, 8] = -805.0 / 4101.0;
            w1[1] = 35.0 / 384.0; w1[2] = 0; w1[3] = 500.0 / 1113.0;
            w1[4] = 125.0 / 192.0; w1[5] = -2187.0 / 6784.0; w1[6] = 11.0 / 84.0;
            w1[7] = 0; w1[8] = 0; w1[9] = 0;
            w2[1] = 5179.0 / 57600.0; w2[2] = 0; w2[3] = 7571.0 / 16695.0;
            w2[4] = 393.0 / 640.0; w2[5] = -92097.0 / 339200.0; w2[6] = 187.0 / 2100.0;
            w2[7] = 1.0 / 40.0; w2[8] = 0; w2[9] = 0;

            e[1] = -35.0 / 384.0 + 5179.0 / 57600.0; e[2] = 0;
            e[3] = -500.0 / 1113.0 + 7571.0 / 16695.0;
            e[4] = -125.0 / 192.0 + 393.0 / 640.0; e[5] = +2187.0 / 6784.0 - 92097.0 / 339200.0;
            e[6] = -11.0 / 84.0 + 187.0 / 2100.0; e[7] = 1.0 / 40.0; e[8] = 0; e[9] = 0;
            FNetap = 9;
            FRzad = 5;
            KX = new double[10, FN + 1];
        }
    }//Koniec MetodaRKDP54Abstract

    /// <summary>
    /// Klasa potomna MetodaRKDP54 klasy MetodaRKDP54Abstract do rozwiązywania 
    /// układów równań różniczkowych metodą Dormand-Prince 5 i 4 rzędu
    /// </summary>
    public class MetodaRKDP54 : MetodaRKDP54Abstract
    {
        /// <summary>
        /// Pole typu delegate FunNielinRowRozniczkoweDelegate wskazujące na funkcję o takich samych
        /// parametrach co abstrakcyjna funkcja FunkcjeNielinioweRownaniaRozniczkowego klasy
        /// RownanieRozniczkoweTypRKParyWlozoneAbstract definijąca prawą stronę postaci normalnej 
        /// ukladu równań różniczkowych
        /// </summary>
        private FunNielinRowRozniczkoweDelegate Rownanie;

        /// <summary>
        /// Implementacja funkcji wektorowej wyznaczającej prawą stronę ukladu równań różniczkowych
        /// tj. obliczających wektor pochodnych wektora stanu w oparciu o przesłanego delegata do klasy
        /// </summary>
        /// <param name="X">Wektor stanu dla chwili czasu t</param>
        /// <param name="t">chwila czasu wektora stanu</param>
        /// <returns></returns>
        public override double[] FunkcjeNielinioweRownaniaRozniczkowego(double[] X, double t)
        {
            return Rownanie(X, t);
        }

        public MetodaRKDP54()
            : base()
        { }

        /// <summary>
        /// Konstruktor klasy MetodaRKDP54 podający adres funkcji wektorowej 
        /// układu równań różniczkowych przez podstawieni Rownanie = Row 
        /// </summary>
        /// <param name="Row">egzemplarz delegata FunNielinRowRozniczkoweDelegate 
        /// do zadawania prawej strony równania różniczkowego w postaci normalnej</param>
        /// <param name="N"> ilość równań</param>
        /// <param name="Tp1">chwila początkowa obliczeń</param>
        /// <param name="Tk1">chwila końcowa obliczeń</param>
        /// <param name="X0">wektor zawierający warunek początkowy</param>
        public MetodaRKDP54(FunNielinRowRozniczkoweDelegate Row, int N, double Tp1, double Tk1, double[] X0)
            : base(N, Tp1, Tk1, X0)
        { Rownanie = Row; }

    }//Koniec MetodaRKDP54   


    /// <summary>
    /// Klasa zawierająca implementację metody 
    /// par włożonych 4 i 5 rzędu Fehlberga 6 etapowa
    /// </summary>
    public abstract class MetodaRKF45Abstract : RownanieRozniczkoweTypRKParyWlozoneAbstract
    {

        /// <summary>
        /// Konstruktor klasy z parametrami
        /// </summary>
        /// <param name="N"> liczba równań</param>
        /// <param name="Tp"> czas początkowy obliczeń</param>
        /// <param name="Tk"> czas końcowy obliczeń</param>
        /// <param name="X0"> wektor zawierający warunek początkowy</param>
        public MetodaRKF45Abstract(int N, double Tp1, double Tk1, double[] X0)
            : base(N, Tp1, Tk1, X0)
        { }
        public MetodaRKF45Abstract()
        { }
        /// <summary>
        /// Metoda par włożonych 4 i 5 rzędu Fehlberga 6 etapowa
        /// </summary>
        public override void TabelaButchera()
        {
            a = new double[7, 7];
            c = new double[7]; 
            w1 = new double[7];
            w2 = new double[7];
            e = new double[7];
            c[1] = 0; a[1, 1] = 0;
            c[2] = 0.25; a[2, 1] = 1.0 / 4.0;
            c[3] = 3.0 / 8.0; a[3, 1] = 3.0 / 32.0; a[3, 2] = 9.0 / 32.0;
            c[4] = 12.0 / 13.0; a[4, 1] = 1932.0 / 2197.0; a[4, 2] = -7200.0 / 2197.0;
            a[4, 3] = 7296.0 / 2197.0;
            c[5] = 1.0; a[5, 1] = 439.0 / 216.0; a[5, 2] = -8.0;
            a[5, 3] = 3680.0 / 513.0; a[5, 4] = -845.0 / 4104.0;
            c[6] = 0.5; a[6, 1] = -8.0 / 27.0; a[6, 2] = 2.0;
            a[6, 3] = -3544.0 / 2565.0; a[6, 4] = 1859.0 / 4104.0; a[6, 5] = -11.0 / 40.0;
            w2[1] = 16.0 / 135.0; w2[2] = 0; w2[3] = 6656.0 / 12825.0;
            w2[4] = 28561.0 / 56430.0; w2[5] = -9.0 / 50.0; w2[6] = 2.0 / 55.0;
            w1[1] = 25.0 / 216.0; w1[2] = 0; w1[3] = 1408.0 / 2565.0;
            w1[4] = 2197.0 / 4104.0; w1[5] = -1.0 / 5.0; w1[6] = 0;
            for (int i = 1; i <= 6; i++) e[i] = w2[i] - w1[i];
            FNetap = 6;
            FRzad = 5;
            KX = new double[7, FN + 1];
        }

    }//Koniec MetodaRKF45Abstract

    /// <summary>
    /// Klasa potomna MetodaRKF45 klasy MetodaRKF45Abstract do rozwiązywania 
    /// układów równań różniczkowych metodą Fehlberga 4 I 5 rzędu
    /// </summary>
    public class MetodaRKF45 : MetodaRKF45Abstract
    {
        /// <summary>
        /// Pole typu delegate FunNielinRowRozniczkoweDelegate wskazujące na funkcję o takich samych
        /// parametrach co abstrakcyjna funkcja FunkcjeNielinioweRownaniaRozniczkowego klasy
        /// RownanieRozniczkoweTypRKParyWlozoneAbstract definijąca prawą stronę postaci normalnej 
        /// ukladu równań różniczkowych
        /// </summary>
        private FunNielinRowRozniczkoweDelegate Rownanie;

        /// <summary>
        /// Implementacja funkcji wektorowej wyznaczającej prawą stronę ukladu równań różniczkowych
        /// tj. obliczających wektor pochodnych wektora stanu w oparciu o przesłanego delegata do klasy
        /// </summary>
        /// <param name="X">Wektor stanu dla chwili czasu t</param>
        /// <param name="t">chwila czasu wektora stanu</param>
        /// <returns></returns>
        public override double[] FunkcjeNielinioweRownaniaRozniczkowego(double[] X, double t)
        {
            return Rownanie(X, t);
        }

        public MetodaRKF45()
            : base()
        { }

        /// <summary>
        /// Konstruktor klasy MetodaRKDP54 podający adres funkcji wektorowej 
        /// układu równań różniczkowych przez podstawieni Rownanie = Row 
        /// </summary>
        /// <param name="Row">egzemplarz delegata FunNielinRowRozniczkoweDelegate 
        /// do zadawania prawej strony równania różniczkowego w postaci normalnej</param>
        /// <param name="N"> ilość równań</param>
        /// <param name="Tp1">chwila początkowa obliczeń</param>
        /// <param name="Tk1">chwila końcowa obliczeń</param>
        /// <param name="X0">wektor zawierający warunek początkowy</param>
        public MetodaRKF45(FunNielinRowRozniczkoweDelegate Row, int N, double Tp1, double Tk1, double[] X0)
            : base(N, Tp1, Tk1, X0)
        { Rownanie = Row; }

    }//Koniec MetodaRKF45   




}
