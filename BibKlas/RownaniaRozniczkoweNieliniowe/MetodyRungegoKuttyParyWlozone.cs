using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibKlas.RownaniaRozniczkoweNieliniowe
{
    /// <summary>
    /// Klasa pochodna względem klasy bazowej RownaniaRozniczkoweBaza
    /// zawierająca metody typu par włożonych Rungego-Kutty 
    /// implementujące rozwiązywanie ukladów równań różniczkowych
    /// </summary>
    public abstract  class MetodyRungegoKuttyParyWlozoneApstrakt :
        RownaniaRozniczkoweBaza //Metody jawne
    {
        public MetodyRungegoKuttyParyWlozoneApstrakt() : base()
        { }

        /// <summary>
        /// Konstruktor klasy z parametrami
        /// </summary>
        /// <param name="N"> liczba równań</param>
        /// <param name="Tp"> czas początkowy obliczeń</param>
        /// <param name="Tk"> czas końcowy obliczeń</param>
        /// <param name="X0"> wektor zawierający warunek początkowy</param>
        public MetodyRungegoKuttyParyWlozoneApstrakt(int N, double Tp1, double Tk1, double[] X0)
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
                X1 = (double[])X0.Clone();
                for (j = 1; j <= i - 1; j++)
                    for (k = 1; k <= FN; k++) X1[k] += KX[j, k] * a[i, j];
                KK = FunkcjeNielinioweRownaniaRozniczkowego(X1, t);
                //KX[i] = KK * h;
                for (j = 1; j <= FN; j++) KX[i, j] = KK[j] * h;
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
        /// Metoda abstrakcyjna, która implementowana jest w klasach 
        /// pochodnych bo musi przekazać tablicę Butchera w więc elementy 
        /// a[i, j] ,w1[i], w2[i], c[i]  dla algorytmów typu Runge-Kutta
        /// par włożonych z podaniem rzędu FRzad i ilości etapów FNetap metody
        /// </summary>
        public abstract void TabelaButchera();

        /// <summary>
        /// Implementacja metody typu Runge-Kutta par włożonych
        /// zadanej w postaci tablicy 8 Butchera
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
            TabelaButchera();
            KKX = 0;
            MaxMaxE = 0;
            //Pobór warunku początkowego 
            Y = (double[])FMS[FLW].Clone();
            t = Y[0];
            if (ZapiszBlad) maxER = new List<double>(0);
            //Wstępne ustalenie kroku całkowania H
            if (FStKr) H = FH0;
            else H = (FTk - t) / 1000.0;
            do
            {
                //Wyznaczanie normy wektora błędu Er oraz wektora XX1 wzór 
                MetodaRungeKuttaPairs(FNetap, H, t, Y, XX1, Er);
                ee = 0; mxt = 0;
                for (j = 1; j <= FN; j++)
                {
                    e = Math.Abs(Er[j]);
                    if (ee < e) ee = e;
                    xt = 0.5 * (Math.Abs(Y[j]) + Math.Abs(XX1[j]));
                    if (mxt < xt) mxt = xt;
                }
                maxE = ee;
                if (maxE > MaxMaxE) MaxMaxE = maxE;
                //Badanie konieczności zmiany kroku całkowania wg. wzoru (29)
                ee /= mxt * FEpsW + FEpsA;
                if (ee == 0) Hph = 0.9;
                else Hph = Math.Exp(Math.Log(ee) / (FRzad + 1));
                h1 = H / Hph;
                alfa = 0.9 / Hph;
                Hmin = FA26 * Math.Abs(t);
                if (Math.Abs(h1) < Hmin && !FStKr)
                    throw new Exception(
                        "Krok całkowania przekroczył dopuszczalną wartość Hmin="
                        + Hmin.ToString() + "w metodzie Obliczaj()"); //Blad = 3;
                else
                {
                    //Warunek akceptujący zapis rozwiązania do tablicy dynamicznej FMS
                    if (Hph <= 1 || FStKr)
                    {
                        Y = (double[])XX1.Clone();
                        t += H;
                        Y[0] = t; XX1[0] = t;
                        //Zapis wektora stanu Y do tablicy dynamicznej
                        Dopisz(Y);
                        if (ZapiszBlad) { DopiszMaxE(maxE); }
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
            return Blad;
        }

    }//Koniec public abstract  class MetodyRungegoKuttyParyWlozoneApstrakt 

    /// <summary>
    /// Klasa zawierająca implementację metody 
    /// par włożonych 4 i 5 rzędu Fehlberga 6 etapowa
    /// </summary>
    public abstract class MetodaCalkowaniaRKF45Abstrakt :
        MetodyRungegoKuttyParyWlozoneApstrakt
    {

        /// <summary>
        /// Konstruktor klasy z parametrami
        /// </summary>
        /// <param name="N"> liczba równań</param>
        /// <param name="Tp"> czas początkowy obliczeń</param>
        /// <param name="Tk"> czas końcowy obliczeń</param>
        /// <param name="X0"> wektor zawierający warunek początkowy</param>
        public MetodaCalkowaniaRKF45Abstrakt(int N, double Tp1, double Tk1, double[] X0)
            : base(N, Tp1, Tk1, X0)
        { }
        public MetodaCalkowaniaRKF45Abstrakt()
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

    }//Koniec  MetodaCalkowaniaRKF45Abstrakt

    /// <summary>
    /// Klasa potomna MetodaRKF45 klasy MetodaRKF45Abstract do rozwiązywania 
    /// układów równań różniczkowych metodą Fehlberga 4 I 5 rzędu
    /// </summary>
    public class MetodaCalkowaniaRKF45 : MetodaCalkowaniaRKF45Abstrakt
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

        public MetodaCalkowaniaRKF45()
            : base()
        { }

        /// <summary>
        /// Konstruktor klasy MetodaRKF45 podający adres funkcji wektorowej 
        /// układu równań różniczkowych przez podstawieni Rownanie = Row 
        /// </summary>
        /// <param name="Row">egzemplarz delegata FunNielinRowRozniczkoweDelegate 
        /// do zadawania prawej strony równania różniczkowego w postaci normalnej</param>
        /// <param name="N"> ilość równań</param>
        /// <param name="Tp1">chwila początkowa obliczeń</param>
        /// <param name="Tk1">chwila końcowa obliczeń</param>
        /// <param name="X0">wektor zawierający warunek początkowy</param>
        public MetodaCalkowaniaRKF45(FunNielinRowRozniczkoweDelegate Row,
              int N, double Tp1, double Tk1, double[] X0)
            : base(N, Tp1, Tk1, X0)
        { Rownanie = Row; }

    }//Koniec MetodaCalkowaniaRKF45  

    /// <summary>
    /// Klasa zawierająca implementację metody 
    /// par włożonych 5 i 4 rzędu Dormanda Prince'a 7 etapowa Tabela 13
    /// </summary>
    public abstract class MetodaCalkowaniaRKDP54Abstract :
        MetodyRungegoKuttyParyWlozoneApstrakt
    {
        /// <summary>
        /// Konstruktor klasy z parametrami
        /// </summary>
        /// <param name="N"> liczba równań</param>
        /// <param name="Tp"> czas początkowy obliczeń</param>
        /// <param name="Tk"> czas końcowy obliczeń</param>
        /// <param name="X0"> wektor zawierający warunek początkowy</param>
        public MetodaCalkowaniaRKDP54Abstract(int N, double Tp1, 
                                        double Tk1, double[] X0)
            : base(N, Tp1, Tk1, X0)
        { }
        public MetodaCalkowaniaRKDP54Abstract()
        { }

        /// <summary>
        /// Metoda par włożonych 5 i 4 rzędu Dormanda Prince'a 7 etapowa
        /// </summary>
        public override void TabelaButchera()
        {
            a = new double[8,8];
            c = new double[8];
            w1 = new double[8];
            w2 = new double[8];
            e = new double[8];
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
            w1[1] = 35.0 / 384.0; w1[2] = 0; w1[3] = 500.0 / 1113.0;
            w1[4] = 125.0 / 192.0; w1[5] = -2187.0 / 6784.0; w1[6] = 11.0 / 84.0;
            w1[7] = 0; 
            w2[1] = 5179.0 / 57600.0; w2[2] = 0; w2[3] = 7571.0 / 16695.0;
            w2[4] = 393.0 / 640.0; w2[5] = -92097.0 / 339200.0; w2[6] = 187.0 / 2100.0;
            w2[7] = 1.0 / 40.0; 
            e[1] = -35.0 / 384.0 + 5179.0 / 57600.0; e[2] = 0;
            e[3] = -500.0 / 1113.0 + 7571.0 / 16695.0;
            e[4] = -125.0 / 192.0 + 393.0 / 640.0; e[5] = +2187.0 / 6784.0 - 92097.0 / 339200.0;
            e[6] = -11.0 / 84.0 + 187.0 / 2100.0; e[7] = 1.0 / 40.0; //e[8] = 0; e[9] = 0;
            FNetap = 7;
            FRzad = 5;
            KX = new double[8, FN + 1];
        }
    }//Koniec MetodaCalkowaniaRKDP54Abstract

    /// <summary>
    /// Klasa potomna MetodaRKDP54 klasy MetodaRKDP54Abstract do rozwiązywania 
    /// układów równań różniczkowych metodą Dormand-Prince 5 i 4 rzędu
    /// </summary>
    public class MetodaCalkowaniaRKDP54 : MetodaCalkowaniaRKDP54Abstract
    {
        /// <summary>
        /// Pole typu delegate FunNielinRowRozniczkoweDelegate wskazujące na funkcję o takich samych
        /// parametrach co abstrakcyjna funkcja FunkcjeNielinioweRownaniaRozniczkowego klasy
        /// RownanieRozniczkoweTypRKParyWlozoneAbstract definijąca prawą stronę postaci normalnej 
        /// układu równań różniczkowych
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

        public MetodaCalkowaniaRKDP54()
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
        public MetodaCalkowaniaRKDP54(FunNielinRowRozniczkoweDelegate Row,
            int N, double Tp1, double Tk1, double[] X0)
            : base(N, Tp1, Tk1, X0)
        { Rownanie = Row; }

    }//Koniec MetodaCalkowaniaRKDP54   


}//namespace BibKlas.RownaniaRozniczkoweNieliniowe
