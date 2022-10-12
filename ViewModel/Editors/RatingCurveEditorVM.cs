using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;

namespace HEC.FDA.ViewModel.Editors
{
    public class RatingCurveEditorVM : CurveEditorVM
    {
        public RatingCurveEditorVM(CurveComponentVM defaultCurve, EditorActionManager actionManager) : base(defaultCurve, actionManager)
        {
        }

        public RatingCurveEditorVM(CurveChildElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
        }

        public override void Save()
        {
            int id = GetElementID<StageDischargeElement>();
            StageDischargeElement elem = new StageDischargeElement(Name, DateTime.Now.ToString("G"), Description, TableWithPlot.CurveComponentVM, id);
            base.Save(elem);
        }
    }
}
