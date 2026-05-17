using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using BugetApp.Models;

namespace BugetApp.WPF
{
    public class DailyChartData
    {
        public string DayName { get; set; } = string.Empty;
        public double IncomeHeight { get; set; }
        public double ExpenseHeight { get; set; }
    }

    public partial class MainWindow : Window
    {
        private const double MIN_SUMA = 1.0;
        private const int MIN_LUNGIME_DESCRIERE = 3;
        private const int MAX_LUNGIME_DESCRIERE = 100;

        private ObservableCollection<Tranzactie> _tranzactii;
        private ObservableCollection<Tranzactie> _filteredTranzactii;
        private ObservableCollection<DailyChartData> _chartData;
        private readonly string _fisierTranzactii = "tranzactii.txt";

        public MainWindow()
        {
            InitializeComponent();
            
            _tranzactii = new ObservableCollection<Tranzactie>();
            _filteredTranzactii = new ObservableCollection<Tranzactie>(_tranzactii);
            _chartData = new ObservableCollection<DailyChartData>();
            
            lstTranzactii.ItemsSource = _filteredTranzactii;
            icChart.ItemsSource = _chartData;
            icDays.ItemsSource = _chartData;

            dpData.SelectedDate = DateTime.Now;
            IncarcaTranzactiiDinFisier();
            UpdateBalance();
            UpdateChart();
        }

        private void btnDeschideAdaugare_Click(object sender, RoutedEventArgs e)
        {
            HomeView.Visibility = Visibility.Collapsed;
            ActivityView.Visibility = Visibility.Collapsed;
            AddView.Visibility = Visibility.Visible;
            
            // Reset form
            txtError.Text = "";
            txtSuma.Text = "";
            txtDescriere.Text = "";
            chkUrgent.IsChecked = false;
            chkPersonal.IsChecked = false;
            chkRecurent.IsChecked = false;
            chkEsential.IsChecked = false;
            rbCheltuiala.IsChecked = true;
            cmbCategorie.SelectedIndex = 0;
            lstMetodaPlata.SelectedIndex = 0;
            dpData.SelectedDate = DateTime.Now;
        }

        private void btnInapoi_Click(object sender, RoutedEventArgs e)
        {
            AddView.Visibility = Visibility.Collapsed;
            HomeView.Visibility = Visibility.Visible;
        }

        private void btnNavHome_Click(object sender, RoutedEventArgs e)
        {
            ActivityView.Visibility = Visibility.Collapsed;
            AddView.Visibility = Visibility.Collapsed;
            HomeView.Visibility = Visibility.Visible;
        }

        private void btnNavActivity_Click(object sender, RoutedEventArgs e)
        {
            HomeView.Visibility = Visibility.Collapsed;
            AddView.Visibility = Visibility.Collapsed;
            ActivityView.Visibility = Visibility.Visible;
            UpdateChart();
        }

        private void btnAdauga_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;
            string errorMessage = "";

            if (!double.TryParse(txtSuma.Text, out double suma) || suma < MIN_SUMA)
            {
                isValid = false;
                errorMessage += $"• Suma trebuie să fie minim {MIN_SUMA}.\n";
            }

            string descriere = txtDescriere.Text.Trim();
            if (descriere.Length < MIN_LUNGIME_DESCRIERE || descriere.Length > MAX_LUNGIME_DESCRIERE)
            {
                isValid = false;
                errorMessage += $"• Descrierea trebuie să aibă între {MIN_LUNGIME_DESCRIERE} și {MAX_LUNGIME_DESCRIERE} caractere.\n";
            }

            if (dpData.SelectedDate == null || dpData.SelectedDate.Value.Date > DateTime.Now.Date)
            {
                isValid = false;
                errorMessage += "• Data nu poate fi în viitor.\n";
            }

            if (!isValid)
            {
                txtError.Text = "Erori:\n" + errorMessage;
                return;
            }

            txtError.Text = "";

            TipTranzactie tip = rbVenit.IsChecked == true ? TipTranzactie.Venit : TipTranzactie.Cheltuiala;
            DateTime data = dpData.SelectedDate.GetValueOrDefault(DateTime.Now);
            
