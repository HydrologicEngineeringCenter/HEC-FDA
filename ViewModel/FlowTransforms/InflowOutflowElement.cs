using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.FlowTransforms
{
    //[Author(q0heccdm, 6 / 8 / 2017 10:33:22 AM)]
    public class InflowOutflowElement : CurveChildElement
    {
        #region Notes
        // Created By: q0heccdm
        // Created Date: 6/8/2017 10:33:22 AM
        #endregion

        #region Constructors
        public InflowOutflowElement(string name, string lastEditDate, string description, ComputeComponentVM inflowOutflowCurve, int id)
            :base(name,lastEditDate,description, inflowOutflowCurve, ImageSources.INFLOW_OUTFLOW_IMAGE, id)
        {
            AddDefaultActions(EditInflowOutflowCurve);
        }

        public InflowOutflowElement(XElement element, int id):base(element, ImageSources.INFLOW_OUTFLOW_IMAGE, id)
        {          
            AddDefaultActions(EditInflowOutflowCurve);
        }

        #endregion

        public void EditInflowOutflowCurve(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            Editors.InflowOutflowEditorVM vm = new Editors.InflowOutflowEditorVM(this, actionManager);

            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditInflowOutflow" + vm.Name);
            Navigate( tab, false, false);
        }

        public override ChildElement CloneElement(ChildElement elementToClone)
        {
            return new InflowOutflowElement(elementToClone.ToXML(), elementToClone.ID);
        }
        
    }
}
