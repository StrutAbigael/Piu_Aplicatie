using System;
using System.Collections.Generic;
using System.IO;
using BugetApp.Models;
using System.Linq;

namespace BugetApp.Persistence
{
    public class FileService
    {
        private string _caleFisier;

        public FileService(string numeFisier = "date_buget.txt")
        {
            _caleFisier = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, numeFisier);
        }

        // Salvare listă în fișier (format CSV simplu)
        public void SalveazaTranzactii(List<Tranzactie> tranzactii)
        {
            List<string> linii = new List<string>();
            foreach (var t in tranzactii)
            {
                // Salvăm datele separate prin virgulă sau punct și virgulă
                string linie = $"{t.Suma};{t.Tip};{(int)t.Optiuni};{t.Data};{t.Descriere}";
                linii.Add(linie);
            }
            File.WriteAllLines(_caleFisier, linii);
        }

        // Încărcare listă din fișier
        public List<Tranzactie> IncarcaTranzactii()
        {
            List<Tranzactie> lista = new List<Tranzactie>();

            if (!File.Exists(_caleFisier)) return lista;

            string[] linii = File.ReadAllLines(_caleFisier);
            foreach (string linie in linii)
            {
                string[] parti = linie.Split(';');
                if (parti.Length == 5)
                {
                    double suma = double.Parse(parti[0]);
                    TipTranzactie tip = (TipTranzactie)Enum.Parse(typeof(TipTranzactie), parti[1]);
                    OptiuniTranzactie opt = (OptiuniTranzactie)int.Parse(parti[2]);
                    DateTime data = DateTime.Parse(parti[3]);
                    string desc = parti[4];

                    lista.Add(new Tranzactie(suma, tip, opt, data, desc));
                }
            }
            return lista;
        }
    }
}