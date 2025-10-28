using HEC.FDA.ViewModel.Saving;
using HEC.FDA.ViewModel.Saving.PersistenceManagers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.Utilities
{
    public abstract class ChildElement : BaseFdaElement
    {
        #region Notes
        #endregion
        #region Fields
        public delegate void AddElementEventHandler(object sender, Saving.ElementAddedEventArgs args);

        public const string HEADER_XML_TAG = "Header";
        private const string NAME_XML_TAG = "Name";
        private const string DESCRIPTION_XML_TAG = "Description";
        private const string LAST_EDIT_DATE_XML_TAG = "LastEditDate";

        private string _Description = "";

        #endregion
        #region Properties
        public string LastEditDate
        {
            get;
            set;
        } = "";
        public int ID { get; set; }
        public bool IsOpenInTabOrWindow { get; set; }


        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }

        #endregion
        #region Constructors
        public ChildElement(XElement element, int id)
        {
            FontSize = 14;
            IsBold = false;
            ID = id;
            ReadHeaderXElement(element.Element(HEADER_XML_TAG));
            CustomTreeViewHeader = new CustomHeaderVM(Name)
            {
                ImageSource = ImageSources.GetImage(this.GetType()),
                Tooltip = StringConstants.CreateLastEditTooltip(LastEditDate)
            };
        }

        public ChildElement(string name, string lastEditDate, string description, int id)
        {
            FontSize = 14;
            IsBold = false;
            Name = name;
            LastEditDate = lastEditDate;
            Description = description;
            ID = id;
            CustomTreeViewHeader = new CustomHeaderVM(name)
            {
                ImageSource = ImageSources.GetImage(this.GetType()),
                Tooltip = StringConstants.CreateLastEditTooltip(lastEditDate)
            };
        }
        #endregion

        public void AddDefaultActions(Action<object, EventArgs> editAction = null, string actionHeader = "")
        {
            List<NamedAction> localActions = new();

            if (editAction != null)
            {
                NamedAction editElement = new()
                {
                    Header = actionHeader,
                    Action = editAction
                };
                localActions.Add(editElement);
            }

            NamedAction removeElement = new()
            {
                Header = StringConstants.REMOVE_MENU,
                Action = RemoveElement
            };

            NamedAction renameElement = new(this)
            {
                Header = StringConstants.RENAME_MENU,
                Action = Rename
            };

            NamedAction duplicateElement = new(this)
            {
                Header = "Duplicate",
                Action = DuplicateElement
            };

            localActions.Add(removeElement);
            localActions.Add(renameElement);
            localActions.Add(duplicateElement);

            Actions = localActions;
        }

        public abstract XElement ToXML();

        public virtual void Rename(object sender, EventArgs e)
        {
            RenameVM renameViewModel = new(this, CloneElement);
            string header = "Rename";
            DynamicTabVM tab = new(header, renameViewModel, "Rename", false, false);
            Navigate(tab);
        }

        public virtual void RemoveElement(object sender, EventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete '" + Name + "'?", "Delete " + Name + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (this is IHaveStudyFiles)
                {
                    bool success = StudyFilesManager.DeleteDirectory(Name, GetType());
                    if (success)
                    {
                        PersistenceFactory.GetElementManager(this).Remove(this);
                    }
                }
                else
                {
                    PersistenceFactory.GetElementManager(this).Remove(this);
                }
            }
        }

        public virtual void DuplicateElement(object sender, EventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Would you like to duplicate '" + Name + "'?", "Duplicate " + Name + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (messageBoxResult != MessageBoxResult.Yes)
                return;

            var clonedElement = CloneElement();
            string originalName = clonedElement.Name;
            string duplicateName = "Copy of " + originalName;
            clonedElement.Name = duplicateName;
            clonedElement.UpdateTreeViewHeader(duplicateName);

            IElementManager savingManager = PersistenceFactory.GetElementManager(clonedElement);
            int id = savingManager.GetNextAvailableId();
            clonedElement.ID = id;
            savingManager.SaveNew(clonedElement);
        }

        /// <summary>
        /// I think this is only being used for renaming elements.
        /// </summary>
        /// <param name="elementToClone"></param>
        /// <returns></returns>
        public ChildElement CloneElement()
        {
            return (ChildElement)Activator.CreateInstance(GetType(), ToXML(), ID);
        }

        public XElement CreateHeaderElement()
        {
            XElement headerElem = new(HEADER_XML_TAG);
            headerElem.SetAttributeValue("ID", ID);
            headerElem.SetAttributeValue(NAME_XML_TAG, Name);
            headerElem.SetAttributeValue(DESCRIPTION_XML_TAG, Description);
            headerElem.SetAttributeValue(LAST_EDIT_DATE_XML_TAG, LastEditDate);
            return headerElem;
        }

        public void ReadHeaderXElement(XElement headerXML)
        {
            //todo: id
            Name = headerXML.Attribute(NAME_XML_TAG).Value;
            Description = headerXML.Attribute(DESCRIPTION_XML_TAG).Value;
            LastEditDate = headerXML.Attribute(LAST_EDIT_DATE_XML_TAG).Value;
        }

        public bool AreHeaderDataEqual(ChildElement elem)
        {
            bool isEqual = true;
            if (!Name.Equals(elem.Name))
            {
                isEqual = false;
            }
            if (!Description.Equals(elem.Description))
            {
                isEqual = false;
            }
            if (!LastEditDate.Equals(elem.LastEditDate))
            {
                isEqual = false;
            }

            return isEqual;
        }

    }
}
