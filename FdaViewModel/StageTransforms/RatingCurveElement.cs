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
        private RatingCurveOwnerElement _OwnerNode;
        private const string TABLE_NAME_CONSTANT = "Rating Curve - ";

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
       
        public RatingCurveElement(string userprovidedname, string creationDate, string desc, Statistics.UncertainCurveDataCollection ratingCurve, Utilities.OwnerElement owner) : base(owner)
        {
            LastEditDate = creationDate;
            _OwnerNode = (RatingCurveOwnerElement)owner;
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
            RatingCurveEditorVM vm = new RatingCurveEditorVM(this, (foo) => ((Utilities.OwnerElement)_Owner).SaveExistingElement(foo), (bar) => ((Utilities.OwnerElement)_Owner).AddOwnerRules(bar));
            Navigate(vm, true, true);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    //LastEditDate = DateTime.Now.ToString("G"); //will be formatted like: 2/27/2009 12:12:22 PM

                    //// if the user has changed the name of the element then we need to update the parent table and the child table if there is one
                    //string originalName = Name;
                    //Statistics.UncertainCurveDataCollection originalRatingCurve = RatingCurve;

                    //Name = vm.Name;//should i disable this way of renaming? if not i need to check for name conflicts.
                    //Description = vm.Description;//is binding two way? is this necessary?
                    //RatingCurve = vm.Curve;

                    //if (DidStateChange() == true)
                    //{
                    //    if (vm.HasSaved == true)//just replace the row
                    //    {
                    //        _OwnerNode.UpdateExistingElement(originalName, this, 0, 1);
                    //    }
                    //    else
                    //    {
                    //        _OwnerNode.UpdateTableRowIfModified((Utilities.OwnerElement)_Owner, originalName, this);
                    //        UpdateTableIfModified(originalName, originalRatingCurve, RatingCurve);
                    //    }
                    //}
                    vm.SaveWhileEditing();
                    //((RatingCurveOwnerElement)_Owner).SaveElementWhileEditing((Utilities.ISaveUndoRedo)vm);

                }
            }
        }

        private bool DidStateChange()
        {
            return true;
        }
        public override void AddValidationRules()
        {
            AddRule(nameof(Name), () => Name != "", "Name cannot be blank.");
        }

        //public override string ChangeTableName()
        //{
        //    return GetTableConstant() + Name + "-ChangeTable";
        //}
        public override void Save()
        {
            if (!Storage.Connection.Instance.IsOpen)
            {
                Storage.Connection.Instance.Open();
            }
            RatingCurve.toSqliteTable(TableName); //will be formatted like: 2/27/2009 12:12:22 PM
        }

        public override object[] RowData()
        {
            return new object[] { Name,LastEditDate, Description, RatingCurve.Distribution, RatingCurve.GetType() };
        }


        public override bool SavesToRow()
        {
            return true;
        }
        public override bool SavesToTable()
        {
            return true;
        }
        public override string GetTableConstant()
        {
            return TABLE_NAME_CONSTANT;
        }

       
        #endregion
        #region Functions
        public override string TableName
        {
            get
            {
                return TABLE_NAME_CONSTANT + LastEditDate;
            }
        }
        #endregion



    }
}
