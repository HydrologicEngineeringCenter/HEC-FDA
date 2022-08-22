using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.StageTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 11:31:34 AM)]
    public class ExteriorInteriorElement : CurveChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 11:31:34 AM
        #endregion
        #region Constructors
        public ExteriorInteriorElement(string name,string lastEditDate, string desc, ComputeComponentVM exteriorInteriorCurve, int id)
            :base(name, lastEditDate, desc, exteriorInteriorCurve, id)
        {
            AddDefaultActions(EditExteriorInteriorCurve, StringConstants.EDIT_EXT_INT_MENU);
        }

        public ExteriorInteriorElement(XElement element, int id) : base(element, id)
        {
            AddDefaultActions(EditExteriorInteriorCurve, StringConstants.EDIT_EXT_INT_MENU);
        }
        #endregion

        public void EditExteriorInteriorCurve(object arg1, EventArgs arg2)
        {         
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            ExteriorInteriorEditorVM vm = new ExteriorInteriorEditorVM(this, actionManager);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditExtInt"+vm.Name);
            Navigate(tab, false, true);         
        }

    }
}
