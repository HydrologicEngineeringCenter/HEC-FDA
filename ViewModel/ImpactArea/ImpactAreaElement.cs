using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.ImpactArea
{
    public class ImpactAreaElement : ChildElement, IHaveStudyFiles
    {
        #region Notes
        #endregion
        #region Fields
        private const String IMPACT_AREA_ROWS_TAG = "ImpactAreaRows";
        #endregion
        #region Properties

        public List<ImpactAreaRowItem> ImpactAreaRows { get; } = new List<ImpactAreaRowItem>();

        #endregion
        #region Constructors
        public ImpactAreaElement(string name, string description, List<ImpactAreaRowItem> collectionOfRows, int id) 
            : base(name, "", description, id)
        {      
            ImpactAreaRows = collectionOfRows;
            AddDefaultActions(Edit, StringConstants.EDIT_IMPACT_AREA_SET_MENU);
        }

        public ImpactAreaElement(XElement impactAreaElement, int id) : base(impactAreaElement, id)
        {
            XElement rowsElem = impactAreaElement.Element(IMPACT_AREA_ROWS_TAG);
            IEnumerable<XElement> rowElems = rowsElem.Elements(ImpactAreaRowItem.ROW_ITEM_TAG);
            foreach (XElement nameElem in rowElems)
            {
                ImpactAreaRows.Add(new ImpactAreaRowItem(nameElem));
            }

            AddDefaultActions(Edit, StringConstants.EDIT_IMPACT_AREA_SET_MENU);
        }

        public override XElement ToXML()
        {
            XElement impactAreaElem = new XElement(StringConstants.ELEMENT_XML_TAG);
            impactAreaElem.Add(CreateHeaderElement());

            XElement impactAreaRows = new XElement(IMPACT_AREA_ROWS_TAG);
            foreach (ImpactAreaRowItem row in ImpactAreaRows)
            {
                impactAreaRows.Add(row.ToXML());
            }

            impactAreaElem.Add(impactAreaRows);

            return impactAreaElem;
        }    

        private string GetScenariosToDeleteMessage()
        {
            List<IASElement> iasElems = StudyCache.GetChildElementsOfType<IASElement>();
            string scenarioMessage = null;
            if (iasElems.Count > 0)
            {
                List<string> scenarios = new List<string>();
                foreach (IASElement set in iasElems)
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

        public override void RemoveElement(object sender, EventArgs e)
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
                    Saving.PersistenceFactory.GetElementManager<ImpactAreaElement>().Remove(this);
                    //remove the directory
                    if(Directory.Exists(Connection.Instance.ImpactAreaDirectory + "\\" + Name))
                    {
                        Directory.Delete(Connection.Instance.ImpactAreaDirectory + "\\" + Name, true);
                    }
                    DeleteAllScenariosAndStageDamages();
                }
            }
        }

        private void DeleteAllScenariosAndStageDamages()
        {
            //delete the scenarios
            List<IASElement> iasElems = StudyCache.GetChildElementsOfType<IASElement>();
            foreach (IASElement set in iasElems)
            {
                Saving.PersistenceFactory.GetIASManager().Remove(set);
            }

            //delete the stage damages
            List<AggregatedStageDamageElement> stageDamageElems = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            foreach (AggregatedStageDamageElement elem in stageDamageElems)
            {
                Saving.PersistenceFactory.GetElementManager<AggregatedStageDamageElement>().Remove(elem);
            }
        }

        private void ShowDefaultDeleteMessage()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete '" + Name + "'?", "Delete " + Name + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                Saving.PersistenceFactory.GetElementManager<ImpactAreaElement>().Remove(this);
            }
            //remove the directory
            if (Directory.Exists(Connection.Instance.ImpactAreaDirectory + "\\" + Name))
            {
                Directory.Delete(Connection.Instance.ImpactAreaDirectory + "\\" + Name, true);
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
            DynamicTabVM tab = new DynamicTabVM(header, vm, header + Name);
            Navigate(tab, false,false);
        }

        #endregion

        public ImpactAreaRowItem GetImpactAreaRow(int impactAreaID)
        {
            ImpactAreaRowItem returnRow = null;
            foreach (ImpactAreaRowItem row in ImpactAreaRows)
            {
                if(row.ID == impactAreaID)
                {
                    returnRow = row;
                }
            }
            return returnRow;
        }

        public bool Equals(ImpactAreaElement elem)
        {
            bool isEqual = true;

            if (!AreHeaderDataEqual(elem))
            {
                isEqual = false;
            }

            if (ImpactAreaRows.Count != elem.ImpactAreaRows.Count)
            {
                isEqual = false;
            }
            else
            {
                for(int i = 0;i<ImpactAreaRows.Count;i++)
                {
                    if(!ImpactAreaRows[i].Equals( elem.ImpactAreaRows[i]))
                    {
                        isEqual = false;
                        break;
                    }
                }
            }
            return isEqual;
        }

    }
}
