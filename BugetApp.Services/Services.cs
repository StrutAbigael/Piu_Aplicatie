using BugetApp.Models; 
using System;
using System.Collections.Generic;
using System.Linq;

namespace BugetApp.Services
{
    public class Buget
    {
        private List<Tranzactie> tranzactii = new List<Tranzactie>();

        public void AdaugaTranzactie(Tranzactie t) => tranzactii.Add(t);

        public List<Tranzactie> GetAll()
        {
            return tranzactii;
        }
        public void SetAll(List<Tranzactie> listaNoua)
        {
            this.tranzactii = listaNoua;
        }
        public void Modifica(int index, Tranzactie noua)
        {
            if (index >= 0 && index < tranzactii.Count)
            {
                tranzactii[index] = noua;
            }
        }
        public void Sterge(int index)
        {
            if (index >= 0 && index < tranzactii.Count)
                tranzactii.RemoveAt(index);
        }
        
        public void AfisareToate()
        {
            foreach (var t in tranzactii) t.Afisare();
        }

        
        public double TotalVenituri() =>
            tranzactii.Where(t => t.Tip == TipTranzactie.Venit).Sum(t => t.Suma);

        public double TotalCheltuieli() =>
            tranzactii.Where(t => t.Tip == TipTranzactie.Cheltuiala).Sum(t => t.Suma);
        
        public double Sold()
        {
            return TotalVenituri() - TotalCheltuieli();
        }

        public void CautaDupaTip(TipTranzactie tipCautat)
        {
            Console.WriteLine($"--- Rezultate pentru tipul: {tipCautat} ---");
            bool gasit = false;
            foreach (var t in tranzactii)
            {
                if (t.Tip == tipCautat)
                {
                    t.Afisare();
                    gasit = true;
                }
            }
            if (!gasit) Console.WriteLine("Nu s-au găsit tranzacții de acest tip.");
        }
    }
}