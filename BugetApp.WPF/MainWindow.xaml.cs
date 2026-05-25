using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using BugetApp.Models;
using BugetApp.Persistence;

namespace BugetApp.WPF
{
    public class DailyChartData
    {
        public string DayName { get; set; } = string.Empty;
        public double IncomeHeight { get; set; }
        public double ExpenseHeight { get; set; }
    }

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private const double MIN_SUMA = 1.0;
        private const int MIN_LUNGIME_DESCRIERE = 3;
        private const int MAX_LUNGIME_DESCRIERE = 100;

        private readonly FileService _fileService;
        private Tranzactie _tranzactieEditata = null;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            
            _fileService = new FileService("tranzactii.txt");
            
            Tranzactii = new ObservableCollection<Tranzactie>();
            FilteredTranzactii = new ObservableCollection<Tranzactie>();
            ChartData = new ObservableCollection<DailyChartData>();

            DataTranzactie = DateTime.Now;

            IncarcaTranzactii();
            UpdateBalance();
            UpdateChart();
        }

        public ObservableCollection<Tranzactie> Tranzactii { get; }
        public ObservableCollection<Tranzactie> FilteredTranzactii { get; }
        public ObservableCollection<DailyChartData> ChartData { get; }

        #region Bound Properties
        private string _searchQuery = "";
        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    ApplySearchFilter();
                }
            }
        }

        private string _balanceText = "0.00 RON";
        public string BalanceText { get => _balanceText; set => SetProperty(ref _balanceText, value); }

        private string _addTitle = "Adăugare Tranzacție";
        public string AddTitle { get => _addTitle; set => SetProperty(ref _addTitle, value); }

        private string _errorMessage = "";
        public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

        private string _sumaText = "";
        public string SumaText { get => _sumaText; set => SetProperty(ref _sumaText, value); }

        private string _descriereText = "";
        public string DescriereText { get => _descriereText; set => SetProperty(ref _descriereText, value); }

        private bool _isCheltuiala = true;
        public bool IsCheltuiala { get => _isCheltuiala; set => SetProperty(ref _isCheltuiala, value); }

        private bool _isVenit = false;
        public bool IsVenit { get => _isVenit; set => SetProperty(ref _isVenit, value); }

        private string _categorieSelectata = "Mâncare";
        public string CategorieSelectata { get => _categorieSelectata; set => SetProperty(ref _categorieSelectata, value); }

        private string _metodaPlataSelectata = "Cash";
        public string MetodaPlataSelectata { get => _metodaPlataSelectata; set => SetProperty(ref _metodaPlataSelectata, value); }

        private DateTime _dataTranzactie;
        public DateTime DataTranzactie { get => _dataTranzactie; set => SetProperty(ref _dataTranzactie, value); }

        private bool _isUrgent = false;
        public bool IsUrgent { get => _isUrgent; set => SetProperty(ref _isUrgent, value); }

        private bool _isPersonal = false;
        public bool IsPersonal { get => _isPersonal; set => SetProperty(ref _isPersonal, value); }

        private bool _isRecurent = false;
        public bool IsRecurent { get => _isRecurent; set => SetProperty(ref _isRecurent, value); }

        private bool _isEsential = false;
        public bool IsEsential { get => _isEsential; set => SetProperty(ref _isEsential, value); }

        private string _sendPhone = "";
        public string SendPhone { get => _sendPhone; set => SetProperty(ref _sendPhone, value); }

        private string _sendName = "";
        public string SendName { get => _sendName; set => SetProperty(ref _sendName, value); }

        private string _sendAmount = "";
        public string SendAmount { get => _sendAmount; set => SetProperty(ref _sendAmount, value); }

        private string _sendDesc = "";
        public string SendDesc { get => _sendDesc; set => SetProperty(ref _sendDesc, value); }
        #endregion

        #region Navigation
        private void btnNavHome_Click(object sender, RoutedEventArgs e)
        {
            HomeView.Visibility = Visibility.Visible;
            ActivityView.Visibility = Visibility.Collapsed;
            AddView.Visibility = Visibility.Collapsed;
            QuickTransferView.Visibility = Visibility.Collapsed;
        }

        private void btnNavActivity_Click(object sender, RoutedEventArgs e)
        {
            HomeView.Visibility = Visibility.Collapsed;
            ActivityView.Visibility = Visibility.Visible;
            AddView.Visibility = Visibility.Collapsed;
            QuickTransferView.Visibility = Visibility.Collapsed;
            UpdateChart();
        }

        private void btnInapoi_Click(object sender, RoutedEventArgs e)
        {
            btnNavHome_Click(sender, e);
        }

        private void btnInapoiTransfer_Click(object sender, RoutedEventArgs e)
        {
            btnNavHome_Click(sender, e);
        }
        #endregion

        #region Actions
        private void btnDeschideAdaugare_Click(object sender, RoutedEventArgs e)
        {
            _tranzactieEditata = null;
            AddTitle = "Adăugare Tranzacție";
            btnStergeEditata.Visibility = Visibility.Collapsed;
            ErrorMessage = "";

            SumaText = "";
            DescriereText = "";
            IsCheltuiala = true;
            IsVenit = false;
            CategorieSelectata = "Mâncare";
            MetodaPlataSelectata = "Cash";
            DataTranzactie = DateTime.Now;

            IsUrgent = false;
            IsPersonal = false;
            IsRecurent = false;
            IsEsential = false;

            HomeView.Visibility = Visibility.Collapsed;
            ActivityView.Visibility = Visibility.Collapsed;
            QuickTransferView.Visibility = Visibility.Collapsed;
            AddView.Visibility = Visibility.Visible;
        }

        private void btnEditTranzactie_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Tranzactie t)
            {
                _tranzactieEditata = t;
                AddTitle = "Editare Tranzacție";
                btnStergeEditata.Visibility = Visibility.Visible;
                ErrorMessage = "";

                SumaText = t.Suma.ToString();
                DescriereText = t.Descriere;
                DataTranzactie = t.Data;

                IsCheltuiala = t.Tip == TipTranzactie.Cheltuiala;
                IsVenit = t.Tip == TipTranzactie.Venit;

                CategorieSelectata = t.Categorie;
                MetodaPlataSelectata = t.MetodaPlata;

                IsUrgent = t.Optiuni.HasFlag(OptiuniTranzactie.Urgent);
                IsPersonal = t.Optiuni.HasFlag(OptiuniTranzactie.Personal);
                IsRecurent = t.Optiuni.HasFlag(OptiuniTranzactie.Recurent);
                IsEsential = t.Optiuni.HasFlag(OptiuniTranzactie.Esential);

                HomeView.Visibility = Visibility.Collapsed;
                ActivityView.Visibility = Visibility.Collapsed;
                QuickTransferView.Visibility = Visibility.Collapsed;
                AddView.Visibility = Visibility.Visible;
            }
        }

        private void btnAdauga_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;
            string errors = "";

            if (!double.TryParse(SumaText, out double suma) || suma < MIN_SUMA)
            {
                isValid = false;
                errors += $"• Suma trebuie să fie minim {MIN_SUMA}.\n";
            }

            string descriere = (DescriereText ?? "").Trim();
            if (descriere.Length < MIN_LUNGIME_DESCRIERE || descriere.Length > MAX_LUNGIME_DESCRIERE)
            {
                isValid = false;
                errors += $"• Descrierea trebuie să aibă între {MIN_LUNGIME_DESCRIERE} și {MAX_LUNGIME_DESCRIERE} caractere.\n";
            }

            if (DataTranzactie.Date > DateTime.Now.Date)
            {
                isValid = false;
                errors += "• Data nu poate fi în viitor.\n";
            }

            if (!isValid)
            {
                ErrorMessage = "Erori:\n" + errors;
                return;
            }

            ErrorMessage = "";

            TipTranzactie tip = IsVenit ? TipTranzactie.Venit : TipTranzactie.Cheltuiala;

            OptiuniTranzactie optiuni = OptiuniTranzactie.None;
            if (IsUrgent) optiuni |= OptiuniTranzactie.Urgent;
            if (IsPersonal) optiuni |= OptiuniTranzactie.Personal;
            if (IsRecurent) optiuni |= OptiuniTranzactie.Recurent;
            if (IsEsential) optiuni |= OptiuniTranzactie.Esential;

            string cat = CategorieSelectata ?? "Altele";
            string met = MetodaPlataSelectata ?? "Cash";

            if (_tranzactieEditata != null)
            {
                _tranzactieEditata.Suma = suma;
                _tranzactieEditata.Tip = tip;
                _tranzactieEditata.Optiuni = optiuni;
                _tranzactieEditata.Data = DataTranzactie;
                _tranzactieEditata.Descriere = descriere;
                _tranzactieEditata.Categorie = cat;
                _tranzactieEditata.MetodaPlata = met;
                _tranzactieEditata = null;
            }
            else
            {
                Tranzactie tNoua = new Tranzactie(suma, tip, optiuni, DataTranzactie, descriere, cat, met);
                Tranzactii.Insert(0, tNoua);
            }

            SalveazaTranzactii();
            ApplySearchFilter();
            UpdateBalance();
            UpdateChart();

            btnNavHome_Click(sender, e);
        }

        private void btnStergeTranzactie_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Tranzactie t)
            {
                var result = MessageBox.Show(
                    $"Ești sigur că vrei să ștergi tranzacția \"{t.Descriere}\"?",
                    "Confirmare ștergere",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Tranzactii.Remove(t);
                    SalveazaTranzactii();
                    ApplySearchFilter();
                    UpdateBalance();
                    UpdateChart();
                }
            }
        }

        private void btnStergeEditata_Click(object sender, RoutedEventArgs e)
        {
            if (_tranzactieEditata != null)
            {
                var result = MessageBox.Show(
                    $"Ești sigur că vrei să ștergi tranzacția \"{_tranzactieEditata.Descriere}\"?",
                    "Confirmare ștergere",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    Tranzactii.Remove(_tranzactieEditata);
                    _tranzactieEditata = null;
                    SalveazaTranzactii();
                    ApplySearchFilter();
                    UpdateBalance();
                    UpdateChart();

                    btnNavHome_Click(sender, e);
                }
            }
        }

        private void btnOpenTransfer_Click(object sender, RoutedEventArgs e)
        {
            SendPhone = "";
            SendName = "";
            SendAmount = "";
            SendDesc = "";

            HomeView.Visibility = Visibility.Collapsed;
            ActivityView.Visibility = Visibility.Collapsed;
            AddView.Visibility = Visibility.Collapsed;
            QuickTransferView.Visibility = Visibility.Visible;
        }

        private void btnTrimiteRapid_Click(object sender, RoutedEventArgs e)
        {
            string telefon = (SendPhone ?? "").Trim();
            string nume = (SendName ?? "").Trim();
            string descriere = (SendDesc ?? "").Trim();

            if (telefon.Length == 0 || nume.Length == 0 || descriere.Length == 0 || string.IsNullOrWhiteSpace(SendAmount))
            {
                MessageBox.Show("Toate câmpurile trebuie completate!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (telefon.Length > 10 || !telefon.All(char.IsDigit))
            {
                MessageBox.Show("Numărul de telefon trebuie să conțină maxim 10 cifre!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(SendAmount, out double suma) || suma <= 0)
            {
                MessageBox.Show("Suma introdusă nu este validă!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double currentBalance = Tranzactii.Where(t => t.Tip == TipTranzactie.Venit).Sum(t => t.Suma) -
                                    Tranzactii.Where(t => t.Tip == TipTranzactie.Cheltuiala).Sum(t => t.Suma);

            if (suma > currentBalance)
            {
                MessageBox.Show($"Fonduri insuficiente! Soldul tău curent este de {currentBalance:N2} RON.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string descriereCompleta = $"Transfer către {nume} ({telefon}) - {descriere}";

            Tranzactie transferNou = new Tranzactie(suma, TipTranzactie.Cheltuiala, OptiuniTranzactie.None, DateTime.Now, descriereCompleta, "Transfer", "Transfer");

            Tranzactii.Insert(0, transferNou);
            SalveazaTranzactii();

            ApplySearchFilter();
            UpdateBalance();
            UpdateChart();

            btnNavHome_Click(sender, e);
            MessageBox.Show($"Ai transferat {suma} RON către {nume} cu succes!", "Transfer Reușit", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        #endregion

        #region Logic Methods
        private void ApplySearchFilter()
        {
            FilteredTranzactii.Clear();
            string query = (SearchQuery ?? "").Trim().ToLower();

            foreach (var t in Tranzactii)
            {
                if (string.IsNullOrEmpty(query) ||
                    t.Descriere.ToLower().Contains(query) ||
                    t.Tip.ToString().ToLower().Contains(query))
                {
                    FilteredTranzactii.Add(t);
                }
            }
        }

        private void UpdateBalance()
        {
            double balance = Tranzactii.Where(t => t.Tip == TipTranzactie.Venit).Sum(t => t.Suma) -
                             Tranzactii.Where(t => t.Tip == TipTranzactie.Cheltuiala).Sum(t => t.Suma);

            BalanceText = $"{balance:N2} RON";
        }

        private void UpdateChart()
        {
            ChartData.Clear();

            DateTime today = DateTime.Today;
            double maxVal = 100;
            var dailyData = new System.Collections.Generic.Dictionary<DateTime, (double inc, double exp)>();

            for (int i = 6; i >= 0; i--)
            {
                DateTime d = today.AddDays(-i);

                double inc = Tranzactii.Where(t => t.Data.Date == d && t.Tip == TipTranzactie.Venit).Sum(t => t.Suma);
                double exp = Tranzactii.Where(t => t.Data.Date == d && t.Tip == TipTranzactie.Cheltuiala).Sum(t => t.Suma);

                dailyData[d] = (inc, exp);

                if (inc > maxVal) maxVal = inc;
                if (exp > maxVal) maxVal = exp;
            }

            double chartMaxHeight = 200.0;

            foreach (var kvp in dailyData)
            {
                double incomeHeight = (kvp.Value.inc / maxVal) * chartMaxHeight;
                double expenseHeight = (kvp.Value.exp / maxVal) * chartMaxHeight;

                if (incomeHeight < 2 && kvp.Value.inc > 0) incomeHeight = 2;
                if (expenseHeight < 2 && kvp.Value.exp > 0) expenseHeight = 2;

                string dayName = kvp.Key.ToString("ddd", CultureInfo.InvariantCulture).Substring(0, 3);

                ChartData.Add(new DailyChartData
                {
                    DayName = dayName,
                    IncomeHeight = incomeHeight,
                    ExpenseHeight = expenseHeight
                });
            }
        }

        private void SalveazaTranzactii()
        {
            try
            {
                _fileService.SalveazaTranzactii(Tranzactii.ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvarea fisierului: {ex.Message}");
            }
        }

        private void IncarcaTranzactii()
        {
            try
            {
                var loaded = _fileService.IncarcaTranzactii();
                Tranzactii.Clear();
                foreach (var t in loaded)
                {
                    Tranzactii.Add(t);
                }
                ApplySearchFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la citirea fisierului: {ex.Message}");
            }
        }
        #endregion
    }
}