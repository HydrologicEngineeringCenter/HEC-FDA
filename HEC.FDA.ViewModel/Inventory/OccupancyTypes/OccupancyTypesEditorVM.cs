using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Utilities;
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
        #region Notes
        // Created By: q0heccdm
        // Created Date: 7/14/2017 1:55:50 PM
        #endregion
        #region Fields
        private OccupancyTypeEditable _SelectedOccType;
        ObservableCollection<string> _DamageCategoriesList = new ObservableCollection<string>();
        #endregion
        #region Properties

        

        public OccupancyTypeEditable SelectedOccType
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

        public ObservableCollection<string> DamageCategoriesList
        {
            get { return _DamageCategoriesList; }
        }

        public OccupancyTypeGroupEditable SelectedOccTypeGroup
        {
            get;
        }

        #endregion
        #region Constructors
        public OccupancyTypesEditorVM( EditorActionManager manager) : base( manager)
        {
            SelectedOccTypeGroup = CreateDefaultEditableGroup();
            SelectedOccType = SelectedOccTypeGroup.Occtypes.FirstOrDefault();
            //registering the child vm's allows the "HasChanges" to bubble up and change this VM's "HasChanges".
            RegisterChildViewModel(SelectedOccTypeGroup);
            foreach (OccupancyTypeEditable ot in SelectedOccTypeGroup.Occtypes)
            {
                RegisterChildViewModel(ot);
                ot.HasChanges = true;
            }
        }

        public OccupancyTypesEditorVM(OccupancyTypesElement elem, EditorActionManager manager) : base(elem, manager)
        {
            LoadDamageCategories(elem.ListOfOccupancyTypes);
            SelectedOccTypeGroup = CreateEditableGroup(elem);
            SelectedOccType = SelectedOccTypeGroup.Occtypes.FirstOrDefault();
            //registering the child vm's allows the "HasChanges" to bubble up and change this VM's "HasChanges".
            RegisterChildViewModel(SelectedOccTypeGroup);
            foreach(OccupancyTypeEditable ot in SelectedOccTypeGroup.Occtypes)
            {
                RegisterChildViewModel(ot);
            }
            ClearAllModifiedLists();
        }

        #endregion
        #region Voids

        private OccupancyTypeGroupEditable CreateDefaultEditableGroup()
        {
            List<OccupancyTypeEditable> editableOcctypes = new List<OccupancyTypeEditable>();       
            OccupancyTypeGroupEditable occTypeGroup = new OccupancyTypeGroupEditable(-1, "Default Occtype Group", editableOcctypes);
            
            return occTypeGroup;
        }
        private OccupancyTypeGroupEditable CreateEditableGroup(OccupancyTypesElement group)
        {
            List<OccupancyTypeEditable> editableOcctypes = new List<OccupancyTypeEditable>();

            foreach (OccupancyType ot in group.ListOfOccupancyTypes)
            {
                OccupancyTypeEditable otEdit = new OccupancyTypeEditable(ot, ref _DamageCategoriesList);
                otEdit.RequestNavigation += this.Navigate;
                editableOcctypes.Add(otEdit);
            }

            OccupancyTypeGroupEditable occTypeGroup = new OccupancyTypeGroupEditable(group.ID, group.Name, editableOcctypes);
            if (occTypeGroup.Occtypes.Count == 0)
            {
                occTypeGroup.Occtypes.Add(CreateDefaultOcctype(group.ID));
            }
            return occTypeGroup;
        }

        private List<string> GetAllOccTypeNames()
        {
            List<string> occtypeNames = new List<string>();
            if (SelectedOccTypeGroup != null)
            {
                foreach (OccupancyTypeEditable ot in SelectedOccTypeGroup.Occtypes)
                {
                    occtypeNames.Add(ot.Name);
                }
            }
            return occtypeNames;
        }
        public void LaunchNewOccTypeWindow()
        {
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
                    string damCatName = "";
                    if (SelectedOccType != null)
                    {
                        damCatName = SelectedOccType.DamageCategory;
                    }

                    //create the new occupancy type
                    OccupancyType newOT = new OccupancyType(vm.Name, damCatName, SelectedOccTypeGroup.ID);
                    OccupancyTypeEditable otEditable = new OccupancyTypeEditable(newOT, ref _DamageCategoriesList, false);
                    otEditable.RequestNavigation += this.Navigate;

                    //add the occtype to the list and select it
                    SelectedOccTypeGroup.Occtypes.Add(otEditable);
                    SelectedOccType = otEditable;
                    otEditable.HasChanges = true;
                    RegisterChildViewModel(otEditable);
                    HasChanges = true;
                }
            }
        }

        public void LaunchCopyOccTypeWindow()
        {
            if (SelectedOccType != null)
            {
                OccupancyType newOT = SelectedOccType.CreateOccupancyType();

                CreateNewDamCatVM vm = new CreateNewDamCatVM(SelectedOccType.Name + "_Copy", GetAllOccTypeNames());
                string header = "Copy Occupancy Type";
                DynamicTabVM tab = new DynamicTabVM(header, vm, "CopyOccupancyType");
                Navigate(tab, true, true);
                if (vm.WasCanceled == false)
                {
                    if (vm.HasFatalError == false)
                    {
                        newOT.Name = vm.Name;
                        OccupancyTypeEditable otEditable = new OccupancyTypeEditable(newOT, ref _DamageCategoriesList, false);
                        otEditable.RequestNavigation += this.Navigate;

                        SelectedOccTypeGroup.Occtypes.Add(otEditable);
                        SelectedOccType = otEditable;
                        otEditable.HasChanges = true;
                        RegisterChildViewModel(otEditable);
                        HasChanges = true;
                    }
                }
            }
        }

        public void DeleteOccType()
        {
            //I don't have to track which occtypes have been deleted. They will just get removed from the occtype list.
            //when the save is called, it will save out the occtypes in the list which means the deleted ones won't get saved.
            if (SelectedOccType != null && SelectedOccTypeGroup != null && SelectedOccTypeGroup.Occtypes != null)
            {
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
                        //create a new default occtype and add it to the group
                        OccupancyTypeEditable otEditable = CreateDefaultOcctype(SelectedOccTypeGroup.ID);

                        //add the occtype to the list and select it
                        SelectedOccTypeGroup.Occtypes.Add(otEditable);
                        SelectedOccType = otEditable;
                        otEditable.HasChanges = true;
                    }
                }
            }
        }

        private OccupancyTypeEditable CreateDefaultOcctype(int groupID)
        {
            OccupancyType newOT = new OccupancyType("New Occupancy Type", "", groupID);
            ObservableCollection<string> damCatOptions = new ObservableCollection<string>();
            OccupancyTypeEditable otEditable = new OccupancyTypeEditable(newOT, ref damCatOptions, false);
            otEditable.RequestNavigation += this.Navigate;
            otEditable.HasChanges = true;
            return otEditable;
        }      

        private void LoadDamageCategories(List<OccupancyType> occtypes)
        {
            foreach (OccupancyType ot in occtypes)
            {
                if (!DamageCategoriesList.Contains(ot.DamageCategory))
                {
                    DamageCategoriesList.Add(ot.DamageCategory);
                }
            }
        }

        /// <summary>
        /// Currently the only errors that will prevent a save are repeat occtype names or a blank occtype name.
        /// </summary>
        /// <returns></returns>
        private FdaValidationResult ValidateEditor()
        {
            FdaValidationResult validationResult = new FdaValidationResult();
            if(SelectedOccTypeGroup.Occtypes.Count == 0)
            {
                validationResult.AddErrorMessage("At least one occupancy type is required to save.");
            }

            List<string> allOTNames = GetAllOccTypeNames();
            foreach(OccupancyTypeEditable ot in SelectedOccTypeGroup.Occtypes)
            {
                validationResult.AddErrorMessage( ot.HasFatalErrors(allOTNames).ErrorMessage);
            }
            return validationResult;
        }

        /// <summary>
        /// This will just save the occupancy type that is selected. It will not save all of the groups or all of the occtypes. To do that see "SaveAll()"
        /// </summary>
        public override void Save()
        {
            FdaValidationResult vr = ValidateEditor();
            if(vr.IsValid)
            {
                OccupancyTypesElement elemToSave = new OccupancyTypesElement(Name, DateTime.Now.ToString("G"), Description, SelectedOccTypeGroup.CreateOcctypes(), SelectedOccTypeGroup.ID);
                if(elemToSave.ID == -1)
                {
                    //the id will be -1 if the user is creating a new one.
                    int newID = GetElementID<OccupancyTypesElement>();
                    elemToSave.ID = newID;
                    SelectedOccTypeGroup.ID = newID;
                }
                
                Save(elemToSave);
            }
            else
            {
                MessageBox.Show(vr.UniqueErrorMessage(), "Errors Preventing Save", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Sets all the "IsModified" flags back to false and clears all the newly created lists.
        /// </summary>
        private void ClearAllModifiedLists()
        {
            foreach (OccupancyTypeEditable ot in SelectedOccTypeGroup.Occtypes)
            {
                    ot.HasChanges = false;               
            }
        }

        #endregion
   

    }
}
