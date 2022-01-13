using Functions;
using Importer;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModel.Editors;
using ViewModel.Utilities;

namespace ViewModel.StageTransforms
{
    public class ImportRatingsFromFDA1VM : ImportFromFDA1VM
    {
        private List<RatingCurveElement> _Elements = new List<RatingCurveElement>();
        public ImportRatingsFromFDA1VM(EditorActionManager manager):base(manager)
        {

        }
        public override void SaveElements()
        {
            Saving.PersistenceManagers.RatingElementPersistenceManager manager = Saving.PersistenceFactory.GetRatingManager();
            foreach(RatingCurveElement elem in _Elements)
            {
                manager.SaveNew(elem);
            }
        }

        public override FdaValidationResult Validate()
        {
            throw new NotFiniteNumberException();
            AsciiImport import = new AsciiImport();
            import.ImportAsciiData(Path, AsciiImport.ImportOptions.ImportEverything);
            RatingFunctionList ratings = GlobalVariables.mp_fdaStudy.GetRatingFunctionList();
            foreach (KeyValuePair<string, RatingFunction> rat in ratings.RatingFunctions)
            {
                RatingCurveElement elem = CreateElement(rat.Value);
                if(elem != null)
                {
                    _Elements.Add(elem);
                }
            }

            //validate the names


        }

        private RatingCurveElement CreateElement(RatingFunction rat)
        {
            string pysr = "(" + rat.PlanName + " " + rat.YearName + " " + rat.StreamName + " " + rat.DamageReachName + ") ";
            string description = pysr + rat.Description;
            double[] stages = rat.GetStage();
            double[] flows = rat.GetDischarge();
            //these arrays might have a bunch of "Study.badNumber" (-901). I need to get rid of them by only grabbing the correct number of points.
            List<double> stagesList = new List<double>();
            List<double> flowsList = new List<double>();
            for (int i = 0; i < rat.NumberOfPoints; i++)
            {
                stagesList.Add(stages[i]);
                flowsList.Add(flows[i]);
            }
            //always use linear. This is the only option in Old Fda.
            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(stagesList, flowsList, InterpolationEnum.Linear);
            IFdaFunction rating = IFdaFunctionFactory.Factory(IParameterEnum.Rating, (IFunction)func);
            //add the plan year stream reach for the description
            RatingCurveElement elem = new RatingCurveElement(rat.Name, rat.CalculationDate, description, rating);
            return elem;
        }

    }
}
