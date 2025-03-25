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
    /// Interaction logic for WorkPlaceUserControl.xaml
    /// </summary>
    public partial class WorkPlaceUserControl : UserControl
    {
        public int Id
        {
            set => _grb_Header.Header = $"Workplace #{value}";
        }

        public bool IsShowingNextOperation
        {
            get => _uc_Objednavka.IsShowingNextOperation;
            set => _uc_Objednavka.IsShowingNextOperation = value;
        }

        public NabytokSimulation.Objednavka? Objednavka
        {
            get => _uc_Objednavka.Objednavka;
            set
            {
                _uc_Objednavka.Objednavka = value;

                _txt_NoObjednavka.Visibility = value is null 
                    ? Visibility.Visible
                    : Visibility.Collapsed;

                _uc_Objednavka.Visibility = value is null
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }
        }

        public WorkPlaceUserControl()
        {
            InitializeComponent();

            _uc_Objednavka.IsShowingNextOperation = true;
        }
    }
}