            OptiuniTranzactie optiuni = OptiuniTranzactie.None;
            if (chkUrgent.IsChecked == true) optiuni |= OptiuniTranzactie.Urgent;
            if (chkPersonal.IsChecked == true) optiuni |= OptiuniTranzactie.Personal;
            if (chkRecurent.IsChecked == true) optiuni |= OptiuniTranzactie.Recurent;
            if (chkEsential.IsChecked == true) optiuni |= OptiuniTranzactie.Esential;

            string categorie = cmbCategorie.SelectedItem is ComboBoxItem cbi ? cbi.Content.ToString() : "Altele";
            string metodaPlata = lstMetodaPlata.SelectedItem is ListBoxItem lbi ? lbi.Content.ToString() : "Cash";

            Tranzactie tranzactieNoua = new Tranzactie(suma, tip, optiuni, data, descriere, categorie, metodaPlata);
            
            _tranzactii.Insert(0, tranzactieNoua);
            
            SalveazaTranzactiiInFisier();
            
            ApplySearchFilter();
            UpdateBalance();
            UpdateChart();
            
            AddView.Visibility = Visibility.Collapsed;
            HomeView.Visibility = Visibility.Visible;
        }

        private void btnOpenTransfer_Click(object sender, RoutedEventArgs e)
        {
            HomeView.Visibility = Visibility.Collapsed;
            ActivityView.Visibility = Visibility.Collapsed;
            AddView.Visibility = Visibility.Collapsed;
            QuickTransferView.Visibility = Visibility.Visible;

            txtSendPhone.Text = "";
            txtSendName.Text = "";
            txtSendAmount.Text = "";
            txtSendDesc.Text = "";
        }

        private void btnInapoiTransfer_Click(object sender, RoutedEventArgs e)
        {
            QuickTransferView.Visibility = Visibility.Collapsed;
            HomeView.Visibility = Visibility.Visible;
        }

