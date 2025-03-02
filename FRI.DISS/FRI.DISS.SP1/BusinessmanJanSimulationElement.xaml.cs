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

namespace FRI.DISS.SP1
{
    /// <summary>
    /// Interaction logic for BusinessmanJanSimulationElement.xaml
    /// </summary>
    public partial class BusinessmanJanSimulationElement : UserControl
    {
        public string Title
        {
            get { return _txt_Title.Header.ToString() ?? ""; }
            set { _txt_Title.Header = value; }
        }

        public BusinessmanJanSimulationElement()
        {
            InitializeComponent();
        }
    }
}
