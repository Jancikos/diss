using FRI.DISS.Libs.Generators;
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
            _cmbx_simRealTimeRatio.SelectedIndex = 6;

        }

        private void _initializeSimulationFromGUI()
        {
            _simulation.ReplicationsCount = _txt_simReplicationsCount.IntValue;

            _simulation.SeedGenerator = _txt_simSeed.HasValue
                ? new SeedGenerator(_txt_simSeed.IntValue)
                : SeedGenerator.Global;

            _setSimulationTimeFromGUI();

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

            if (mode == EventDrivenSimulationTimeMode.RealTime)
            {
                _simulation.RealTimeRatio = (EventDrivenSimulationRealTimeRatios)_cmbx_simRealTimeRatio.SelectedItem;
            }
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
                    case EventDrivenSimulationEventArgsType.SimulationEventDone:
                        _refreshEventsCalendar();
                        break;
                }
            });
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
                    IsExpanded = true
                };

                if (e is NabytokSimulation.NabytokSimulationEvent nabytokEvent)
                {
                    item.Items.Add(new TreeViewItem()
                    {
                        Header = $"O: {nabytokEvent.Objednavka}"
                    });

                    item.Items.Add(new TreeViewItem()
                    {
                        Header = $"S: {nabytokEvent.Stolar}"
                    });
                }

                _trv_expEventsCalendar.Items.Add(item);
            }
        }

        private void _refreshTime()
        {
            _txt_expTime.Value = _simulation.CurrentTimeFormatted;
            _txt_expDay.Value = _simulation.CurrentTimeDayFormatted.ToString();
        }

        private void _manipulateSimulation(Action action)
        {
            try
            {
                Task.Run(action);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() =>
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error));
            }
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
        }

        private void _btn_simResume_Click(object sender, RoutedEventArgs e)
        {
            _manipulateSimulation(_simulation.ResumeSimulation);
        }

        private void _btn_simPause_Click(object sender, RoutedEventArgs e)
        {
            _manipulateSimulation(_simulation.PauseSimulation);
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