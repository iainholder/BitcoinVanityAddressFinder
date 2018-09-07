using System.Windows;
using BitcoinVanityAddressFinder.ViewModel;

namespace BitcoinVanityAddressFinder
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // This is all that's required for this simple app
            DataContext = new VanityAddressViewModel();
        }
    }
}