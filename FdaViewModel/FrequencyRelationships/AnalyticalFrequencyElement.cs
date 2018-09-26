using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.FrequencyRelationships
{
    public class AnalyticalFrequencyElement : Utilities.OwnedElement
    {
        #region Notes
        #endregion
        #region Fields
        private const string _TableConstant = "Analytical Frequency - ";

        private string _Description = "";
        private Statistics.LogPearsonIII _Distribution;
        //private Utilities.OwnerElement _Owner;
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return _TableConstant;
        }
        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }
        public Statistics.LogPearsonIII Distribution
        {
            get { return _Distribution; }
            set { _Distribution = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public AnalyticalFrequencyElement(string name, string lastEditDate, string desc, Statistics.LogPearsonIII dist, Utilities.OwnerElement owner) : base(owner)
        {
            LastEditDate = lastEditDate;
            Name = name;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/FrequencyCurve.png");

            Description = desc;
            if (Description == null) Description = "";
            Distribution = dist;
            Utilities.NamedAction editflowfreq = new Utilities.NamedAction();
            editflowfreq.Header = "Edit Analyitical Flow Frequency Relationship";
            editflowfreq.Action = EditFlowFreq;

            Utilities.NamedAction removeflowfreq = new Utilities.NamedAction();
            removeflowfreq.Header = "Remove";
            removeflowfreq.Action = Remove;

            Utilities.NamedAction renameElement = new Utilities.NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editflowfreq);
            localActions.Add(removeflowfreq);
            localActions.Add(renameElement);



            Actions = localActions;
        }


        #endregion
        #region Voids
        public void EditFlowFreq(object arg1, EventArgs arg2)
        {
            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM(this, (AnalyticalFrequencyOwnerElement)_Owner);// Name, Distribution, Description, _Owner);
            Navigate(vm, true, true);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {

                    LastEditDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

                    string originalName = Name;
                    Name = vm.Name;
                    Description = vm.Description;
                    Distribution = vm.Distribution;
                    ChangeIndex = vm.ChangeIndex;

                    ((AnalyticalFrequencyOwnerElement)_Owner).UpdateTableRowIfModified(originalName, this);

                    AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(vm.Name, Utilities.Transactions.TransactionEnum.EditExisting, "Name " + Name + " Description: " + Description + " Mean: " + Distribution.GetMean + " Standard Deviation: " + Distribution.GetStDev + " Skew: " + Distribution.GetG + " EYOR: " + Distribution.GetSampleSize, nameof(AnalyticalFrequencyElement)));
                }
            }
        }
    
    public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
        public override void Save()
        {
            throw new NotImplementedException();
        }

        public override object[] RowData()
        {
            return new object[] { Name, LastEditDate, Description, Distribution.GetMean, Distribution.GetStDev, Distribution.GetG, Distribution.GetSampleSize };
        }

        public override bool SavesToRow()
        {
            return true;
        }
        public override bool SavesToTable()
        {
            return false;
        }
        #endregion
        #region Functions
        public override string TableName
        {
            get
            {
                throw new NotImplementedException();
            }
        }
        #endregion

    }
}
