using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities.Transactions;

namespace FdaViewModel.Study
{
    public class FdaStudyVM : BaseFdaElement, IDisposable 
    {
        #region Notes
        #endregion
        #region Fields
        private List<Utilities.OwnerElement> _MainStudyTree;
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return TableName;
        }
        public List<Utilities.OwnerElement> MainStudyTree
        {
            get { return _MainStudyTree; }
            set { _MainStudyTree = value;  NotifyPropertyChanged(nameof(MainStudyTree)); }
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
            _MainStudyTree = new List<Utilities.OwnerElement>();
            StudyElement s = new StudyElement(this);
            s.RequestNavigation += Navigate;
            s.RequestShapefilePaths += ShapefilePaths;
            s.RequestShapefilePathsOfType += ShapefilePathsOfType;
            s.RequestAddToMapWindow += AddToMapWindow;
            s.RequestRemoveFromMapWindow += RemoveFromMapWindow;
            s.TransactionEvent += WriteTransactions;
            FdaModel.Utilities.Messager.Logger.Instance.RequestFlushLogFile += Instance_RequestFlushLogFile;
            s.AddBaseElements();
            _MainStudyTree.Add(s);
            FdaModel.Utilities.Initialize.InitializeGDAL();

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
            
        }

        public override void Save()
        {
            throw new NotImplementedException();
        }
        #endregion
        #region Functions
        public override BaseFdaElement GetElementOfTypeAndName(Type t, string name)
        {
            //check if the type is FdaStudyVM and if the name matches.
            //check if the type is _MainStudyTree.GetType() and the name matches.
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
        #endregion

    }
}
