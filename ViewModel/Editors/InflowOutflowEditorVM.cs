using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;

namespace HEC.FDA.ViewModel.Editors
{
    public class InflowOutflowEditorVM : CurveEditorVM
    {
        public InflowOutflowEditorVM(CurveComponentVM defaultCurve, EditorActionManager actionManager) : base(defaultCurve, actionManager)
        {
        }

        public InflowOutflowEditorVM(CurveChildElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
        }

        public override void Save()
        {
            UpdateCurveMetaData();
            int id = GetElementID<InflowOutflowElement>();
            InflowOutflowElement elem = new InflowOutflowElement(Name, DateTime.Now.ToString("G"), Description, TableWithPlot.CurveComponentVM, id);
            base.Save(elem);
        }
    }
}