        private void btnTrimiteRapid_Click(object sender, RoutedEventArgs e)
        {
            string telefon = txtSendPhone.Text.Trim();
            string nume = txtSendName.Text.Trim();
            string descriere = txtSendDesc.Text.Trim();

            if (telefon.Length == 0 || nume.Length == 0 || descriere.Length == 0 || string.IsNullOrWhiteSpace(txtSendAmount.Text))
            {
                MessageBox.Show("Toate câmpurile trebuie completate!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (telefon.Length > 10 || !telefon.All(char.IsDigit))
            {
                MessageBox.Show("Numărul de telefon trebuie să conțină maxim 10 cifre!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!double.TryParse(txtSendAmount.Text, out double suma) || suma <= 0)
            {
                MessageBox.Show("Suma introdusă nu este validă!", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            double currentBalance = _tranzactii.Where(t => t.Tip == TipTranzactie.Venit).Sum(t => t.Suma) - 
                                    _tranzactii.Where(t => t.Tip == TipTranzactie.Cheltuiala).Sum(t => t.Suma);

            if (suma > currentBalance)
            {
                MessageBox.Show($"Fonduri insuficiente! Soldul tău curent este de {currentBalance:N2} RON.", "Eroare", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string descriereCompleta = $"Transfer către {nume} ({telefon}) - {descriere}";
            
            Tranzactie transferNou = new Tranzactie(suma, TipTranzactie.Cheltuiala, OptiuniTranzactie.None, DateTime.Now, descriereCompleta, "Transfer", "Transfer");
            
            _tranzactii.Insert(0, transferNou);
            SalveazaTranzactiiInFisier();
            
            ApplySearchFilter();
            UpdateBalance();
            UpdateChart();

            txtSendPhone.Text = "";
            txtSendName.Text = "";
            txtSendAmount.Text = "";
            txtSendDesc.Text = "";

            QuickTransferView.Visibility = Visibility.Collapsed;
            HomeView.Visibility = Visibility.Visible;

            MessageBox.Show($"Ai transferat {suma} RON către {nume} cu succes!", "Transfer Reușit", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplySearchFilter();
        }

        private void ApplySearchFilter()
        {
            string query = txtSearch.Text.Trim().ToLower();

            _filteredTranzactii.Clear();

            foreach (var t in _tranzactii)
            {
                // Search by description or type
                if (string.IsNullOrEmpty(query) || 
                    t.Descriere.ToLower().Contains(query) ||
                    t.Tip.ToString().ToLower().Contains(query))
                {
                    _filteredTranzactii.Add(t);
                }
            }
        }

        private void UpdateBalance()
        {
            double balance = _tranzactii.Where(t => t.Tip == TipTranzactie.Venit).Sum(t => t.Suma) - 
                             _tranzactii.Where(t => t.Tip == TipTranzactie.Cheltuiala).Sum(t => t.Suma);
            
            txtBalance.Text = $"{balance:N2} RON";
        }

        private void UpdateChart()
        {
            _chartData.Clear();
            
            // Generate data for the last 7 days (including today)
            DateTime today = DateTime.Today;
            
            double maxVal = 100; // default max height base
            var dailyData = new System.Collections.Generic.Dictionary<DateTime, (double inc, double exp)>();

            for (int i = 6; i >= 0; i--)
            {
                DateTime d = today.AddDays(-i);
                
                double inc = _tranzactii.Where(t => t.Data.Date == d && t.Tip == TipTranzactie.Venit).Sum(t => t.Suma);
                double exp = _tranzactii.Where(t => t.Data.Date == d && t.Tip == TipTranzactie.Cheltuiala).Sum(t => t.Suma);
                
                dailyData[d] = (inc, exp);
                
                if (inc > maxVal) maxVal = inc;
                if (exp > maxVal) maxVal = exp;
            }

            double chartMaxHeight = 200.0; // Pxls

            foreach (var kvp in dailyData)
            {
                double incomeHeight = (kvp.Value.inc / maxVal) * chartMaxHeight;
                double expenseHeight = (kvp.Value.exp / maxVal) * chartMaxHeight;

                // Make sure even zero values have a tiny height to be visible (or just 0)
                if (incomeHeight < 2 && kvp.Value.inc > 0) incomeHeight = 2;
                if (expenseHeight < 2 && kvp.Value.exp > 0) expenseHeight = 2;

                string dayName = kvp.Key.ToString("ddd", CultureInfo.InvariantCulture).Substring(0, 3);

                _chartData.Add(new DailyChartData
                {
                    DayName = dayName,
                    IncomeHeight = incomeHeight,
                    ExpenseHeight = expenseHeight
                });
            }
        }

        private void SalveazaTranzactiiInFisier()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(_fisierTranzactii))
                {
                    foreach (var t in _tranzactii)
                    {
                        // Format CSV: Suma,Tip,Optiuni,Data,Descriere,Categorie,MetodaPlata
                        string linie = $"{t.Suma},{t.Tip},{(int)t.Optiuni},{t.Data:yyyy-MM-dd HH:mm:ss},{t.Descriere},{t.Categorie},{t.MetodaPlata}";
                        sw.WriteLine(linie);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la salvarea fisierului: {ex.Message}");
            }
        }

        private void IncarcaTranzactiiDinFisier()
        {
            if (!File.Exists(_fisierTranzactii)) return;

            try
            {
                string[] linii = File.ReadAllLines(_fisierTranzactii);
                foreach (string linie in linii)
                {
                    if (string.IsNullOrWhiteSpace(linie)) continue;

                    string[] fragmente = linie.Split(',');
                    if (fragmente.Length >= 5)
                    {
                        if (double.TryParse(fragmente[0], out double suma) &&
                            Enum.TryParse(fragmente[1], out TipTranzactie tip) &&
                            Enum.TryParse(fragmente[2], out OptiuniTranzactie optiuni) &&
                            DateTime.TryParse(fragmente[3], out DateTime data))
                        {
                            string descriere = fragmente[4];
                            string categorie = fragmente.Length > 5 ? fragmente[5] : "Altele";
                            string metodaPlata = fragmente.Length > 6 ? fragmente[6] : "Cash";

                            Tranzactie t = new Tranzactie(suma, tip, optiuni, data, descriere, categorie, metodaPlata);
                            _tranzactii.Add(t);
                        }
                    }
                }
                
                // Actualizeaza lista filtrata initial
                _filteredTranzactii.Clear();
                foreach (var t in _tranzactii)
                {
                    _filteredTranzactii.Add(t);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eroare la citirea fisierului: {ex.Message}");
            }
        }
    }
}