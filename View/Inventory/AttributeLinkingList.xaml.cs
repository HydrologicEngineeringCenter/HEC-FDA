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
    /// Interaction logic for AttributeLinkingList.xaml
    /// </summary>
    public partial class AttributeLinkingList : UserControl
    {
        //private Dictionary<string, string> _OcctypesDictionary = new Dictionary<string, string>();
        //private List<AttributeLinkingListRowItem> ListOfRows = new List<AttributeLinkingListRowItem>();


        public AttributeLinkingList()
        {
            InitializeComponent();
        }
        
        //private void AddRow(string columnOneText,List<Consequences_Assist.ComputableObjects.OccupancyType> columnTwoList)
        //{
        //    RowDefinition newRow = new RowDefinition();
            
        //    newRow.Height = new GridLength(23);

        //    LinkingGrid.RowDefinitions.Add(newRow);
        //    TextBlock column1 = new TextBlock();
        //    column1.Text = columnOneText;
        //    Grid.SetRow(column1, LinkingGrid.RowDefinitions.Count - 1);
        //    Grid.SetColumn(column1, 0);
        //    LinkingGrid.Children.Add(column1);

        //    ComboBox column2 = new ComboBox();
        //    column2.ItemsSource = columnTwoList;
        //    column2.DisplayMemberPath = "Name";
        //    Grid.SetRow(column2, LinkingGrid.RowDefinitions.Count - 1);
        //    Grid.SetColumn(column2, 1);
        //    column2.SelectedIndex = -1;

        //    LinkingGrid.Children.Add(column2);

        //    //automatically select the right choice if the strings match. this could probably be enhanced further
        //    if (columnTwoList != null)
        //    {
        //        for (int i = 0; i < columnTwoList.Count; i++)
        //        {

        //            if (columnTwoList[i].Name.Equals(columnOneText))
        //            {
        //                column2.SelectedIndex = i;
        //                break;
        //            }
        //        }
        //    }

        //    //column2.SelectionChanged += Column2_SelectionChanged;

        //}

        //private void AddNextRow(string columnOneText)
        //{
        //    AttributeLinkingListRowItem ri = new AttributeLinkingListRowItem(columnOneText);
        //    RowDefinition newRow = new RowDefinition();

        //    newRow.Height = new GridLength(23);

        //    LinkingGrid.RowDefinitions.Add(newRow);
        //    Grid.SetRow(ri, LinkingGrid.RowDefinitions.Count - 1);
        //    Grid.SetColumn(ri, 0);
        //    Grid.SetColumnSpan(ri, 2);
        //    LinkingGrid.Children.Add(ri);

        //    ListOfRows.Add(ri);

            

        //}

        //private void AddNextRowWithAlreadySelectedItem(string columnOneText, List<string> columnTwoList,string selectedValue)
        //{
        //    AttributeLinkingListRowItem ri = new AttributeLinkingListRowItem(columnOneText);
        //    ListOfRows.Add(ri);

        //    RowDefinition newRow = new RowDefinition();

        //    newRow.Height = new GridLength(23);

        //    LinkingGrid.RowDefinitions.Add(newRow);
        //    Grid.SetRow(ri, LinkingGrid.RowDefinitions.Count - 1);
        //    Grid.SetColumn(ri, 0);
        //    Grid.SetColumnSpan(ri, 2);
        //    LinkingGrid.Children.Add(ri);

        //    //automatically select the previously selected value
        //    if(selectedValue == "" || selectedValue == null)
        //    {
        //        ri.SetSelectedIndex(-1);
        //        return;
        //    }
        //    if (columnTwoList != null)
        //    {
        //        for (int i = 0; i < columnTwoList.Count; i++)
        //        {

        //            if (columnTwoList[i].Equals(selectedValue))
        //            {
        //                ri.SetSelectedIndex(i);
        //                //_OcctypesDictionary.Add(ri.OldOccType, ri.NewOccType);

        //                break;
        //            }
        //        }
        //    }


        //}
        
      

        //private void UserControl_Loaded(object sender, RoutedEventArgs e)
        //{
            //ViewModel.Inventory.AttributeLinkingListVM vm = (ViewModel.Inventory.AttributeLinkingListVM)this.DataContext;
            //if (vm == null)
            //{
            //}
            //else if(vm.IsInEditMode == true)
            //{
                
            //    //get all the occtypes that are in the study
            //    //Consequences_Assist.ComputableObjects.OccupancyType occtype = new Consequences_Assist.ComputableObjects.OccupancyType("", "");
            //    List<string> theListOfOccTypesWithBlankOption = new List<string>();
            //    if (vm.SelectedListOfOccTypeStrings != null)
            //    {
            //        theListOfOccTypesWithBlankOption = vm.SelectedListOfOccTypeStrings;
                    
            //    }
                
            //    List<string> keys = vm.OccupancyTypesDictionary.Keys.ToList<string>();
            //    foreach(string k in keys)
            //    {
            //        string keyString = k;
            //        string valueString;
            //        bool hasKey= vm.OccupancyTypesDictionary.TryGetValue(k,out valueString);
            //        AddNextRowWithAlreadySelectedItem(keyString,theListOfOccTypesWithBlankOption, valueString);
            //    }

            //    ///////////////////////////////////////
            //    //   display the occtype groups with the check boxes checked 
            //    ///////////////////////////////////////////
            //    bool checkOne = false;
            //    bool checkTwo = false;
            //    for (int i = 0; i < vm.ListOfOccTypeGroups.Count; i = i + 2)
            //    {
            //        if (i + 1 == vm.ListOfOccTypeGroups.Count)
            //        {
            //            if (vm.ListOfSelectedOccTypeGroups.Contains(vm.ListOfOccTypeGroups[i]))
            //            {
            //                checkOne = true;
            //            }
            //            else
            //            {
            //                checkOne = false;
            //            }
            //            AddOccTypeGroupWithCheckBox(vm.ListOfOccTypeGroups[i],null,checkOne);

            //        }
            //        else
            //        {
            //            if (vm.ListOfSelectedOccTypeGroups.Contains(vm.ListOfOccTypeGroups[i]))
            //            {
            //                checkOne = true;
            //            }
            //            else
            //            {
            //                checkOne = false;
            //            }
            //            if (vm.ListOfSelectedOccTypeGroups.Contains(vm.ListOfOccTypeGroups[i+1]))
            //            {
            //                checkTwo = true;
            //            }
            //            else
            //            {
            //                checkTwo = false;
            //            }
            //            AddOccTypeGroupWithCheckBox(vm.ListOfOccTypeGroups[i], vm.ListOfOccTypeGroups[i + 1],checkOne,checkTwo);

            //        }
            //        checkOne = false;
            //        checkTwo = false;
            //    }
            //}
            //else
            //{
            //    ////add a blank occtype option to the combobox
            //    //Consequences_Assist.ComputableObjects.OccupancyType occtype = new Consequences_Assist.ComputableObjects.OccupancyType("", "");
            //    //List<Consequences_Assist.ComputableObjects.OccupancyType> theListOfOccTypesWithBlankOption = new List<Consequences_Assist.ComputableObjects.OccupancyType>();
            //    //if (vm.ListOfSelectedOccupancyTypes != null)
            //    //{
            //    //    theListOfOccTypesWithBlankOption = vm.ListOfSelectedOccupancyTypes;
            //    //    theListOfOccTypesWithBlankOption.Insert(0, occtype);
            //    //}
                

            //    for (int i = 0; i < vm.OccupancyTypesInFile.Count; i++)
            //    {
            //        //AddRow(vm.OccupancyTypesInFile[i], theListOfOccTypesWithBlankOption);
            //        AddNextRow(vm.OccupancyTypesInFile[i]);

            //    }


                
            //    for (int i = 0; i < vm.ListOfOccTypeGroups.Count; i=i+2)
            //    {
            //        if (i + 1 == vm.ListOfOccTypeGroups.Count)
            //        {
            //            AddOccTypeGroupWithCheckBox(vm.ListOfOccTypeGroups[i]);
            //        }
            //        else
            //        {
            //            AddOccTypeGroupWithCheckBox(vm.ListOfOccTypeGroups[i], vm.ListOfOccTypeGroups[i + 1]);

            //        }
            //    }





            //}
        //}

        //private void AddOccTypeGroupWithCheckBox(ViewModel.Inventory.OccupancyTypes.OccupancyTypesElement eleA, ViewModel.Inventory.OccupancyTypes.OccupancyTypesElement eleB = null, bool checkFirstElement = false, bool checkSecondElement = false)
        //{

        //    RowDefinition newRow = new RowDefinition();
        //    newRow.Height = new GridLength(23);

        //    if (eleB != null)
        //    {
                
        //        CheckBox cb = new CheckBox();
        //        cb.Content = eleA.Name;

        //        CheckBox cb2 = new CheckBox();
        //        cb2.Content = eleB.Name;

        //        grd_OccTypeGroupNames.RowDefinitions.Add(newRow);

        //        Grid.SetRow(cb, grd_OccTypeGroupNames.RowDefinitions.Count - 1);
        //        Grid.SetColumn(cb, 0);

        //        Grid.SetRow(cb2, grd_OccTypeGroupNames.RowDefinitions.Count - 1);
        //        Grid.SetColumn(cb2, 1);

        //        grd_OccTypeGroupNames.Children.Add(cb);
        //        grd_OccTypeGroupNames.Children.Add(cb2);

        //        cb.Checked += Cb_Checked;
        //        cb.Unchecked += Cb_Unchecked;
        //        cb2.Checked += Cb_Checked;
        //        cb2.Unchecked += Cb_Unchecked;
        //        if (checkFirstElement == true)
        //        {
        //            cb.IsChecked = true;
        //        }
        //        if (checkSecondElement == true)
        //        {
        //            cb2.IsChecked = true;
                    
        //        }
        //    }
        //    else //there is one last one. Put it in column 0
        //    {
        //        CheckBox cb = new CheckBox();
        //        cb.Content = eleA.Name;           
        //        grd_OccTypeGroupNames.RowDefinitions.Add(newRow);
        //        Grid.SetRow(cb, grd_OccTypeGroupNames.RowDefinitions.Count - 1);
        //        Grid.SetColumn(cb, 0);               
        //        grd_OccTypeGroupNames.Children.Add(cb);
        //        cb.Checked += Cb_Checked;
        //        cb.Unchecked += Cb_Unchecked;
        //        if(checkFirstElement == true)
        //        {
        //            cb.IsChecked = true;
        //        }

        //    }



        //}

        //private void Cb_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    //remove this groups occtypes from the list of selected occtypes in the vm
            
        //    ViewModel.Inventory.AttributeLinkingListVM vm = (ViewModel.Inventory.AttributeLinkingListVM)this.DataContext;
        //    string groupName = ((CheckBox)sender).Content.ToString();
        //    vm.RemoveSelectedGroupFromList(groupName);

        //    AutoSelectOccType();
        //    ////automatically select the right choice if the strings match. this could probably be enhanced further
        //    //foreach (AttributeLinkingListRowItem ri in ListOfRows)
        //    //{
        //    //    if (ri.cmb_NewOccType.SelectedIndex == -1)
        //    //    {
        //    //        foreach (ViewModel.Inventory.OccupancyTypes.OccupancyTypesElement ele in ViewModel.Inventory.OccupancyTypes.OccupancyTypesOwnedElement.ListOfOccupancyTypesGroups)
        //    //        {
        //    //            if (ele.OccTypesGroupName == groupName)
        //    //            {
        //    //                //foreach(Consequences_Assist.ComputableObjects.OccupancyType ot in ele.ListOfOccupancyTypes)
        //    //                for (int i = 0; i < ele.ListOfOccupancyTypes.Count; i++)
        //    //                {
        //    //                    if (ele.ListOfOccupancyTypes[i].Name == ri.OldOccType)
        //    //                    {
        //    //                        ri.cmb_NewOccType.SelectedIndex = i;
        //    //                    }
        //    //                }
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //}

        //private void Cb_Checked(object sender, RoutedEventArgs e)
        //{
        //    ViewModel.Inventory.AttributeLinkingListVM vm = (ViewModel.Inventory.AttributeLinkingListVM)this.DataContext;
        //    string groupName = ((CheckBox)sender).Content.ToString();
        //    vm.AddSelectedGroupToList(groupName);

        //    AutoSelectOccType();
           

        //}

        //private void AutoSelectOccType()
        //{
        //    ViewModel.Inventory.AttributeLinkingListVM vm = (ViewModel.Inventory.AttributeLinkingListVM)this.DataContext;

        //    string[] occtypeAndGroupName = new string[2];
        //    //automatically select the right choice if the strings match. this could probably be enhanced further
        //    foreach (AttributeLinkingListRowItem ri in ListOfRows)
        //    {
        //        if (ri.cmb_NewOccType.SelectedIndex == -1)
        //        {

        //            foreach(string s in vm.SelectedListOfOccTypeStrings)
        //            {
        //                occtypeAndGroupName = vm.ParseOccTypeNameAndGroupNameFromCombinedString(s);
        //                if(occtypeAndGroupName[0] == ri.OldOccType)
        //                {
        //                    ri.cmb_NewOccType.SelectedItem = s;
        //                }

        //            }

        //            //foreach (ViewModel.Inventory.OccupancyTypes.OccupancyTypesElement ele in ViewModel.Inventory.OccupancyTypes.OccupancyTypesOwnedElement.ListOfOccupancyTypesGroups)
        //            //{
        //            //    if (ele.OccTypesGroupName == groupName)
        //            //    {
        //            //        //foreach(Consequences_Assist.ComputableObjects.OccupancyType ot in ele.ListOfOccupancyTypes)
        //            //        for (int i = 0; i < ele.ListOfOccupancyTypes.Count; i++)
        //            //        {
        //            //            if (ele.ListOfOccupancyTypes[i].Name == ri.OldOccType)
        //            //            {
        //            //                ri.cmb_NewOccType.SelectedIndex = i + 1; // +1 becuase i have added a blank space at spot 0
        //            //            }
        //            //        }
        //            //    }
        //            //}
        //        }
        //    }

            
        //}

        //private void rad_FromFile_Checked(object sender, RoutedEventArgs e)
        //{
        //    grp_DefaultOccTypeAssignments.Visibility = Visibility.Collapsed;

        //}

        //private void rad_UseDefaults_Checked(object sender, RoutedEventArgs e)
        //{
        //    grp_DefaultOccTypeAssignments.Visibility = Visibility.Visible;

        //}

        
    }
}
