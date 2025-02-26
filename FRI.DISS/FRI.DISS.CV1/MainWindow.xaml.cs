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

namespace FRI.DISS.CV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int repCount = 100000;
            int l = 5;
            int d = 10;

            var buffonExp = new BuffonNeedle()
            {
                D = d,
                L = l
            };

            var piEst = buffonExp.RunExperiment(repCount);
            // var piEst = (2 * l) / (p * d);

            MessageBox.Show($"Estimated Pi: {piEst}");
        }
    }
}