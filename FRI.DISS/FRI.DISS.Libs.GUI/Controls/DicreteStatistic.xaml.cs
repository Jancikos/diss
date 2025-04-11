using FRI.DISS.Libs.Generators;
using System;
using System.Collections.Generic;
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
using ScottPlot;
using ScottPlot.AxisPanels;
using ScottPlot.Plottables;
using System.Diagnostics;
using ScottPlot.Panels;

namespace FRI.DISS.Libs.GUI.Controls
{
    /// <summary>
    /// Interaction logic for DicreteStatistic.xaml
    /// </summary>
    public partial class DicreteStatistic : UserControl
    {
        public string Title
        {
            get { return _grbx_Header.Header.ToString() ?? ""; }
            set { _grbx_Header.Header = value; }
        }

        public bool PlotShow {get; set; }  = false;
        public string PlotTitle { get; set; } = "";
        public string PlotXLabel { get; set; } = "";
        public string PlotYLabel { get; set; } = "";

        public int SkipFirstCount = 120;
        protected DataLogger? _dataLoggerMean;
        protected DataLogger? _dataLoggerIntervalMin;
        protected DataLogger? _dataLoggerIntervalMax;
        protected Statistics? _plotStatsMean;
        protected Statistics? _plotStatsIntervalMin;
        protected Statistics? _plotStatsIntervalMax;

        protected bool PlotHasSkippedFirst = false;
        protected bool PlotShowingInterval = false;
        public int LastRenderedCount { get; protected set; } = 0;

        public bool TransformFromSecondsToHours { get; set; } = false;
        public bool TransformToPercentage { get; set; } = false;

        public DicreteStatistic()
        {
            InitializeComponent();

            Loaded += DicreteStatistic_Loaded;
        }

        private void DicreteStatistic_Loaded(object sender, RoutedEventArgs e)
        {
            if (PlotShow)
            {
                InitializePlot();
            }
        }
        
        private void _addPlotSample(Statistics value)
        {  
            if (_dataLoggerMean is null || value.Count < LastRenderedCount)
            {
                _reinitializePlot();
            }

            if (value.CanCalculateInterval && !PlotShowingInterval)
            {
                _reinitializePlotInterval();
            }

            if (value.Count > SkipFirstCount && !PlotHasSkippedFirst)
            {
                _skipFirstPlotSamples();
            }

            double transformedValueY = _transformValue(value.Mean);
            double valueX = value.Count;
            _plotStatsMean!.AddSample(transformedValueY);
            _dataLoggerMean!.Add(valueX, transformedValueY);

            if (value.CanCalculateInterval)
            {
                _dataLoggerIntervalMin!.Add(valueX, _transformValue(value.IntervalLowerBound));
                _dataLoggerIntervalMax!.Add(valueX, _transformValue(value.IntervalUpperBound));

                _plotStatsIntervalMin!.AddSample(_transformValue(value.IntervalLowerBound));
                _plotStatsIntervalMax!.AddSample(_transformValue(value.IntervalUpperBound));
            }

             _plot.Plot.Axes.SetLimitsX(0, valueX);
            _plot.Plot.Axes.SetLimitsY(
                value.CanCalculateInterval ? _plotStatsIntervalMin!.Min : _plotStatsMean.Min,
                value.CanCalculateInterval ? _plotStatsIntervalMax!.Max : _plotStatsMean.Max
            );

            _plot.Refresh();
        }

        private void _skipFirstPlotSamples()
        {
            _plot.Plot.Clear();

            if (PlotShowingInterval)
            {
                _reinitializePlotInterval();
            }
            else
            {
                _reinitializePlot();
            }

            PlotHasSkippedFirst = true;
        }

        private void _reinitializePlotInterval()
        {
            _reinitializePlot();

            _dataLoggerIntervalMin = _plot.Plot.Add.DataLogger();
            _dataLoggerIntervalMax = _plot.Plot.Add.DataLogger();

            _plotStatsIntervalMin = new Statistics();
            _plotStatsIntervalMax = new Statistics();

            PlotShowingInterval = true;
        }

        private void _reinitializePlot()
        {
            _plot.Plot.Clear();

            _dataLoggerMean = _plot.Plot.Add.DataLogger();
            
            _plotStatsMean = new Statistics();

            PlotShowingInterval = false;
            PlotHasSkippedFirst = false;
        }

        public void InitializePlot()
        {
            _plot.Visibility = Visibility.Visible;

            _plot.Plot.Title(PlotTitle);
            _plot.Plot.XLabel(PlotXLabel);
            _plot.Plot.YLabel(PlotYLabel);

            PlotShow = true;
        }

        public void Update(Statistics stats)
        {
            if (LastRenderedCount == stats.Count || stats.Count == 0)
                return;

            _txt_Count.Value = stats.Count.ToString();
            _txt_Min.Value = _transformValue(stats.Min).ToString("F2");
            _txt_Max.Value = _transformValue(stats.Max).ToString("F2");
            _txt_Mean.Value = _transformValue(stats.Mean).ToString("F2");
            
            _txt_SampleStdDev.Value = _transformValue(stats.SampleStandardDeviation).ToString("F2");

            _txt_Interval.Title = $"Interval ({stats.CurrentIntervalT_alfa.ToString().Split("_").Last()}%)";
            _txt_Interval.Value = stats.CanCalculateInterval 
                ? $"<{_transformValue(stats.IntervalLowerBound):F2}; {_transformValue(stats.IntervalUpperBound):F2}>"
                : "--";
            
            if (PlotShow)
            {
                _addPlotSample(stats);
            }

            LastRenderedCount = stats.Count;
        }

        private double _transformValue(double value)
        {
            if (TransformFromSecondsToHours)
            {
                return value / 60.0 / 60.0;
            }

            if (TransformToPercentage)
            {
                return value * 100.0;
            }

            return value;
        }

        public void Clear()
        {
            LastRenderedCount = 0;

            _txt_Count.Value = "0";
            _txt_Min.Value = "0";
            _txt_Max.Value = "0";
            _txt_Mean.Value = "0";
            _txt_SampleStdDev.Value = "0";
            _txt_Interval.Value = "-";

            _reinitializePlot();
            _plot.Refresh();
        }
    }
}
