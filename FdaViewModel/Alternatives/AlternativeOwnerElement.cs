using FdaViewModel.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Alternatives
{
    public class AlternativeOwnerElement: ParentElement
    {
        public AlternativeOwnerElement():base()
        {
            Name = "Alternatives";
            CustomTreeViewHeader = new Utilities.CustomHeaderVM(Name);

            Utilities.NamedAction addAlternativeAction = new Utilities.NamedAction();
            addAlternativeAction.Header = "Create New Alternative";
            addAlternativeAction.Action = AddNewAlternative;

            //Utilities.NamedAction ImportRatingCurve = new Utilities.NamedAction();
            //ImportRatingCurve.Header = "Import Rating Curve From ASCII";
            //ImportRatingCurve.Action = ImportRatingCurvefromAscii;

            List<Utilities.NamedAction> localActions = new List<Utilities.NamedAction>();
            localActions.Add(addAlternativeAction);
            ////localActions.Add(ImportRatingCurve);

            Actions = localActions;

            //StudyCache.RatingAdded += AddRatingCurveElement;
            //StudyCache.RatingRemoved += RemoveRatingCurveElement;
            //StudyCache.RatingUpdated += UpdateRatingCurveElement;
        }

        public void AddNewAlternative(object arg1, EventArgs arg2)
        {
            Editors.EditorActionManager actionManager = new Editors.EditorActionManager()
                .WithSiblingRules(this);

            List<string> plans = new List<string>();
            for(int i = 0;i<5;i++)
            {
                plans.Add("plan " + i);
            }
           

            CreateNewAlternativeVM vm = new CreateNewAlternativeVM(plans, actionManager);
            string header = "Create Alternative";
            DynamicTabVM tab = new DynamicTabVM(header, vm, "CreateNewAlternative");
            Navigate(tab, false, true);
        }

    }
}
