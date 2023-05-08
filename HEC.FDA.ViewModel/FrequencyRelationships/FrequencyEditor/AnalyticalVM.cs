using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
    public class AnalyticalVM: BaseViewModel
    {
        #region Fields
        private bool _isFitToFlows = false; //new windows open with manual entry vm open
        #endregion

        #region Properties
        public FitToFlowVM FitToFlowVM {get;set;}
        public ParameterEntryVM ParameterEntryVM { get; set; }
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
            XElement ele = new(GetType().Name);
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
                if (child.Name.LocalName.Equals(typeof(ParameterEntryVM).Name)){
                    ParameterEntryVM = new ParameterEntryVM(child);
                }
                else if (child.Name.LocalName.Equals(typeof(FitToFlowVM).Name))
                {
                    FitToFlowVM = new FitToFlowVM(child);
                }
            }
        }
        #endregion
    }
}
