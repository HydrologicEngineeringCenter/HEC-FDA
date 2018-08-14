using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FdaModel;
using FdaModel.Utilities.Attributes;
using System.Threading.Tasks;

namespace FdaViewModel.GeoTech
{
    //[Author(q0heccdm, 6 / 8 / 2017 1:11:19 PM)]
    public class LeveeFeatureElement:Utilities.OwnedElement
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
        public override string GetTableConstant()
        {
            return _TableConstant;
        }
        public string Description { get { return _Description; } set { _Description = value; NotifyPropertyChanged(); } }

        public double Elevation
        {
            get { return _Elevation; }
            set { _Elevation = value; NotifyPropertyChanged(); }
        }


        #endregion
        #region Constructors
        public LeveeFeatureElement(string userProvidedName, string description, double elevation, BaseFdaElement owner):base(owner)
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
            removeLeveeFeature.Action = Remove;

            Utilities.NamedAction renameElement = new Utilities.NamedAction();
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
        public void EditLeveeFeature(object arg1, EventArgs arg2)
        {
            LeveeFeatureEditorVM vm = new LeveeFeatureEditorVM(Name, Description, Elevation,true);
            Navigate(vm, true, true);
            if (!vm.WasCancled)
            {
                if (!vm.HasError)
                {
                    bool hasChanges = false;
                    //if(vm.hasChanges)//does this actually reflect truth?
                    if (Name != vm.Name)
                    {
                        //if it is renamed, make sure to tell the fragility curves that are assoicated to update their row items also...
                        hasChanges = true;
                        //throw new NotImplementedException("we need to update related elemtents so users do not have to force the save.");
                        

                    }
                    if (Description != vm.Description)
                    {
                        hasChanges = true;
                    }
                    if (Elevation != vm.Elevation)
                    {
                        hasChanges = true;
                    }
                    if (hasChanges)
                    {

                        List<FailureFunctionElement> ffList = GetElementsOfType<FailureFunctionElement>();

                        foreach (FailureFunctionElement ffe in ffList)
                        {
                            if (ffe.SelectedLateralStructure.Name == Name)
                            {
                                ffe.SelectedLateralStructure = this;
                                
                            }
                        }
                        List<FailureFunctionOwnerElement> dummyList = GetElementsOfType<FailureFunctionOwnerElement>();

                        if (dummyList.FirstOrDefault() != null)
                        {                        
                            dummyList.FirstOrDefault().Save();
                        }

                        string oldName = Name;
                        Name = vm.Name;//should i disable this way of renaming? if not i need to check for name conflicts.
                        Description = vm.Description;//is binding two way? is this necessary?
                        Elevation = vm.Elevation;
                        AddTransaction(this, new Utilities.Transactions.TransactionEventArgs(vm.Name, Utilities.Transactions.TransactionEnum.EditExisting, "Previous Name: " + oldName + " Description: " + Description + " Elevation: " + Elevation));
                        _Owner.Save();
                        
                      
                    }
                }
            }
        }
        public override string TableName
        {
            get
            {
                throw new NotImplementedException();
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
            return new object[] { Name , Description, Elevation};
        }

        public override bool SavesToRow()
        {
            return true;
        }
        public override bool SavesToTable()
        {
            return false;
        }
    }
        #endregion
        #region Functions
        #endregion
    }
