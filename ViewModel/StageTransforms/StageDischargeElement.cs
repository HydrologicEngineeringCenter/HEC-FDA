using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.StageTransforms
{
    public class StageDischargeElement : CurveChildElement
    {
        
        #region Constructors

        public StageDischargeElement(string name, string lastEditDate, string desc, ComputeComponentVM ratingCurve, int id) 
            : base(name, lastEditDate, desc,ratingCurve, id)
        {
            AddDefaultActions(EditRatingCurve);
        }

        public StageDischargeElement(XElement elem, int id):base(elem, id)
        {
            AddDefaultActions(EditRatingCurve);
        }

        #endregion
        #region Voids

        public void EditRatingCurve(object arg1, EventArgs arg2)
        {       
            EditorActionManager actionManager = new EditorActionManager()
                .WithSiblingRules(this);

            RatingCurveEditorVM vm = new RatingCurveEditorVM(this, actionManager);
            string header = "Edit " + vm.Name;
            DynamicTabVM tab = new DynamicTabVM(header, vm, "EditRatingCurve" + vm.Name);
            Navigate(tab,false, false);   
        }

        #endregion
    }
}
