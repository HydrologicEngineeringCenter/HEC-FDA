using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaLogging;
using FdaViewModel.Inventory.DamageCategory;
using FdaViewModel.Saving.PersistenceManagers;
using FdaViewModel.Utilities.Transactions;
using FunctionsView.ViewModel;

namespace FdaViewModel.Inventory.OccupancyTypes
{
    /// <summary>
    /// This represents an IOccupancyType while it is in the occupancy type editor. All the values can be edited.
    /// If the user saves, then a new occupancy type based off of these values will replace the original.
    /// </summary>
    public class OccupancyTypeEditable : BaseViewModel,IDisplayLogMessages, IOccupancyTypeEditable
    {
        public event EventHandler UpdateMessagesEvent;

        private bool _CalculateStructureDamage;
        private bool _CalculateContentDamage;
        private bool _CalculateVehicleDamage;
        private bool _CalculateOtherDamage;
        private string _Name;
        private string _Description;
        private IDamageCategory _DamageCategory;
        private bool _IsModified;


        private ObservableCollection<FdaLogging.LogItem> _MessageRows = new ObservableCollection<FdaLogging.LogItem>();
        private LoggingLevel _SaveStatusLevel;
        private bool _IsExpanded;
        private bool _SaveStatusVisible;

        //public IOccupancyType OccType { get; set; }
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
            get
            {
                return _IsModified;
            }
            set
            {
                _IsModified = value;
                NotifyPropertyChanged();
            }
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; IsModified = true; IsModified = true; }
        }

        public IDamageCategory DamageCategory
        {
            get { return _DamageCategory; }
            set
            {
                _DamageCategory = value;
                NotifyPropertyChanged();
                IsModified = true;
            }
        }

        public bool CalculateStructureDamage
        {
            get { return _CalculateStructureDamage; }
            set { _CalculateStructureDamage = value; IsModified = true; }
        }

        public bool CalculateContentDamage
        {
            get { return _CalculateContentDamage; }
            set { _CalculateContentDamage = value; IsModified = true; }
        }

        public bool CalculateVehicleDamage
        {
            get { return _CalculateVehicleDamage; }
            set { _CalculateVehicleDamage = value; IsModified = true; }
        }

        public bool CalculateOtherDamage
        {
            get { return _CalculateOtherDamage; }
            set { _CalculateOtherDamage = value; IsModified = true; }
        }
        public ValueUncertaintyVM StructureValueUncertainty 
        { 
            get; set; }
        public ValueUncertaintyVM ContentValueUncertainty { get; set; }
        public ValueUncertaintyVM VehicleValueUncertainty { get; set; }
        public ValueUncertaintyVM OtherValueUncertainty { get; set; }
        public ValueUncertaintyVM FoundationHeightUncertainty { get; set; }

        public CoordinatesFunctionEditorVM StructureEditorVM { get; set; }
        public CoordinatesFunctionEditorVM ContentEditorVM { get; set; }
        public CoordinatesFunctionEditorVM VehicleEditorVM { get; set; }
        public CoordinatesFunctionEditorVM OtherEditorVM { get; set; }

        public LoggingLevel SaveStatusLevel
        {
            get { return _SaveStatusLevel; }
            set
            {
                if (_SaveStatusLevel != value)
                {
                    _SaveStatusLevel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {

                if (_IsExpanded != value)
                {
                    _IsExpanded = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public ObservableCollection<LogItem> MessageRows
        {
            get { return _MessageRows; }
            set
            {
                _MessageRows = value;
                NotifyPropertyChanged(nameof(SaveStatusLevel));
                NotifyPropertyChanged("MessageRows");
                NotifyPropertyChanged("MessageCount");
            }
        }

        public int MessageCount
        {
            get { return _MessageRows.Count; }
        }

        public List<LogItem> TempErrors
        {
            get;
            set;
        }

        public OccupancyTypeEditable(IOccupancyType occtype)
        {
            //clone the occtype so that changes to it will not go into effect
            //unless the user saves.
            IOccupancyType clonedOcctype = Saving.PersistenceFactory.GetOccTypeManager().CloneOccType(occtype);

            Name = clonedOcctype.Name;
            Description = clonedOcctype.Description;
            DamageCategory = clonedOcctype.DamageCategory;
            CalculateStructureDamage = clonedOcctype.CalculateStructureDamage;
            CalculateContentDamage = clonedOcctype.CalculateContentDamage;
            CalculateVehicleDamage = clonedOcctype.CalculateVehicleDamage;
            CalculateOtherDamage = clonedOcctype.CalculateOtherDamage;

            //create the curve editors
            string xLabel = "XLabel";
            string yLabel = "YLabel";
            string chartTitle = "ChartTitle";
            StructureEditorVM = new CoordinatesFunctionEditorVM(clonedOcctype.StructureDepthDamageFunction, xLabel, yLabel, chartTitle);
            ContentEditorVM = new CoordinatesFunctionEditorVM(clonedOcctype.ContentDepthDamageFunction, xLabel, yLabel, chartTitle);
            VehicleEditorVM = new CoordinatesFunctionEditorVM(clonedOcctype.VehicleDepthDamageFunction, xLabel, yLabel, chartTitle);
            OtherEditorVM = new CoordinatesFunctionEditorVM(clonedOcctype.OtherDepthDamageFunction, xLabel, yLabel, chartTitle);

            //capture the table changed event from each of the editors so that we can set the occtype modified.
            StructureEditorVM.TableChanged += StructureEditorVM_TableChanged;
            ContentEditorVM.TableChanged += StructureEditorVM_TableChanged;
            VehicleEditorVM.TableChanged += StructureEditorVM_TableChanged;
            OtherEditorVM.TableChanged += StructureEditorVM_TableChanged;

            //create the value uncertainty vm's
            StructureValueUncertainty = new ValueUncertaintyVM(clonedOcctype.StructureValueUncertainty, clonedOcctype.StructureUncertaintyType);
            ContentValueUncertainty = new ValueUncertaintyVM(clonedOcctype.ContentValueUncertainty, clonedOcctype.ContentUncertaintyType);
            VehicleValueUncertainty = new ValueUncertaintyVM(clonedOcctype.VehicleValueUncertainty, clonedOcctype.VehicleUncertaintyType);
            OtherValueUncertainty = new ValueUncertaintyVM(clonedOcctype.OtherValueUncertainty, clonedOcctype.OtherUncertaintyType);

            FoundationHeightUncertainty = new ValueUncertaintyVM(clonedOcctype.FoundationHeightUncertainty, clonedOcctype.FoundationHtUncertaintyType); 
            //todo: foundation height uncertainty?
        }

        private void StructureEditorVM_TableChanged(object sender, EventArgs e)
        {
            IsModified = true;
        }

        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != null, "Name cannot be empty.");
            AddRule(nameof(Name), () => Name != "", "Name cannot be empty.");

            //AddRule(nameof(OriginalPath), () => OriginalPath != null, "Path cannot be null.");
            //AddRule(nameof(OriginalPath), () => OriginalPath != "", "Path cannot be null.");

            //AddRule(nameof(TerrainPath), () =>
            //{ return System.IO.File.Exists(TerrainPath) != true; }, "A file with this name already exists.");

        }

        public void UpdateMessages(bool saving)
        {
            //todo: just fire update message to the occtype editor
            UpdateMessagesEvent?.Invoke(this, new EventArgs());
        }

        public void FilterRowsByLevel(LoggingLevel level)
        {
            throw new NotImplementedException();
        }

        public void DisplayAllMessages()
        {
            throw new NotImplementedException();
        }
    }
}
