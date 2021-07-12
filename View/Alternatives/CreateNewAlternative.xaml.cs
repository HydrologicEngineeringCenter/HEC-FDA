using FdaViewModel.AlternativeComparisonReport;
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

namespace View.Alternatives
{
    /// <summary>
    /// Interaction logic for CreateNewAlternative.xaml
    /// </summary>
    public partial class CreateNewAlternative : UserControl
    {
        //private const string ADD_MSG = "Add Increment";
        //private const string REMOVE_MSG = "Remove Increment";
        private int _Index = 0;
        public CreateNewAlternative()
        {
            InitializeComponent();
        }
        private void AddFirstIncrement()
        {
            CreateNewAlternativeComparisonReportVM vm = (CreateNewAlternativeComparisonReportVM)this.DataContext;
            //add increment
            vm.AddIncrement();
            //create row item without delete button
            _Index++;
            IncrementRowItem ri = new IncrementRowItem(_Index, true);
            ri.DataContext = vm.Increments.Last();
            ri.HorizontalAlignment = HorizontalAlignment.Stretch;
            ri.SetSpecialBindingForFirstRow(vm.Increments[0]);
            lst_Increments.Items.Add(ri);

            
        }

        private void btn_addIncrement_Click(object sender, RoutedEventArgs e)
        {
            CreateNewAlternativeComparisonReportVM vm = (CreateNewAlternativeComparisonReportVM)this.DataContext;
            //add increment
            vm.AddIncrement();
            // grab the latest increment from vm

            //create a new Row UI
            _Index++;
            IncrementRowItem ri = new IncrementRowItem(_Index);
            ri.DataContext = vm.Increments.Last();
            ri.HorizontalAlignment = HorizontalAlignment.Stretch;
            ri.DeleteThisRow += Ri_DeleteThisRow;
            lst_Increments.Items.Add(ri);
            if (vm.Increments.Count > 1)
            {
                ri.IncrementToBindWith = vm.Increments[vm.Increments.Count - 2];
                ri.cmb_Plans2.IsEnabled = false;
            }
        }

        private void Ri_DeleteThisRow(object sender, EventArgs e)
        {
            _Index--;
            IncrementRowItem ri = (IncrementRowItem)sender;
            int riIndex = lst_Increments.Items.IndexOf(ri);
            //remove this item from the vm and the ui
            CreateNewAlternativeComparisonReportVM vm = (CreateNewAlternativeComparisonReportVM)this.DataContext;
            vm.Increments.RemoveAt(riIndex);
            lst_Increments.Items.RemoveAt(riIndex);

            //if there is an increment above this one and this isn't the first one, then i need to update the binding
            int lastRow = lst_Increments.Items.Count;
            if(riIndex !=0 && riIndex<lastRow)
            {
                //riIndex will now be the next row because we removed a row at riIndex
                IncrementRowItem riNeedsUpdating = (IncrementRowItem)lst_Increments.Items[riIndex];
                riNeedsUpdating.IncrementToBindWith = vm.Increments[riIndex - 1];
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //add the first increment.
            AddFirstIncrement();
        }
    }
}
