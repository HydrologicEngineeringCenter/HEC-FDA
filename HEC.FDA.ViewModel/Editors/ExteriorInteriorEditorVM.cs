﻿using HEC.FDA.ViewModel.StageTransforms;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;

namespace HEC.FDA.ViewModel.Editors
{
    public class ExteriorInteriorEditorVM:CurveEditorVM
    {
        public ExteriorInteriorEditorVM(CurveComponentVM defaultCurve, EditorActionManager actionManager) : base(defaultCurve, actionManager)
        {
        }

        public ExteriorInteriorEditorVM(CurveChildElement elem, EditorActionManager actionManager) : base(elem, actionManager)
        {
        }

        public override void Save()
        {
            UpdateCurveMetaData();
            int id = GetElementID<ExteriorInteriorElement>();
            ExteriorInteriorElement elem = new ExteriorInteriorElement(Name, DateTime.Now.ToString("G"), Description, TableWithPlot.CurveComponentVM, id);
            base.Save(elem);
        }

    }
}
