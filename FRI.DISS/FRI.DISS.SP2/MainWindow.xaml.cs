using FRI.DISS.Libs.Generators;
using FRI.DISS.Libs.GUI.Controls;
using FRI.DISS.Libs.Simulations.EventDriven;
using FRI.DISS.SP2.Controls;
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
        protected NabytokSimulation _simulation;

        public MainWindow()
        {
            InitializeComponent();

            _simulation = new NabytokSimulation();
            _simulation.GUIEventHappened += _simulation_GUIEventHappened;

            _initializeGUI();
        }

        private void _initializeGUI()
        {
            // cmbx timeRatio
            _cmbx_simRealTimeRatio.ItemsSource = Enum.GetValues(typeof(EventDrivenSimulationRealTimeRatios)).Cast<EventDrivenSimulationRealTimeRatios>();
            // _cmbx_simRealTimeRatio.SelectedIndex = 0;
            _cmbx_simRealTimeRatio.SelectedIndex = 9;


            // stolari types
            Enum.GetValues(typeof(NabytokSimulation.StolarType)).Cast<NabytokSimulation.StolarType>().ToList().ForEach(stolarType =>
            {
                _lst_expStolariTypes.Children.Add(new StolariUserControl() { StolarType = stolarType });
                _lst_expStolariTypesQueues.Children.Add(new StolariQueueUserControl() { StolarType = stolarType });

                _lst_repsStolariTypes.Children.Add(new DicreteStatistic() { Title = $"Vyťaženie stolárov {stolarType} (%)", PlotShow = true, TransformToPercentage = true });
                _lst_repsStolarTypes.Children.Add(new StolariUserControl() { StolarType = stolarType });
            });
        }

        private void _initializeSimulationFromGUI()
        {
            _simulation.ReplicationsCount = _txt_simReplicationsCount.IntValue;

            _simulation.SeedGenerator = _txt_simSeed.HasValue
                ? new SeedGenerator(_txt_simSeed.IntValue)
                : SeedGenerator.Global;

            _setSimulationTimeFromGUI();
            _txt_repsStartTime.Value = DateTime.Now.ToString("HH:mm:ss");
            _txt_repsEndTime.Value = "--:--:--";

            // stolari count
            _simulation.StolariCount[NabytokSimulation.StolarType.A] = _txt_simStolariACount.IntValue;
            _simulation.StolariCount[NabytokSimulation.StolarType.B] = _txt_simStolariBCount.IntValue;
            _simulation.StolariCount[NabytokSimulation.StolarType.C] = _txt_simStolariCCount.IntValue;
        }

        private void _setSimulationTimeFromGUI()
        {
            var mode = _chk_simMaxSpeed.IsChecked == true
                ? EventDrivenSimulationTimeMode.FastForward
                : EventDrivenSimulationTimeMode.RealTime;

            _simulation.TimeMode = mode;

            switch (mode)
            {
                case EventDrivenSimulationTimeMode.RealTime:
                    _simulation.RealTimeRatio = (EventDrivenSimulationRealTimeRatios)_cmbx_simRealTimeRatio.SelectedItem;
                    _grbx_expRealTimeMode.Visibility = Visibility.Visible;
                    _grbx_expFastForwardMode.Visibility = Visibility.Collapsed;
                    break;

                case EventDrivenSimulationTimeMode.FastForward:
                    _grbx_expRealTimeMode.Visibility = Visibility.Collapsed;
                    _grbx_expFastForwardMode.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void _simulation_GUIEventHappened(object? sender, EventDrivenSimulationGUIEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                switch (e.Type)
                {
                    case EventDrivenSimulationEventArgsType.RefreshTime:
                        _refreshTime();
                        break;
                    case EventDrivenSimulationEventArgsType.SimulationEventDone:
                    case EventDrivenSimulationEventArgsType.RefreshGUI:
                        _refreshExperimentGUI();
                        break;
                    case EventDrivenSimulationEventArgsType.SimulationStarted:
                        _clearExperimentGUI();
                        _clearReplicationsGUI();
                        break;
                    case EventDrivenSimulationEventArgsType.SimulationExperimentDone:
                        _refreshReplicationsStats();
                        break;
                }
            });
        }

        private void _clearReplicationsGUI()
        {
            _lst_repsStolarTypes.Children.Cast<StolariUserControl>().ToList().ForEach(stolariUc =>
            {
                stolariUc.Clear();
            });
        }

        private void _clearExperimentGUI()
        {
            _lst_expWorkplaces.Items.Clear();
            _sts_expObjednavkaTime.Clear();
        }

        private void _refreshReplicationsStats()
        {
            // reps fast forward mode stas
            _txt_expCurrentReplication.Text = _simulation.ReplicationsDone.ToString();
            _txt_expTotalReplications.Text = _simulation.ReplicationsCount.ToString();

            if (_simulation.ReplicationsDone  % _txt_simReplicationsStatsRefresh.IntValue != 0 && _simulation.ReplicationsDone  != _simulation.ReplicationsCount && _simulation.State == Libs.Simulations.SimulationState.Running)
                return;
            
            if (_simulation.ReplicationsDone  == _simulation.ReplicationsCount || _simulation.State != Libs.Simulations.SimulationState.Running)
            {
                _txt_repsEndTime.Value = DateTime.Now.ToString("HH:mm:ss");
            }

            _txt_repsDone.Value = _simulation.ReplicationsDone.ToString();

            _sts_repsObjednavkaTime.Update(_simulation.ReplicationsStatistics.ObjednavkaTime);

            _sts_repsObjednavkaReceivedCount.Update(_simulation.ReplicationsStatistics.ObjednavkyRecieved);
            _sts_repsObjednavkaNotDoneCount.Update(_simulation.ReplicationsStatistics.ObjednavkyNotWorkingOn);

            var i = 0;
            var stolariStatsUCs = _lst_repsStolariTypes.Children.Cast<DicreteStatistic>().ToList();
            _lst_repsStolarTypes.Children.Cast<StolariUserControl>().ToList().ForEach(stolarUC =>
            {
                var stolarType = stolarUC.StolarType;
                var stolari = _simulation.ExperimentData.Stolari[stolarType];
                var ratios = _simulation.ReplicationsStatistics.StolarWorkTimeRatio[stolarType];
                var totalRatio = _simulation.ReplicationsStatistics.StolariWorkTimeRatio[stolarType];

                stolarUC.UpdateGUI(stolari, ratios, totalRatio);
                stolariStatsUCs[i].Update(totalRatio);

                ++i;
            });
        }

        private void _refreshExperimentGUI()
        {
            switch (_simulation.TimeMode)
            {
                case EventDrivenSimulationTimeMode.RealTime:
                    _txt_expStatus.Value = _simulation.State.ToString();
                    _txt_expReplication.Value = _simulation.ReplicationsDone.ToString();

                    _refreshTime();
                    _refreshEventsCalendar();
                    _refreshWorkplaces();
                    _refreshStolariExp();
                    _refreshExperimentStats();
                    break;
            }
        }

        private void _refreshExperimentStats()
        {
            _sts_expObjednavkaTime.Update(_simulation.ExperimentStatistics.ObjednavkaTotalTime);
        }

        private void _refreshStolariExp()
        {
            _lst_expStolariTypes.Children.Cast<StolariUserControl>().ToList().ForEach(stolarUC =>
            {
                var stolarType = stolarUC.StolarType;
                var stolari = _simulation.ExperimentData.Stolari[stolarType];

                stolarUC.UpdateGUI(stolari);
            });

            _lst_expStolariTypesQueues.Children.Cast<StolariQueueUserControl>().ToList().ForEach(stolariQueueUC =>
            {
                var stolarType = stolariQueueUC.StolarType;
                var objednavky = _simulation.ExperimentData.StolariQueues[stolarType];

                stolariQueueUC._updateGUI(objednavky.ToList());
            });
        }

        private void _refreshWorkplaces()
        {
            _txt_expWorkplacesCount.Value = _simulation.ExperimentData.Workplaces.Count.ToString();
            _txt_expWorkplacesOccupiedCount.Value = _simulation.ExperimentData.ObjednavkyInSystem.ToString();

            for (int i = 0; i < _simulation.ExperimentData.Workplaces.Count; i++)
            {
                var objednavka = _simulation.ExperimentData.Workplaces[i];

                if (_lst_expWorkplaces.Items.Count <= i)
                {
                    _lst_expWorkplaces.Items.Add(new WorkPlaceUserControl() { Id = i + 1 });
                }

                var ucWorkplace = (WorkPlaceUserControl)_lst_expWorkplaces.Items[i];

                ucWorkplace.Objednavka = objednavka;
            }
        }

        private void _refreshEventsCalendar()
        {
            _trv_expEventsCalendar.Items.Clear();

            if (_chk_repEventsCalendarRender.IsChecked != true)
                return;

            var i = 0;
            foreach (var e in _simulation.EventsCalendar.GetOrderedEvents())
            {
                var item = new TreeViewItem()
                {
                    Header = $"{++i}. {e}",
                    IsExpanded = true,
                    VerticalContentAlignment = VerticalAlignment.Center
                };

                if (e is NabytokSimulation.NabytokSimulationEvent nabytokEvent)
                {
                    item.Items.Add(new TreeViewItem()
                    {
                        Header = $"O: {nabytokEvent.Objednavka}",
                        VerticalContentAlignment = VerticalAlignment.Center
                    });


                    item.Items.Add(new TreeViewItem()
                    {
                        Header = $"S: {nabytokEvent.Stolar}",
                        VerticalContentAlignment = VerticalAlignment.Center
                    });
                }

                _trv_expEventsCalendar.Items.Add(item);
            }
        }

        private void _refreshTime()
        {
            _txt_expTime.Value = _simulation.CurrentTimeFormatted;
            _txt_expDay.Value = _simulation.CurrentTimeDayFormatted.ToString();
            _txt_expTimeRaw.Value = _simulation.CurrentTime.ToString("F2");
        }

        private void _manipulateSimulation(Action action)
        {
            Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Dispatcher.Invoke(() =>
                        MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
                }
            });
        }

        #region  GUI Event Handlers
        private void _mnitem_StanicaSimWindow_Click(object sender, RoutedEventArgs e)
        {
            var window = new StanicaWindow();
            window.Show();
        }

        private void _mnitem_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _btn_simStart_Click(object sender, RoutedEventArgs e)
        {
            if (_simulation.State == Libs.Simulations.SimulationState.Running)
            {
                MessageBox.Show("Simulation is already running", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _initializeSimulationFromGUI();
            _manipulateSimulation(_simulation.StartSimulation);
        }

        private void _btn_simStop_Click(object sender, RoutedEventArgs e)
        {
            _manipulateSimulation(_simulation.StopSimulation);
            _refreshReplicationsStats();
        }

        private void _btn_simResume_Click(object sender, RoutedEventArgs e)
        {
            _manipulateSimulation(_simulation.ResumeSimulation);
        }

        private void _btn_simPause_Click(object sender, RoutedEventArgs e)
        {
            _manipulateSimulation(_simulation.PauseSimulation);
            _refreshReplicationsStats();
        }

        private void _cmbx_simRealTimeRatio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _setSimulationTimeFromGUI();
        }

        private void _chk_simMaxSpeed_SelectionChanged(object sender, RoutedEventArgs e)
        {
            _setSimulationTimeFromGUI();
        }
    }
    #endregion
}