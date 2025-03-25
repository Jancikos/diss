using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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
    /// Interaction logic for StolariUserControl.xaml
    /// </summary>
    public partial class StolariUserControl : UserControl
    {
        protected NabytokSimulation.StolarType _stolarType = NabytokSimulation.StolarType.A;
        public NabytokSimulation.StolarType StolarType
        {
            get => _stolarType;
            set
            {
                _grb_Header.Header = $"Stolári {value}";
                _stolarType = value;
            }
        }

        public StolariUserControl()
        {
            InitializeComponent();
        }

        public void _updateGUI(List<NabytokSimulation.Stolar> stolari)
        {
            _txt_TotalCount.Value = stolari.Count.ToString();

            int workingCount = 0;
            for (int i = 0; i < stolari.Count; i++)
            {
                var stolar = stolari[i];

                if (stolar.IsWorking)
                {
                    workingCount++;
                }

                if (_lst.Items.Count <= i)
                {
                    _lst.Items.Add(new StolarUserControl());
                }

                var stolarUC = (StolarUserControl)_lst.Items[i];
                stolarUC.Stolar = stolar;
            }

            _txt_FreeCount.Value = (stolari.Count - workingCount).ToString();
        }
    }
}
