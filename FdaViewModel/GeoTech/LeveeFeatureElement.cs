using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FdaViewModel.Utilities;

namespace FdaViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 1:11:19 PM)]
    public class LeveeFeatureElement : Utilities.ChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 1:11:19 PM
        #endregion
        #region Fields
        private const string _TableConstant = "Levee Feature - ";

        private string _Description;
        private double _Elevation;
        #endregion
        #region Properties
       

        public double Elevation
        {
            get { return _Elevation; }
            set { _Elevation = value; NotifyPropertyChanged(); }
        }


        #endregion
        #region Constructors
        public LeveeFeatureElement(string userProvidedName, string description, double elevation) : base()
        {
            Name = userProvidedName;
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name, "pack://application:,,,/Fda;component/Resources/LeveeFeature.png");

            Description = description;
            if (Description == null) Description = "";
            Elevation = elevation;

            Utilities.NamedAction editLeveeFeature = new Utilities.NamedAction();
            editLeveeFeature.Header = "Edit Levee Feature";
            editLeveeFeature.Action = EditLeveeFeature;

            Utilities.NamedAction removeLeveeFeature = new Utilities.NamedAction();
            removeLeveeFeature.Header = "Remove";
            removeLeveeFeature.Action = RemoveElement;

            Utilities.NamedAction renameElement = new Utilities.NamedAction(this);
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(editLeveeFeature);
            localActions.Add(removeLeveeFeature);
            localActions.Add(renameElement);

            Actions = localActions;

        }
        #endregion
        #region Voids
        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetLeveeManager().Remove(this);
        }
        public void EditLeveeFeature(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);
               //.WithParentGuid(this.GUID)
               //.WithCanOpenMultipleTimes(false);

            LeveeFeatureEditorVM vm = new LeveeFeatureEditorVM(this, actionManager);
            string header = "Edit " + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditLevee" + Name);
            Navigate(tab, false,false);
            //if (!vm.WasCanceled)
            //{
            //    if (!vm.HasError)
            //    {
            //        string oldName = Name;
            //        Name = vm.Name;
            //        Description = vm.Description;
            //        Elevation = vm.Elevation;

            //        ((LeveeFeatureOwnerElement)_Owner).UpdateTableRowIfModified((Utilities.ParentElement)_Owner, oldName, this);
            //        UpdateAndSaveFailureFunctionsWithNewLevee(oldName);

            //        AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(vm.Name, Utilities.Transactions.TransactionEnum.EditExisting, "Previous Name: " + oldName + " Description: " + Description + " Elevation: " + Elevation));
            //    }
            //}
        }

        private void UpdateAndSaveFailureFunctionsWithNewLevee(string oldName)
        {
            ////get the list of failure functions and update their selected levees
            //List<FailureFunctionElement> ffList = GetElementsOfType<FailureFunctionElement>();

            //foreach (FailureFunctionElement ffe in ffList)
            //{
            //    if (ffe.SelectedLateralStructure.Name == oldName)
            //    {
            //        ffe.SelectedLateralStructure = this;
            //    }
            //}
            ////save the changes
            //List<FailureFunctionOwnerElement> ffOwners = GetElementsOfType<FailureFunctionOwnerElement>();
            //if (ffOwners.FirstOrDefault() != null)
            //{
            //    ffOwners.FirstOrDefault().Save();
            //}
        }

        //public override string TableName
        //{
        //    get
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

       

        //public override void Save()
        //{
        //    throw new NotImplementedException();
        //}

        //public override object[] RowData()
        //{
        //    return new object[] { Name , Description, Elevation};
        //}

        //public override bool SavesToRow()
        //{
        //    return true;
        //}
        //public override bool SavesToTable()
        //{
        //    return false;
        //}

        #endregion
        #region Functions
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            LeveeFeatureElement elem = (LeveeFeatureElement)elementToClone;
            return new LeveeFeatureElement(elem.Name, elem.Description, elem.Elevation);
        }
        #endregion
    }


}
