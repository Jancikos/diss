using FRI.DISS.Libs.Simulations.EventDriven;
using System.Diagnostics;
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

namespace FRI.DISS.SP2
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

        private void _mnitem_File_Click(object sender, RoutedEventArgs e)
        {
            var sim = new StanicaSimulation()
            {
                ReplicationsCount = 1000,
                TimeMode = EventDrivenSimulationTimeMode.FastForward
            };

            sim.RunSimulation();

            Debug.WriteLine("Simulation done:");
            Debug.WriteLine("Replications done: " + sim.ReplicationsDone);
            Debug.WriteLine("Customers served: " + sim.ReplicationsStatistics.ServedCustomersCount.MeanToString(true));
            Debug.WriteLine("Avg. Queue Count: " + sim.ReplicationsStatistics.CustomersInQueueCount.MeanToString(true));
            Debug.WriteLine("Avg. Queue Time: " + sim.ReplicationsStatistics.CustomerWaitingTime.MeanToString(true));
            Debug.WriteLine("Avg. Service Time: " + sim.ReplicationsStatistics.CustomersServiceTime.MeanToString(true));
            Debug.WriteLine("Avg. Total Time: " + sim.ReplicationsStatistics.CustomersInSystemTime.MeanToString(true));
        }

        private void _mnitem_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}