using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BibKlas.EkstrapolacjaRozniczkowanieCalkowanie1
{

    /// <summary>
    /// Klasa abstrakcyjna do realizacji różniczkowania numerycznego
    /// </summary>
    public abstract class RozniczkowanieFunkcjiAbstract : EkstrapolacjaAbstr
    {
        protected double X0;       //Współrzędne obliczenia pochodnej
        protected int TypPochodnej;//Wybór typu pochodnej

        /// <summary>
        /// Wybór procesu ekstrapolacji do obliczenia pochodnej 
        /// funkcji rzeczywistej zmiennej rzeczywistej
        /// </summary>
        protected void EkstrapolacjaRozniczkowa()
        {
            int blad;
            q = 2;
            blad = EkstrapolacjaAitkena();
            //blad =EkstrapolacjaRichardsonaPotParzyste(2);
            if (blad != 0)
            {  PiszKomunikat(blad); }
        }

        
        /// <summary>
        ///  Funkcja poddawana operacji różniczkowania
        /// </summary>
        /// <param name="x">Argument funkcji</param>
        /// <returns>Zwracana wartość</returns>
        public abstract double FunRealReal(double x);


        /// <summary>
        /// Definicja ekstrapolacji różniczkowania przy pomocy
        /// funkcji abstrakcyjnej FunRealReal, która implementuje 
        /// wzory różniczkowania. Słowo kluczowe override pokazuje, 
        /// że metoda ta predefiniowana została w klasie abstrakcyjnej 
        /// EkstrapolacjaAbstr do wykonania obliczeń ekstrapolacji
        /// Richardsona i Aitkiena
        /// </summary>
        /// <param name="h">Krok różniczkowania</param>
        /// <returns>Zwracana wartość ilorazu różniczkowego</returns>
        public override double EkstapolacjaProcesu(double h)
        {
            double f1, f2, f3, f4, f5, h2;//Zmienne pomocnicze
            switch (TypPochodnej)
            {
                case 1: //rzędu drugiego - pierwsza pochodna wzór (3.36)
                    {
                        f1 = FunRealReal(X0 + h); f2 = FunRealReal(X0 - h);
                        return 0.5 * (f1 - f2) / h;
                    }
                case 2: //rzędu drugiego - druga pochodna   wzór (3.40)
                    {
                        f1 = FunRealReal(X0 + h); f2 = FunRealReal(X0 - h); f3 = FunRealReal(X0);
                        return (f1 - 2 * f3 + f2) / (h * h);
                    }
                case 3: //rzędu czwartego - pierwsza pochodna   wzór (3.44)
                    {
                        h2 = 2 * h;
                        f1 = FunRealReal(X0 + h2); f2 = FunRealReal(X0 + h);
                        f3 = FunRealReal(X0 - h); f4 = FunRealReal(X0 - h2);
                        return (8 * f2 - 8 * f3 - f1 + f4) / (12 * h);
                    }
                case 4://rzędu czwartego - druga pochodna      wzór (3.45)
                    {
                        h2 = 2 * h;
                        f1 = FunRealReal(X0 + h2); f2 = FunRealReal(X0 + h); f3 = FunRealReal(X0 - h);
                        f4 = FunRealReal(X0 - h2); f5 = FunRealReal(X0);
                        return (-f1 + 16 * f2 - 30 * f5 + 16 * f3 - f4) / (12 * h * h);
                    }
               
                default: return 0;
            }
        }

        /// <summary>
        /// Konstruktor który nic nie robi
        /// </summary>
        public RozniczkowanieFunkcjiAbstract() : base() { }

        /// <summary>
        /// Konstruktor przekazujący parametry do obliczeń
        /// </summary>
        /// <param name="x0">Punkt, w którym obliczamy pochodną</param>
        /// <param name="h0">Wstępnie ustalony krok różniczkowania</param>
        /// <param name="q">iloraz próbnych kroków różniczkowania ; q>1</param>
        /// <param name="eps"> Dokładność iteracji różniczkowania</param>
        /// <param name="maxit">Maksymalna liczba iteracji po której kończymy
        /// obliczenia nie osiągnąwszy zadanej dokładności</param>
        public RozniczkowanieFunkcjiAbstract(double x0, 
            double h0, double q, double eps, int maxit) :
            base(h0, q, eps, maxit)
        {
            X0 = x0;
            TypPochodnej = 0;
        }

        /// <summary>
        /// Pierwsza pochodna dla błędu obcięcia rzędu 2 ,wzór (3.36)
        /// </summary>
        /// <returns>Zwraca pochodną</returns>
        public double PierwszPochodnaR2()
        {
            TypPochodnej = 1;
            EkstrapolacjaRozniczkowa();
            //F0- wynik ekstrapolacji jako wynik pochodnej 
            //    funkcji w wybranym punkcie
            //F0 - pole klasy bazowej EkstrapolacjaAbstr
            return F0;
        }
    
        /// <summary>
        /// Druga pochodna dla błędu obcięcia rzędu 2 ,wzór (3.40)
        /// </summary>
        /// <returns>Zwraca druga pochodna pochodną</returns>
        public double DrugaPochodnaR2()
        {
            TypPochodnej = 2;
            EkstrapolacjaRozniczkowa();
            //F0- wynik ekstrapolacji jako wynik drugiej pochodnej 
            //    funkcji w wybranym punkcie
            //F0 - pole klasy bazowej EkstrapolacjaAbstr
            return F0;
        }

        /// <summary>
        /// Pierwsza pochodna dla błędu obcięcia rzędu 4 ,wzór (3.44)
        /// </summary>
        /// <returns>Zwraca pochodną</returns>
        public double PierwszPochodnaR4()
        {
            TypPochodnej = 3;
            EkstrapolacjaRozniczkowa();
            //F0- wynik ekstrapolacji jako wynik pochodnej 
            //    funkcji w wybranym punkcie
            //F0 - pole klasy bazowej EkstrapolacjaAbstr
            return F0;
        }

        /// <summary>
        /// Druga pochodna dla błędu obcięcia rzędu 4 ,wzór (3.44)
        /// </summary>
        /// <returns>Zwraca drugą pochodną</returns>
        public double DrugaPochodnaR4()
        {
            TypPochodnej = 4;
            EkstrapolacjaRozniczkowa();
            //F0- wynik ekstrapolacji jako wynik pochodnej 
            //    funkcji w wybranym punkcie
            //F0 - pole klasy bazowej EkstrapolacjaAbstr
            return F0;
        }

    }//Koniec public abstract class RozniczkowanieFunkcjiAbstract

  
    
    /// <summary>
    /// Klasa stosowana do różniczkowana funkcji przekazanej
    /// za pośrednictwem konstruktora
    /// </summary>
    public class RozniczkowanieFunkcji : RozniczkowanieFunkcjiAbstract
    {
        /// <summary>
        /// Egzemplarz FunReRe delegata jako pole klasy
        /// </summary>
        public FunkcjaRealeReale FunReRe;

        /// <summary>
        /// Konstruktor klasy
        /// </summary>
        /// <param name="FRR">Przekazany egzemplarz funkcji rzeczywistej</param>
        /// <param name="x0">Punkt, w którym obliczamy pochodną</param>
        /// <param name="h0">Wstępnie ustalony krok różniczkowania</param>
        /// <param name="q">Iloraz próbnych kroków różniczkowania ; q>1</param>
        /// <param name="eps"> Dokładność iteracji różniczkowania</param>
        /// <param name="maxit">Maksymalna liczba iteracji po której kończymy
        /// obliczenia nie osiągnąwszy zadanej dokładności</param>
        public RozniczkowanieFunkcji(FunkcjaRealeReale FRR,
            double x0, double h0, double q, double eps, int maxit)
            : base(x0, h0, q, eps, maxit)
        {
            FunReRe = FRR;
            //Przekazanie egzemplarza delegata FRR do klasy pochodnej 
            //RozniczkowanieFunkcjiAbstract po adres FunReRe
        }

        /// <summary>
        /// Predefiniowanie funkcji zgodnie definicją w klasie 
        /// bazowej RozniczkowanieFunkcjiAbstract
        /// </summary>
        /// <param name="X">Argument funkcji rzeczywistej X</param>
        /// <returns>Zwraca wartość funkcji </returns>
        public override double FunRealReal(double x)
        {
            return FunReRe(x);
            //Zwraca egzemplarz delegata przekazany prze konstruktora klasy
        }

    }//Koniec  public class RozniczkowanieFunkcji

}//Koniec namespace BibKlas.EkstrapolacjaRozniczkowanieCalkowanie1
