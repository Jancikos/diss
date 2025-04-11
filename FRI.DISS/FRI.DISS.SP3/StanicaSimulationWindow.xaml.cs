using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FRI.DISS.SP3
{
    /// <summary>
    /// Interaction logic for StanicaSimulationWindow.xaml
    /// </summary>
    public partial class StanicaSimulationWindow : Window
    {
        public StanicaSimulationWindow()
        {
            InitializeComponent();
        }

        private void _mnitem_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _mnitem_RunSimulation_Click(object sender, RoutedEventArgs e)
        {
           MessageBox.Show("Simulation started!");
        }
    }
}
