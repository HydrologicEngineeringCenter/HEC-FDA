using HEC.FDA.ViewModel.AggregatedStageDamage;
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
        private List<ImpactAreaRowItem> _ImpactAreaRows;
        #endregion
        #region Properties
      
        public string SelectedPath { get; set; }
        
        public List<ImpactAreaRowItem> ImpactAreaRows
        {
            get { return _ImpactAreaRows; }
            set { _ImpactAreaRows = value; NotifyPropertyChanged(); }
        }
        #endregion
        #region Constructors
        public ImpactAreaElement(string userdefinedname, string description, List<ImpactAreaRowItem> collectionOfRows, int id) : this(userdefinedname,description,collectionOfRows, "", id)
        {
        }
        public ImpactAreaElement(string userdefinedname,string description, List<ImpactAreaRowItem> collectionOfRows, string selectedPath, int id ) : base(id)
        {
            Name = userdefinedname;
            CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.IMPACT_AREAS_IMAGE);
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

        }

        private string GetScenariosToDeleteMessage()
        {
            List<IASElementSet> iasElems = StudyCache.GetChildElementsOfType<IASElementSet>();
            string scenarioMessage = null;
            if (iasElems.Count > 0)
            {
                List<string> scenarios = new List<string>();
                foreach (IASElementSet set in iasElems)
                {
                    scenarios.Add(set.Name);
                }

                string scenarioCSV = string.Join(", ", scenarios);

                scenarioMessage = "Deleting the impact area will also delete all existing scenarios: " + scenarioCSV;            
            }
            return scenarioMessage;
        }

        private string GetStageDamagesToDeleteMessage()
        {
            List<AggregatedStageDamageElement> stageDamageElems = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            string stageDamageMessage = null;
            List<string> stageDamages = new List<string>();
            if (stageDamageElems.Count > 0)
            {
                foreach (AggregatedStageDamageElement stageDamage in stageDamageElems)
                {
                    stageDamages.Add(stageDamage.Name);
                }

                string stageDamageCSV = string.Join(", ", stageDamages);
                stageDamageMessage = "Deleting the impact area will also delete all existing stage damage relationships: " + stageDamageCSV;
            }
            return stageDamageMessage;
        }

        public void RemoveElement(object sender, EventArgs e)
        {
            string scenariosToDeleteMessage = GetScenariosToDeleteMessage();

            string stageDamagesToDeleteMessage = GetStageDamagesToDeleteMessage();

            StringBuilder sb = new StringBuilder();

            if(scenariosToDeleteMessage != null)
            {
                sb.AppendLine(scenariosToDeleteMessage);
            }

            if(stageDamagesToDeleteMessage != null)
            {
                sb.AppendLine(stageDamagesToDeleteMessage);
            }

            if(scenariosToDeleteMessage == null && stageDamagesToDeleteMessage == null)
            {
                ShowDefaultDeleteMessage();
            }
            else
            {
                var result = MessageBox.Show(sb.ToString() 
                     + Environment.NewLine + "Do you want to continue with the delete?", "Do You Want to Continue", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    Saving.PersistenceFactory.GetImpactAreaManager().Remove(this);
                    DeleteAllScenariosAndStageDamages();
                }
            }
        }

        private void DeleteAllScenariosAndStageDamages()
        {
            //delete the scenarios
            List<IASElementSet> iasElems = StudyCache.GetChildElementsOfType<IASElementSet>();
            foreach (IASElementSet set in iasElems)
            {
                Saving.PersistenceFactory.GetIASManager().Remove(set);
            }

            //delete the stage damages
            List<AggregatedStageDamageElement> stageDamageElems = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            foreach (AggregatedStageDamageElement elem in stageDamageElems)
            {
                Saving.PersistenceFactory.GetStageDamageManager().Remove(elem);
            }
        }

        private void ShowDefaultDeleteMessage()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete '" + Name + "'?", "Delete " + Name + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Saving.PersistenceFactory.GetImpactAreaManager().Remove(this);
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
