using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ViewModel.ImpactAreaScenario;
using ViewModel.Utilities;

namespace ViewModel.Alternatives
{
    public class AlternativeElement : ChildElement
    {
        private const string ALTERNATIVE = "Alternative";
        private const string NAME = "Name";
        private const string DESCRIPTION = "Description";
        private const string IAS_SET = "IASSet";
        private const string ID = "ID";

        public List<int> IASElementSets { get; set; }

        #region Constructors

        /// <summary>
        /// Ctor for constructing new alternative element.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="IASElements"></param>
        public AlternativeElement(string name, string description, List<int> IASElements)
        {
            Name = name;
            Description = description;
            IASElementSets = IASElements;
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Condition.png");
            AddActions();
        }

        /// <summary>
        /// Ctor for loading an element from the database.
        /// </summary>
        /// <param name="xml"></param>
        public AlternativeElement(string xml)
        {
            XDocument doc = XDocument.Parse(xml);
            XElement altElement = doc.Element(ALTERNATIVE);
            Name = altElement.Attribute(NAME).Value;
            Description = altElement.Attribute(DESCRIPTION).Value;

            IEnumerable<XElement> iasElements = altElement.Elements(IAS_SET);
            IASElementSets = new List<int>();
            foreach (XElement elem in iasElements)
            {
                int iasID = Int32.Parse(elem.Attribute(ID).Value);
                IASElementSets.Add(iasID);
            }
            CustomTreeViewHeader = new CustomHeaderVM(Name, "pack://application:,,,/View;component/Resources/Condition.png");
            AddActions();
        }

        #endregion

        private void AddActions()
        {
            NamedAction edit = new NamedAction();
            edit.Header = "Edit Alternative";
            edit.Action = EditAlternative;

            NamedAction compute = new NamedAction();
            compute.Header = "Calculate Average Annual Equivalent Damage";
            compute.Action = ComputeAlternative;

            NamedAction viewResults = new NamedAction();
            viewResults.Header = "View Results";
            viewResults.Action = ViewResults;

            NamedAction removeCondition = new NamedAction();
            removeCondition.Header = "Remove";
            removeCondition.Action = RemoveElement;

            NamedAction renameElement = new NamedAction(this);
            renameElement.Header = "Rename";
            renameElement.Action = Rename;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(edit);
            localActions.Add(compute);
            localActions.Add(viewResults);
            localActions.Add(removeCondition);
            localActions.Add(renameElement);

            Actions = localActions;
        }

        public void RemoveElement(object sender, EventArgs e)
        {
            Saving.PersistenceFactory.GetAlternativeManager().Remove(this);
        }
        public void ComputeAlternative(object arg1, EventArgs arg2)
        {
            //todo: waiting for hec to put the new model in.
        }
        public void ViewResults(object arg1, EventArgs arg2)
        {

            //todo: Cody will do this in part 2 of task2.
        }
        public void EditAlternative(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            CreateNewAlternativeVM vm = new CreateNewAlternativeVM(this, actionManager);
            string header = "Edit Alternative " + Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditAlternative" + Name);
            Navigate(tab, false, true);
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            if(elementToClone is AlternativeElement)
            {
                AlternativeElement elem = (AlternativeElement)elementToClone;
                AlternativeElement elemToReturn = new AlternativeElement(elem.Name, elem.Description, elem.IASElementSets);
                return elemToReturn;
            }
            return null;
        }

        
        public string WriteToXML()
        {
            XElement altElement = new XElement(ALTERNATIVE);
            altElement.SetAttributeValue(NAME, Name);
            altElement.SetAttributeValue(DESCRIPTION, Description);

            foreach (int elemID in IASElementSets)
            {
                XElement setElement = new XElement(IAS_SET);
                setElement.SetAttributeValue(ID, elemID);
                altElement.Add(setElement);
            }
            return altElement.ToString();
        }

        

    }
}
