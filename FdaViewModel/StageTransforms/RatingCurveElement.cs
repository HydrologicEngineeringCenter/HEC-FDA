using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.StageTransforms
{
    public class RatingCurveElement : Utilities.OwnedElement
    {
        #region Notes
        #endregion
        #region Fields
        private string _Description;
        private Statistics.UncertainCurveDataCollection _Curve;
        #endregion
        #region Properties
        public string Description { get { return _Description; } set { _Description = value; NotifyPropertyChanged(); } }

        public Statistics.UncertainCurveDataCollection RatingCurve
        {
            get { return _Curve; }
            set { _Curve = value;  NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public RatingCurveElement(string userprovidedname, string desc, Statistics.UncertainCurveDataCollection ratingCurve,  BaseFdaElement owner) : base(owner)
        {
            Name = userprovidedname;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/RatingCurve.png");

            RatingCurve = ratingCurve;
            Description = desc;
            if (Description == null) Description = "";
            Utilities.NamedAction editRatingCurve = new Utilities.NamedAction();
            editRatingCurve.Header = "Edit Rating Curve";
            editRatingCurve.Action = EditRatingCurve;

            Utilities.NamedAction removeRatingCurve = new Utilities.NamedAction();
            removeRatingCurve.Header = "Remove";
            removeRatingCurve.Action = Remove;

            Utilities.NamedAction renameElement = new Utilities.NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editRatingCurve);
            localActions.Add(removeRatingCurve);
            localActions.Add(renameElement);



            Actions = localActions;
        }


        #endregion
        #region Voids
        public void EditRatingCurve(object arg1, EventArgs arg2)
        {
            RatingCurveEditorVM vm = new RatingCurveEditorVM(Name, Description, RatingCurve);
            Navigate(vm, true, true);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    Name = vm.Name;//should i disable this way of renaming? if not i need to check for name conflicts.
                    Description = vm.Description;//is binding two way? is this necessary?
                    RatingCurve = vm.Curve;
                }
            }
        }
        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
            AddRule(nameof(Name), () => Name != "asdf", "Name cannot be asdf.");
        }

        public override void Save()
        {
            RatingCurve.toSqliteTable(TableName);
        }

        public override object[] RowData()
        {
            return new object[] { Name, Description, RatingCurve.Distribution, RatingCurve.GetType() };
        }

        public override string GetTableConstant()
        {
            return "Rating Curve - ";
        }

        public override bool SavesToRow()
        {
           return true;
        }
        public override bool SavesToTable()
        {
            return true;
        }
        #endregion
        #region Functions
        public override string TableName
        {
            get
            {
                return "Rating Curve - " + Name;
            }
        }
        #endregion



    }
}
