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
        private GraphicalVM _graphicalVM;
        private bool _isGraphical = false; //new windows open with analytical vm open
        #endregion

        #region Properties
        public ParameterEntryVM ParameterEntryVM
        {
            get;
            set;
        }
		public GraphicalVM MyGraphicalVM
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
        /// <summary>
        /// from owner element
        /// </summary>
        /// <param name="actionManager"></param>
        public FrequencyEditorVM(EditorActionManager actionManager) : base(actionManager)
        {
            ParameterEntryVM = new();
            MyGraphicalVM = new GraphicalVM(StringConstants.GRAPHICAL_FREQUENCY, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.DISCHARGE);
        }
        /// <summary>
        /// from frequency element
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="actionManager"></param>
        public FrequencyEditorVM(FrequencyElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
            FromXML(elem.FrequencyEditorXML);
        }
        public FrequencyEditorVM() : base(null)
        {
            ParameterEntryVM = new();
            MyGraphicalVM = new GraphicalVM(StringConstants.GRAPHICAL_FREQUENCY, StringConstants.EXCEEDANCE_PROBABILITY, StringConstants.DISCHARGE);
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
            ele.Add(MyGraphicalVM.ToXML());
            ele.Add(ParameterEntryVM.ToXML());
            return ele;
        }
        public void FromXML(XElement ele)
        {
            //Add try parse because people can edit this on disk and be diks. 
            IsGraphical = bool.Parse(ele.Attribute(nameof(IsGraphical)).Value);
            foreach(XElement child in ele.Elements())
            {
                string childname = child.Name.LocalName;
                if (childname.Equals(typeof(TableWithPlotVM).Name) || childname.Equals(typeof(GraphicalVM).Name)){
                    MyGraphicalVM = new GraphicalVM(child);
                }
                else if (child.Name.LocalName.Equals(typeof(ParameterEntryVM).Name))
                {
                    ParameterEntryVM = new(child);
                }
                else if (child.Name.LocalName.Equals("AnalyticalVM")) {
                    BackwardCompatibilityFromXML(child);
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

        /// <summary>
        /// initializes the confidence bounds of the component vms. to be used only when an editor is opened. 
        /// </summary>
        public void InitializeConfidenceBounds()
        {
            ParameterEntryVM.InitializePlotModel();
            ParameterEntryVM.UpdatePlot();
            MyGraphicalVM.ComputeConfidenceLimits();
            return;
        }

    }
}
