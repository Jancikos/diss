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
using FRI.DISS.Libs.Helpers;
using FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;

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

        private void _mnitem_RunSim_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Simulating...");

            var replicationsCount = 1000;
            var renderInterval = replicationsCount / 50;
            var endTime = TimeHelper.HoursToSeconds(8 * 249);

            var sim = new MySimulation();

            int repsDone = 0;
            sim.OnReplicationDidFinish((_) =>
            {
                repsDone++;

                if (repsDone % renderInterval == 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        // _sts_repQueueLength.Update(sim.CustomersQueueCount!);
                        // _sts_repQueueTime.Update(sim.CustomersQueueTime!);
                        // _sts_repTotalTime.Update(sim.CustomersTotalTime!);
                        // _sts_repTotalCustomerCount.Update(sim.CustomersCount!);
                    });
                }
            });

            sim.SetMaxSimSpeed();

            sim.SimulateAsync(replicationsCount, endTime);

            Debug.WriteLine("Finished...");
        }
    }
}