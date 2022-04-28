using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;

namespace HEC.FDA.ViewModel.ImpactArea
{
    public class ImpactAreaElement : ChildElement
    {
        #region Notes
        #endregion
        #region Fields
        private ObservableCollection<ImpactAreaRowItem> _ImpactAreaRows;
        #endregion
        #region Properties
      
        public string SelectedPath { get; set; }
        
        public ObservableCollection<ImpactAreaRowItem> ImpactAreaRows
        {
            get { return _ImpactAreaRows; }
            set { _ImpactAreaRows = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ImpactAreaElement(string userdefinedname, string description, ObservableCollection<ImpactAreaRowItem> collectionOfRows, int id) : this(userdefinedname,description,collectionOfRows, "", id)
        {
        }
        public ImpactAreaElement(string userdefinedname,string description, ObservableCollection<ImpactAreaRowItem> collectionOfRows, string selectedPath, int id ) : base(id)
        {
            Name = userdefinedname;
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/ImpactAreas.png");
            Description = description;
            ImpactAreaRows = collectionOfRows;

            SelectedPath = selectedPath;

            NamedAction edit = new NamedAction();
            edit.Header = StringConstants.EDIT_IMPACT_AREA_SET_MENU;
            edit.Action = Edit;

            NamedAction removeImpactArea = new NamedAction();
            removeImpactArea.Header = StringConstants.REMOVE_MENU;
            removeImpactArea.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            List<NamedAction> localactions = new List<NamedAction>();
            localactions.Add(edit);
            localactions.Add(removeImpactArea);
            localactions.Add(renameElement);

            Actions = localactions;

            TableContainsGeoData = true;
        }

        public void RemoveElement(object sender, EventArgs e)
        {
            List<IASElementSet> iasElems = StudyCache.GetChildElementsOfType<IASElementSet>();
            if (iasElems.Count > 0)
            {
                StringBuilder sb = new StringBuilder(Environment.NewLine).Append(Environment.NewLine);
                foreach (IASElementSet set in iasElems)
                {
                    sb.Append("\t").Append("* ").Append(set.Name).Append(Environment.NewLine);
                }

                var result = MessageBox.Show("Deleting the impact area will also delete all existing impact area scenarios: " +
                    sb.ToString() + Environment.NewLine + "Do you want to continue with the delete?", "Do You Want to Continue", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Saving.PersistenceFactory.GetImpactAreaManager().Remove(this);
                    //delete the IAS's.
                    foreach (IASElementSet set in iasElems)
                    {
                        Saving.PersistenceFactory.GetIASManager().Remove(set);
                    }
                }
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete '" + Name + "'?", "Delete " + Name + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    Saving.PersistenceFactory.GetImpactAreaManager().Remove(this);
                }
            }

        }
        #endregion
        #region Voids
        private void Edit(object arg1, EventArgs arg2)
        {
            //create an observable collection of all the available paths
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            ImpactAreaImporterVM vm = new ImpactAreaImporterVM(this, ImpactAreaRows, actionManager);
            string header = StringConstants.EDIT_IMPACT_AREA_SET_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, header);
            Navigate(tab, false,false);
        }

        #endregion
        #region Functions 
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            ImpactAreaElement elem = (ImpactAreaElement)elementToClone;
            return new ImpactAreaElement(elem.Name, elem.Description,elem.ImpactAreaRows,elem.SelectedPath, elem.ID);
        }
        #endregion
    }
}
