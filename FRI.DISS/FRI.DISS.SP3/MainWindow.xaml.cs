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

namespace FRI.DISS.SP3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // _mnitem_StanicaSimWindow_Click(null, null);
        }

        private void _mnitem_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _mnitem_StanicaSimWindow_Click(object sender, RoutedEventArgs e)
        {
            var stanicaSimulationWindow = new StanicaSimulationWindow();

            stanicaSimulationWindow.Owner = this;
            stanicaSimulationWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            stanicaSimulationWindow.ShowDialog();
        }
    }
}