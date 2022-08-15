using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using HEC.Plotting.SciChart2D.Charts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    //[Author(q0heccdm, 7 / 14 / 2017 1:55:50 PM)]
    public class OccupancyTypesEditorVM : BaseEditorVM
    {
        public event EventHandler CloseEditor;
        private List<IOccupancyTypeGroupEditable> _GroupsToUpdateInParentTable = new List<IOccupancyTypeGroupEditable>();
        private IOccupancyTypeEditable _SelectedOccType;
        private ObservableCollection<IOccupancyTypeGroupEditable> _OccTypeGroups;

        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/14/2017 1:55:50 PM
        #endregion
        #region Fields
        private string _SavingText;
        private IOccupancyTypeGroupEditable _SelectedOccTypeGroup;
        private List<string> _DamageCategoriesList = new List<string>();
        private string _Year;
        private string _Module;
        #endregion
        #region Properties

        public string SavingText
        {
            get { return _SavingText; }
            set { _SavingText = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<IOccupancyTypeGroupEditable> OccTypeGroups
        {
            get { return _OccTypeGroups; }
            set { _OccTypeGroups = value; NotifyPropertyChanged(); }
        }
        public IOccupancyTypeEditable SelectedOccType
        {
            get { return _SelectedOccType; }
            set
            {
                _SelectedOccType = value;
                if (_SelectedOccType == null)
                {
                    return;
                }
                NotifyPropertyChanged();
            }
        }
        public Chart2D Chart { get; set; }
     
        public string Year
        {
            get { return _Year; }
            set { _Year = value; NotifyPropertyChanged(); }
        }

        public string Module
        {
            get { return _Module; }
            set { _Module = value; }
        }
       
        public List<string> DamageCategoriesList
        {
            get { return _DamageCategoriesList; }
            set { _DamageCategoriesList = value; NotifyPropertyChanged(); }
        }

        public IOccupancyTypeGroupEditable SelectedOccTypeGroup
        {
            get { return _SelectedOccTypeGroup; }
            set 
            { 
                if (value == null) 
                { 
                    return; 
                } 
                _SelectedOccTypeGroup = value; 
                NotifyPropertyChanged(); 
            }
        }
       
        #endregion
        #region Constructors
        public OccupancyTypesEditorVM(EditorActionManager manager) : base(manager)
        {
            //I just needed some name so that it doesn't fail the empty name test that is now universal.
            Name = "OccTypeEditor";
            Chart = new Chart2D();
            OccTypeGroups = new ObservableCollection<IOccupancyTypeGroupEditable>();
        }

        public void FillEditor(List<OccupancyTypesElement> occtypeElements)
        {
            if (occtypeElements.Count == 0)
            {
                CreateDefaultOccTypeGroup();
            }
            else
            {
                ObservableCollection<IOccupancyTypeGroupEditable> editableGroups = CreateEditableOcctypeGroups(occtypeElements);
                OccTypeGroups = editableGroups;
                LoadDamageCategoriesList();
            }
            SelectedOccTypeGroup = OccTypeGroups.First();
            SelectedOccType = SelectedOccTypeGroup.Occtypes.FirstOrDefault();
            ClearAllModifiedLists();
        }
        #endregion
        #region Voids

        #region create editable occtype objects
        //group id to list of dam cat names
        private Dictionary<int, ObservableCollection<string>> _GroupsToDamcats = new Dictionary<int, ObservableCollection<string>>();

        private ObservableCollection<IOccupancyTypeGroupEditable> CreateEditableOcctypeGroups(List<OccupancyTypesElement> occtypeElements)
        {
            ObservableCollection<IOccupancyTypeGroupEditable> editableGroups = new ObservableCollection<IOccupancyTypeGroupEditable>();
            foreach (OccupancyTypesElement group in occtypeElements)
            {
                editableGroups.Add(CreateEditableGroup(group));
            }
            return editableGroups;
        }

        private IOccupancyTypeGroupEditable CreateEditableGroup(OccupancyTypesElement group)
        {
            ObservableCollection<string> uniqueDamCatsForGroup = GetDamageCategoriesList(group);
            _GroupsToDamcats.Add(group.ID, uniqueDamCatsForGroup);
            List<IOccupancyTypeEditable> editableOcctypes = new List<IOccupancyTypeEditable>();

            foreach (IOccupancyType ot in group.ListOfOccupancyTypes)
            {
                OccupancyTypeEditable otEdit = new OccupancyTypeEditable(ot, ref uniqueDamCatsForGroup);
                otEdit.RequestNavigation += this.Navigate;
                editableOcctypes.Add(otEdit);
            }

            //now we have a list of all the occtypes. They get cloned in the OccupancyTypeEditable ctor.
            //OccTypePersistenceManager manager = PersistenceFactory.GetOccTypeManager();
            //int groupID = manager.GetGroupId(group.Name);
            IOccupancyTypeGroupEditable occTypeGroup = new OccupancyTypeGroupEditable(group.ID, group.Name, editableOcctypes);
            if (occTypeGroup.Occtypes.Count == 0)
            {
                occTypeGroup.Occtypes.Add(CreateDefaultOcctype(group.ID));
            }
            return occTypeGroup;
        }
        private ObservableCollection<string> GetDamageCategoriesList(OccupancyTypesElement group)
        {
            ObservableCollection<string> uniqueDamCats = new ObservableCollection<string>();
            foreach (IOccupancyType ot in group.ListOfOccupancyTypes)
            {
                if (!uniqueDamCats.Contains(ot.DamageCategory))
                {
                    uniqueDamCats.Add(ot.DamageCategory);
                }
            }
            return uniqueDamCats;
        }

        #endregion

        private List<string> GetAllOccTypeNames()
        {
            List<string> occtypeNames = new List<string>();
            if (SelectedOccTypeGroup == null) { return occtypeNames; }
            foreach (IOccupancyTypeEditable ot in SelectedOccTypeGroup.Occtypes)
            {
                occtypeNames.Add(ot.Name);
            }
            return occtypeNames;
        }
        public void LaunchNewOccTypeWindow()
        {
            //we want a new occtype. If there is an occtype and occtype group then we can use
            //some of that data. We don't want to just copy the other occtype, however, that is
            //another option. It seems to me that if there is no occtype group, a user shouldn't be
            //allowed to create an occtype. The user is able to delete all groups and/or all occtypes.

            if (SelectedOccTypeGroup == null)
            {
                MessageBox.Show("An occupancy type group must first be imported in order to create a new occupancy type.", "No Occupancy Type Group", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }

            CreateNewDamCatVM vm = new CreateNewDamCatVM(GetAllOccTypeNames());
            string header = "New Occupancy Type";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "NewOccupancyType");
            Navigate(tab, true, true);
            if (!vm.WasCanceled)
            {
                if (!vm.HasFatalError)
                {
                    ObservableCollection<string> damCatOptions = new ObservableCollection<string>();
                    string damCatName = "";
                    if (SelectedOccType != null)
                    {
                        damCatName = SelectedOccType.DamageCategory;
                        damCatOptions = SelectedOccType.DamageCategoriesList;
                    }

                    //create the new occupancy type
                    IOccupancyType newOT = new OccupancyType(vm.Name, damCatName, SelectedOccTypeGroup.ID);
                    OccupancyTypeEditable otEditable = new OccupancyTypeEditable(newOT, ref damCatOptions, false);
                    otEditable.RequestNavigation += this.Navigate;

                    //add the occtype to the list and select it
                    SelectedOccTypeGroup.Occtypes.Add(otEditable);
                    SelectedOccType = otEditable;
                    otEditable.IsModified = true;
                }
            }
        }

        public void LaunchCopyOccTypeWindow()
        {
            if (SelectedOccType != null)
            {
                IOccupancyType newOT = SelectedOccType.CreateOccupancyType();

                CreateNewDamCatVM vm = new CreateNewDamCatVM(SelectedOccType.Name + "_Copy", GetAllOccTypeNames());
                string header = "Copy Occupancy Type";
                DynamicTabVM tab = new DynamicTabVM(header, vm, "CopyOccupancyType");
                Navigate(tab, true, true);
                if (vm.WasCanceled == false)
                {
                    if (vm.HasFatalError == false)
                    {
                        newOT.Name = vm.Name;
                        ObservableCollection<string> damcats = _GroupsToDamcats[newOT.GroupID];
                        OccupancyTypeEditable otEditable = new OccupancyTypeEditable(newOT, ref damcats, false);
                        otEditable.RequestNavigation += this.Navigate;

                        SelectedOccTypeGroup.Occtypes.Add(otEditable);
                        SelectedOccType = otEditable;
                        otEditable.IsModified = true;
                    }
                }
            }
        }

        public void DeleteOccType()
        {
            if(SelectedOccType == null || SelectedOccTypeGroup == null || SelectedOccTypeGroup.Occtypes == null)
            {
                return;
            }

            //pop up a message box that asks the user if they want to permanently delete the occtype
            if (MessageBox.Show("Do you want to permanently delete this occupancy type?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                //then permanently delete it.
                IElementManager manager = PersistenceFactory.GetElementManager<OccupancyTypesElement>();
                //todo: bring back to life
                //manager.DeleteOcctype(SelectedOccType);

                int selectedIndex = SelectedOccTypeGroup.Occtypes.IndexOf(SelectedOccType);
                SelectedOccTypeGroup.Occtypes.Remove(SelectedOccType);
                //set the selected occtype to be the one before, unless at 0
                if (selectedIndex > 0)
                {
                    SelectedOccType = SelectedOccTypeGroup.Occtypes[selectedIndex - 1];
                }
                else //we just deleted the zeroth item
                {
                    if (SelectedOccTypeGroup.Occtypes.Count > 0)
                    {
                        SelectedOccType = SelectedOccTypeGroup.Occtypes[0];
                    }
                    else //there are no more occtypes
                    {
                        if (MessageBox.Show("This is the last occupancy type in this group. Do you want to permanently delete the group?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            //delete the group
                            DeleteOccTypeGroup();
                        }
                        else
                        {
                            //create a new default occtype and add it to the group
                            IOccupancyTypeEditable otEditable = CreateDefaultOcctype(SelectedOccTypeGroup.ID);

                            //add the occtype to the list and select it
                            SelectedOccTypeGroup.Occtypes.Add(otEditable);
                            SelectedOccType = otEditable;
                            otEditable.IsModified = true;
                        }

                    }
                }
            }
            else
            {
                return;
            }
        }

        private IOccupancyTypeEditable CreateDefaultOcctype(int groupID)
        {
            IOccupancyType newOT = new OccupancyType("New Occupancy Type", "", groupID);
            ObservableCollection<string> damCatOptions = new ObservableCollection<string>();
            OccupancyTypeEditable otEditable = new OccupancyTypeEditable(newOT, ref damCatOptions, false);
            otEditable.RequestNavigation += this.Navigate;
            return otEditable;
        }

        public void DeleteOccTypeGroup()
        {
            if(SelectedOccTypeGroup == null)
            {
                return;
            }

            if (MessageBox.Show("Do you want to permanently delete this occupancy type group?", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                int selectedIndex = OccTypeGroups.IndexOf(SelectedOccTypeGroup);
                if (_GroupsToDamcats.ContainsKey(SelectedOccTypeGroup.ID))
                {
                    _GroupsToDamcats.Remove(SelectedOccTypeGroup.ID);
                }
                OccTypeGroups.Remove(SelectedOccTypeGroup);

                OccTypePersistenceManager manager = PersistenceFactory.GetOccTypeManager();
               // manager.DeleteOcctypeGroup(SelectedOccTypeGroup.ID);
                //set the selected occtype to be the one before, unless at 0
                if (selectedIndex > 0)
                {
                    SelectedOccTypeGroup = OccTypeGroups[selectedIndex - 1];
                }
                else //we just deleted the zeroth item
                {
                    if (OccTypeGroups.Count > 0)
                    {
                        SelectedOccTypeGroup = OccTypeGroups[0];
                    }
                    else //there are no more occtype groups
                    {
                        //if no, then create a new default group? If yes, then exit?
                        if (MessageBox.Show("There are no more occtype groups. Do you want to exit the editor.", "Confirm",
                            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            CloseEditor?.Invoke(this, new EventArgs());
                        }
                        else
                        {
                            //create a default occtype group?
                            CreateDefaultOccTypeGroup();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This creates and then saves a new occtype element. When this element gets saved,
        /// the study cache gets updated which then tells the owner element a new element has been added.
        /// The owner element will see if this editor is open and will add it in "AddGroup()".
        /// </summary>
        public void CreateDefaultOccTypeGroup()
        {
            IElementManager manager = PersistenceFactory.GetElementManager<OccupancyTypesElement>();
            string groupName = "Occupancy Type Group";
            int groupId = manager.GetNextAvailableId();
            OccupancyTypesElement elem = new OccupancyTypesElement(groupName, new List<IOccupancyType>(), groupId);
            //calling the save here should add it to the cache, which tells the occtype owner to add it to this editor
            //if it is open. see AddGroup() in this class.
            manager.SaveNew(elem);
        }

        /// <summary>
        /// Updates the list of groups from the study cache after new occtypes have been imported
        /// </summary>
        public void AddGroup(OccupancyTypesElement element)
        {
            IOccupancyTypeGroupEditable group = CreateEditableGroup(element);
            if(group.Occtypes.Count == 0)
            {
                group.Occtypes.Add(CreateDefaultOcctype(group.ID));
            }
            OccTypeGroups.Add(group);
            SelectedOccTypeGroup = group;
        }

        public void LaunchImportNewOccTypeGroup()
        {
            //get the parent from the studycache and launch the importer
            OccupancyTypesOwnerElement owner = StudyCache.GetParentElementOfType<OccupancyTypesOwnerElement>();
            owner.ImportFromFile(this, new EventArgs());
            //i need to update the group list in here. The owner will import like normal and then
            //check to see if the occtype editor (this) is open. If it is then it will update the list
            //in here.
        }

        public void LaunchRenameOcctypeGroup()
        {
            CreateNewDamCatVM vm = new CreateNewDamCatVM(SelectedOccTypeGroup.Name, DamageCategoriesList);
            string header = "Rename Occupancy Type Group";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "RenameOccupancyTypeGroup");
            Navigate(tab, true, true);
            if (!vm.WasCanceled)
            {
                if (!vm.HasFatalError)
                {
                    string newName = vm.Name;
                    SelectedOccTypeGroup.Name = newName;

                    if (!_GroupsToUpdateInParentTable.Contains(SelectedOccTypeGroup))
                    {
                        _GroupsToUpdateInParentTable.Add(SelectedOccTypeGroup);
                    }
                }
            }
        }

        private void LoadDamageCategoriesList()
        {
            if (_SelectedOccTypeGroup == null)
            {
                return;
            }

            List<string> uniqueDamCats = new List<string>();
            foreach (IOccupancyTypeEditable ot in _SelectedOccTypeGroup.Occtypes)
            {
                if (!uniqueDamCats.Contains(ot.DamageCategory))
                {
                    uniqueDamCats.Add(ot.DamageCategory);
                }
            }
            DamageCategoriesList = uniqueDamCats;
        }

        /// <summary>
        /// This will just save the occupancy type that is selected. It will not save all of the groups or all of the occtypes. To do that see "SaveAll()"
        /// </summary>
        public override void Save()
        {
            IElementManager manager = PersistenceFactory.GetElementManager<OccupancyTypesElement>();
            //todo: bring this back to life.
            //manager.SaveModifiedGroups(_GroupsToUpdateInParentTable);          

            List<SaveAllReportGroupVM> warningReports = new List<SaveAllReportGroupVM>();
            List<SaveAllReportGroupVM> fatalErrorReports = new List<SaveAllReportGroupVM>();

            foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
            {
                SaveAllReportGroupVM saveAllReport = group.SaveAll();
                if (saveAllReport != null && saveAllReport.HasWarnings)
                {
                    warningReports.Add(saveAllReport);
                }
                if (saveAllReport != null && saveAllReport.HasFatalErrors)
                {
                    fatalErrorReports.Add(saveAllReport);
                }
                string lastEditDate = DateTime.Now.ToString("G");
                SavingText = "Last Saved: " + lastEditDate;
            }

            //we only want to show the save report if there are occtypes that did not save
            if (warningReports.Count > 0)
            {
                SaveAllReportVM report = new SaveAllReportVM(warningReports);
                string header = "Save Warnings";
                DynamicTabVM tab = new DynamicTabVM(header, report, "SaveAllReport", false, false);
                Navigate(tab, true, true);
            }
            if (fatalErrorReports.Count > 0)
            {
                OcctypeErrorsReportVM report = new OcctypeErrorsReportVM(fatalErrorReports);
                string errorHeader = "Occupancy Type Errors";
                DynamicTabVM errorTab = new DynamicTabVM(errorHeader, report, "ErrorReport");
                Navigate(errorTab, true, true);
            }

            //clear the modified groups
            _GroupsToUpdateInParentTable.Clear();
        }

        /// <summary>
        /// Sets all the "IsModified" flags back to false and clears all the newly created lists.
        /// </summary>
        public void ClearAllModifiedLists()
        {
            foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
            {
                group.IsModified = false;
                foreach (IOccupancyTypeEditable ot in group.Occtypes)
                {
                    if (ot.HasBeenSaved)
                    {
                        ot.IsModified = false;
                    }
                }
            }
        }

        #endregion
        #region Functions

        public override bool IsOkToClose()
        {
            bool areUnsavedChanges = false;
            //also need to check that the group names have been saved
            if(_GroupsToUpdateInParentTable.Count>0)
            {
                areUnsavedChanges = true;
            }
            foreach (IOccupancyTypeGroupEditable group in OccTypeGroups)
            {
                if(group.ModifiedOcctypes.Count>0)
                {
                    //there are occtypes that have not been saved
                    areUnsavedChanges = true;
                    break;
                }             
            }

            if (areUnsavedChanges)
            {
                if (MessageBox.Show("There are unsaved changes. Are you sure you want to close?", "Unsaved Changes", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //this prevents the tab controller from asking if we want to close with unsaved changes.
                HasChanges = false;
                return true;
            }
        }

        #endregion

    }
}
