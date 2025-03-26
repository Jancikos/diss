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

        public bool ShowPlot {get; protected set; }  = false;

        public int SkipFirstCount = 0;
        protected DataLogger? _dataLoggerMean;
        protected IYAxis? _yAxisMean;

        protected Statistics? _stats;
        public Statistics? Stats
        {
            get => _stats;
            set
            {
                _stats = value;
                Reload();

                if (ShowPlot && value is not null)
                {
                    _addPlotSample(value);
                }
            }
        }

        private void _addPlotSample(Statistics value)
        {
            if (_dataLoggerMean is null)
            {
                _reinitializePlot();
            }


            _dataLoggerMean!.Add(value.Count, value.Mean);
            _plot.Refresh();
        }

        private void _reinitializePlot()
        {
            _plot.Plot.Clear();

            _dataLoggerMean = _plot.Plot.Add.DataLogger();
            _plot.Plot.Axes.AutoScale();

            _dataLoggerMean.Axes.YAxis = _yAxisMean!;
            _dataLoggerMean.Color = _yAxisMean!.Label.ForeColor;


            var axis = (RightAxis)_plot.Plot.Axes.Right;
            axis.Color(_dataLoggerMean.Color);
        }

        public DicreteStatistic()
        {
            InitializeComponent();
        }
        
        public void InitializePlot()
        {
            _plot.Plot.Title(Title);
            // _plot.Plot.XLabel("Replications done", 12);
            _yAxisMean = _plot.Plot.Axes.Left;
            _yAxisMean.Label.ForeColor = ScottPlot.Color.FromColor(System.Drawing.Color.Red);

            ShowPlot = true;
        }

        public void Reload()
        {
            if (Stats is null)
                throw new InvalidOperationException("Stats is not set");

            _txt_Count.Value = Stats.Count.ToString();
            _txt_Min.Value = Stats.Min.ToString("F2");
            _txt_Max.Value = Stats.Max.ToString("F2");
            _txt_Avg.Value = Stats.Mean.ToString("F2");
            _txt_StdDev.Value = Stats.StandardDeviation.ToString("F2");
            _txt_Variance.Value = Stats.Variance.ToString("F2");
        }

        public void Clear()
        {
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
