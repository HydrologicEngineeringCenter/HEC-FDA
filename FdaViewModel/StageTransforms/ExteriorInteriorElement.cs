using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.StageTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 11:31:34 AM)]
    public class ExteriorInteriorElement : Utilities.ChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 11:31:34 AM
        #endregion
        #region Fields
        private const string _TableConstant = "Exterior Interior - ";

        private string _Description;
        private Statistics.UncertainCurveDataCollection _Curve;
        #endregion
        #region Properties
        public override string GetTableConstant()
        {
            return _TableConstant;
        }
        public string Description { get { return _Description; } set { _Description = value; NotifyPropertyChanged(); } }

        public Statistics.UncertainCurveDataCollection ExteriorInteriorCurve
        {
            get { return _Curve; }
            set { _Curve = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ExteriorInteriorElement(string userProvidedName,string lastEditDate, string desc, Statistics.UncertainCurveDataCollection exteriorInteriorCurve, Utilities.ParentElement owner):base(owner)
        {
            LastEditDate = lastEditDate;
            Name = userProvidedName;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/ExteriorInteriorStage.png");

            Description = desc;
            if (Description == null) Description = "";
            ExteriorInteriorCurve = exteriorInteriorCurve;

            Utilities.NamedAction editExteriorInteriorCurve = new Utilities.NamedAction();
            editExteriorInteriorCurve.Header = "Edit Exterior Interior Curve";
            editExteriorInteriorCurve.Action = EditExteriorInteriorCurve;

            Utilities.NamedAction removeExteriorInteriorCurve = new Utilities.NamedAction();
            removeExteriorInteriorCurve.Header = "Remove";
            removeExteriorInteriorCurve.Action = Remove;

            Utilities.NamedAction renameElement = new Utilities.NamedAction();
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editExteriorInteriorCurve);
            localActions.Add(removeExteriorInteriorCurve);
            localActions.Add(renameElement);

            Actions = localActions;
        }
        #endregion
        #region Voids
        public void EditExteriorInteriorCurve(object arg1, EventArgs arg2)
        {
            //ExteriorInteriorEditorVM vm = new ExteriorInteriorEditorVM(this, (foo) => ((Utilities.OwnerElement)_Owner).SaveExistingElement(foo), (bar) => ((Utilities.OwnerElement)_Owner).AddOwnerRules(bar));// Name, Description, ExteriorInteriorCurve, 0);
            //Navigate(vm, true, true);
            //if (!vm.WasCancled)
            //{
            //    if (!vm.HasError)
            //    {
                  
            //        vm.SaveWhileEditing();
            //    }
            //}
        }
        #endregion
        #region Functions
        #endregion
        public override string TableName
        {
            get
            {
                return GetTableConstant() + LastEditDate;
            }
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override void Save()
        {
            _Curve.toSqliteTable(TableName);
        }

        public override object[] RowData()
        {
            return new object[] { Name, LastEditDate, Description, ExteriorInteriorCurve.Distribution };
        }

        public override bool SavesToRow()
        {
            return true;
        }
        public override bool SavesToTable()
        {
            return true;
        }
    }
}
