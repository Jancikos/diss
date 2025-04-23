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

namespace FRI.DISS.SP3.Controls
{
    /// <summary>
    /// Interaction logic for PracoviskoUserControl.xaml
    /// </summary>
    public partial class PracoviskoUserControl : UserControl
    {
        public Pracovisko Pracovisko 
        {
            set
            {
                _grb_Header.Header = $"Workplace #{value.Id}";
                _txt_Order.Text = value.IsFree
                    ? ""
                    : $"#{value.CurrentNabytok!.Objednavka.Id}";
                _txt_Nabytok.Text = value.IsFree
                    ? "empty"
                    : $"{value.CurrentNabytok!.Type}#{value.CurrentNabytok.Id}";
                _txt_Status.Text = 
                    value.IsFree
                        ? ""
                        : $"[{value.CurrentNabytok!.State}]";

                var stolariCount = value.StolarTypesCount;
                _txt_StolariA.Text = $"A: {(stolariCount.ContainsKey(StolarType.A) ? stolariCount[StolarType.A] : 0)}";
                _txt_StolariB.Text = $"B: {(stolariCount.ContainsKey(StolarType.B) ? stolariCount[StolarType.B] : 0)}";
                _txt_StolariC.Text = $"C: {(stolariCount.ContainsKey(StolarType.C) ? stolariCount[StolarType.C] : 0)}";
            }
        }

        public PracoviskoUserControl()
        {
            InitializeComponent();
        }
    }
}
