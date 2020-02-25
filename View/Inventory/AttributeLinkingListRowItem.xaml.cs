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

namespace View.Inventory
{
    /// <summary>
    /// Interaction logic for AttributeLinkingListRowItem.xaml
    /// </summary>
    public partial class AttributeLinkingListRowItem : UserControl
    {
        public string OldOccType
        {
            get;set;
        }
        public string NewOccType
        {
            get;set;
        }
        public AttributeLinkingListRowItem()
        {
            InitializeComponent();
        }
        public AttributeLinkingListRowItem(string oldOcctype,List<Consequences_Assist.ComputableObjects.OccupancyType> listOfOcctypes)
        {
            InitializeComponent();

            OldOccType = oldOcctype;
            txt_OldOccType.Text = oldOcctype;
            cmb_NewOccType.ItemsSource = listOfOcctypes;
            cmb_NewOccType.DisplayMemberPath = "Name";
        }

        public AttributeLinkingListRowItem(string oldOcctype)
        {
            InitializeComponent();

            OldOccType = oldOcctype;
            txt_OldOccType.Text = oldOcctype;
            //cmb_NewOccType.ItemsSource = listOfOcctypes;
            //cmb_NewOccType.DisplayMemberPath = "Name";
        }

        public void SetSelectedIndex(int i)
        {
            cmb_NewOccType.SelectedIndex = i;
        }

        private void cmb_NewOccType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmb_NewOccType.SelectedIndex == -1)
            {
                NewOccType = "";
            }
            else
            {
                NewOccType = cmb_NewOccType.SelectedItem.ToString();// ((Consequences_Assist.ComputableObjects.OccupancyType)cmb_NewOccType.SelectedItem).Name;

            }
            FdaViewModel.Inventory.AttributeLinkingListVM vm = (FdaViewModel.Inventory.AttributeLinkingListVM)this.DataContext;
            if (vm != null)
            {
                vm.UpdateOcctypeDictionary(OldOccType, NewOccType);
            }
        }
    }
}
