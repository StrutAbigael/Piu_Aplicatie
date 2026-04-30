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
        public Guid Id { get; private set; }
        public double Suma { get; set; }
        public TipTranzactie Tip { get; set; } 
        public OptiuniTranzactie Optiuni { get; set; } 
        public DateTime Data { get; set; }
        public string Descriere { get; set; }
        public string Categorie { get; set; }
        public string MetodaPlata { get; set; }

        public Tranzactie(double suma, TipTranzactie tip, OptiuniTranzactie optiuni, DateTime data, string descriere, string categorie = "Altele", string metodaPlata = "Cash")
        {
            Id = Guid.NewGuid();
            Suma = suma;
            Tip = tip;
            Optiuni = optiuni;
            Data = data;
            Descriere = descriere;
            Categorie = categorie;
            MetodaPlata = metodaPlata;
        }

        public void Afisare()
        {
           
            Console.WriteLine($"{Data.ToShortDateString()} | {Tip} | {Optiuni} | {Suma} RON | {Descriere}");
        }
    }
}