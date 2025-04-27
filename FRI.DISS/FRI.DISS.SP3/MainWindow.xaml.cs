using System.Diagnostics;
using System.IO;
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
using FRI.DISS.Libs.Generators;
using FRI.DISS.Libs.GUI.Controls;
using FRI.DISS.Libs.Helpers;
using FRI.DISS.Libs.Simulations.EventDriven;
using FRI.DISS.SP3.Controls;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentModelu;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentPracovisk;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolariA;
using FRI.DISS.SP3.Libs.NabytokSimulation.Agents.AgentStolarov;
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;
using OSPABA;

namespace FRI.DISS.SP3
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MySimulation _simulation;

        public MainWindow()
        {
            InitializeComponent();

            _simulation = new MySimulation();

            _simulation.OnRefreshUI(_onRefreshUI);
            _simulation.OnReplicationWillStart(_onReplicationWillStart);
            _simulation.OnReplicationDidFinish(_onReplicationDidFinish);
            _simulation.OnSimulationWillStart(_onSimulationWillStart);
            _simulation.OnSimulationDidFinish(_onSimulationDidFinish);

            _initializeGUI();
        }

        #region Simulation events 

        private void _onRefreshUI(Simulation simulation)
        {
            Dispatcher.Invoke(() =>
            {
                _refreshTimeGUI();

                _refreshOrders();
                _refreshWorkplaces();
                _refreshStolariExp();
                _refreshOperationsQueuesExp();
                _refreshExperimentStats();
            });
        }

        private void _onSimulationWillStart(Simulation simulation)
        {

        }
        private void _onSimulationDidFinish(Simulation simulation)
        {
            var repsDone = _simulation.CurrentReplication;

            Dispatcher.InvokeAsync(() =>
            {
                _txt_repsEndTime.Value = DateTime.Now.ToString("HH:mm:ss");

                _refreshReplicationsDoneCounters(repsDone);
                _refreshReplicationsStats(_simulation.ReplicationsStatistics);
            });

            // save simulation results to csv file
            if (repsDone > 300)
            {
                NabytokReplicationsStatisticsCsvWriter.Instance.Write(
                    new FileInfo("./experiments.csv"),
                    _simulation
                );
            }
        }

        private void _onReplicationWillStart(Simulation simulation)
        {
        }
        private void _onReplicationDidFinish(Simulation simulation)
        {
            var repRefershInterval = _simulation.RepliactionsRefreshStatisticsInterval;
            var repsDone = _simulation.CurrentReplication;

            Dispatcher.InvokeAsync(() =>
            {
                _refreshReplicationsDoneCounters(repsDone);
            });

            if (!_shouldRefreshReplicationsStats(repsDone, repRefershInterval))
                return;

            Dispatcher.InvokeAsync(() =>
            {
                _refreshReplicationsStats(_simulation.ReplicationsStatistics);
            });
        }

        private bool _shouldRefreshReplicationsStats(int repsDone, int repRefershInterval)
        {
            // is first
            if (repsDone == 0)
                return true;

            // is last
            if (repsDone == _simulation.ReplicationCount)
                return true;

            // is refresh interval
            if (repsDone % repRefershInterval == 0)
                return true;

            return false;
        }

        #endregion Simulation events

        #region GUI methods

        private void _initializeGUI()
        {
            // cmbx timeRatio
            _cmbx_simRealTimeRatio.ItemsSource = Enum.GetValues(typeof(EventDrivenSimulationRealTimeRatios)).Cast<EventDrivenSimulationRealTimeRatios>();
            // _cmbx_simRealTimeRatio.SelectedIndex = 0;
            _cmbx_simRealTimeRatio.SelectedIndex = 9;

            // stolari types
            Enum.GetValues(typeof(StolarType)).Cast<StolarType>().ToList().ForEach(stolarType =>
            {
                _lst_expStolariTypes.Children.Add(new StolariUserControl() { StolarType = stolarType });
                // _lst_expStolariTypesQueues.Children.Add(new StolariQueueUserControl() { StolarType = stolarType });

                _lst_repsStolariTypes.Children.Add(new DicreteStatistic() { Title = $"Vyťaženie stolárov {stolarType} (%)", PlotShow = true, TransformToPercentage = true });
                _lst_repsStolarTypes.Children.Add(new StolariUserControl() { StolarType = stolarType });
            });

            // operation queues
            Enum.GetValues(typeof(NabytokOperation)).Cast<NabytokOperation>().ToList().ForEach(nabytokOperation =>
            {
                _lst_expOperationsQueues.Children.Add(new OperationQueueUserControl() { NabytokOperation = nabytokOperation });
            });
        }

        private void _initializeSimulationFromGUI()
        {
            _simulation.SeedGenerator = _txt_simSeed.HasValue
                ? new SeedGenerator(_txt_simSeed.IntValue)
                : SeedGenerator.Global;

            _simulation.RepliactionsRefreshStatisticsInterval = _txt_simReplicationsStatsRefresh.IntValue;

            _setSimulationTimeFromGUI();
            _txt_repsStartTime.Value = DateTime.Now.ToString("HH:mm:ss");
            _txt_repsEndTime.Value = "--:--:--";

            _sts_expObjednavkaTime.SkipFirstCount = _txt_simPlotsSkipFirstCount.IntValue;
            _sts_repsObjednavkaTime.SkipFirstCount = _txt_simPlotsSkipFirstCount.IntValue;
            _sts_repsObjednavkaNotDoneCount.SkipFirstCount = _txt_simPlotsSkipFirstCount.IntValue;
            _sts_repsObjednavkaReceivedCount.SkipFirstCount = _txt_simPlotsSkipFirstCount.IntValue;
            _sts_expObjednavkaTime.SkipFirstCount = _txt_simPlotsSkipFirstCount.IntValue;
            _lst_repsStolariTypes.Children.Cast<DicreteStatistic>().ToList().ForEach(stolariUC =>
            {
                stolariUC.SkipFirstCount = _txt_simPlotsSkipFirstCount.IntValue;
            });

            // stolari count
            _simulation.StolariCount[StolarType.A] = _txt_simStolariACount.IntValue;
            _simulation.StolariCount[StolarType.B] = _txt_simStolariBCount.IntValue;
            _simulation.StolariCount[StolarType.C] = _txt_simStolariCCount.IntValue;

            // workplaces count
            _simulation.PracoviskaCount = _txt_simWorkplacesCount.IntValue;
        }

        private void _setSimulationTimeFromGUI()
        {
            var mode = _chk_simMaxSpeed.IsChecked == true
                ? EventDrivenSimulationTimeMode.FastForward
                : EventDrivenSimulationTimeMode.RealTime;

            switch (mode)
            {
                case EventDrivenSimulationTimeMode.RealTime:
                    var ratio = (EventDrivenSimulationRealTimeRatios)_cmbx_simRealTimeRatio.SelectedItem;
                    var ratioValue = (int)ratio;

                    _simulation.SetSimSpeed(ratioValue * 1, 1);

                    _grbx_expRealTimeMode.Visibility = Visibility.Visible;
                    _grbx_expFastForwardMode.Visibility = Visibility.Collapsed;
                    break;

                case EventDrivenSimulationTimeMode.FastForward:
                    _simulation.SetMaxSimSpeed();

                    _grbx_expRealTimeMode.Visibility = Visibility.Collapsed;
                    _grbx_expFastForwardMode.Visibility = Visibility.Visible;
                    break;
            }
        }


        private void _refreshReplicationsDoneCounters(int repsDone)
        {
            ++repsDone;
            // reps fast forward mode stas
            _txt_expCurrentReplication.Text = repsDone.ToString();

            _txt_repsDone.Value = repsDone.ToString();
        }

        private void _refreshReplicationsStats(NabytokReplicationsStatistics repStats)
        {
            Debug.WriteLine($"JKO_SP3 Replications done: {repStats.ObjednavkaTime.Count}");

            _sts_repsObjednavkaTime.Update(repStats.ObjednavkaTime);


            _sts_repsObjednavkaReceivedCount.Update(repStats.ObjednavkyRecieved);
            _sts_repsObjednavkaNotDoneCount.Update(repStats.ObjednavkyNotWorkingOn);

            var i = 0;
            var stolariStatsUCs = _lst_repsStolariTypes.Children.Cast<DicreteStatistic>().ToList();
            _lst_repsStolarTypes.Children.Cast<StolariUserControl>().ToList().ForEach(stolarUC =>
            {
                var stolarType = stolarUC.StolarType;
                var stolari = ((IAgentStolari)_simulation.FindAgent(Mc.GetAgentByStolarType(stolarType))).Stolari.Values.ToList();
                var ratios = repStats.StolarWorkTimeRatio[stolarType];
                var totalRatio = repStats.StolariWorkTimeRatio[stolarType];

                stolarUC.UpdateGUI(stolari, ratios, totalRatio);
                stolariStatsUCs[i].Update(totalRatio);

                ++i;
            });
        }

        private void _refreshExperimentStats()
        {
            var agentModelu = (AgentModelu)_simulation.FindAgent(SimId.AgentModelu);

            _sts_expObjednavkaTime.Update(agentModelu.ObjednavkaTotalTime);
        }
        private void _refreshTimeGUI()
        {
            _txt_expTime.Value = _simulation.CurrentTimeFormatted;
            _txt_expDay.Value = _simulation.CurrentTimeDayFormatted.ToString();
            _txt_expTimeRaw.Value = _simulation.CurrentTime.ToString("F2");
        }
        private void _refreshOrders()
        {
            var agentModelu = (AgentModelu)_simulation.FindAgent(SimId.AgentModelu);

            _txt_expOrdersCount.Value = agentModelu.ObjednavkyCount.ToString() ?? "";
            _txt_expOrdersDoneCount.Value = agentModelu.ObjednavkyDoneCount.ToString() ?? "";

            _lst_expOrders.Items.Clear();
            if (_chk_expOrdersRender.IsChecked == true)
            {
                for (int i = 0; i < agentModelu.ObjednavkyCount; i++)
                {
                    var objednavka = agentModelu.ObjednavkyTotal[i];

                    if (_lst_expOrders.Items.Count <= i)
                    {
                        _lst_expOrders.Items.Add(new ObjednavkaUserControl());
                    }

                    var ucObjednavka = (ObjednavkaUserControl)_lst_expOrders.Items[i];
                    ucObjednavka.Objednavka = objednavka;
                }
            }
        }

        private void _refreshWorkplaces()
        {
            var agentPracovisk = (AgentPracovisk)_simulation.FindAgent(SimId.AgentPracovisk);

            _txt_expWorkplacesCount.Value = agentPracovisk.Pracoviska.Count.ToString() ?? "";
            _txt_expWorkplacesOccupiedCount.Value = (agentPracovisk.Pracoviska.Count - agentPracovisk.FreePracoviska.Count).ToString() ?? "";

            if (_lst_expWorkplaces.Items.Count == 0)
                _lst_expWorkplaces.Items.Add(new PracoviskoUserControl());
            var ucSklad = (PracoviskoUserControl)_lst_expWorkplaces.Items[0];
            ucSklad.Pracovisko = Pracovisko.Sklad;

            for (int i = 1; i <= agentPracovisk.Pracoviska.Count; i++)
            {
                var pracovisko = agentPracovisk.Pracoviska[i - 1];

                if (_lst_expWorkplaces.Items.Count <= i)
                {
                    _lst_expWorkplaces.Items.Add(new PracoviskoUserControl());
                }

                var ucPracovisko = (PracoviskoUserControl)_lst_expWorkplaces.Items[i];
                ucPracovisko.Pracovisko = pracovisko;
            }
        }

        private void _refreshStolariExp()
        {
            var agents = new Dictionary<StolarType, IAgentStolari>()
            {
                { StolarType.A, (IAgentStolari)_simulation.FindAgent(SimId.AgentStolariA) },
                { StolarType.B, (IAgentStolari)_simulation.FindAgent(SimId.AgentStolariB) },
                { StolarType.C, (IAgentStolari)_simulation.FindAgent(SimId.AgentStolariC) },
            };

            _lst_expStolariTypes.Children.Cast<StolariUserControl>().ToList().ForEach(stolarUC =>
            {
                var stolarType = stolarUC.StolarType;
                var stolari = agents[stolarType].Stolari;

                stolarUC.UpdateGUI(stolari.Values.ToList());
            });
        }

        private void _refreshOperationsQueuesExp()
        {
            var agentStolarov = (AgentStolarov)_simulation.FindAgent(SimId.AgentStolarov);
            var agentPracovisk = (AgentPracovisk)_simulation.FindAgent(SimId.AgentPracovisk);

            _lst_expOperationsQueues.Children.Cast<OperationQueueUserControl>().ToList().ForEach(operationQueueUC =>
            {
                var nabytokOperation = operationQueueUC.NabytokOperation;
                var items = nabytokOperation == NabytokOperation.PriradovaniePracoviska
                    ? agentPracovisk.WaitingForPracovisko.ToList()
                    : agentStolarov.OperationsQueues[nabytokOperation].ToList();

                operationQueueUC._updateGUI(items);
            });
        }
        #endregion
        #region GUI events

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

        private void _btn_simStart_Click(object sender, RoutedEventArgs e)
        {
            var repsCount = _txt_simReplicationsCount.IntValue;
            _txt_expTotalReplications.Text = repsCount.ToString();

            _initializeSimulationFromGUI();
            _simulation.SimulateAsync(repsCount, _simulation.Endtime);
        }

        private void _btn_simStop_Click(object sender, RoutedEventArgs e)
        {
            _simulation.StopSimulation();
        }

        private void _btn_simResume_Click(object sender, RoutedEventArgs e)
        {
            _simulation.ResumeSimulation();
        }

        private void _btn_simPause_Click(object sender, RoutedEventArgs e)
        {
            _simulation.PauseSimulation();
        }

        private void _chk_simMaxSpeed_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _setSimulationTimeFromGUI();
        }

        private void _cmbx_simRealTimeRatio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _setSimulationTimeFromGUI();
        }
    }
    #endregion GUI events
}