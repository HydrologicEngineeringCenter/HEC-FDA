using FdaLogging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Utilities;
using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.Inventory.DamageCategory;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Utilities;
using HEC.FDA.ViewModel.Utilities.Transactions;
using ViewModel.Inventory.OccupancyTypes;
using HEC.FDA.ViewModel.TableWithPlot;
using Statistics;

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

        //private bool _CalculateStructureDamage;
        //private bool _CalculateContentDamage;
        //private bool _CalculateVehicleDamage;
        //private bool _CalculateOtherDamage;
        private string _Name;
        private string _Description;
        //private IDamageCategory _DamageCategory2;
        private string _DamageCategory;
        private bool _IsModified;
        private ObservableCollection<string> _DamageCategoriesList = new ObservableCollection<string>();
        //private TableWithPlotVM _StructureTableWithPlot;
        //private TableWithPlotVM _ContentTableWithPlot;
        //private TableWithPlotVM _VehicleTableWithPlot;
        //private TableWithPlotVM _OtherTableWithPlot;




        #endregion

        #region Properties
        //public IOccupancyType OccType { get; set; }
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

        //public bool CalculateStructureDamage
        //{
        //    get { return _CalculateStructureDamage; }
        //    set { _CalculateStructureDamage = value; IsModified = true; }
        //}

        //public bool CalculateContentDamage
        //{
        //    get { return _CalculateContentDamage; }
        //    set { _CalculateContentDamage = value; IsModified = true; }
        //}

        //public bool CalculateVehicleDamage
        //{
        //    get { return _CalculateVehicleDamage; }
        //    set { _CalculateVehicleDamage = value; IsModified = true; }
        //}

        //public bool CalculateOtherDamage
        //{
        //    get { return _CalculateOtherDamage; }
        //    set { _CalculateOtherDamage = value; IsModified = true; }
        //}

        //private ValueUncertaintyVM _StructureValueUncertainty;
        //public ValueUncertaintyVM StructureValueUncertainty 
        //{
        //    get { return _StructureValueUncertainty; }
        //    set 
        //    { 
        //        _StructureValueUncertainty = value;
        //        _StructureValueUncertainty.WasModified += SomethingChanged;
        //        IsModified = true; 
        //    }
        //}

        //private ValueUncertaintyVM _ContentValueUncertainty;
        //public ValueUncertaintyVM ContentValueUncertainty 
        //{
        //    get { return _ContentValueUncertainty; }
        //    set 
        //    { 
        //        _ContentValueUncertainty = value;
        //        _ContentValueUncertainty.WasModified += SomethingChanged;
        //        IsModified = true; 
        //    }
        //}
        private ValueUncertaintyVM _ContentToStructureValueUncertainty;
        public ValueUncertaintyVM ContentToStructureValueUncertainty
        {
            get { return _ContentToStructureValueUncertainty; }
            set
            {
                _ContentToStructureValueUncertainty = value;
                _ContentToStructureValueUncertainty.WasModified += SomethingChanged;
                IsModified = true;
            }
        }
        private double _ContentToStructure;
        public double ContentToStructure
        {
            get { return _ContentToStructure; }
            set { _ContentToStructure = value; NotifyPropertyChanged(); }
        }

        private ValueUncertaintyVM _OtherToStructureValueUncertainty;
        public ValueUncertaintyVM OtherToStructureValueUncertainty
        {
            get { return _OtherToStructureValueUncertainty; }
            set
            {
                _OtherToStructureValueUncertainty = value;
                _OtherToStructureValueUncertainty.WasModified += SomethingChanged;
                IsModified = true;
            }
        }

        private double _OtherToStructure;
        public double OtherToStructure
        {
            get { return _OtherToStructure; }
            set { _OtherToStructure = value; NotifyPropertyChanged(); }
        }

        //private ValueUncertaintyVM _VehicleValueUncertainty;
        //public ValueUncertaintyVM VehicleValueUncertainty 
        //{
        //    get { return _VehicleValueUncertainty; }
        //    set 
        //    { 
        //        _VehicleValueUncertainty = value;
        //        _VehicleValueUncertainty.WasModified += SomethingChanged;
        //        IsModified = true; 
        //    }
        //}

        //private ValueUncertaintyVM _OtherValueUncertainty;
        //public ValueUncertaintyVM OtherValueUncertainty 
        //{
        //    get { return _OtherValueUncertainty; }
        //    set 
        //    { 
        //        _OtherValueUncertainty = value;
        //        _OtherValueUncertainty.WasModified += SomethingChanged;
        //        IsModified = true; 
        //    }
        //}

        private ValueUncertaintyVM _FoundationHeightUncertainty;
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

        //public object OccType
        //{
        //    get;
        //    set;
        //}

        //private CoordinatesFunctionEditorVM _StructureEditorVM;
        //public CoordinatesFunctionEditorVM StructureEditorVM
        //{
        //    get { return _StructureEditorVM; }
        //    set { _StructureEditorVM = value; NotifyPropertyChanged(); }
        //}
        //private CoordinatesFunctionEditorVM _ContentEditorVM;

        //public CoordinatesFunctionEditorVM ContentEditorVM
        //{
        //    get { return _ContentEditorVM; }
        //    set { _ContentEditorVM = value; NotifyPropertyChanged(); }
        //}
        //private CoordinatesFunctionEditorVM _VehicleEditorVM;

        //public CoordinatesFunctionEditorVM VehicleEditorVM
        //{
        //    get { return _VehicleEditorVM; }
        //    set { _VehicleEditorVM = value; NotifyPropertyChanged(); }
        //}
        //private CoordinatesFunctionEditorVM _OtherEditorVM;

        //public CoordinatesFunctionEditorVM OtherEditorVM
        //{
        //    get { return _OtherEditorVM; }
        //    set { _OtherEditorVM = value; NotifyPropertyChanged(); }
        //}

        public OccTypeItem StructureItem { get; set; }
        public OccTypeItem ContentItem { get; set; }
        public OccTypeItem VehicleItem { get; set; }
        public OccTypeItem OtherItem { get; set; }

        //public TableWithPlotVM StructureTableWithPlot
        //{
        //    get { return _StructureTableWithPlot; }
        //    set { _StructureTableWithPlot = value; NotifyPropertyChanged(); }
        //}
        //public TableWithPlotVM ContentTableWithPlot
        //{
        //    get { return _ContentTableWithPlot; }
        //    set { _ContentTableWithPlot = value; NotifyPropertyChanged(); }
        //}
        //public TableWithPlotVM VehicleTableWithPlot
        //{
        //    get { return _VehicleTableWithPlot; }
        //    set { _VehicleTableWithPlot = value; NotifyPropertyChanged(); }
        //}
        //public TableWithPlotVM OtherTableWithPlot
        //{
        //    get { return _OtherTableWithPlot; }
        //    set { _OtherTableWithPlot = value; NotifyPropertyChanged(); }
        //}



        /// <summary>
        /// This indicates if the occtype has ever been saved before. If false, then
        /// this is a brand new occtype. It needs to be saved new and not just an update on 
        /// an existing one in the database.
        /// </summary>
        public bool HasBeenSaved { get; set; }

        #endregion

        #region Constructors

        //copy constructor
        //public OccupancyTypeEditable(IOccupancyTypeEditable ot)
        //{
        //    DamageCategoriesList = ot.DamageCategoriesList;
        //    HasBeenSaved = false;
        //    Name = ot.Name;
        //    Description = ot.Description;
        //    DamageCategory = ot.DamageCategory;
            
        //    CalculateStructureDamage = ot.CalculateStructureDamage;
        //    CalculateContentDamage = ot.CalculateContentDamage;
        //    CalculateVehicleDamage = ot.CalculateVehicleDamage;
        //    CalculateOtherDamage = ot.CalculateOtherDamage;

        //    GroupID = ot.GroupID;

        //    string xLabel = "XLabel";
        //    string yLabel = "YLabel";
        //    string chartTitle = "ChartTitle";
        //    StructureEditorVM = new CoordinatesFunctionEditorVM(ot.str, xLabel, yLabel, chartTitle);
        //    ContentEditorVM = new CoordinatesFunctionEditorVM(clonedOcctype.ContentDepthDamageFunction, xLabel, yLabel, chartTitle);
        //    VehicleEditorVM = new CoordinatesFunctionEditorVM(clonedOcctype.VehicleDepthDamageFunction, xLabel, yLabel, chartTitle);
        //    OtherEditorVM = new CoordinatesFunctionEditorVM(clonedOcctype.OtherDepthDamageFunction, xLabel, yLabel, chartTitle);
        //}
        public OccupancyTypeEditable(IOccupancyType occtype,ref ObservableCollection<string> damageCategoriesList, bool occtypeHasBeenSaved = true)
        {
            StructureItem = new OccTypeItem(occtype.StructureItem.IsChecked, occtype.StructureItem.Curve, occtype.StructureItem.ValueUncertainty.Distribution);
            ContentItem = new OccTypeItem(occtype.ContentItem.IsChecked, occtype.ContentItem.Curve, occtype.ContentItem.ValueUncertainty.Distribution);
            VehicleItem = new OccTypeItem(occtype.VehicleItem.IsChecked, occtype.VehicleItem.Curve, occtype.VehicleItem.ValueUncertainty.Distribution);
            OtherItem = new OccTypeItem(occtype.OtherItem.IsChecked, occtype.OtherItem.Curve, occtype.OtherItem.ValueUncertainty.Distribution);

            //StructureTableWithPlot = new TableWithPlotVM(StructureItem.Curve);
            //ContentTableWithPlot = new TableWithPlotVM(ContentItem.Curve);
            //VehicleTableWithPlot = new TableWithPlotVM(VehicleItem.Curve);
            //OtherTableWithPlot = new TableWithPlotVM(OtherItem.Curve);

            DamageCategoriesList = damageCategoriesList;
            HasBeenSaved = occtypeHasBeenSaved;
            //clone the occtype so that changes to it will not go into effect
            //unless the user saves.
            IOccupancyType clonedOcctype = new OccupancyType(occtype); //Saving.PersistenceFactory.GetOccTypeManager().CloneOccType(occtype);

            Name = clonedOcctype.Name;
            Description = clonedOcctype.Description;
            DamageCategory = clonedOcctype.DamageCategory;

            ID = clonedOcctype.ID;
            GroupID = clonedOcctype.GroupID;



            //create the curve editors
            //string xLabel = "XLabel";
            //string yLabel = "YLabel";
            //string chartTitle = "ChartTitle";


            //capture the table changed event from each of the editors so that we can set the occtype modified.
            //StructureEditorVM.TableChanged += SomethingChanged;
            //ContentEditorVM.TableChanged += SomethingChanged;
            //VehicleEditorVM.TableChanged += SomethingChanged;
            //OtherEditorVM.TableChanged += SomethingChanged;

            //create the value uncertainty vm's
            //string valueUncertaintyLabel = "% of Mean";
            //StructureValueUncertainty = new MonetaryValueUncertaintyVM(clonedOcctype.StructureItem.ValueUncertainty);
            //ContentValueUncertainty = new MonetaryValueUncertaintyVM(clonedOcctype.ContentItem.ValueUncertainty);
            //VehicleValueUncertainty = new MonetaryValueUncertaintyVM(clonedOcctype.VehicleItem.ValueUncertainty);
            //OtherValueUncertainty = new MonetaryValueUncertaintyVM(clonedOcctype.OtherItem.ValueUncertainty);

            ContentToStructureValueUncertainty = new OtherValueUncertaintyVM(clonedOcctype.ContentToStructureValueUncertainty);
            OtherToStructureValueUncertainty = new OtherValueUncertaintyVM(clonedOcctype.OtherToStructureValueUncertainty);

            //string foundationHtLabel = "";
            FoundationHeightUncertainty = new FoundationValueUncertaintyVM(clonedOcctype.FoundationHeightUncertainty);
            HasChanges = false;
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
                    //SetDamageCategory();
                    //LoadDamageCategoriesList();


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

            //AddRule(nameof(OriginalPath), () => OriginalPath != null, "Path cannot be null.");
            //AddRule(nameof(OriginalPath), () => OriginalPath != "", "Path cannot be null.");

            //AddRule(nameof(TerrainPath), () =>
            //{ return System.IO.File.Exists(TerrainPath) != true; }, "A file with this name already exists.");

        }

        //public void UpdateMessages(bool saving = false)
        //{
        //    //todo: just fire update message to the occtype editor
        //    //UpdateMessagesEvent?.Invoke(this, new EventArgs());
        //    //there are three places that messages come from.
        //    // 1.) The sqlite database
        //    // 2.) Temp messages from the validation of the "rules" (ie. Name cannot be blank)
        //    // 3.) Temp messages from any object that implements IValidate. These messages come out of the model, stats, functions

        //    //this gets called when still constructing everything. Exit is everything is still null
        //    if(StructureEditorVM == null)
        //    {
        //        return;
        //    }

        //    //get rid of any temp logs
        //    ObservableCollection<LogItem> tempList = new ObservableCollection<LogItem>();
        //    foreach (LogItem li in MessageRows)
        //    {
        //        //exclude any temp logs
        //        if (!li.IsTempLog())
        //        {
        //            //if (li.Message.Equals("Last Saved"))
        //            //{
        //            //    li.Message = "Last Saved: " + li.Date;
        //            //}
        //            tempList.Add(li);
        //        }

        //    }


        //    //get IMessages from the coord func editor
        //    //and convert them into temp log messages
        //    List<LogItem> funcLogs = GetTempLogsFromCoordinatesFunctionEditor();
        //    //add them to the temp errors so that they will be included
        //    TempErrors.AddRange(funcLogs);

        //    //i want all of these messages to be put on the top of the list, but i want to respect their order. This 
        //    //means i need to insert at 0 and start with the last in the list
        //    for (int i = TempErrors.Count - 1; i >= 0; i--)
        //    {
        //        tempList.Insert(0, TempErrors[i]);
        //    }
        //    MessageRows = tempList;
        //    TempErrors.Clear();
        //    //if we are saving then we want the save status to be visible
        //    if (saving)
        //    {
        //        UpdateSaveStatusLevel();
        //    }
        //    else
        //    {
        //        SaveStatusLevel = LoggingLevel.Debug;
        //    }
        //}

        private List<LogItem> GetTempLogsFromCoordinatesFunctionEditor()
        {
            List<LogItem> logs = new List<LogItem>();
            //List<IMessage> messagesFromEditor = new List<IMessage>();

            ////get messages from the editors
            //messagesFromEditor.AddRange(StructureEditorVM.Messages);
            //messagesFromEditor.AddRange(ContentEditorVM.Messages);
            //messagesFromEditor.AddRange(VehicleEditorVM.Messages);
            //messagesFromEditor.AddRange(OtherEditorVM.Messages);

            ////get messages from the editor's function
            //if (StructureEditorVM.Function.Messages != null)
            //{
            //    messagesFromEditor.AddRange(StructureEditorVM.Function.Messages);
            //}
            //if (ContentEditorVM.Function.Messages != null)
            //{
            //    messagesFromEditor.AddRange(ContentEditorVM.Function.Messages);
            //}
            //if (VehicleEditorVM.Function.Messages != null)
            //{
            //    messagesFromEditor.AddRange(VehicleEditorVM.Function.Messages);
            //}
            //if (OtherEditorVM.Function.Messages != null)
            //{
            //    messagesFromEditor.AddRange(OtherEditorVM.Function.Messages);
            //}

            //foreach (IMessage message in messagesFromEditor)
            //{
            //    LoggingLevel level = TranslateValidationLevelToLogLevel(message.Level);
            //    logs.Add(LogItemFactory.FactoryTemp(level, message.Notice));
            //}
            ////order the list by the log level. Highest on top
            //var sortedLogList = logs
            //    .OrderByDescending(x => (int)(x.LogLevel))
            //    .ToList();

            return logs;
        }

        private LoggingLevel TranslateValidationLevelToLogLevel(IMessageLevels level)
        {
            LoggingLevel logLevel = LoggingLevel.Info;
            switch (level)
            {
                case IMessageLevels.FatalError:
                    {
                        logLevel = LoggingLevel.Fatal;
                        break;
                    }
                case IMessageLevels.Error:
                    {
                        logLevel = LoggingLevel.Error;
                        break;
                    }
                case IMessageLevels.Message:
                    {
                        logLevel = LoggingLevel.Warn;
                        break;
                    }
            }
            return logLevel;
        }

        private void UpdateSaveStatusLevel()
        {
            //This method will also expand the 
            //basically i want to set the error level to be the highest log level in my list
            //this will change the background color of the Save button
            //if (ContainsLogLevel(LoggingLevel.Fatal))
            //{
            //    IsExpanded = true;
            //    SaveStatusLevel = LoggingLevel.Fatal;
            //}
            //else if (ContainsLogLevel(LoggingLevel.Error))
            //{
            //    IsExpanded = true;
            //    SaveStatusLevel = LoggingLevel.Error;
            //}
            //else if (ContainsLogLevel(LoggingLevel.Warn))
            //{
            //    IsExpanded = true;
            //    SaveStatusLevel = LoggingLevel.Warn;
            //}
            //else if (ContainsLogLevel(LoggingLevel.Info))
            //{
            //    SaveStatusLevel = LoggingLevel.Info;
            //}
            //else
            //{
            //    //this is being used here to just be the default lowest level.
            //    SaveStatusLevel = LoggingLevel.Debug;
            //}
        }

        //private bool ContainsLogLevel(LoggingLevel level)
        //{
        //    bool retval = false;
        //    foreach (LogItem li in MessageRows)
        //    {
        //        if (li.LogLevel == level)
        //        {
        //            retval = true;
        //            break;
        //        }
        //    }
        //    return retval;
        //}

        public void FilterRowsByLevel(FdaLogging.LoggingLevel level)
        {

            //MessageRows = Saving.PersistenceFactory.GetElementManager(CurrentElement).GetLogMessagesByLevel(level, CurrentElement.Name);
        }

        public void DisplayAllMessages()
        {
            //MessageRows = Saving.PersistenceFactory.GetElementManager(CurrentElement).GetLogMessages(CurrentElement.Name);
        }

        /// <summary>
        /// Gets called by clicking the "Save" button on the individual occtype control.
        /// Saves the occtype to the database and updates the study cache.
        /// </summary>
        public void Save()
        {
            SaveWithReturnValue();



            //is the curve in a valid state to save
            //String isOTValid = AssignCoordinateFunction(SelectedOccType);

            ////todo get this message to go to the messages control
            //if(isOTValid != null)
            //{
            //    MessageBox.Show(isOTValid, "Could not save occupancy type", MessageBoxButton.OK);
            //    return;
            //}

            //bool areValidCoordinatesFunction = AssignCoordinatesFunctions();
            //if(!areValidCoordinatesFunction)
            //{
            //    return;
            //}

            //get the occtypes that have been modified
            //List<IOccupancyTypeEditable> occtypesToUpdateInOcctypesTable = new List<IOccupancyTypeEditable>();


            //    //foreach (IOccupancyTypeEditable ot in SelectedOccTypeGroup.Occtypes)
            //    {
            //        if (SelectedOccType.IsModified)
            //        {
            //            occtypesToUpdateInOcctypesTable.Add(SelectedOccType);                        
            //        }
            //    }






            //we just saved so set all the "isModified" flags back to false
            //ClearAllModifiedLists();

            //i need some way to update the UI so that the '*' goes away

            //i need to update the elements in the study cache which is what gets pulled in, the next time the 
            //occupancy types editor is opened.
            //todo: for now i will just update all the groups in the cache. In the future we could get smarter about
            //it and just do the ones that have changed. That probably isn't hard to do with the lists that i already
            //am tracking at the top of this class.

            //todo: i need to tell this group in the cache that an ot has been updated. or just replace it in the cache.
            //in theory i just need to update this one occtype, but to update the in memory object that the parent is holding, i need to
            //remove the whole group and add the whole group. At least the way it is written right now.


            //so in order for the occtype parent to get updated it needs to remove a whole group and i need to add a new group.
            //so i need to update just this one occtype in the correct group. Then remove the group with that id in the cache 
            //which will fire the remove event up in the parent element. Then add the modified group to the cache which will fire
            //the add event up in the parent.
            //  manager.UpdateOccTypeInCache(ot);
            // manager.UpdateOccTypeInStudyCache(SelectedOccTypeGroup, SelectedOccType);
            // manager.UpdateOccTypeGroupsInStudyCache(new List<IOccupancyTypeGroupEditable>() { ot });





        }
        public bool SaveWithReturnValue()
        {
            //This "IsModified" stuff was mostly working, but was not working for the value uncertainty stuff changing
            //Ideally the save button would be disabled/enabled. I am just going to allow the save to happen every time
            //the save is clicked.
            if (!IsModified)
            {
                //nothing new to save
                String time = DateTime.Now.ToString();

                LogItem li = LogItemFactory.FactoryTemp(LoggingLevel.Info, "No new changes to save." + time);
                //TempErrors.Insert(0, li);
                //SaveStatusLevel = LoggingLevel.Debug;
                //UpdateMessages();
                
                return true;

            }

            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();

            List<LogItem> errors = new List<LogItem>();
            IOccupancyType ot = CreateOccupancyType( out errors);
            if (ot == null)
            {
                //if the occtype is null then it failed. There should be errors to add.
                //TempErrors.AddRange(errors);
                //UpdateMessages(true);
                return false;
            }
            else if (HasBeenSaved)
            {
                //update the existing occtype
                manager.SaveModifiedOcctype(ot);
            }
            else if (!HasBeenSaved)
            {
                //save this as a new occtype
                //if it has never been saved then we need a new occtype id for it.
                //todo: this is weird and i am doing something similar with the occtype groups
                //i want them to have a property of their id. But i also feel like i shouldn't have 
                //to be figuring out the id on my own. I should just let the database do it. Like i could 
                //save it and then ask it what id it was given by the database.
                ot.ID = Saving.PersistenceFactory.GetOccTypeManager().GetIdForNewOccType(ot.GroupID);
                manager.SaveNewOccType(ot);
            }

            

            this.IsModified = false;
            string lastEditDate = DateTime.Now.ToString("G");
            //SavingText = "Last Saved: " + lastEditDate;
            //UpdateMessages(true);
            //this will disable the save button.
            HasChanges = false;
            return true;
        }



        public IOccupancyType CreateOccupancyType(out List<LogItem> errors)
        {
            bool success = true;
            StringBuilder errorMsg = new StringBuilder("Occupancy Type: ").Append(Name);
            List<LogItem> constructionErrors = new List<LogItem>();

            ContinuousDistribution foundHtUncertainty = null;
            try
            {
                foundHtUncertainty = FoundationHeightUncertainty.CreateOrdinate();
            }
            catch (Exception e)
            {
                success = false;
                errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
                                        .Append("Foundation Height Uncertainty: ").Append(e.Message);
                string logMessage = "Error constructing foundation height uncertainty: " + e.Message;
                constructionErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, logMessage));
            }

            ContinuousDistribution contentToStructureUncertainty = null;
            try
            {
                contentToStructureUncertainty = ContentToStructureValueUncertainty.CreateOrdinate();
            }
            catch (Exception e)
            {
                success = false;
                errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
                                        .Append("Content to structure: ").Append(e.Message);
                string logMessage = "Error constructing content to structure uncertainty: " + e.Message;
                constructionErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, logMessage));
            }

            ContinuousDistribution otherToStructureUncertainty = null;
            try
            {
                otherToStructureUncertainty = OtherToStructureValueUncertainty.CreateOrdinate();
            }
            catch (Exception e)
            {
                success = false;
                errorMsg.Append(Environment.NewLine).Append('\t').Append('\t').Append('\t')
                                        .Append("Other to structure: ").Append(e.Message);
                string logMessage = "Error constructing other to structure uncertainty: " + e.Message;
                constructionErrors.Add(LogItemFactory.FactoryTemp(LoggingLevel.Fatal, logMessage));
            }

            OccupancyType ot = new OccupancyType(Name, Description, GroupID,DamageCategory,StructureItem,ContentItem,VehicleItem,OtherItem,
                foundHtUncertainty,contentToStructureUncertainty, otherToStructureUncertainty, ID);

            if (success)
            {
                errors = null;
                return ot;
            }
            else
            {
                errors = constructionErrors;
                return null;
            }

        }

        public List<LogItem> SaveOcctype()
        {
            FdaValidationResult vr = new FdaValidationResult();

            OccTypePersistenceManager manager = Saving.PersistenceFactory.GetOccTypeManager();

            List<LogItem> errors = new List<LogItem>();
            IOccupancyType ot = CreateOccupancyType(out errors);
            if (ot == null)
            {
                //if the occtype is null then it failed. There should be errors to add.
                
                return errors;
            }
            else if (HasBeenSaved)
            {
                //update the existing occtype
                manager.SaveModifiedOcctype(ot);
            }
            else if (!HasBeenSaved)
            {
                //save this as a new occtype
                //if it has never been saved then we need a new occtype id for it.
                //todo: this is weird and i am doing something similar with the occtype groups
                //i want them to have a property of their id. But i also feel like i shouldn't have 
                //to be figuring out the id on my own. I should just let the database do it. Like i could 
                //save it and then ask it what id it was given by the database.
                ot.ID = Saving.PersistenceFactory.GetOccTypeManager().GetIdForNewOccType(ot.GroupID);
                manager.SaveNewOccType(ot);
            }



            IsModified = false;
            
            //this will disable the save button.
            HasChanges = false;
            if(errors == null)
            {
                errors = new List<LogItem>();
            }
            return errors;
        }

    }
}
