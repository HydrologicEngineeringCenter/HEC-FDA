using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ViewModel.Inventory.OccupancyTypes;
using static HEC.FDA.ViewModel.Inventory.OccupancyTypes.OccTypeAsset;

namespace HEC.FDA.ViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// This represents an OccupancyType while it is in the occupancy type editor. All the values can be edited.
    /// If the user saves, then a new occupancy type based off of these values will replace the original.
    /// </summary>
    public class OccupancyTypeEditable : BaseViewModel
    {
        #region Fields

        private string _Name;
        private string _Description;
        private string _DamageCategory;
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
                HasChanges = true;
            }
        }
        public int ID { get;}
        public int GroupID { get;  }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; HasChanges = true; }
        }

        public string DamageCategory
        {
            get { return _DamageCategory; }
            set
            {
                _DamageCategory = value;
                NotifyPropertyChanged();
                HasChanges = true;
            }
        }

        public ValueUncertaintyVM FoundationHeightUncertainty
        {
            get { return _FoundationHeightUncertainty; }
            set
            {
                _FoundationHeightUncertainty = value;
                _FoundationHeightUncertainty.WasModified += SomethingChanged;
                HasChanges = true;
            }
        }

        public OccTypeAsset StructureItem { get; set; }
        public OccTypeAssetWithRatio ContentItem { get; set; }
        public OccTypeAsset VehicleItem { get; set; }
        public OccTypeAssetWithRatio OtherItem { get; set; }

        /// <summary>
        /// This indicates if the occtype has ever been saved before. If false, then
        /// this is a brand new occtype. It needs to be saved new and not just an update on 
        /// an existing one in the database.
        /// </summary>
        public bool HasBeenSaved { get; set; }

        #endregion

        #region Constructors      
        public OccupancyTypeEditable(OccupancyType occtype,ref ObservableCollection<string> damageCategoriesList, bool occtypeHasBeenSaved = true)
        {
            //clone the occtype so that changes to it will not go into effect unless the user saves.
            OccupancyType clonedOcctype = new OccupancyType(occtype.ToXML());

            StructureItem = new OccTypeAsset(OcctypeAssetType.structure, clonedOcctype.StructureItem.IsChecked, clonedOcctype.StructureItem.Curve, clonedOcctype.StructureItem.ValueUncertainty.Distribution);
            ContentItem = new OccTypeAssetWithRatio(clonedOcctype.ContentItem);               
            VehicleItem = new OccTypeAsset(OcctypeAssetType.vehicle, clonedOcctype.VehicleItem.IsChecked, clonedOcctype.VehicleItem.Curve, clonedOcctype.VehicleItem.ValueUncertainty.Distribution);
            OtherItem = new OccTypeAssetWithRatio(clonedOcctype.OtherItem);   

            StructureItem.DataModified += OcctypeItemDataModified;
            ContentItem.DataModified += OcctypeItemDataModified;
            VehicleItem.DataModified += OcctypeItemDataModified;
            OtherItem.DataModified += OcctypeItemDataModified;

            StructureItem.Curve.SetMinMaxValues(0, 100);

            DamageCategoriesList = damageCategoriesList;
            HasBeenSaved = occtypeHasBeenSaved;

            Name = clonedOcctype.Name;
            Description = clonedOcctype.Description;
            DamageCategory = clonedOcctype.DamageCategory;

            ID = clonedOcctype.ID;

            FoundationHeightUncertainty = new FoundationValueUncertaintyVM(clonedOcctype.FoundationHeightUncertainty);

            HasChanges = false;
        }

        private void OcctypeItemDataModified(object sender, EventArgs e)
        {
            HasChanges = true;
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
                if (!vm.HasFatalError)
                {
                    DamageCategory = vm.Name;
                    _DamageCategoriesList.Add(vm.Name);
                }
            }
        }

        private void SomethingChanged(object sender, EventArgs e)
        {
            HasChanges = true;
        }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => !string.IsNullOrWhiteSpace(Name), "Name cannot be blank or whitespace.");
        }

        public FdaValidationResult HasFatalErrors(List<string> occtypeNames)
        {
            FdaValidationResult vr = new FdaValidationResult();
            //check for blank name and name conflicts
            if (HasFatalError)
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

        public OccupancyType CreateOccupancyType()
        {
            return new OccupancyType(Name, Description, DamageCategory, StructureItem, ContentItem, VehicleItem, OtherItem,
                FoundationHeightUncertainty.CreateOrdinate(), ID);
        }
      
    }
}
