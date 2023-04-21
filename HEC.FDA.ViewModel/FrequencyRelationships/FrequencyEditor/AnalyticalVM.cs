using HEC.FDA.ViewModel.TableWithPlot;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
    public class AnalyticalVM: BaseViewModel
    {
        #region Fields
        private CurveComponentVM _fitToFlowVM;
        private CurveComponentVM _parameterEntryVM;
        private bool _isFitToFlows = false; //new windows open with manual entry vm open
        #endregion
        #region Properties
        public CurveComponentVM FitToFlowVM
        {
            get { return _fitToFlowVM; }
            set { _fitToFlowVM = value; }
        }
        public CurveComponentVM ParameterEntryVM
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
        #region Methods
        public XElement ToXML()
        {
            XElement ele = new XElement(this.GetType().Name);
            ele.Add(FitToFlowVM.ToXML());
            ele.Add(ParameterEntryVM.ToXML());
            return ele;
        }
        private void LoadFromXML(XElement ele)
        {
            var elements = ele.Descendants();
            foreach (XElement element in elements)
            {
                string elementName = element.Name.ToString();
                if (elementName.Equals("LogPearson3VM"))
                {
                    FitToFlowVM = new FitToFlowVM(element);
                }
                else if (elementName.Equals("TableWithPlotVM"))
                {
                }
            }
        }
        #endregion
    }
}
