using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using FRI.DISS.SP3.Libs.StanicaSimulation.Simulation;

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
            Debug.WriteLine("Simulating...");

            var replicationsCount = 10;
            var endTime = 1000;

            var sim = new MySimulation();
            
            sim.Simulate(replicationsCount, endTime);

            Debug.WriteLine("Finished...");
        }
    }
}
