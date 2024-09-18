using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FrequencyRelationships.FrequencyEditor
{
	public class FrequencyEditorVM:BaseEditorVM
    {
        #region Fields
        private AnalyticalVM _analyticalVM;
        private TableWithPlotVM _graphicalVM;
        private bool _isGraphical = false; //new windows open with analytical vm open
        #endregion

        #region Properties
        public ParameterEntryVM ParameterEntryVM
        {
            get;
            set;
        }
        public AnalyticalVM AnalyticalVM // FUCK you. You're on the chopping block. 
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
            }
        }
        #endregion

        #region Constructors
        public FrequencyEditorVM(EditorActionManager actionManager) : base(actionManager)
        {
            AnalyticalVM = new AnalyticalVM();
            GraphicalVM = new TableWithPlotVM(new GraphicalVM(StringConstants.GRAPHICAL_FREQUENCY,StringConstants.EXCEEDANCE_PROBABILITY,StringConstants.DISCHARGE),true,true,true);
        }
        public FrequencyEditorVM(FrequencyElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            FromXML(elem.FrequencyEditorXML);
        }
        public FrequencyEditorVM() : base(null)
        {
            AnalyticalVM = new AnalyticalVM();
            GraphicalVM = new TableWithPlotVM(new GraphicalVM(StringConstants.GRAPHICAL_FREQUENCY, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.DISCHARGE), true, true, true);
        }
        public FrequencyEditorVM(XElement ele) : base(null)
        {
            FromXML(ele);
        }
        #endregion

        #region Save and Load
        public XElement ToXML()
        {
            XElement ele = new(GetType().Name);
            ele.SetAttributeValue(nameof(IsGraphical), IsGraphical);
            ele.Add(GraphicalVM.ToXML());
            ele.Add(ParameterEntryVM.ToXML());
            return ele;
        }
        public void FromXML(XElement ele)
        {
            //Add try parse because people can edit this on disk and be diks. 
            IsGraphical = bool.Parse(ele.Attribute(nameof(IsGraphical)).Value);
            foreach(XElement childs in ele.Elements())
            {
                if (childs.Name.LocalName.Equals(typeof(TableWithPlotVM).Name)){
                    GraphicalVM = new TableWithPlotVM(childs);
                }
                else if (childs.Name.LocalName.Equals(typeof(ParameterEntryVM).Name))
                {
                    ParameterEntryVM = new(childs);
                }
                else if (childs.Name.LocalName.Equals(typeof(AnalyticalVM).Name)) {
                    BackwardCompatibilityFromXML(childs);
                }
            }
        }

        private void BackwardCompatibilityFromXML(XElement ele)
        {
            foreach (XElement child in ele.Elements())
            {
                if (child.Name.LocalName.Equals(typeof(ParameterEntryVM).Name))
                {
                    ParameterEntryVM = new ParameterEntryVM(child);
                }
                else if (child.Name.LocalName.Equals(typeof(FitToFlowVM).Name))
                {
                    //Just ignore it. We don't support fit to flow anymore as of 9/18/2024. No released version ever has or will. 
                }
            }
        }

        public override void Save()
        {
            int id = GetElementID<FrequencyElement>();
            string lastEditDate = DateTime.Now.ToString("G");
            FrequencyElement elem = new(Name, lastEditDate, Description,id,this);
            Save(elem);
        }
        #endregion


    }
}
