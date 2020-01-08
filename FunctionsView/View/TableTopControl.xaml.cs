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

namespace FunctionsView.View
{
    /// <summary>
    /// Interaction logic for TableTopControl.xaml
    /// </summary>
    public partial class TableTopControl : UserControl
    {
        public TableTopControl()
        {
            InitializeComponent();
        }

        public TableTopControl(int[] columnWidths):this()
        {
            Col_Left.Width = new GridLength( ColumnWidths.COL_X_WIDTH + 11);
            Col_Middle.Width = new GridLength(columnWidths.Sum() + ColumnWidths.COL_DIST_WIDTH + 2);
            Col_Right.Width = new GridLength(ColumnWidths.COL_INTERP_WIDTH + 10);
        }
    }
}
