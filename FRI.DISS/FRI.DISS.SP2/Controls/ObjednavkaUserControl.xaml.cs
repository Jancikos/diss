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
    /// Interaction logic for ObjednavkaUserControl.xaml
    /// </summary>
    public partial class ObjednavkaUserControl : UserControl
    {
        protected NabytokSimulation.Objednavka? _objednavka;

        public NabytokSimulation.Objednavka? Objednavka
        {
            get => _objednavka;
            set
            {
                _objednavka = value;
                _updateGUI();
            }
        }

        public bool IsShowingNextOperation { get; set; } = false;

        public ObjednavkaUserControl()
        {
            InitializeComponent();
        }
        
        private void _updateGUI()
        {
            if (Objednavka is null)
            {
                _txt_Id.Text = "#";
                _txt_Nabytok.Text = "N/A";
                _txt_Status.Text = "[N/A]";
                return;
            }

            _txt_Id.Text = $"#{Objednavka.Id}";
            _txt_Nabytok.Text = Objednavka.Nabytok.ToString();

            string status = Objednavka.Status.ToString();
            if (IsShowingNextOperation && Objednavka.Status < NabytokSimulation.ObjednavkaStatus.Poskladana)
            {
                status += $" -> {Objednavka.MapStatusToNextOperation()}";
            }
            _txt_Status.Text = $"[{status}]";
        }
    }
}
