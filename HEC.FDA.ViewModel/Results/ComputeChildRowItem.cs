using HEC.FDA.ViewModel.ImpactAreaScenario;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEC.FDA.ViewModel.Results
{
    public class ComputeChildRowItem : BaseViewModel
    {
        private bool _HasError;
        private string _ErrorMessage;
        private bool _IsSelected;
        private string _Name;
        private ChildElement _ChildElement;
        public ChildElement ChildElement
        {
            get { return _ChildElement; }
            set { _ChildElement = value; NotifyPropertyChanged(); }
        }
        public string Name
        {
            get { return _Name; }
            set { _Name = value; NotifyPropertyChanged(); }
        }
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; NotifyPropertyChanged(); }
        }
        public bool HasError
        {
            get { return _HasError; }
            set { _HasError = value; NotifyPropertyChanged(); }
        }
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
            set { _ErrorMessage = value; NotifyPropertyChanged(); }
        }
        public bool HasResults{get;set;}
        public string HasComputeMessage { get; set; }

        public ComputeChildRowItem(ChildElement childElement)
        {
            Load(childElement);
        }

        public void Update(ChildElement childElement)
        {
            Load(childElement);
        }

        public void Load(ChildElement childElement)
        {
            Name = childElement.Name;
            ChildElement = childElement;
            if (childElement is IASElement)
            {
                IASElement elem = (IASElement)childElement;
                HasResults = elem.Results != null;
                if (HasResults)
                {
                    HasComputeMessage = CreateHasResultsMessage(elem);
                }
            }
        }

        public string CreateHasResultsMessage(IASElement elem)
        {
            string computeDate = elem.Results.ComputeDate;
            if (computeDate == null)
            {
                computeDate = "NA";
            }
            StringBuilder sb = new StringBuilder("\t* Has Compute Results: " + elem.Results.ResultsList.Count + " Impact Areas.");
            sb.AppendLine("\n\t* Last Computed: " + computeDate);
            sb.AppendLine("\t* Last Edited: " + elem.LastEditDate);
            return sb.ToString();
        }

        public void MarkInError(string errorMessage)
        {
            HasError = true;
            ErrorMessage = errorMessage;
            IsSelected = false;
        }

        public void ClearErrorStatus()
        {
            HasError = false;
            ErrorMessage = null;
            IsSelected = false;
        }
    }
}
