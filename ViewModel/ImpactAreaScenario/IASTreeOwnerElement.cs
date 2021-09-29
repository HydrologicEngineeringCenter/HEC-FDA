using ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.ImpactAreaScenario
{
    public class IASTreeOwnerElement : ParentElement
    {
        public event EventHandler UpdateConditionsTree;

        private IASOwnerElement _StudyTreeConditionsOwnerElement;
        /// <summary>
        /// The conditions owner element from the study gets passed in here.
        /// This is so that we can call it to rename, remove, edit, add. We do not
        /// save anything that is under this class (parent node). We always change
        /// what is in the study tree and then update this view to reflect the 
        /// study tree.
        /// </summary>
        /// <param name="conditionsOwnerElement"></param>
        public IASTreeOwnerElement(IASOwnerElement conditionsOwnerElement) : base()
        {
            _StudyTreeConditionsOwnerElement = conditionsOwnerElement;
            Name = "Conditions";
            CustomTreeViewHeader = new CustomHeaderVM(Name);

            NamedAction addCondition = new NamedAction();
            addCondition.Header = "Create New Condition";
            addCondition.Action = AddNewCondition;

            List<NamedAction> localActions = new List<NamedAction>();
            localActions.Add(addCondition);

            Actions = localActions;
        }

        private void AddNewCondition(object arg1, EventArgs arg2)
        {
            if (_StudyTreeConditionsOwnerElement != null)
            {
                _StudyTreeConditionsOwnerElement.AddNewCondition(arg1, arg2);

                if (UpdateConditionsTree != null)
                {
                    UpdateConditionsTree.Invoke(arg1, arg2);
                }
            }
        }
        public void UpdateElementExpandedValue(object sender, EventArgs e)
        {
            ChildElement sendingElement = (ChildElement)sender;
            foreach (ChildElement elem in _StudyTreeConditionsOwnerElement.Elements)
            {
                if (elem.Name.Equals((sendingElement).Name))
                {
                    ((IASElement)elem).IsExpanded = ((IASElement)sendingElement).IsExpanded; 
                    return;
                }
            }
        }
        public void EditCondition(object sender, EventArgs e)
        {
            foreach (ChildElement elem in _StudyTreeConditionsOwnerElement.Elements)
            {
                if (elem.Name.Equals(((ChildElement)sender).Name))
                {
                    ((IASElement)elem).EditCondition(sender, e);
                    //UpdateTree(); I need to update the tree but only after the editor is closed
                    return;
                }
            }
        }

        public void RemoveElement(object sender, EventArgs e)
        {
            foreach (ChildElement elem in _StudyTreeConditionsOwnerElement.Elements)
            {
                if (elem.Name.Equals(((ChildElement)sender).Name))
                {
                    ((IASElement)elem).RemoveElement(sender, e);
                    UpdateTree();
                    return;
                }
            }

            

        }

        public void RenameElement(object sender, EventArgs e)
        {
            foreach (ChildElement elem in _StudyTreeConditionsOwnerElement.Elements)
            {
                if (elem.Name.Equals(((ChildElement)sender).Name))
                {
                    ((IASElement)elem).Rename(sender, e);
                    UpdateTree();
                    return;
                }
            }
        }


        private void UpdateTree()
        {
            if (UpdateConditionsTree != null)
            {
                UpdateConditionsTree.Invoke(this, new EventArgs());
            }
        }

      

        public  string TableName
        {
            get
            {
                return "ConditionsTree";
            }
        }

        //public override void AddBaseElements()
        //{
        //    //throw new NotImplementedException();
        //}

        //public override void AddElementFromRowData(object[] rowData)
        //{
        //    //throw new NotImplementedException();
        //}

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

       

       

      
    }
}
