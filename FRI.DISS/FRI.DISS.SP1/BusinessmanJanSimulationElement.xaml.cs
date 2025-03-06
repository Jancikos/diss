using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Versioning;
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
using FRI.DISS.Libs.MonteCarlo;
using ScottPlot;
using ScottPlot.AxisPanels;
using ScottPlot.Plottables;

namespace FRI.DISS.SP1
{
    /// <summary>
    /// Interaction logic for BusinessmanJanSimulationElement.xaml
    /// </summary>
    public partial class BusinessmanJanSimulationElement : UserControl
    {
        public BusinessmanJan? Simulation { get; set; }

        public int SkipFirstCount = 0;
        protected DataLogger? _dataLoggerCosts;
        protected IYAxis? _yAxisCosts;
        protected DataLogger? _dataLoggerSuppliersReliability;
        protected IYAxis? _yAxisSuppliersReliability;
        protected DataLogger? _dataLoggerWerehousesItemsLeft;
        protected IYAxis? _yAxisWerehousesItemsLeft;
        protected DataLogger? _dataLoggerMissingDemandItemsCount;
        protected IYAxis? _yAxisMissingDemandItemsCount;

        protected Stopwatch _stopwatch = new();

        public string Title
        {
            get { return _txt_Title.Header.ToString() ?? ""; }
            set { _txt_Title.Header = value; }
        }

        public bool IsActive
        {
            get { return _chkbx_Active.IsChecked == true; }
            set { _chkbx_Active.IsChecked = value; }
        }

        public BusinessmanJanSimulationElement()
        {
            InitializeComponent();

            _initializePlot();
            _clearGUI();
        }

        private void _initializePlot()
        {
            _yAxisCosts = _plot.Plot.Axes.Left;
            _yAxisCosts.Label.Text = "Total costs";
            _yAxisCosts.Label.FontSize = 12;
            _yAxisCosts.Label.ForeColor = ScottPlot.Color.FromColor(System.Drawing.Color.Black);

            _yAxisSuppliersReliability = _plot.Plot.Axes.AddLeftAxis();
            _yAxisSuppliersReliability.Label.Text = "Suppliers Reliability";
            _yAxisSuppliersReliability.Label.FontSize = 12;
            _yAxisSuppliersReliability.Label.ForeColor = ScottPlot.Color.FromColor(System.Drawing.Color.Blue);

            _yAxisWerehousesItemsLeft = _plot.Plot.Axes.AddRightAxis();
            _yAxisWerehousesItemsLeft.Label.Text = "Werehouses Items Left";
            _yAxisWerehousesItemsLeft.Label.FontSize = 12;
            _yAxisWerehousesItemsLeft.Label.ForeColor = ScottPlot.Color.FromColor(System.Drawing.Color.Orange);

            _yAxisMissingDemandItemsCount = _plot.Plot.Axes.AddRightAxis();
            _yAxisMissingDemandItemsCount.Label.Text = "Missing Demand Items";
            _yAxisMissingDemandItemsCount.Label.FontSize = 12;
            _yAxisMissingDemandItemsCount.Label.ForeColor = ScottPlot.Color.FromColor(System.Drawing.Color.Red);
        }

        public void StartSimulation()
        {
            if (Simulation == null)
            {
                throw new InvalidOperationException("Simulation not set");
            }
            Dispatcher.Invoke(_clearGUI);

            _plot.Plot.Clear();

            _dataLoggerCosts = _plot.Plot.Add.DataLogger();
            _dataLoggerCosts.Axes.YAxis = _yAxisCosts!;
            _dataLoggerCosts.Color = _yAxisCosts!.Label.ForeColor;

            _dataLoggerSuppliersReliability = _plot.Plot.Add.DataLogger();
            _dataLoggerSuppliersReliability.Axes.YAxis = _yAxisSuppliersReliability!;
            _dataLoggerSuppliersReliability.Color = _yAxisSuppliersReliability!.Label.ForeColor;

            _dataLoggerWerehousesItemsLeft = _plot.Plot.Add.DataLogger();
            _dataLoggerWerehousesItemsLeft.Axes.YAxis = _yAxisWerehousesItemsLeft!;
            _dataLoggerWerehousesItemsLeft.Color = _yAxisWerehousesItemsLeft!.Label.ForeColor;

            _dataLoggerMissingDemandItemsCount = _plot.Plot.Add.DataLogger();
            _dataLoggerMissingDemandItemsCount.Axes.YAxis = _yAxisMissingDemandItemsCount!;
            _dataLoggerMissingDemandItemsCount.Color = _yAxisMissingDemandItemsCount!.Label.ForeColor;

            var axis = (RightAxis)_plot.Plot.Axes.Right;
            axis.Color(_dataLoggerCosts.Color);

            Simulation.UpdateStatsCallback = (mc, repDone, resultRaw) =>
            {
                if (repDone < SkipFirstCount)
                {
                    return;
                }

                Dispatcher.Invoke(() =>
                {
                    _updateStats();

                    // update plot
                    _dataLoggerCosts!.Add(repDone, Simulation.ProcessExperimentResult());
                    _dataLoggerSuppliersReliability!.Add(repDone, Simulation.ResultSuplliersReliability!.Mean);
                    _dataLoggerWerehousesItemsLeft!.Add(repDone, Simulation.ResultWarehouseItemsLeftCount!.Mean);
                    _dataLoggerMissingDemandItemsCount!.Add(repDone, Simulation.ResultMissingDemandItemsCount!.Mean);

                    _plot.Refresh();
                });
            };

            _startTimer();

            Simulation.RunSimulation();
        }

        private void _startTimer()
        {
            _stopwatch.Restart();
            Task.Run(() =>
            {
                while (true)
                {
                    if (Simulation?.State == MonteCarloState.Done)
                    {
                        break;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        _txt_TimeElapsed.Value = _stopwatch.Elapsed.ToString(@"mm\:ss\:fff");
                    });
                    Task.Delay(100).Wait();
                }
                _stopwatch.Stop();
            });
        }

        private void _clearGUI()
        {
            // clear plot
            _plot.Plot.Clear();

            // clear stats
            _txt_TimeElapsed.Value = "00:00:000";
            _txt_RepDone.Value = "0";
            _txt_StatsMeanTotalCost.Value = "0";
            _txt_StatsMeanItemsLeftInStock.Value = "0";
            _txt_StatsMeanMissingDemand.Value = "0";
            _txt_StatsMeanSuppliersReliabily.Value = "0";
        }

        private void _updateStats()
        {
            _txt_RepDone.Value = Simulation?.ResultRaw?.Count.ToString() ?? "0";
            _txt_StatsMeanTotalCost.Value = Simulation?.ResultRaw?.MeanToString(true) ?? "0";
            _txt_StatsMeanItemsLeftInStock.Value = Simulation?.ResultWarehouseItemsLeftCount?.MeanToString(true) ?? "0";
            _txt_StatsMeanMissingDemand.Value = Simulation?.ResultMissingDemandItemsCount?.MeanToString(true) ?? "0";
            _txt_StatsMeanSuppliersReliabily.Value = Simulation?.ResultSuplliersReliability?.MeanToString(true) ?? "0";
        }

        public void StopSimulation()
        {
            Simulation?.StopSimulation();
        }

        #region GUI Events

        private void _btn_Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StartSimulation();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void _btn_Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StopSimulation();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
    #endregion
}
