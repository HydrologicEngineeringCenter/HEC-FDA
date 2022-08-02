using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.IndexPoints
{
    public class IndexPointsElement:ChildElement
    {
        private static String INDEX_POINTS_TAG = "IndexPoints";
        private static String NAME_TAG = "Name";
        private static String DESCRIPTION_TAG = "Description";
        private static String LAST_EDIT_DATE_TAG = "LastEditDate";
        private static String INDEX_POINT_NAMES_TAG = "IndexPointNames";

        #region Properties
        public List<string> IndexPoints { get; } = new List<string>();      
        #endregion

        #region Constructors
        public IndexPointsElement(string name, string description, List<string> indexPoints, int id) : base(id)
        {
            Name = name;
            CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.IMPACT_AREAS_IMAGE);
            Description = description;
            IndexPoints = indexPoints;
            AddActions();          
        }

        public IndexPointsElement(string xmlString, int id):base(id)
        {
            ID = id;
            XDocument doc = XDocument.Parse(xmlString);
            XElement itemElem = doc.Element(INDEX_POINTS_TAG);
            Name = itemElem.Attribute(NAME_TAG).Value;
            CustomTreeViewHeader = new CustomHeaderVM(Name, ImageSources.IMPACT_AREAS_IMAGE);

            Description = itemElem.Attribute(DESCRIPTION_TAG).Value;
            LastEditDate = itemElem.Attribute(LAST_EDIT_DATE_TAG).Value;

            XElement indexPointsElem = itemElem.Element(INDEX_POINT_NAMES_TAG);
            IEnumerable<XElement> nameElems = indexPointsElem.Elements(NAME_TAG);
            foreach(XElement nameElem in nameElems)
            {
                IndexPoints.Add(nameElem.Value);
            }

            AddActions();
        }

        private void AddActions()
        {
            NamedAction edit = new NamedAction();
            edit.Header = StringConstants.EDIT_INDEX_POINTS_MENU;
            edit.Action = Edit;

            NamedAction removeElement = new NamedAction();
            removeElement.Header = StringConstants.REMOVE_MENU;
            removeElement.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = StringConstants.RENAME_MENU;
            renameElement.Action = Rename;

            List<NamedAction> localactions = new List<NamedAction>();
            localactions.Add(edit);
            localactions.Add(removeElement);
            localactions.Add(renameElement);

            Actions = localactions;
        }

        #endregion
        #region Voids
        private void Edit(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            IndexPointsEditorVM vm = new IndexPointsEditorVM(this, IndexPoints, actionManager);
            string header = StringConstants.EDIT_INDEX_POINTS_HEADER;
            DynamicTabVM tab = new DynamicTabVM(header, vm, header + Name);
            Navigate(tab, false, false);
        }

        public override void Rename(object sender, EventArgs e)
        {
            string originalName = Name;
            RenameVM renameViewModel = new RenameVM(this, CloneElement);
            string header = "Rename";
            DynamicTabVM tab = new DynamicTabVM(header, renameViewModel, "Rename", false, false);
            Navigate(tab);
            if (!renameViewModel.WasCanceled)
            {
                string newName = renameViewModel.Name;
                //rename the folders in the study.
                if (!originalName.Equals(newName))
                {
                    try
                    {
                        string sourceFilePath = Connection.Instance.IndexPointsDirectory + "\\" + originalName;
                        string destinationFilePath = Connection.Instance.IndexPointsDirectory + "\\" + newName;
                        Directory.Move(sourceFilePath, destinationFilePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Renaming the index points directory failed.\n" + ex.Message, "Rename Failed", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
        }

        private string GetStageDamageMessage()
        {
            //todo: check the stage damages for any that use these index points.
            List<AggregatedStageDamageElement> iasElems = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            return null;
        }

        public override void RemoveElement(object sender, EventArgs e)
        {
            string stageDamageMessage = GetStageDamageMessage();
            if (stageDamageMessage != null)
            {
                var result = MessageBox.Show(stageDamageMessage, "Do You Want to Continue", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteIndexPointsDirectory();
                    //todo: delete stage damages with these index points?
                }
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete '" + Name + "'?", "Delete " + Name + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    DeleteIndexPointsDirectory();
                }
            }
        }

        private void DeleteIndexPointsDirectory()
        {
            //this will handle removing the sqlite data
            Saving.PersistenceFactory.GetIndexPointsPersistenceManager().Remove(this);
            //remove the directory
            if (Directory.Exists(Connection.Instance.IndexPointsDirectory + "\\" + Name))
            {
                Directory.Delete(Connection.Instance.IndexPointsDirectory + "\\" + Name, true);
            }
        }

        #endregion
        #region Functions 
        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            IndexPointsElement elem = (IndexPointsElement)elementToClone;
            return new IndexPointsElement(elem.Name, elem.Description, elem.IndexPoints, elem.ID);
        }

        public XElement ToXML()
        {
            XElement indexPointsElem = new XElement(INDEX_POINTS_TAG);
            indexPointsElem.SetAttributeValue(NAME_TAG, Name);
            indexPointsElem.SetAttributeValue(DESCRIPTION_TAG, Description);
            indexPointsElem.SetAttributeValue(LAST_EDIT_DATE_TAG, LastEditDate);

            XElement indexPointNames = new XElement(INDEX_POINT_NAMES_TAG);
            foreach(string name in IndexPoints)
            {
                indexPointNames.Add(new XElement(NAME_TAG, name));
            }

            indexPointsElem.Add(indexPointNames);

            return indexPointsElem;
        }
        #endregion
    }
}
