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

        public bool ShowPlot {get; set; }  = false;

        public int SkipFirstCount = 0;
        protected DataLogger? _dataLoggerMean;
        protected Statistics? _plotStatsMean;

        public int LastRenderedCount { get; protected set; } = 0;


        public DicreteStatistic()
        {
            InitializeComponent();

            Loaded += DicreteStatistic_Loaded;
        }

        private void DicreteStatistic_Loaded(object sender, RoutedEventArgs e)
        {
            if (ShowPlot)
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

            _plotStatsMean!.AddSample(value.Mean);

            _dataLoggerMean!.Add(_plotStatsMean.Count, value.Mean);

            _plot.Plot.Axes.SetLimitsX(0, _plotStatsMean.Count);
            _plot.Plot.Axes.SetLimitsY(0, _plotStatsMean.Max);

            _plot.Refresh();
        }

        private void _reinitializePlot()
        {
            _plot.Plot.Clear();

            _dataLoggerMean = _plot.Plot.Add.DataLogger();
            _plotStatsMean = new Statistics();
        }

        public void InitializePlot()
        {
            _plot.Visibility = Visibility.Visible;
            ShowPlot = true;
        }

        public void Update(Statistics stats)
        {
            if (LastRenderedCount == stats.Count)
                return;

            _txt_Count.Value = stats.Count.ToString();
            _txt_Min.Value = stats.Min.ToString("F2");
            _txt_Max.Value = stats.Max.ToString("F2");
            _txt_Avg.Value = stats.Mean.ToString("F2");
            _txt_StdDev.Value = stats.StandardDeviation.ToString("F2");
            _txt_Variance.Value = stats.Variance.ToString("F2");
            
            if (ShowPlot)
            {
                _addPlotSample(stats);
            }

            LastRenderedCount = stats.Count;
        }

        public void Clear()
        {
            LastRenderedCount = 0;
            _txt_Count.Value = "0";
            _txt_Min.Value = "0";
            _txt_Max.Value = "0";
            _txt_Avg.Value = "0";
            _txt_StdDev.Value = "0";
            _txt_Variance.Value = "0";

            _reinitializePlot();
        }
    }
}
