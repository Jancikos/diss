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
using FRI.DISS.Libs.Simulations.EventDriven;

namespace FRI.DISS.SP2.Controls
{
    /// <summary>
    /// Interaction logic for StolarUserControl.xaml
    /// </summary>
    public partial class StolarUserControl : UserControl
    {
        protected NabytokSimulation.Stolar? _stolar;

        public NabytokSimulation.Stolar Stolar
        {
            get => _stolar ?? throw new InvalidOperationException("Stolar is null");
            set
            {
                if (value is null) 
                    throw new InvalidOperationException("Stolar cannot be null");

                _stolar = value;
                _updateGUI();
            }
        }

        public bool ShowWorkplace
        {
            get => _txt_Workplace.Visibility == Visibility.Visible;
            set => _txt_Workplace.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
        public bool ShowIsWorking
        {
            get => _elp_IsWorking.Visibility == Visibility.Visible;
            set => _elp_IsWorking.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
        public bool ShowRatio 
        {
            get => _txt_WorkRatio.Visibility == Visibility.Visible;
            set => _txt_WorkRatio.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }
        public string Ratio
        {
            get => _txt_WorkRatio.Text;
            set => _txt_WorkRatio.Text = value;
        }

        public StolarUserControl()
        {
            InitializeComponent();
        }

        private void _updateGUI()
        {
            _txt_Id.Text = $"#{Stolar.Id}";

            if (ShowWorkplace)
            {
                _txt_Workplace.Text = $"WP{Stolar.CurrentPlace}";
            }

            if (ShowIsWorking) 
            {
                _elp_IsWorking.Fill = Stolar.IsWorking
                    ? Brushes.Red
                    : Brushes.Green;
            }
        }

    }
}
