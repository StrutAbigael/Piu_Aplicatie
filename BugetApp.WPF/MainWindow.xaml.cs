using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BugetApp.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AfiseazaDatePursePalette();
        }

        private void AfiseazaDatePursePalette()
        {
           
            txtDescriere.Text = "Descriere: Programare salon";
            txtSuma.Text = "Sumă: 250 RON";
            txtTip.Text = "Tip: Cheltuială";
            txtData.Text = "Data: " + DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}