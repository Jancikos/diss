using System.IO;
using System.Windows;
using System.Windows.Controls;
using FRI.DISS.Libs.Generators;
using FRI.DISS.Libs.MonteCarlo;
using Microsoft.Win32;

namespace FRI.DISS.SP1
{
    public enum SimulationsType
    {
        DefinedStrategies,
        KostorStrategies,
        CustomStrategy
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<BusinessmanJanSimulationElement>[] _simulationElements;

        public SimulationsType SimulationsType
        {
            get { return (SimulationsType)_cmbx_SimulationType.SelectedItem; }
        }
        protected FileInfo? _loadedSuppliersStrategyConfig;


        public MainWindow()
        {
            InitializeComponent();

            _simulationElements = new List<BusinessmanJanSimulationElement>[Enum.GetValues(typeof(SimulationsType)).Length];

            _initializeGUI();
        }

        private void _initializeGUI()
        {
            // init simulation elements
            _simulationElements[(int)SimulationsType.DefinedStrategies] = new List<BusinessmanJanSimulationElement>(){
                _simStrategyA,
                _simStrategyB,
                _simStrategyC,
                _simStrategyD
            };
            _simStrategyA.Simulation = new BusinessmanJanStrategyA();
            _simStrategyB.Simulation = new BusinessmanJanStrategyB();
            _simStrategyC.Simulation = new BusinessmanJanStrategyC();
            _simStrategyD.Simulation = new BusinessmanJanStrategyD();

            // init kostor strategies
            _simulationElements[(int)SimulationsType.KostorStrategies] = new List<BusinessmanJanSimulationElement>(){
                _simStrategyKostorA,
                _simStrategyKostorB,
                _simStrategyKostorC,
                _simStrategyKostorD
            };

            _simStrategyKostorA.Simulation = new BusinessmanJanStrategyKostorA();
            _simStrategyKostorB.Simulation = new BusinessmanJanStrategyKostorB();
            _simStrategyKostorC.Simulation = new BusinessmanJanStrategyKostorC();
            _simStrategyKostorD.Simulation = new BusinessmanJanStrategyKostorD();

            // init custom strategy
            _simulationElements[(int)SimulationsType.CustomStrategy] = new List<BusinessmanJanSimulationElement>(){
                _simStrategyCustom
            };
            _simStrategyCustom.Simulation = new BusinessmanJanCustomStrategy();

            // Initialize ComboBox
            foreach (SimulationsType type in Enum.GetValues(typeof(SimulationsType)))
            {
                _cmbx_SimulationType.Items.Add(type);
            }
            _cmbx_SimulationType.SelectedIndex = (int)SimulationsType.DefinedStrategies;
        }

        private void _mnitem_Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _btn_Simulation_Start_Click(object sender, RoutedEventArgs e)
        {
            var seed = _txtbx_SimulationSeed.IntValue;
            var replicationCount = _txtbx_SimulationRepliations.IntValue;
            var renderUpdateStatsInterval = _txtbx_SimulationRenderInterval.IntValue;
            var renderSkipFirst = _txtbx_SimulationRenderSkipFirst.IntValue;

            foreach (var simElement in _simulationElements[(int)SimulationsType])
            {
                if (!simElement.IsActive)
                {
                    continue;
                }

                if (simElement.Simulation == null)
                {
                    throw new InvalidOperationException("Simulation not set");
                }

                simElement.Simulation.SeedGenerator = new SeedGenerator(seed);
                simElement.Simulation.ReplicationsCount = replicationCount;
                simElement.Simulation.UpdateStatsInterval = renderUpdateStatsInterval;
                simElement.SkipFirstCount = renderSkipFirst;

                if (SimulationsType == SimulationsType.CustomStrategy)
                {
                    ((BusinessmanJanCustomStrategy)simElement.Simulation).SuppliersStrategyConfig = _loadedSuppliersStrategyConfig;
                }

                Task.Run(() =>
                {
                    try
                    {
                        simElement.StartSimulation();
                    }
                    catch (Exception ex)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show($"Error starting simulation: {ex.Message}", "Simulation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                });
            }

        }

        private void _btn_Simulation_Stop_Click(object sender, RoutedEventArgs e)
        {
            foreach (var simElement in _simulationElements[(int)SimulationsType])
            {
                try
                {
                    if (!simElement.IsActive)
                    {
                        continue;
                    }
                    simElement.StopSimulation();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error stopping simulation {simElement.Title}: {ex.Message}", "Simulation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void _cmbx_SimulationType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedType = (SimulationsType)_cmbx_SimulationType.SelectedItem;

            for (int i = 0; i < _simulationElements.Length; i++)
            {
                foreach (var simElement in _simulationElements[i])
                {
                    simElement.Visibility = i == (int)selectedType
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }

            _grbx_SupplierStrategyConfigFile.Visibility = selectedType == SimulationsType.CustomStrategy
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void _btn_LoadSuppliersStrategyConfig_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
                Title = "Select suppliers strategy configuration"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            _loadedSuppliersStrategyConfig = new FileInfo(dialog.FileName);
            _txt_SupplierStrategyConfigFile.Text = _loadedSuppliersStrategyConfig.FullName;
        }
    }
}