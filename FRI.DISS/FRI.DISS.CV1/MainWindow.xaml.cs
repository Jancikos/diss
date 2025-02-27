using FRI.DISS.Libs.MonteCarlo;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FRI.DISS.CV1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MonteCarlo? _simulation;

        Scatter? _myScatter;
        Crosshair MyCrosshair;

        public MainWindow()
        {
            InitializeComponent();
            
            
            MyCrosshair = WpfPlot1.Plot.Add.Crosshair(0, 0);
            MyCrosshair.IsVisible = false;
            MyCrosshair.MarkerShape = MarkerShape.OpenCircle;
            MyCrosshair.MarkerSize = 15;
            
            WpfPlot1.MouseMove += (s, e) =>
            {
                // determine where the mouse is and get the nearest point
                Pixel mousePixel = new(e.GetPosition(WpfPlot1).X, e.GetPosition(WpfPlot1).Y);
                Coordinates mouseLocation = WpfPlot1.Plot.GetCoordinates(mousePixel);

                DataPoint? nearest = _myScatter?.Data.GetNearest(mouseLocation, WpfPlot1.Plot.LastRender, 15);

                // hide the crosshair when no point is selected
                if (nearest is null || !nearest.Value.IsReal && MyCrosshair.IsVisible)
                {
                    MyCrosshair.IsVisible = false;
                    WpfPlot1.Refresh();
                    Title = $"No point selected";

                    return;
                }

                // place the crosshair over the highlighted point
                if (nearest.Value.IsReal)
                {
                    MyCrosshair.IsVisible = true;
                    MyCrosshair.Position = nearest.Value.Coordinates;
                    WpfPlot1.Refresh();
                    Title = $"Selected Index={nearest.Value.Index}, X={nearest.Value.X:0}, Y={nearest.Value.Y:0.####}";
                }
            };
        }

        private void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            int repCount = int.Parse(_txtbx_RepCount.Value);
            int l = 5;
            int d = 10;

            _simulation = new BuffonNeedle()
            {
                D = d,
                L = l,
                UpdateStatsInterval = int.Parse(_txtbx_UpdateIntervalCount.Value)
            };

            WpfPlot1.Plot.Clear();
            WpfPlot1.Plot.Axes.SetLimitsY(2, 4);

            var buffonResults = new List<double>();
            var buffonIterations = new List<int>();

            _simulation.UpdateStatsCallback = (i, result) =>
            {
                Debug.WriteLine($"Iteration: {i}, Results: {result}");

                buffonResults.Add(result);
                buffonIterations.Add(i);

                Dispatcher.Invoke(() =>
                {
                    WpfPlot1.Plot.Clear();
                    _myScatter = WpfPlot1.Plot.Add.Scatter(buffonIterations.ToArray(), buffonResults.ToArray(), color: ScottPlot.Color.FromColor(System.Drawing.Color.Blue));

                    if (i > 1)
                    {
                        WpfPlot1.Plot.Axes.SetLimitsX(0, i);
                    }

                    WpfPlot1.Refresh();
                });
            };

            Task.Run(() =>
            {
                var piEst = _simulation.RunExperiment(repCount);
                Debug.WriteLine($"Runned iterations: {buffonIterations.Last()}");
                Debug.WriteLine($"Estimated Pi: {piEst}");
                // var piEst = (2 * l) / (p * d);
                MessageBox.Show($"Estimated Pi: {piEst}");
            });
        }

        private void Button_Plot_Click(object sender, RoutedEventArgs e)
        {
            double[] dataX = { 1, 2, 3, 4, 5 };
            double[] dataY = { 1, 4, 9, 16, 25 };

            WpfPlot1.Plot.Add.Scatter(dataX, dataY);
            WpfPlot1.Refresh();
        }

        private void Button_Stop_Click(object sender, RoutedEventArgs e)
        {
            try {
                _simulation?.StopExperiment();
            } catch (System.InvalidOperationException ex) {
                MessageBox.Show(ex.Message);
            }
        }
    }
}