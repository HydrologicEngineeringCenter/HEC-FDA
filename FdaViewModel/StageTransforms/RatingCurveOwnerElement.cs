using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.StageTransforms
{
    class RatingCurveOwnerElement : Utilities.OwnerElement
    {
        #region Notes
        #endregion
        #region Fields

        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return TableName;
        }
        #endregion
        #region Constructors
        public RatingCurveOwnerElement(BaseFdaElement owner) : base(owner)
        {
            Name = "Rating Curves";
            IsBold = false;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addRatingCurve = new Utilities.NamedAction();
            addRatingCurve.Header = "Create New Rating Curve";
            addRatingCurve.Action = AddNewRatingCurve;

            Utilities.NamedAction ImportRatingCurve = new Utilities.NamedAction();
            ImportRatingCurve.Header = "Import Rating Curve From ASCII";
            ImportRatingCurve.Action = ImportRatingCurvefromAscii;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addRatingCurve);
            //localActions.Add(ImportRatingCurve);

            Actions = localActions;
        }


        #endregion
        #region Voids
        private void ImportRatingCurvefromAscii(object arg1, EventArgs arg2)
        {
            throw new NotImplementedException();
        }

        public void AddNewRatingCurve(object arg1, EventArgs arg2)
        {
            RatingCurveEditorVM vm = new RatingCurveEditorVM();
            Navigate(vm);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    RatingCurveElement ele = new RatingCurveElement(vm.Name, vm.Description, vm.Curve, this);
                    AddElement(ele);
                    AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(ele.Name, Utilities.Transactions.TransactionEnum.CreateNew, "", nameof(RatingCurveElement)));
                }
            }
        }
        public override void AddBaseElements()
        {
            //throw new NotImplementedException();
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }
        #endregion
        #region Functions
        public override string TableName
        {
            get
            {
                return "Rating Curves";
            }
        }
        public override string[] TableColumnNames()
        {
            return new string[] { "Rating Curve Name", "Description", "Curve Distribution Type", "Curve Type" };
        }
        public override Type[] TableColumnTypes()
        {
            return new Type[] { typeof(string), typeof(string), typeof(string),typeof(string) };
        }

        public override void AddElement(object[] rowData)
        {
            //creates an empty curve
            Statistics.UncertainCurveIncreasing emptyCurve = new Statistics.UncertainCurveIncreasing((Statistics.UncertainCurveDataCollection.DistributionsEnum)Enum.Parse(typeof(Statistics.UncertainCurveDataCollection.DistributionsEnum), (string)rowData[2]));

            RatingCurveElement rc = new RatingCurveElement((string)rowData[0], (string)rowData[1], emptyCurve, this);
            //loads the curve with the values from it's table
            rc.RatingCurve.fromSqliteTable(rc.TableName);
            AddElement(rc,false);

        }
        #endregion
    }
}
