using FunctionsView.ViewModel;
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
    /// Interaction logic for CoordinatesFunctionEditor.xaml
    /// </summary>
    public partial class CoordinatesFunctionEditor : UserControl
    {
        public CoordinatesFunctionEditor()
        {
            InitializeComponent();
        }
        public static readonly DependencyProperty RowsProperty = DependencyProperty.Register("Rows", typeof(List<CoordinatesFunctionRowItem>), typeof(CoordinatesFunctionEditor), new FrameworkPropertyMetadata(new List<CoordinatesFunctionRowItem>(), new PropertyChangedCallback(RowsChangedCallBack)));

        private List<CoordinatesFunctionRowItem> _Rows;
        public List<CoordinatesFunctionRowItem> Rows
        {
            get { return _Rows; }
            set
            {
                _Rows = value;
                foreach (CoordinatesFunctionRowItem row in _Rows)
                {
                    row.ChangedDistributionType += UpdateView;
                    row.ChangedInterpolationType += UpdateView;
                }
            }
        }

      

        private static void RowsChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CoordinatesFunctionEditor owner = d as CoordinatesFunctionEditor;
            owner.Rows = e.NewValue as List<CoordinatesFunctionRowItem>;
            //call some sort of update method that re sorts and organizes the tables and replots.
            owner.UpdateView(owner, new EventArgs());
        }

        private void UpdateView(object sender, EventArgs e)
        {
            //CoordinatesFunctionRowItem rowThatChanged = sender as CoordinatesFunctionRowItem;
            //List<CoordinatesFunctionRowItem> rowsAbove = GetRowsAboveRow(rowThatChanged);
            //List<CoordinatesFunctionRowItem> rowsBelow = GetRowsBelowRow(rowThatChanged);
            //List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
            //foreach(CoordinatesFunctionRowItem row in _Rows)
            //{
            //    distType = row.SelectedDistributionType;
            //}
            //lst_tables.Items.Add(new NoneTable(Rows));
            lst_tables.Items.Clear();

            if (_Rows.Count > 0)
            {
                lst_tables.Items.Add(new TableTopControl());

                List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
                string distType = _Rows[0].SelectedDistributionType;
                string interpType = _Rows[0].SelectedInterpolationType;
                rows.Add(_Rows[0]);
                int i = 1;
                while (i < _Rows.Count)
                {
                    CoordinatesFunctionRowItem row = _Rows[i];
                    if (row.SelectedDistributionType.Equals(distType) && row.SelectedInterpolationType.Equals(interpType))
                    {
                        rows.Add(_Rows[i]);
                    }
                    else
                    {
                        //the dist type changed
                        CreateTable(rows, distType);
                        //set the new dist type and add it to the list
                        distType = row.SelectedDistributionType;
                        interpType = row.SelectedInterpolationType;
                        rows = new List<CoordinatesFunctionRowItem>();
                        rows.Add(row);
                    }

                    i++;
                }
                //need to create the final table
                CreateTable(rows, distType);

                lst_tables.Items.Add(new TableBottomControl());
            }


        }
        private void CreateTable(List<CoordinatesFunctionRowItem> rows, string distType)
        {
            lst_tables.Items.Add(new CoordinatesFunctionTable(rows));
            //switch (distType)
            //{
            //    case "None":
            //        {

            //            lst_tables.Items.Add(new CoordinateFunctionTable(rows));  //new NoneTable(rows));
            //            return;
            //        }
            //    case "Normal":
            //        {

            //            lst_tables.Items.Add(new CoordinateFunctionTable(rows)); //new NormalTable(rows));
            //            return;
            //        }
            //    case "Triangular":
            //        {
            //            lst_tables.Items.Add(new TriangularTable(rows));
            //                return;
            //        }

            //}

        }

        //private List<CoordinatesFunctionRowItem> GetRowsAboveRow(CoordinatesFunctionRowItem row)
        //{
        //    List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
        //    int i;
        //    for (i = 0; i < Rows.Count; i++)
        //    {
        //        if (Rows[i] == row)
        //        {
        //            break;
        //        }
        //        else
        //        {
        //            rows.Add(Rows[i]);
        //        }
        //    }
        //    return rows;
        //}
        //private List<CoordinatesFunctionRowItem> GetRowsBelowRow(CoordinatesFunctionRowItem row)
        //{
        //    int i;
        //    for (i = 0; i < Rows.Count; i++)
        //    {
        //        if (Rows[i] == row)
        //        {
        //            break;
        //        }
        //    }
        //    List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
        //    i++;
        //    for (int j = i; j < Rows.Count; j++)
        //    {
        //        rows.Add(Rows[j]);
        //    }
        //    return rows;
        //}
    }
}
