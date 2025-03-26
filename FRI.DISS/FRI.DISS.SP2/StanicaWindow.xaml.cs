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
using FRI.DISS.Libs.Simulations;
using FRI.DISS.Libs.Simulations.EventDriven;

namespace FRI.DISS.SP2
{
    /// <summary>
    /// Interaction logic for StanicaWindow.xaml
    /// </summary>
    public partial class StanicaWindow : Window
    {
        protected StanicaSimulation _simulation;

        public StanicaWindow()
        {
            InitializeComponent();

            _cmbx_repRealTimeRatio.ItemsSource = Enum.GetValues(typeof(EventDrivenSimulationRealTimeRatios)).Cast<EventDrivenSimulationRealTimeRatios>();
            _cmbx_repRealTimeRatio.SelectedIndex = 0;

            _simulation = new StanicaSimulation();

            _simulation.GUIEventHappened += _simulation_GUIEventHappened;
        }

        private void _simulation_GUIEventHappened(object? sender, EventDrivenSimulationGUIEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                switch (e.Type)
                {
                    case EventDrivenSimulationEventArgsType.RefreshTime:
                        _refreshTime();
                        break;
                    case EventDrivenSimulationEventArgsType.SimulationExperimentDone:
                        _refreshReplicationsStats();
                        break;
                    case EventDrivenSimulationEventArgsType.SimulationEventDone:
                        _refreshExperimentStats();
                        break;
                }
            });
        }

        private void _refreshTime()
        {
            _txt_expTime.Value = $"{_simulation.CurrentTimeFormatted} <{_simulation.CurrentTime:F2}>";
        }

        private void _refreshExperimentStats()
        {
            _refreshTime();

            _txt_expTotalCustomers.Value = _simulation.ExperimentData.VisitedCustomers.ToString();
            _txt_expCustomersServed.Value = _simulation.ExperimentData.ServicedCustomers.ToString();

            _expStatsCustomersServiceTime.Update(_simulation.ExperimentStatistics.CustomersServiceTime);

            _expStatsCustomersWaitingTime.Update(_simulation.ExperimentStatistics.CustomerWaitingTime);
            _expStatsCustomersTotalTime.Update(_simulation.ExperimentStatistics.CustomersInSystemTime);
        }

        private void _refreshReplicationsStats()
        {
            _repStatsServedCustomers.Update(_simulation.ReplicationsStatistics.ServedCustomersCount);
            _repStatsCustomersWaitingTime.Update(_simulation.ReplicationsStatistics.CustomerWaitingTime);
            _repStatsCustomersServiceTime.Update(_simulation.ReplicationsStatistics.CustomersServiceTime);
            _repStatsCustomersTotalTime.Update(_simulation.ReplicationsStatistics.CustomersInSystemTime);
        }

        private void _mnitem_Run_Click(object sender, RoutedEventArgs e)
        {
            _simulation.ReplicationsCount = 100;
            _simulation.RealTimeRatio = (EventDrivenSimulationRealTimeRatios)_cmbx_repRealTimeRatio.SelectedItem;


            Task.Run(() =>
            {
                try
                {
                    _simulation.StartSimulation();

                    Debug.WriteLine("Simulation done:");
                    Debug.WriteLine("Replications done: " + _simulation.ReplicationsDone);
                    Debug.WriteLine("Customers served: " + _simulation.ReplicationsStatistics.ServedCustomersCount.MeanToString(true));
                    Debug.WriteLine("Avg. Queue Count: " + _simulation.ReplicationsStatistics.CustomersInQueueCount.MeanToString(true));
                    Debug.WriteLine("Avg. Queue Time: " + _simulation.ReplicationsStatistics.CustomerWaitingTime.MeanToString(true));
                    Debug.WriteLine("Avg. Service Time: " + _simulation.ReplicationsStatistics.CustomersServiceTime.MeanToString(true));
                    Debug.WriteLine("Avg. Total Time: " + _simulation.ReplicationsStatistics.CustomersInSystemTime.MeanToString(true));

                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Error: " + ex.Message);
                }
            });
        }

        private void _mnitem_Stop_Click(object sender, RoutedEventArgs e)
        {
            _simulation.StopSimulation();
        }

        private void _mnitem_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
