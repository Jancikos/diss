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
using FRI.DISS.Libs.MonteCarlo;

namespace FRI.DISS.SP1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<BusinessmanJanSimulationElement> _simulationElements = new();

        public MainWindow()
        {
            InitializeComponent();

            _initialize();
        }

        private void _initialize()
        {   
            // init simulation elements
            _simulationElements.Add(_simStrategyA);
            _simStrategyA.Simulation = new BusinessmanJanStrategyA();

            _simulationElements.Add(_simStrategyB);
            _simStrategyB.Simulation = new BusinessmanJanStrategyB();

            _simulationElements.Add(_simStrategyC);
            _simStrategyC.Simulation = new BusinessmanJanStrategyC();

            _simulationElements.Add(_simStrategyD);
            _simStrategyD.Simulation = new BusinessmanJanStrategyD();
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

            foreach (var simElement in _simulationElements)
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

                Task.Run(() =>
                {
                    simElement.StartSimulation();
                });
            }

        }

        private void _btn_Simulation_Stop_Click(object sender, RoutedEventArgs e)
        {
            foreach (var simElement in _simulationElements)
            {
                try {
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
    }
}