using HEC.FDA.ViewModel.FlowTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;

namespace HEC.FDA.ViewModel.Editors
{
    public class InflowOutflowEditorVM : CurveEditorVM
    {
        public InflowOutflowEditorVM(ComputeComponentVM defaultCurve, EditorActionManager actionManager) : base(defaultCurve, actionManager)
        {
        }

        public InflowOutflowEditorVM(CurveChildElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
        }

        public override void Save()
        {
            int id = GetElementID(Saving.PersistenceFactory.GetInflowOutflowManager());
            InflowOutflowElement elem = new InflowOutflowElement(Name, DateTime.Now.ToString("G"), Description, TableWithPlot.ComputeComponentVM, id);
            base.Save(elem);
        }
    }
}
