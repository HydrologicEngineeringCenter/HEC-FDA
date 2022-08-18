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
        private int _FontSize = 14;
        private bool _IsBold = false;

        #endregion
        #region Properties
        public string LastEditDate { get; set; } = "";
        public int ID { get; set; }
        public bool IsOpenInTabOrWindow { get; set; }

        public int FontSize
        {
            get { return _FontSize; }
            set { _FontSize = value; NotifyPropertyChanged(nameof(FontSize)); }
        }
        public bool IsBold
        {
            get { return _IsBold; }
            set { _IsBold = value; NotifyPropertyChanged(nameof(IsBold)); }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }

        #endregion
        #region Constructors
        public ChildElement(XElement element, int id)
        {
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
            List<NamedAction> localActions = new List<NamedAction>();

            if (editAction != null)
            {
                NamedAction editInflowOutflowCurve = new NamedAction();
                editInflowOutflowCurve.Header = actionHeader;
                editInflowOutflowCurve.Action = editAction;
                localActions.Add(editInflowOutflowCurve);
            }

            NamedAction removeInflowOutflowCurve = new NamedAction();
            removeInflowOutflowCurve.Header = StringConstants.REMOVE_MENU;
            removeInflowOutflowCurve.Action = RemoveElement;

            NamedAction renameInflowOutflowCurve = new NamedAction(this);
            renameInflowOutflowCurve.Header = StringConstants.RENAME_MENU;
            renameInflowOutflowCurve.Action = Rename;

            localActions.Add(removeInflowOutflowCurve);
            localActions.Add(renameInflowOutflowCurve);

            Actions = localActions;
        }

        public abstract XElement ToXML();

        public virtual void Rename(object sender, EventArgs e)
        {
            RenameVM renameViewModel = new RenameVM(this, CloneElement);
            string header = "Rename";
            DynamicTabVM tab = new DynamicTabVM(header, renameViewModel, "Rename",false,false);
            Navigate(tab);
        }

        public virtual void RemoveElement(object sender, EventArgs e)
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Are you sure you want to delete '" + Name + "'?", "Delete " + Name + "?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(messageBoxResult == MessageBoxResult.Yes)
            {
                Saving.PersistenceFactory.GetElementManager(this).Remove(this);
            }
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
            XElement headerElem = new XElement(HEADER_XML_TAG);
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
            if(!Name.Equals(elem.Name))
            {
                isEqual = false;
            }
            if(!Description.Equals(elem.Description))
            {
                isEqual = false;
            }
            if(!LastEditDate.Equals(elem.LastEditDate))
            {
                isEqual = false;
            }

            return isEqual;
        }

    }
}
