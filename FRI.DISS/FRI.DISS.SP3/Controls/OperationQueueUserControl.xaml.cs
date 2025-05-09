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
using FRI.DISS.SP3.Libs.NabytokSimulation.Entities;
using FRI.DISS.SP3.Libs.NabytokSimulation.Simulation;

namespace FRI.DISS.SP3.Controls
{
    /// <summary>
    /// Interaction logic for OperationQueueUserControl.xaml
    /// </summary>
    public partial class OperationQueueUserControl : UserControl
    {
        protected NabytokOperation _nabytokOperation = NabytokOperation.Lakovanie;
        public NabytokOperation NabytokOperation
        {
            get => _nabytokOperation;
            set
            {
                _grb_Header.Header = $"{value} queue";
                _nabytokOperation = value;
            }
        }

        public OperationQueueUserControl()
        {
            InitializeComponent();
        }
        
        public void _updateGUI(List<MyMessage> items)
        {
            _txt_TotalCount.Value = items.Count.ToString();

            for (int i = 0; i < items.Count; i++)
            {
                var objednavka = items[i];

                if (_lst.Items.Count <= i)
                {
                    _lst.Items.Add("");
                }

                // var workplaceUc = (WorkPlaceUserControl)_lst.Items[i];
                // workplaceUc.Id = objednavka.WorkplaceIndex + 1;
                // workplaceUc.Objednavka = objednavka;

                _lst.Items[i] = $"#{objednavka.Objednavka!.Id} {objednavka.Nabytok!.Type}#{objednavka.Nabytok!.Id}";
            }

            if (_lst.Items.Count > items.Count)
            {
                for (int i = _lst.Items.Count - 1; i >= items.Count; i--)
                {
                    _lst.Items.RemoveAt(i);
                }
            }
        }
    }
}
