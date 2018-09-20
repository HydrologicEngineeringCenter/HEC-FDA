using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Conditions
{
    public class ConditionsTreeOwnerElement : OwnerElement
    {
        public event EventHandler UpdateConditionsTree;

        private ConditionsOwnerElement _StudyTreeConditionsOwnerElement;
        /// <summary>
        /// The conditions owner element from the study gets passed in here.
        /// This is so that we can call it to rename, remove, edit, add. We do not
        /// save anything that is under this class (parent node). We always change
        /// what is in the study tree and then update this view to reflect the 
        /// study tree.
        /// </summary>
        /// <param name="conditionsOwnerElement"></param>
        public ConditionsTreeOwnerElement(ConditionsOwnerElement conditionsOwnerElement) : base(conditionsOwnerElement)
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

        public void EditCondition(object sender, EventArgs e)
        {
            foreach (OwnedElement elem in _StudyTreeConditionsOwnerElement.Elements)
            {
                if (elem.Name.Equals(((OwnedElement)sender).Name))
                {
                    ((ConditionsElement)elem).EditCondition(sender, e);
                    UpdateTree();
                    return;
                }
            }
        }

        public void RemoveElement(object sender, EventArgs e)
        {
            foreach (OwnedElement elem in _StudyTreeConditionsOwnerElement.Elements)
            {
                if (elem.Name.Equals(((OwnedElement)sender).Name))
                {
                    ((ConditionsElement)elem).Remove(sender, e);
                    UpdateTree();
                    return;
                }
            }
        }

        public void RenameElement(object sender, EventArgs e)
        {
            foreach (OwnedElement elem in _StudyTreeConditionsOwnerElement.Elements)
            {
                if (elem.Name.Equals(((OwnedElement)sender).Name))
                {
                    ((ConditionsElement)elem).Rename(sender, e);
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

        public override void Save()
        {
            //do nothing
        }

        public override string TableName
        {
            get
            {
                return "ConditionsTree";
            }
        }

        public override void AddBaseElements()
        {
            //throw new NotImplementedException();
        }

        public override void AddElement(object[] rowData)
        {
            //throw new NotImplementedException();
        }

        public override void AddValidationRules()
        {
            //throw new NotImplementedException();
        }

        public override string GetTableConstant()
        {
            return "";
        }

        public override string[] TableColumnNames()
        {
            return new string[0];
        }

        public override Type[] TableColumnTypes()
        {
            return new Type[0];
        }
    }
}
