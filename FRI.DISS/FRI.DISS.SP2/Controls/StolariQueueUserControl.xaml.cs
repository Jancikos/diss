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
    /// Interaction logic for StolariQueueUserControl.xaml
    /// </summary>
    public partial class StolariQueueUserControl : UserControl
    {
        protected NabytokSimulation.StolarType _stolarType = NabytokSimulation.StolarType.A;
        public NabytokSimulation.StolarType StolarType
        {
            get => _stolarType;
            set
            {
                _grb_Header.Header = $"Stolári {value} queue";
                _stolarType = value;
            }
        }

        public StolariQueueUserControl()
        {
            InitializeComponent();
        }

        public void _updateGUI(List<NabytokSimulation.Objednavka> objednavky)
        {
            _txt_TotalCount.Value = objednavky.Count.ToString();

            for (int i = 0; i < objednavky.Count; i++)
            {
                var objednavka = objednavky[i];

                if (_lst.Items.Count <= i)
                {
                    _lst.Items.Add(new WorkPlaceUserControl() { IsShowingNextOperation = false });
                }

                var workplaceUc = (WorkPlaceUserControl)_lst.Items[i];
                workplaceUc.Id = objednavka.Workplace + 1;
                workplaceUc.Objednavka = objednavka;
            }

            if (_lst.Items.Count > objednavky.Count)
            {
                for (int i = _lst.Items.Count - 1; i >= objednavky.Count; i--)
                {
                    _lst.Items.RemoveAt(i);
                }
            }
        }

    }
}
