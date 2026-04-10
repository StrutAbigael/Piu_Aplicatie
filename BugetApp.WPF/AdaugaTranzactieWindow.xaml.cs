using System;
using System.Windows;
using System.Windows.Media;
using BugetApp.Models;

namespace BugetApp.WPF
{
    public partial class AdaugaTranzactieWindow : Window
    {
        private const double MIN_SUMA = 1.0;
        private const int MIN_LUNGIME_DESCRIERE = 3;
        private const int MAX_LUNGIME_DESCRIERE = 100;

        // Culoarea textului corect bazat pe paleta (#FBE4D8)
        private readonly SolidColorBrush _culoareValid = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FBE4D8")); 
        
        // Cand e incorect ramane roșu pentru vizibilitate
        private readonly SolidColorBrush _culoareInvalid = new SolidColorBrush(Colors.Red);

        public AdaugaTranzactieWindow()
        {
            InitializeComponent();
            
            cmbTip.ItemsSource = Enum.GetValues(typeof(TipTranzactie));
            cmbTip.SelectedIndex = 0; 
            dpData.SelectedDate = DateTime.Now;
        }

        private void btnAdauga_Click(object sender, RoutedEventArgs e)
        {
            bool isValid = true;
            string errorMessage = "";

            lblSuma.Foreground = _culoareValid;
            lblDescriere.Foreground = _culoareValid;
            lblData.Foreground = _culoareValid;

            double suma;
            if (!double.TryParse(txtSuma.Text, out suma) || suma < MIN_SUMA)
            {
                lblSuma.Foreground = _culoareInvalid;
                isValid = false;
                errorMessage += $"• Suma introdusă nu este validă. Trebuie să fie minim {MIN_SUMA}.\n";
            }

            string descriere = txtDescriere.Text.Trim();
            if (descriere.Length < MIN_LUNGIME_DESCRIERE || descriere.Length > MAX_LUNGIME_DESCRIERE)
            {
                lblDescriere.Foreground = _culoareInvalid;
                isValid = false;
                errorMessage += $"• Descrierea trebuie să aibă între {MIN_LUNGIME_DESCRIERE} și {MAX_LUNGIME_DESCRIERE} caractere.\n";
            }

            if (dpData.SelectedDate == null || dpData.SelectedDate.Value.Date > DateTime.Now.Date)
            {
                lblData.Foreground = _culoareInvalid;
                isValid = false;
                errorMessage += "• Data nu poate fi necompletată sau o dată din viitor.\n";
            }

            if (!isValid)
            {
                txtError.Foreground = _culoareInvalid;
                txtError.Text = "Erori de validare:\n" + errorMessage;
                return;
            }

            txtError.Text = "";

            TipTranzactie tip = (TipTranzactie)cmbTip.SelectedItem;
            DateTime data = dpData.SelectedDate.Value;
            
            OptiuniTranzactie optiuni = OptiuniTranzactie.None;
            if (chkUrgent.IsChecked == true) optiuni |= OptiuniTranzactie.Urgent;
            if (chkPersonal.IsChecked == true) optiuni |= OptiuniTranzactie.Personal;
            if (chkRecurent.IsChecked == true) optiuni |= OptiuniTranzactie.Recurent;
            if (chkEsential.IsChecked == true) optiuni |= OptiuniTranzactie.Esential;

            Tranzactie tranzactieNoua = new Tranzactie(suma, tip, optiuni, data, descriere);
            
            MessageBox.Show("Tranzacția a fost salvată cu succes!\n\n" + 
                $"Descriere: {tranzactieNoua.Descriere}\n" +
                $"Sumă: {tranzactieNoua.Suma} RON\n" +
                $"Tip: {tranzactieNoua.Tip}\n" +
                $"Dată: {tranzactieNoua.Data.ToShortDateString()}\n" +
                $"Opțiuni: {tranzactieNoua.Optiuni}", 
                "Succes", MessageBoxButton.OK, MessageBoxImage.Information);
                
            txtSuma.Text = "";
            txtDescriere.Text = "";
            chkUrgent.IsChecked = false;
            chkPersonal.IsChecked = false;
            chkRecurent.IsChecked = false;
            chkEsential.IsChecked = false;
            cmbTip.SelectedIndex = 0;
            dpData.SelectedDate = DateTime.Now;
        }
    }
}
