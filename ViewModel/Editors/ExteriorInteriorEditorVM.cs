using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;

namespace HEC.FDA.ViewModel.Editors
{
    public class ExteriorInteriorEditorVM:CurveEditorVM
    {
        public ExteriorInteriorEditorVM(ComputeComponentVM defaultCurve, EditorActionManager actionManager) : base(defaultCurve, actionManager)
        {
        }

        public ExteriorInteriorEditorVM(CurveChildElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
        }

        public override void Save()
        {
            int id = GetElementID<ExteriorInteriorElement>();
            ExteriorInteriorElement elem = new ExteriorInteriorElement(Name, DateTime.Now.ToString("G"), Description, TableWithPlot.ComputeComponentVM, id);
            base.Save(elem);
        }

    }
}
