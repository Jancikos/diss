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
    /// Interaction logic for ObjednavkaUserControl.xaml
    /// </summary>
    public partial class ObjednavkaUserControl : UserControl
    {
        public Objednavka Objednavka 
        {
            set
            {
                _txt_Id.Text = $"#{value.Id}";
                _txt_NabytoksCount.Text = $"{value.NabytokDoneCount}/{value.NabytokCount}";
                _txt_Status.Text = value.IsDone
                    ? "done"
                    : "WIP";
            }
        }

        public ObjednavkaUserControl()
        {
            InitializeComponent();
        }
    }
}
