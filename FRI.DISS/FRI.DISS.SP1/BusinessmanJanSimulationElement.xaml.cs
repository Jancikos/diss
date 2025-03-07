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
        protected DataLogger? _dataLoggerTotalCosts;
        protected IYAxis? _yAxisTotalCosts;

        protected DataLogger? _dataDailyCosts;
        protected IYAxis? _yAxisDailyCosts;

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
            _plotTotal.Plot.Title("Avg. total costs through replications");
            _plotTotal.Plot.XLabel("Replications done", 12);
            _yAxisTotalCosts = _plotTotal.Plot.Axes.Left;
            _yAxisTotalCosts.Label.Text = "Total costs";
            _yAxisTotalCosts.Label.FontSize = 12;
            _yAxisTotalCosts.Label.ForeColor = ScottPlot.Color.FromColor(System.Drawing.Color.Red);

            _plotDaily.Plot.Title("Cumulated daily costs of the first replication");
            _plotDaily.Plot.XLabel("Day", 12);
            _yAxisDailyCosts = _plotDaily.Plot.Axes.Left;
            _yAxisDailyCosts.Label.Text = "Daily costs";
            _yAxisDailyCosts.Label.FontSize = 12;
            _yAxisDailyCosts.Label.ForeColor = ScottPlot.Color.FromColor(System.Drawing.Color.Blue);
        }

        public void StartSimulation()
        {
            if (Simulation == null)
            {
                throw new InvalidOperationException("Simulation not set");
            }
            Dispatcher.Invoke(_clearGUI);

            _plotTotal.Plot.Clear();

            _dataLoggerTotalCosts = _plotTotal.Plot.Add.DataLogger();
            _dataLoggerTotalCosts.Axes.YAxis = _yAxisTotalCosts!;
            _dataLoggerTotalCosts.Color = _yAxisTotalCosts!.Label.ForeColor;

            var axis = (RightAxis)_plotTotal.Plot.Axes.Right;
            axis.Color(_dataLoggerTotalCosts.Color);

            _plotDaily.Plot.Clear();

            _dataDailyCosts = _plotDaily.Plot.Add.DataLogger();
            _dataDailyCosts.Axes.YAxis = _yAxisDailyCosts!;
            _dataDailyCosts.Color = _yAxisDailyCosts!.Label.ForeColor;

            var axisDaily = (RightAxis)_plotDaily.Plot.Axes.Right;
            axisDaily.Color(_dataDailyCosts.Color);

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
                    _dataLoggerTotalCosts!.Add(repDone, Simulation.ProcessExperimentResult());
                    _plotTotal.Refresh();
                });
            };

            Simulation.UpdateStatsDailyCallback = (week, day, totalCost) =>
            {
                Dispatcher.Invoke(() =>
                {
                    _dataDailyCosts!.Add(week * 7 + day + 1, totalCost);
                    _plotDaily.Refresh();
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
            // clear plots
            _plotTotal.Plot.Clear();
            _plotDaily.Plot.Clear();

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
            _txt_StatsMeanItemsLeftInStock.Value = Simulation?.ResultWarehouseCosts?.MeanToString(true) ?? "0";
            _txt_StatsMeanMissingDemand.Value = Simulation?.ResultMissingDemandPenalty?.MeanToString(true) ?? "0";
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
