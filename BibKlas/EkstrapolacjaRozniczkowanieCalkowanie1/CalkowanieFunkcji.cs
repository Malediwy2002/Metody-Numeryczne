using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibKlas.EkstrapolacjaRozniczkowanieCalkowanie1
{
    
    /// <summary>
    /// Klasa abstrakcyjna do całkowania numerycznego funkcji rzeczywistej
    /// </summary>
    public abstract class CalkowanieFunkcjiAbstract : EkstrapolacjaAbstr
    {
        protected double a; //Punkt początkowy całkowania
        protected double b; //Punkt końcowy całkowania
        protected double h; //Krok całkowania wstępnie ustalony przez konstruktora
        protected int N; //Liczba kroków całkowania
        protected int M; //Liczba par kroków całkowania w metodzie Simpsona

        protected int TypWzoruCalkowego; //TypWzoruCalkowego przyjmuje wartość :
        //1 - dla metody trapezów
        //2 - dla metody Simpsona
        //3 - dla metody prostokątów

        // Funkcja poddawana operacji całkowania
        public abstract double FunRealReal(double x);

        /// <summary>
        /// Wzory całkowania numerycznego tj. wzory prostokątów, 
        /// trapezów i wzory Simpsona
        /// </summary>
        /// <param name="h">Krok całkowania</param>
        /// <returns>Zwracana wartość jako suma 
        /// przybliżająca całkę oznaczoną </returns>
        public override double EkstapolacjaProcesu(double h)
        {
            int i;
            //Parametry pomocnicze jak zmienne lokalne
            double Suma, Suma1, Suma2, xi, xip, xin, h2;
            switch (TypWzoruCalkowego)
            {
                case 1:   //Wzór trapezów  (3.24)
                    {
                        Suma = 0.5 * FunRealReal(a) + 0.5 * FunRealReal(b);
                        N = (int)Math.Round((b - a) / h); h = (b - a) / N;
                        for (i = 1; i <= N - 1; i++)
                        {   xi = a + i * h;
                            Suma += FunRealReal(xi);
                        }
                        return Suma * h;
                    }
                case 2:  //Wzór Simpsona (3.30)
                    {
                        N = (int)Math.Round((b - a) / h);
                        if (N % 2 != 0) N++;
                        M = (int)Math.Round((float)N / 2.0);
                        h = (b - a) / N;
                        Suma1 = 0; Suma2 = 0;
                        for (i = 1; i <= M - 1; i++)
                        { xip = a + 2 * i * h; Suma2 += FunRealReal(xip); };
                        for (i = 0; i <= M - 1; i++)
                        { xin = a + (2 * i + 1) * h; Suma1 += FunRealReal(xin); };
                        return h * (FunRealReal(a) + FunRealReal(b) + 4.0 * Suma1 + 2.0 * Suma2) / 3.0;
                    }
                case 3: //Wzór prostokątów  (3.16)
                    {
                        Suma = 0;
                        N = (int)Math.Round((b - a) / h);
                        h = (b - a) / N; 
                        h2 = h / 2;
                        for (i = 1; i <= N; i++)
                        {
                            xi = a + i * h - h2;
                            Suma += FunRealReal(xi);
                        }
                        return Suma * h;
                    }
                default: return 0;
            }

        }

        /// <summary>
        /// Konstruktor który nic nie robi
        /// </summary>
        public CalkowanieFunkcjiAbstract()
            : base()
        { }

        /// <summary>
        /// Konstruktor klasy przekazujący parametry
        /// </summary>
        /// <param name="a">punkt początkowy całkowania</param>
        /// <param name="b">punkt końcowy całkowania</param>
        /// <param name="h0">krok całkowania wstępnie ustalony</param>
        /// <param name="q">iloraz próbnych kroków całkowania ,q>1</param>
        /// <param name="eps">dokładność iteracji</param>
        /// <param name="maxit">maksymalna liczba iteracji</param>
        public CalkowanieFunkcjiAbstract(double a, double b, double h0,
                                   double q, double eps, int maxit)
            : base(h0, q, eps, maxit)
        {
            this.a = a; this.b = b;
            TypWzoruCalkowego = 0;
        }

        /// <summary>
        /// Funkcja implementująca wzór prostokątów (3.24) 
        ///oraz ekstrapolacji Aitkena (3.12)
        /// </summary>
        /// <returns>Zwracana wartość jako całka oznaczona</returns>
        public double MetodaAitkenaDlaProstokatow()
        {
            int blad;
            q = 2; TypWzoruCalkowego = 3;
            blad = EkstrapolacjaAitkena();
            if (blad == 0) return F0;
            else
            {
                PiszKomunikat(blad);
                return F0;
            }
        }

        /// <summary>
        /// Funkcja implementująca wzór prostokątów (3.24) 
        /// oraz ekstrapolacji Aitkena (3.12)
        /// </summary>
        /// <returns>Zwracana wartość jako całka oznaczona</returns>
        public double MetodaRichardsonaDlaProstokatow()
        {
            int blad;
            q = 2; TypWzoruCalkowego = 3;
            blad = EkstrapolacjaRichardsonaPotParzyste(2);
            if (blad == 0) return F0;
            else
            {
                PiszKomunikat(blad);
                return F0;
            }
        }

        /// <summary>
        /// Funkcja implementująca wzór trapezów (3.24) 
        /// oraz ekstrapolacji Aitkena (3.12)
        /// </summary>
        /// <returns>Zwracana wartość jako całka oznaczona</returns>
        public double MetodaAitkenaDlaTrapezow()
        {
            int blad;
            q = 2; TypWzoruCalkowego = 1;
            blad = EkstrapolacjaAitkena();
            if (blad == 0) return F0;
            else
            {
                PiszKomunikat(blad);
                return F0;
            }
        }

        /// <summary>
        /// Funkcja implementująca wzór trapezów oraz 
        /// ekstrapolacji  Richardsona (3.12) Romberga
        /// </summary>
        /// <returns>Zwracana wartość jako całka oznaczona</returns>
        public double MetodaRichardsonaDlaTrapezow()
        {
            int blad;
            q = 2; TypWzoruCalkowego = 1;
            blad = EkstrapolacjaRichardsonaPotParzyste(2);
            if (blad == 0) return F0;
            else
            {
                PiszKomunikat(blad);
                return F0;
            }
        }

        /// <summary>
        /// Funkcja implementująca wzór Simsona (3.30) 
        /// oraz ekstrapolację Aitkena (3.12)
        /// </summary>
        /// <returns>Zwracana wartość jako całka oznaczona</returns>
        public double MetodaAitkenaSimpsona()
        {
            int blad;
            q = 2; TypWzoruCalkowego = 2;
            blad = EkstrapolacjaAitkena();
            if (blad == 0) return F0;
            else
            {
                PiszKomunikat(blad);
                return F0;
            }
        }

        /// <summary>
        /// Funkcja implementująca wzór Simsona (3.30) 
        /// oraz ekstrapolację Richardsona
        /// </summary>
        /// <returns>Zwracana wartość jako całka oznaczona</returns>
        public double MetodaRichardsonaDlaSimpsona()
        {
            int blad;
            q = 2; TypWzoruCalkowego = 2;
            blad = EkstrapolacjaRichardsonaPotParzyste(2);
            if (blad == 0) return F0;
            else
            {
                PiszKomunikat(blad);
                return F0;
            }
        }

    }//Koniec CalkowanieFunkcjiAbstract

    /// <summary>
    /// Klasa pochodna względem klasy  CalkowanieFunkcjiAbstract
    /// do obliczenia całki z funkcji rzeczywistej przekazanej 
    /// za pośrednictwem konstruktora
    /// </summary>
    public class CalkowanieFunkcji : CalkowanieFunkcjiAbstract
    {
        /// <summary>
        /// Egzemplarz FunReRe delegata jako pole klasy
        /// </summary>
        public FunkcjaRealeReale FunReRe;

        /// <summary>
        /// Konstruktor klasy przekazujący parametry
        /// </summary>
        /// <param name="FRR">Egzemplarz funkcji do całkowania</param>
        /// <param name="a">Punkt początkowy całkowania</param>
        /// <param name="b">Punkt końcowy całkowania</param>
        /// <param name="h0">Krok całkowania wstępnie ustalony</param>
        /// <param name="q">Iloraz próbnych kroków całkowania ,q>1</param>
        /// <param name="eps">Dokładność iteracji w ekstrapolacji</param>
        /// <param name="maxit">Maksymalna liczba iteracji po której kończymy
        /// obliczenia nie osiągnąwszy zadanej dokładności</param>
        public CalkowanieFunkcji(FunkcjaRealeReale FRR, 
            double a, double b, double h0, double q, double eps, int maxit)
            : base(a, b, h0, q, eps, maxit)
        {
            FunReRe = FRR;
            //Przekazanie egzemplarza delegata FRR do klasy pochodnej 
            //CalkowanieFunkcjiAbstract po adres FunReRe
        }

        /// <summary>
        /// Predefiniowanie funkcji zgodnie definicją w klasie 
        /// bazowej CalkowanieFunkcjiAbstract
        /// </summary>
        /// <param name="X">Argument funkcji rzeczywistej X</param>
        /// <returns>Zwraca wartość funkcji </returns>
        public override double FunRealReal(double x)
        {
            return FunReRe(x);
            //Zwraca egzemplarz delegata przekazany prze konstruktora klasy

        }
    }//Koniec CalkowanieFunkcji

}//Koniec namespace BibKlas.EkstrapolacjaRozniczkowanieCalkowanie1
