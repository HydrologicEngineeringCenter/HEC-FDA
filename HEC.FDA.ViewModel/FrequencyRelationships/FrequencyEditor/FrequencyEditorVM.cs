using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using HEC.MVVMFramework.ViewModel.Implementations;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
	public class FrequencyEditorVM:ValidatingBaseViewModel
    {
        #region Fields
        private AnalyticalVM _analyticalVM;
        private TableWithPlotVM _graphicalVM;
        private bool _isGraphical = false; //new windows open with analytical vm open
        #endregion

        #region Properties
        public AnalyticalVM AnalyticalVM
		{
			get { return _analyticalVM; }
			set { _analyticalVM = value; }
		}
		public TableWithPlotVM GraphicalVM
		{
			get { return _graphicalVM; }
			set { _graphicalVM = value; }
		}
        public bool IsGraphical
        {
            get { return _isGraphical; }
            set
            {
                _isGraphical = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(IsAnalytic));
            }
        }
        public bool IsAnalytic
        {
            get { return !IsGraphical; }
        }
        #endregion

        #region Constructors
        public FrequencyEditorVM()
        {
            AnalyticalVM = new AnalyticalVM();
            GraphicalVM = new TableWithPlotVM(new GraphicalVM(StringConstants.GRAPHICAL_FREQUENCY,StringConstants.EXCEEDANCE_PROBABILITY,StringConstants.DISCHARGE),true,true,true);
        }
        #endregion

        #region Save and Load
        public XElement ToXML()
        {
            XElement ele = new XElement(GetType().Name);
            ele.SetAttributeValue(nameof(IsGraphical), IsGraphical);
            ele.Add(GraphicalVM.ToXML());
            ele.Add(AnalyticalVM.ToXML());
            return ele;
        }
        public void FromXML(XElement ele)
        {
            IsGraphical = bool.Parse(ele.Attribute(nameof(IsGraphical)).Value);
            foreach(XElement childs in ele.Elements())
            {
                if (childs.Name.Equals(typeof(TableWithPlotVM).Name)){
                    GraphicalVM = new TableWithPlotVM(childs);
                }
                else if (childs.Name.Equals(typeof(AnalyticalVM).Name)) {
                    AnalyticalVM = new AnalyticalVM(childs);
                }
            }
        }
        #endregion


    }
}
