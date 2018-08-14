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
        public AnalyticalFrequencyElement(string name, string desc, Statistics.LogPearsonIII dist, BaseFdaElement owner) : base(owner)
        {
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
            AnalyticalFrequencyEditorVM vm = new AnalyticalFrequencyEditorVM(Name, Distribution, Description);
            Navigate(vm, true, true);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    //check for changes.
                    bool hasChanges = false;
                    if (Name != vm.Name) { hasChanges = true; }
                    if (Description != vm.Description) { hasChanges = true; }
                    if (Distribution != vm.Distribution) { hasChanges = true; }
                    if (hasChanges)
                    {

                        Description = vm.Description;
                        Distribution = vm.Distribution;
                        AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(vm.Name, Utilities.Transactions.TransactionEnum.EditExisting, "Previous Name " + Name + " Description: " + Description + " Mean: " + Distribution.GetMean + " Standard Deviation: " + Distribution.GetStDev + " Skew: " + Distribution.GetG + " EYOR: " + Distribution.GetSampleSize, nameof(AnalyticalFrequencyElement)));
                        Name = vm.Name;//should i disable this way of renaming? if not i need to check for name conflicts.
                        _Owner.Save();
                    }
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
            return new object[] { Name, Description, Distribution.GetMean, Distribution.GetStDev, Distribution.GetG, Distribution.GetSampleSize };
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
