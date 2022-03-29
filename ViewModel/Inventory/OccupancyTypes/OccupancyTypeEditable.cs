using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ViewModel.Inventory.OccupancyTypes;
using static HEC.FDA.ViewModel.Inventory.OccupancyTypes.OccTypeItem;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// This represents an IOccupancyType while it is in the occupancy type editor. All the values can be edited.
    /// If the user saves, then a new occupancy type based off of these values will replace the original.
    /// </summary>
    public class OccupancyTypeEditable : BaseViewModel, IOccupancyTypeEditable
    {
        #region Fields
        public event EventHandler UpdateMessagesEvent;

        private string _Name;
        private string _Description;
        private string _DamageCategory;
        private bool _IsModified;
        private ObservableCollection<string> _DamageCategoriesList = new ObservableCollection<string>();
        private ValueUncertaintyVM _FoundationHeightUncertainty;

        #endregion

        #region Properties 

        public ObservableCollection<string> DamageCategoriesList
        {
            get { return _DamageCategoriesList; }
            set { _DamageCategoriesList = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
                NotifyPropertyChanged();
                IsModified = true;
            }
        }
        public int ID { get; set; }
        public int GroupID { get; set; }
        /// <summary>
        /// This is used by the occtype editor to determine if this occtype
        /// was edited. This value should be set to false every time the editor
        /// is opened.
        /// </summary>
        public bool IsModified
        {
            get {return _IsModified;}
            set{ _IsModified = value; NotifyPropertyChanged();}
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; IsModified = true; }
        }

        public string DamageCategory
        {
            get { return _DamageCategory; }
            set
            {
                _DamageCategory = value;
                NotifyPropertyChanged();
                IsModified = true;
            }
        }

        public ValueUncertaintyVM FoundationHeightUncertainty
        {
            get { return _FoundationHeightUncertainty; }
            set
            {
                _FoundationHeightUncertainty = value;
                _FoundationHeightUncertainty.WasModified += SomethingChanged;
                IsModified = true;
            }
        }

        public OccTypeItem StructureItem { get; set; }
        public OccTypeItemWithRatio ContentItem { get; set; }
        public OccTypeItem VehicleItem { get; set; }
        public OccTypeItemWithRatio OtherItem { get; set; }

        /// <summary>
        /// This indicates if the occtype has ever been saved before. If false, then
        /// this is a brand new occtype. It needs to be saved new and not just an update on 
        /// an existing one in the database.
        /// </summary>
        public bool HasBeenSaved { get; set; }

        #endregion

        #region Constructors      
        public OccupancyTypeEditable(IOccupancyType occtype,ref ObservableCollection<string> damageCategoriesList, bool occtypeHasBeenSaved = true)
        {
            StructureItem = new OccTypeItem(OcctypeItemType.structure, occtype.StructureItem.IsChecked, occtype.StructureItem.Curve, occtype.StructureItem.ValueUncertainty.Distribution);
            ContentItem = new OccTypeItemWithRatio( occtype.ContentItem);               
            VehicleItem = new OccTypeItem(OcctypeItemType.vehicle, occtype.VehicleItem.IsChecked, occtype.VehicleItem.Curve, occtype.VehicleItem.ValueUncertainty.Distribution);
            OtherItem = new OccTypeItemWithRatio( occtype.OtherItem);   

            StructureItem.DataModified += OcctypeItemDataModified;
            ContentItem.DataModified += OcctypeItemDataModified;
            VehicleItem.DataModified += OcctypeItemDataModified;
            OtherItem.DataModified += OcctypeItemDataModified;

            DamageCategoriesList = damageCategoriesList;
            HasBeenSaved = occtypeHasBeenSaved;
            //clone the occtype so that changes to it will not go into effect
            //unless the user saves.
            IOccupancyType clonedOcctype = new OccupancyType(occtype);

            Name = clonedOcctype.Name;
            Description = clonedOcctype.Description;
            DamageCategory = clonedOcctype.DamageCategory;

            ID = clonedOcctype.ID;
            GroupID = clonedOcctype.GroupID;

            FoundationHeightUncertainty = new FoundationValueUncertaintyVM(clonedOcctype.FoundationHeightUncertainty);

            HasChanges = false;
            IsModified = false;
        }

        private void OcctypeItemDataModified(object sender, EventArgs e)
        {
            IsModified = true;
        }

        #endregion
        public void LaunchNewDamCatWindow()
        {
            CreateNewDamCatVM vm = new CreateNewDamCatVM(DamageCategoriesList.ToList());
            string header = "New Damage Category";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "NewDamageCategory");
            Navigate(tab, true, true);
            if (vm.WasCanceled == false)
            {
                if (vm.HasError == false)
                {
                    //store the new damage category
                    DamageCategory = vm.Name;
                    _DamageCategoriesList.Add(vm.Name);
                }
            }
        }

        private void SomethingChanged(object sender, EventArgs e)
        {
            IsModified = true;
        }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != null, "Name cannot be empty.");
            AddRule(nameof(Name), () => Name != "", "Name cannot be empty.");
        }

        public FdaValidationResult HasWarnings()
        {
            FdaValidationResult vr = new FdaValidationResult();
            vr.AddErrorMessage(IsValueUncertaintyConstructable(FoundationHeightUncertainty, "foundation height uncertainty").ErrorMessage);
            vr.AddErrorMessage(StructureItem.IsItemValid().ErrorMessage);
            vr.AddErrorMessage(ContentItem.IsItemValid().ErrorMessage);
            vr.AddErrorMessage(VehicleItem.IsItemValid().ErrorMessage);
            vr.AddErrorMessage(OtherItem.IsItemValid().ErrorMessage);

            return vr;
        }

        public FdaValidationResult HasFatalErrors(List<string> occtypeNames)
        {
            FdaValidationResult vr = new FdaValidationResult();
            //check for blank name and name conflicts
            if(HasFatalError)
            {
                vr.AddErrorMessage(Error);
            }

            IEnumerable<string> duplicates = occtypeNames.GroupBy(x => x)
                         .Where(group => group.Count() > 1)
                         .Select(group => group.Key);

            if(duplicates.Contains(Name))
            {
                vr.AddErrorMessage("The name '" + Name + "' already exists. Occupancy type names must be unique.");
            }

            return vr;
        }

        private FdaValidationResult IsValueUncertaintyConstructable(ValueUncertaintyVM uncertaintyVM, string uncertaintyName)
        {
            FdaValidationResult uncertaintyVR = uncertaintyVM.IsValueUncertaintyValid();
            if (!uncertaintyVR.IsValid)
            {
                uncertaintyVR.InsertNewLineMessage(0, "Foundation Height Uncertainty:");
            }
            return uncertaintyVR;
        }

        public IOccupancyType CreateOccupancyType()
        {
            return new OccupancyType(Name, Description, GroupID, DamageCategory, StructureItem, ContentItem, VehicleItem, OtherItem,
                FoundationHeightUncertainty.CreateOrdinate(), ID);
        }

        /// <summary>
        /// Tries to save the occtype. If the occtype was not constructable, then a list of errors
        /// is returned and the occtype didn't save.
        /// </summary>
        /// <returns></returns>
        public void SaveOcctype()
        {
            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();
            IOccupancyType ot = CreateOccupancyType();

            if (HasBeenSaved)
            {
                manager.SaveModifiedOcctype(ot);
            }
            else if (!HasBeenSaved)
            {
                //save this as a new occtype
                //if it has never been saved then we need a new occtype id for it.
                ot.ID = manager.GetIdForNewOccType(ot.GroupID);
                manager.SaveNewOccType(ot);
            }

            IsModified = false;
            //this will disable the save button.
            HasChanges = false;
        }       
    }
}
