using System.Xml.Linq;
using Utility.Extensions.Attributes;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
    public class AnalyticalVM: BaseViewModel
    {
        #region Fields
        private FitToFlowVM _fitToFlowVM;
        private ParameterEntryVM _parameterEntryVM;
        private bool _isFitToFlows = false; //new windows open with manual entry vm open
        #endregion

        #region Properties
        public FitToFlowVM FitToFlowVM
        {
            get { return _fitToFlowVM; }
            set { _fitToFlowVM = value; }
        }
        public ParameterEntryVM ParameterEntryVM
        {
            get { return _parameterEntryVM; }
            set { _parameterEntryVM = value; }
        }
        public bool IsFitToFlows
        {
            get { return _isFitToFlows; }
            set
            {
                _isFitToFlows = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsParameterEntry));
            }
        }
        public bool IsParameterEntry
        {
            get { return !IsFitToFlows; }
        }
        #endregion

        #region Constructors
        public AnalyticalVM()
        {
            FitToFlowVM = new FitToFlowVM();
            ParameterEntryVM = new ParameterEntryVM();
        }
        public AnalyticalVM(XElement ele)
        {
            LoadFromXML(ele);
        }
        #endregion

        #region Loading and Saving
        public XElement ToXML()
        {
            XElement ele = new XElement(this.GetType().Name);
            ele.SetAttributeValue(nameof(IsFitToFlows), IsFitToFlows);
            ele.Add(FitToFlowVM.ToXML());
            ele.Add(ParameterEntryVM.ToXML());
            return ele;
        }
        private void LoadFromXML(XElement ele)
        {
            IsFitToFlows = bool.Parse(ele.Attribute(nameof(IsFitToFlows)).Value);
            foreach (XElement child in ele.Elements())
            {
                if (child.Name.Equals(typeof(ParameterEntryVM).Name)){
                    ParameterEntryVM = new ParameterEntryVM(child);
                }
                else if (child.Name.Equals(typeof(FitToFlowVM).Name))
                {
                    FitToFlowVM = new FitToFlowVM(child);
                }
            }
        }
        #endregion
    }
}
