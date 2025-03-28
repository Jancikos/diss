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
    /// Interaction logic for InlineTextBox.xaml
    /// </summary>
    public partial class InlineTextBox : UserControl
    {
        public string Title 
        {
            get
            {
                return _txt_Title.Text;
            }
            set
            {
                _txt_Title.Text = value;
            }
        }
        
        public string Value
        {
            get
            {
                return _txtbx_Value.Text;
            }
            set
            {
                _txtbx_Value.Text = value;
            }
        }

        public bool HasValue
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Value);
            }
        }

        public int IntValue => int.Parse(Value);
        public double DoubleValue => double.Parse(Value);

        public TextAlignment TextAlignment
        {
            get
            {
                return _txtbx_Value.TextAlignment;
            }
            set
            {
                _txtbx_Value.TextAlignment = value;
            }
        }

        public string Tooltip 
        {
            get
            {
                return _grid.ToolTip.ToString() ?? "";
            }
            set
            {
                _grid.ToolTip = value;
            }
        }

        public InlineTextBox()
        {
            InitializeComponent();
        }
    }
}
