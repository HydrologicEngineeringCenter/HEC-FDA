using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities.Transactions;
using System.Collections.ObjectModel;
using FdaViewModel.Conditions;
using FdaViewModel.Utilities;

namespace FdaViewModel.Study
{
    public class FdaStudyVM : BaseFdaElement, IDisposable 
    {
        #region Notes
        #endregion
        #region Fields
        private List<OwnerElement> _MainStudyTree;
        private ObservableCollection<OwnerElement> _ConditionsTree;
        private StudyElement _StudyElement;
        private int _SelectedTab = 0;
        #endregion
        #region Properties
        
        public int SelectedTab
        {
            get { return _SelectedTab; }
            set { _SelectedTab = value; TabChangedEvent(value); }
        }
        public override string GetTableConstant()
        {
            return TableName;
        }
        public List<OwnerElement> MainStudyTree
        {
            get { return _MainStudyTree; }
            set { _MainStudyTree = value;  NotifyPropertyChanged(nameof(MainStudyTree)); }
        }
        public ObservableCollection<OwnerElement> ConditionsTree
        {
            get { return _ConditionsTree; }
            set { _ConditionsTree = value; NotifyPropertyChanged(); }
        }

        public StudyElement StudyElement
        {
            get { return _StudyElement; }
            set { _StudyElement = value; }
        }

        public override string TableName
        {
            get
            {
                return "";
            }
        }
        #endregion
        #region Constructors
        public FdaStudyVM(): base()
        {
            //fill the main study tree
            _MainStudyTree = new List<Utilities.OwnerElement>();
            _StudyElement = new StudyElement(this);
            _StudyElement.RequestNavigation += Navigate;
            _StudyElement.RequestShapefilePaths += ShapefilePaths;
            _StudyElement.RequestShapefilePathsOfType += ShapefilePathsOfType;
            _StudyElement.RequestAddToMapWindow += AddToMapWindow;
            _StudyElement.RequestRemoveFromMapWindow += RemoveFromMapWindow;
            _StudyElement.TransactionEvent += WriteTransactions;
            _StudyElement.ClearStudy += ClearCurrentStudy;
            FdaModel.Utilities.Messager.Logger.Instance.RequestFlushLogFile += Instance_RequestFlushLogFile;
            _StudyElement.AddBaseElements();
            _MainStudyTree.Add(_StudyElement);

            FdaModel.Utilities.Initialize.InitializeGDAL();
        }

  

        private void ClearCurrentStudy(object sender, EventArgs e)
        {
            _MainStudyTree[0].Elements.Clear();
            _StudyElement = new StudyElement(this);
        }
        private void Instance_RequestFlushLogFile(object sender, EventArgs e)
        {
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                FdaModel.Utilities.Messager.Logger.Instance.Flush(Storage.Connection.Instance.Reader);
            }
        }

        private void WriteTransactions(object sender, TransactionEventArgs args)
        {
            if (!Storage.Connection.Instance.IsConnectionNull)
            {
                DataBase_Reader.DataTableView dtv = Storage.Connection.Instance.GetTable("Transactions");
                if (dtv == null)
                {
                    Storage.Connection.Instance.CreateTable("Transactions", new string[] { "Element Name", "Element Type", "Action", "Date", "User", "Notes" }, new Type[] { typeof(string), typeof(string), typeof(string), typeof(string), typeof(string), typeof(string) });
                }
                dtv = Storage.Connection.Instance.GetTable("Transactions");
                dtv.AddRow(new object[] { args.OriginatorName, args.OriginatorType, args.Action.ToString(), args.TransactionDate, args.User, args.Notes });
                dtv.ApplyEdits();
            }
        }

        public FdaStudyVM(string filepath) : base()
        {
            _MainStudyTree = new List<Utilities.OwnerElement>();
            StudyElement s = new StudyElement(this);
            s.RequestNavigation += Navigate;
            s.RequestShapefilePaths += ShapefilePaths;
            s.RequestShapefilePathsOfType += ShapefilePathsOfType;
            s.RequestAddToMapWindow += AddToMapWindow;
            s.RequestRemoveFromMapWindow += RemoveFromMapWindow;
            s.AddBaseElements();
            _MainStudyTree.Add(s);

        }
        #endregion
        #region Voids
      
        public override void AddValidationRules()
        {
            AddRule(nameof(StudyElement), () => { return AreConditionsValid(); }, GetConditionErrors());
        }

        private bool AreConditionsValid()
        {
            return false;
        }

        private string GetConditionErrors()
        {
            return "Condition 1 is in error\nCondition 2 is in error.";
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Functions
        public override BaseFdaElement GetElementOfTypeAndName(Type t, string name)
        {
            return _MainStudyTree[0].GetElementOfTypeAndName(t,name);
        }
        public override List<T> GetElementsOfType<T>()
        {
            return _MainStudyTree[0].GetElementsOfType<T>();
        }
        public override void Dispose()
        {
            FdaModel.Utilities.Messager.Logger.Instance.Flush(Storage.Connection.Instance.Reader);
            FdaModel.Utilities.Initialize.DisposeGDAL();
        }

        /// <summary>
        /// The integer value will be 0 for the study tree tab
        /// and 1 for the conditions tree tab
        /// </summary>
        /// <param name="value"></param>
        public void TabChangedEvent(int value)
        {
            if (value == 1)
            {
                UpdateTheConditionsTree(this, new EventArgs());
            }       
        }

        /// <summary>
        /// The study tree tab shows in real time the state of the study.
        /// When you switch to the conditions tab this method will grab the state of the 
        /// study tree conditions and mirror that in the conditions tab.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateTheConditionsTree(object sender, EventArgs e)
        {
            ObservableCollection<OwnedElement> conditions = new ObservableCollection<OwnedElement>();
            //get all the current conditions
            ConditionsOwnerElement studyTreeCondOwnerElement = null;
            if (_StudyElement.Elements.Count > 0)
            {
                foreach (OwnerElement owner in _StudyElement.Elements)
                {
                    if (owner.GetType() == typeof(ConditionsOwnerElement))
                    {
                        conditions = owner.Elements;
                        studyTreeCondOwnerElement = (ConditionsOwnerElement)owner;
                    }
                }
            }

            ConditionsTreeOwnerElement condTreeCondOwnerElement = new ConditionsTreeOwnerElement(studyTreeCondOwnerElement);
            condTreeCondOwnerElement.RequestNavigation += Navigate;
            condTreeCondOwnerElement.UpdateConditionsTree += UpdateTheConditionsTree;

            if (conditions.Count > 0)
            {
                foreach (OwnedElement elem in conditions)
                {
                    //create a new conditions element and change the way it renames, removes, and edits. The parent node
                    //will then tell the study tree what to do
                    ConditionsElement condElem = new ConditionsElement((ConditionsElement)elem, condTreeCondOwnerElement);
                    condElem.EditConditionsTreeElement += condTreeCondOwnerElement.EditCondition;
                    condElem.RemoveConditionsTreeElement += condTreeCondOwnerElement.RemoveElement;
                    condElem.RenameConditionsTreeElement += condTreeCondOwnerElement.RenameElement;
                    condElem.UpdateExpansionValueInTreeElement += condTreeCondOwnerElement.UpdateElementExpandedValue;
                    condTreeCondOwnerElement.AddElement(condElem, false);
                }

            }

            //have to make it new to call the notified prop changed
            ConditionsTree = new ObservableCollection<OwnerElement>() { condTreeCondOwnerElement };
        }



        #endregion

    }
}
