using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

    public class Tranzactie : INotifyPropertyChanged
    {
        private Guid _id;
        private double _suma;
        private TipTranzactie _tip;
        private OptiuniTranzactie _optiuni;
        private DateTime _data;
        private string _descriere;
        private string _categorie;
        private string _metodaPlata;

        public Guid Id
        {
            get => _id;
            private set { _id = value; OnPropertyChanged(); }
        }
        
        public double Suma
        {
            get => _suma;
            set { _suma = value; OnPropertyChanged(); }
        }
        
        public TipTranzactie Tip
        {
            get => _tip;
            set { _tip = value; OnPropertyChanged(); }
        }
        
        public OptiuniTranzactie Optiuni
        {
            get => _optiuni;
            set { _optiuni = value; OnPropertyChanged(); }
        }
        
        public DateTime Data
        {
            get => _data;
            set { _data = value; OnPropertyChanged(); }
        }
        
        public string Descriere
        {
            get => _descriere;
            set { _descriere = value; OnPropertyChanged(); }
        }
        
        public string Categorie
        {
            get => _categorie;
            set { _categorie = value; OnPropertyChanged(); }
        }
        
        public string MetodaPlata
        {
            get => _metodaPlata;
            set { _metodaPlata = value; OnPropertyChanged(); }
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}