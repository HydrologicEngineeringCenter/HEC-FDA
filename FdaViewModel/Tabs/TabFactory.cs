using FdaViewModel.Study;
using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Tabs
{
    public sealed class TabFactory : BaseViewModel
    {
        public delegate void TabAddedEventHandler(object sender, TabAddedEventArgs args);
        public event TabAddedEventHandler TabAdded;

        private Dictionary<Guid, List<IDynamicTab>> _TabsDictionary;


        private static readonly TabFactory _Instance = new TabFactory();
        private FdaStudyVM _fdaStudyVM;

        public ObservableCollection<IDynamicTab> _Tabs = new ObservableCollection<IDynamicTab>();

        #region Properties
        public ObservableCollection<IDynamicTab> Tabs
        {
            get { return _Tabs; }
            set { _Tabs = value; NotifyPropertyChanged(); }
        }

        #endregion


        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static TabFactory()
        {
        }

        private TabFactory()
        {
        }

        public static TabFactory Instance
        {
            get
            {
                return _Instance;
            }
        }


        public void AddTab(IDynamicTab tab)
        {
            _Tabs.Add(tab);
            TabAdded?.Invoke(this, new TabAddedEventArgs(tab));
        }

        //public void AddTab(IDynamicTab dynamicTabVM, bool poppingIn = false)
        //{

        //    //if (dynamicTabVM.BaseVM.CanOpenMultipleTimes == false)
        //    {
        //        if (dynamicTabVM.BaseVM.ParentGUID != null)
        //        {
        //            if (_TabsDictionary.ContainsKey(dynamicTabVM.BaseVM.ParentGUID))
        //            {
        //                foreach (IDynamicTab tab in _TabsDictionary[dynamicTabVM.BaseVM.ParentGUID])
        //                {
        //                    if (tab.BaseVM.GetType() == dynamicTabVM.BaseVM.GetType()) //might have to do tab.gettype == dynamicTabVM.gettype
        //                    {
        //                        //we have a duplicate, select the open one
        //                        int index = Tabs.IndexOf(tab);
        //                        if (index != -1)
        //                        {
        //                            SelectedDynamicTabIndex = index;
        //                        }

        //                        return;
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    //    //some tabs we do not want to allow to be openned multiple times and some we do
        //    //    if (dynamicTabVM.BaseVM.CanOpenMultipleTimes == false)
        //    //{
        //    //    //foreach(KeyValuePair<Guid,IDynamicTab> entry in _TabsDictionary)
        //    //    {
        //    //        if(_TabsDictionary.ContainsKey(uniqueID))
        //    //        //if(entry.Value.Header.Equals(dynamicTabVM.Header) && entry.Value.BaseVM.Name.Equals(dynamicTabVM.BaseVM.Name))
        //    //        {
        //    //            //SelectedDynamicTabIndex = Tabs.IndexOf(tab);
        //    //            return;
        //    //        }
        //    //    }
        //    //foreach (IDynamicTab tab in Tabs)
        //    //{
        //    //    if (tab.Header.Equals(dynamicTabVM.Header))
        //    //    {
        //    //        SelectedDynamicTabIndex = Tabs.IndexOf(tab);
        //    //        return;
        //    //    }
        //    //}
        //    //if (poppingIn == false)
        //    //{
        //    //    foreach (IDynamicTab tab in PoppedOutTabs)
        //    //    {
        //    //        if (tab.Header.Equals(dynamicTabVM.Header))
        //    //        {
        //    //            //I wanted to bring the focus to the currect window. This is possible by looping through the open windows and then 
        //    //            //checking the datacontext to get the vm and maybe comparing. Then set the focus on it. I don't have access to the view from
        //    //            //here and it seems like more work than its worth.
        //    //            //foreach (var Window in Application.Current.Windows)
        //    //            //{
        //    //            //    int i = 0;
        //    //            //   //if(Window.)
        //    //            //    // TODO: write what you want here
        //    //            //}
        //    //            return;
        //    //        }
        //    //    }
        //    //}
        //    //}


        //    StudyStatusBar.SaveStatus = StudyStatusBar.UnsavedChangesMessage;

        //    // Guid uniqueID = Guid.NewGuid();
        //    //dynamicTabVM.BaseVM.TabUniqueID = uniqueID;
        //    dynamicTabVM.RemoveEvent += RemoveTab;
        //    dynamicTabVM.PopTabOutEvent += PopTabOut;
        //    //this is used to go from a window back to a tab after it has been popped out
        //    dynamicTabVM.BaseVM.AddPopThisIntoATabAction((dynamicTab, isPoppingIn) => AddTab(dynamicTab, isPoppingIn));
        //    Tabs.Add(dynamicTabVM);
        //    //Avalon.Add(dynamicTabVM.BaseVM);

        //    AddTabToTabsDictionary(dynamicTabVM);
        //    //_TabsDictionary.Add(uniqueID, dynamicTabVM);
        //    SelectedDynamicTabIndex = Tabs.Count - 1;//I want to make the tab we just added be selected. But some crazy stuff
        //    //happens and it magically sets the selected tab to be count -2. Maybe its an order thing and the tab isn't actually 
        //    //in yet or something.
        //}

        //private void AddTabToTabsDictionary(IDynamicTab tab)
        //{
        //    if (_TabsDictionary.ContainsKey(tab.BaseVM.ParentGUID))
        //    {
        //        _TabsDictionary[tab.BaseVM.ParentGUID].Add(tab);
        //    }
        //    else
        //    {
        //        _TabsDictionary.Add(tab.BaseVM.ParentGUID, new List<IDynamicTab>() { tab });
        //    }
        //}

        //public void AddCreateNewStudyTab()
        //{

        //    NewStudyVM vm = new NewStudyVM(CurrentStudyElement);
        //    vm.ParentGUID = CurrentStudyElement.GUID;
        //    DynamicTabVM newStudyTab = new DynamicTabVM("Create New Study", vm, true);
        //    newStudyTab.Name = "CreateStudyTab";//i use this to query the dictionary later to remove it
        //    AddTab(newStudyTab);
        //    SelectedDynamicTabIndex = Tabs.Count - 1;
        //}


        //private void SaveTheTabs(object sender, EventArgs e)
        //{
        //    List<Guid> keys = new List<Guid>();
        //    foreach (KeyValuePair<Guid, List<IDynamicTab>> entry in _TabsDictionary)
        //    {
        //        keys.Add(entry.Key);
        //    }


        //    bool allTabsSaved = true;
        //    int tabsCount = _TabsDictionary.Count;
        //    for (int j = 0; j < tabsCount; j++)
        //    //foreach (KeyValuePair<Guid,List<IDynamicTab>> entry in _TabsDictionary)
        //    {
        //        List<IDynamicTab> tabsForKey = _TabsDictionary[keys[j]];
        //        for (int i = 0; i < tabsForKey.Count; i++)
        //        //foreach (IDynamicTab tab in entry.Value)
        //        {
        //            IDynamicTab tab = tabsForKey[i];
        //            if (tab.BaseVM.GetType().BaseType == typeof(Editors.BaseEditorVM))
        //            {
        //                tab.BaseVM.Validate();
        //                if (tab.BaseVM.HasError)
        //                {
        //                    //do something?
        //                }
        //                if (tab.BaseVM.HasFatalError)
        //                {
        //                    allTabsSaved = false;
        //                    TransactionRows.Add(new TransactionRowItem(DateTime.Now.ToString("G"), "Unable to save tab: '" + tab.Header + "' because it is in an error state.", "i forget how to get the user"));
        //                }
        //                else
        //                {
        //                    ((Editors.BaseEditorVM)tab.BaseVM).Save();
        //                }
        //            }
        //        }
        //    }
        //    if (allTabsSaved)
        //    {
        //        SaveStatus = "Saved " + DateTime.Now.ToString("G");
        //    }
        //    else
        //    {
        //        SaveStatus = "Unsaved Changes";

        //    }
        //    //foreach(IDynamicTab tab in Tabs)
        //    //{
        //    //    if(tab.BaseVM.GetType().BaseType == typeof(Editors.BaseEditorVM))
        //    //    {
        //    //        tab.BaseVM.Validate();
        //    //        if(tab.BaseVM.HasError)
        //    //        {
        //    //            //do something?
        //    //        }
        //    //        if(tab.BaseVM.HasFatalError)
        //    //        {
        //    //            TransactionRows.Add(new TransactionRowItem(DateTime.Now.ToString("G"), "Unable to save tab: '" + tab.Header + "' because it is in an error state.","i forget how to get the user"));
        //    //        }
        //    //        else
        //    //        {
        //    //            ((Editors.BaseEditorVM)tab.BaseVM).Save();
        //    //        }
        //    //    }
        //    //}

        //    ////now save the open windows
        //    //foreach (IDynamicTab tab in PoppedOutTabs)
        //    //{
        //    //    if (tab.BaseVM.GetType().BaseType == typeof(Editors.BaseEditorVM))
        //    //    {
        //    //        tab.BaseVM.Validate();
        //    //        if (tab.BaseVM.HasError)
        //    //        {
        //    //            //do something?
        //    //        }
        //    //        if (tab.BaseVM.HasFatalError)
        //    //        {
        //    //            TransactionRows.Add(new TransactionRowItem(DateTime.Now.ToString("G"), "Unable to save window: '" + tab.Header + "' because it is in an error state.", "i forget how to get the user"));
        //    //        }
        //    //        else
        //    //        {
        //    //            ((Editors.BaseEditorVM)tab.BaseVM).Save();
        //    //        }
        //    //    }
        //    //}

        //}

        //public void RemoveCreateNewStudyTab(object sender, EventArgs e)
        //{
        //    //BaseFdaElement studyElement =  StudyCache.GetParentElementOfType<StudyElement>();
        //    //if (studyElement != null)
        //    {
        //        if (_TabsDictionary.ContainsKey(CurrentStudyElement.GUID))
        //        {

        //            //foreach (IDynamicTab tab in _TabsDictionary[CurrentStudyElement.GUID])
        //            for (int i = 0; i < _TabsDictionary[CurrentStudyElement.GUID].Count; i++)
        //            {
        //                if (_TabsDictionary[CurrentStudyElement.GUID][i].BaseVM.GetType() == typeof(Study.NewStudyVM))
        //                {
        //                    Tabs.Remove(_TabsDictionary[CurrentStudyElement.GUID][i]);
        //                    _TabsDictionary[CurrentStudyElement.GUID].Remove(_TabsDictionary[CurrentStudyElement.GUID][i]);
        //                }
        //            }
        //        }
        //    }
        //}

        ///// <summary>
        ///// This one gets called when closing a window. There is no dynamicTab just a vm in the window.
        ///// </summary>
        ///// <param name="vm"></param>
        //public void RemoveTabFromDictionary(BaseViewModel vm)
        //{
        //    if (vm is Plots.iConditionsImporter)
        //    {
        //        DealWithTheConditionsEditorImporters((Plots.iConditionsImporter)vm);
        //    }
        //    if (vm.ParentGUID != null)
        //    {
        //        if (_TabsDictionary.ContainsKey(vm.ParentGUID))
        //        {
        //            //foreach (IDynamicTab tab in _TabsDictionary[vm.ParentGUID])
        //            for (int i = 0; i < _TabsDictionary[vm.ParentGUID].Count; i++)
        //            {
        //                if (_TabsDictionary[vm.ParentGUID][i].BaseVM.GetType() == vm.GetType())
        //                {
        //                    if (vm.Name == null && _TabsDictionary[vm.ParentGUID][i].BaseVM.Name == null)
        //                    {
        //                        _TabsDictionary[vm.ParentGUID].Remove(_TabsDictionary[vm.ParentGUID][i]);
        //                    }
        //                    else if (vm.Name.Equals(_TabsDictionary[vm.ParentGUID][i].BaseVM.Name))
        //                    {
        //                        _TabsDictionary[vm.ParentGUID].Remove(_TabsDictionary[vm.ParentGUID][i]);
        //                    }
        //                }
        //            }
        //        }
        //        //RemoveTabFromDictionary(parentElement);
        //    }
        //    //Tabs.Remove(tab);
        //    // _TabsDictionary.Remove(id);

        //}

        //public void RemoveTab(IDynamicTab tab)
        //{
        //    if (tab.BaseVM is Plots.iConditionsImporter)
        //    {
        //        DealWithTheConditionsEditorImporters((Plots.iConditionsImporter)tab.BaseVM);
        //    }

        //    if (tab.BaseVM.ParentGUID != null)
        //    {
        //        if (_TabsDictionary.ContainsKey(tab.BaseVM.ParentGUID))
        //        {

        //            if (_TabsDictionary[tab.BaseVM.ParentGUID].Contains(tab))
        //            {
        //                _TabsDictionary[tab.BaseVM.ParentGUID].Remove(tab);
        //            }
        //            else
        //            {
        //                for (int i = 0; i < _TabsDictionary[tab.BaseVM.ParentGUID].Count; i++)
        //                {
        //                    if (_TabsDictionary[tab.BaseVM.ParentGUID][i].BaseVM == tab.BaseVM)
        //                    {
        //                        _TabsDictionary[tab.BaseVM.ParentGUID].RemoveAt(i);
        //                    }
        //                }
        //            }
        //            Tabs.Remove(tab);
        //        }
        //    }
        //}
        //public void RemoveTabAtIndex(int index)
        //{
        //    IDynamicTab tab = Tabs[index];
        //    RemoveTab(tab);
        //}


        //public void PopTabOut(object sender, EventArgs e)
        //{
        //    DynamicTabVM tabToPopOut = (DynamicTabVM)sender;
        //    //remove the tab from the tabs list
        //    RemoveTab(sender, e);
        //    //hook up the event for how to remove from the dictionary if the window is closed
        //    tabToPopOut.BaseVM.RemoveFromTabsDictionary = RemoveTabFromDictionary;
        //    Navigate(tabToPopOut.BaseVM, true, false, tabToPopOut.Header);
        //}

        //private bool DoesUserWantToDeleteTab(IDynamicTab tab)
        //{
        //    if (tab.BaseVM.HasChanges)
        //    {
        //        CustomMessageBoxVM vm = new CustomMessageBoxVM(CustomMessageBoxVM.ButtonsEnum.OK_Cancel, "Are you sure you want to remove tab '" + tab.Header + "'?");
        //        Navigate(vm, true, true, "Remove Tab");
        //        return (vm.ClickedButton == CustomMessageBoxVM.ButtonsEnum.Cancel) ? false : true;
        //    }
        //    return true;
        //}
        //public void RemoveTab(object sender, EventArgs e)
        //{
        //    IDynamicTab tab = (IDynamicTab)sender;
        //    RemoveTab(tab);

        //}

    }
}
