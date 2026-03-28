using System;

namespace BugetApp.Models
{
    public enum TipTranzactie
    {
        Venit,
        Cheltuiala
    }

    [Flags]
    public enum OptiuniTranzactie
    {
        None = 0,
        Urgent = 1,
        Personal = 2,
        Recurent = 4,
        Esential = 8
    }

    public class Tranzactie
    {
        public double Suma { get; set; }
        public TipTranzactie Tip { get; set; } 
        public OptiuniTranzactie Optiuni { get; set; } 
        public DateTime Data { get; set; }
        public string Descriere { get; set; }

        public Tranzactie(double suma, TipTranzactie tip, OptiuniTranzactie optiuni, DateTime data, string descriere)
        {
            Suma = suma;
            Tip = tip;
            Optiuni = optiuni;
            Data = data;
            Descriere = descriere;
        }

        public void Afisare()
        {
           
            Console.WriteLine($"{Data.ToShortDateString()} | {Tip} | {Optiuni} | {Suma} RON | {Descriere}");
        }
    }
}