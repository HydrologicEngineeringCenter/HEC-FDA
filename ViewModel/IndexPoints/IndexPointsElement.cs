using HEC.FDA.ViewModel.AggregatedStageDamage;
using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using HEC.FDA.ViewModel.Storage;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.IndexPoints
{
    public class IndexPointsElement:ChildElement, IHaveStudyFiles
    {
        public const String INDEX_POINTS_TAG = "IndexPoints";
        private const String NAME_TAG = "Name";
        private const String INDEX_POINT_NAMES_TAG = "IndexPointNames";

        #region Properties
        public List<string> IndexPoints { get; } = new List<string>();      
        #endregion

        #region Constructors
        public IndexPointsElement(string name, string description, List<string> indexPoints, int id) : base(name, "", description, id)
        {
            IndexPoints = indexPoints;
            AddDefaultActions(Edit, StringConstants.EDIT_INDEX_POINTS_MENU);
        }

        public IndexPointsElement(XElement childElem, int id):base(childElem, id)
        {
            XElement indexPointsElem = childElem.Element(INDEX_POINT_NAMES_TAG);
            IEnumerable<XElement> nameElems = indexPointsElem.Elements(NAME_TAG);
            foreach(XElement nameElem in nameElems)
            {
                IndexPoints.Add(nameElem.Value);
            }

            AddDefaultActions(Edit, StringConstants.EDIT_INDEX_POINTS_MENU);
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

        private FdaValidationResult DoStageDamagesDependOnMe()
        {
            FdaValidationResult vr = new FdaValidationResult();
            List<AggregatedStageDamageElement> stageDamageElems = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            List<string> stageDamageNames = new List<string>();
            foreach(AggregatedStageDamageElement elem in stageDamageElems)
            {
                if(!elem.IsManual && elem.SelectedIndexPoints == this.ID)
                {
                    stageDamageNames.Add(elem.Name);
                }
            }

            if(stageDamageNames.Count == 1)
            {
                vr.AddErrorMessage("The aggregated stage damage function '" + stageDamageNames[0] + "' uses this index points element and will also be deleted.");
            }
            else if(stageDamageNames.Count > 1)
            {
                vr.AddErrorMessage("The following aggregated stage damage functions use this index points element and will also be deleted: " + string.Join(", ", stageDamageNames));
            }

            return vr;
        }

        private void DeleteStageDamagesWithThisIndexPointID()
        {
            List<AggregatedStageDamageElement> iasElems = StudyCache.GetChildElementsOfType<AggregatedStageDamageElement>();
            foreach (AggregatedStageDamageElement elem in iasElems)
            {
                if (!elem.IsManual && elem.SelectedIndexPoints == this.ID)
                {
                    PersistenceFactory.GetElementManager<AggregatedStageDamageElement>().Remove(elem);
                }
            }
        }

        public override void RemoveElement(object sender, EventArgs e)
        {
            FdaValidationResult areStageDamagesDependingOnThis = DoStageDamagesDependOnMe();
            if (!areStageDamagesDependingOnThis.IsValid)
            {
                var result = MessageBox.Show(areStageDamagesDependingOnThis.ErrorMessage, "Do You Want to Continue?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DeleteStageDamagesWithThisIndexPointID();
                    PersistenceFactory.GetElementManager<IndexPointsElement>().Remove(this);
                    StudyFilesManager.DeleteDirectory(Name, GetType());
                }
            }
            else
            {
                MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete '" + Name + "'?", "Delete " + Name + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    PersistenceFactory.GetElementManager<IndexPointsElement>().Remove(this);
                    StudyFilesManager.DeleteDirectory(Name, GetType());
                }
            }
        }

        #endregion
        #region Functions 


        public override XElement ToXML()
        {
            XElement indexPointsElem = new XElement(INDEX_POINTS_TAG);

            indexPointsElem.Add(CreateHeaderElement());

            XElement indexPointNames = new XElement(INDEX_POINT_NAMES_TAG);
            foreach(string name in IndexPoints)
            {
                indexPointNames.Add(new XElement(NAME_TAG, name));
            }

            indexPointsElem.Add(indexPointNames);

            return indexPointsElem;
        }

        public bool Equals(IndexPointsElement elem)
        {
            bool isEqual = true;

            if (!AreHeaderDataEqual(elem))
            {
                isEqual = false;
            }

            if (IndexPoints.Count != elem.IndexPoints.Count)
            {
                isEqual = false;
            }
            else
            {
                for (int i = 0; i < IndexPoints.Count; i++)
                {
                    if (!IndexPoints[i].Equals(elem.IndexPoints[i]))
                    {
                        isEqual = false;
                        break;
                    }
                }
            }
            return isEqual;
        }


        #endregion
    }
}
