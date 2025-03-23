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

        protected Statistics? _stats;
        public Statistics? Stats
        {
            get => _stats;
            set
            {
                _stats = value;
                Reload();
            }
        }

        public DicreteStatistic()
        {
            InitializeComponent();
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
    }
}
