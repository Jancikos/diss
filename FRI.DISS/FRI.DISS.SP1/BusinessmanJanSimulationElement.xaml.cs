using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        protected DataLogger? _dataLogger;
        protected Stopwatch _stopwatch = new();

        public string Title
        {
            get { return _txt_Title.Header.ToString() ?? ""; }
            set { _txt_Title.Header = value; }
        }

        public BusinessmanJanSimulationElement()
        {
            InitializeComponent();

        }

        public void StartSimulation()
        {
            if (Simulation == null)
            {
                throw new InvalidOperationException("Simulation not set");
            }

            Dispatcher.Invoke(_clearGUI);

            _dataLogger = _plot.Plot.Add.DataLogger();

            var axis = (RightAxis)_plot.Plot.Axes.Right;
            axis.Color(_dataLogger.Color);


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
                    _dataLogger.Add(repDone, Simulation.ProcessExperimentResult());
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
            _txt_StatsMean.Value = "0";
            _txt_StatsVariance.Value = "0";
            _txt_StatsMax.Value = "0";
            _txt_StatsMin.Value = "0";
        }

        private void _updateStats()
        {
            var stats = Simulation?.ResultRaw;
            if (stats == null)
            {
                return;
            }

            _txt_RepDone.Value = stats.Count.ToString();
            _txt_StatsMean.Value = stats.Mean.ToString("0.####");
            _txt_StatsVariance.Value = stats.Variance.ToString("0.####");
            _txt_StatsMax.Value = stats.Max.ToString("0.##");
            _txt_StatsMin.Value = stats.Min.ToString("0.##");
        }

        public void StopSimulation()
        {

            Simulation?.StopSimulation();
        }
    }
}
