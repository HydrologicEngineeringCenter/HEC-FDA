using HEC.FDA.ViewModel.Editors;
using HEC.FDA.ViewModel.TableWithPlot;
using HEC.FDA.ViewModel.Utilities;
using System;
using System.Xml.Linq;

namespace HEC.FDA.ViewModel.StageTransforms
{
    public class RatingCurveElement : CurveChildElement
    {
        
        #region Constructors

        public RatingCurveElement(string name, string lastEditDate, string desc, ComputeComponentVM ratingCurve, int id) 
            : base(name, lastEditDate, desc,ratingCurve, ImageSources.RATING_IMAGE, id)
        {
            AddDefaultActions(EditRatingCurve);
        }

        public RatingCurveElement(XElement elem, int id):base(elem, ImageSources.RATING_IMAGE, id)
        {
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
