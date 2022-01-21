//using Functions;
//using Importer;
//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using ViewModel.ImpactArea;
//using ViewModel.Utilities;

//namespace ViewModel.AggregatedStageDamage
//{
//    //Todo: this class still needs validation.
//    public class ImportStageDamageFromFDA1Helper: BaseViewModel
//    {
//        public List<AggregatedStageDamageElement> Elements { get; } = new List<AggregatedStageDamageElement>();
//        public ImportStageDamageFromFDA1Helper()
//        {

//        }

//        public List<ChildElement> ImportStageDamages()
//        {
//            List<AggregatedStageDamageElement> Elements = new List<AggregatedStageDamageElement>();

//        //FdaValidationResult vr = new FdaValidationResult();

//        //read the file
//        //AsyncLogger logger = new AsyncLogger();
//        //AsciiImport importer = new AsciiImport(logger);
//        //importer.ImportAsciiData(path, AsciiImport.ImportOptions.ImportEverything);
//        AggregateDamageFunctionList aggDamageList = GlobalVariables.mp_fdaStudy.GetAggDamgFuncList();

//            //get the curves from the importer
//            IList<AggregateDamageFunction> curves = aggDamageList.GetAggDamageFunctions.Values;
//            //sort the curves by their plan and year.
//            List<List<AggregateDamageFunction>> groupedCurves = curves.GroupBy(curve => new { curve.PlanName, curve.YearName })
//                .Select(group => group.ToList())
//                .ToList();

//            //now create elements from the groups of curves
//            foreach (List<AggregateDamageFunction> funcs in groupedCurves)
//            {
//                AggregatedStageDamageElement stageDamElem = CreateElement(funcs);
//                if(stageDamElem != null)
//                {
//                    Elements.Add(stageDamElem);
//                }
//            }

//            return Elements;
//            //return vr;
//        }

//        private AggregatedStageDamageElement CreateElement(List<AggregateDamageFunction> funcs)
//        {
//            //for the creation date, i am grabbing the creation date from one of the curves

//            List<StageDamageCurve> curves = new List<StageDamageCurve>();
//            foreach (AggregateDamageFunction func in funcs)
//            {
//                SingleDamageFunction totalDamageFunc = func.DamageFunctions[(int)StructureValueType.TOTAL];
//                StageDamageCurve stageDamageCurve = CreateStageDamageCurve(totalDamageFunc, func.DamageReachName, func.CategoryName);
//                if (stageDamageCurve != null)
//                {
//                    curves.Add(stageDamageCurve);
//                }
//            }

//            //todo what if the count is zero
//            string name = funcs[0].PlanName.Trim() + " - " + funcs[0].YearName.Trim();

//            if (curves.Count > 0)
//            {
//                return new AggregatedStageDamageElement(name, funcs[0].CalculationDate, funcs[0].Description, -1, -1, curves, true);
//            }
//            else
//            {
//                return null;
//            }
//        }

//        private StageDamageCurve CreateStageDamageCurve(SingleDamageFunction sdf, string damageReachName, string damCat)
//        {
//            damageReachName = damageReachName.Trim();
//            damCat = damCat.Trim();

//            StageDamageCurve curve = null;
//            double[] depths = sdf.Depth;
//            double[] damages = sdf.Damage;

//            List<double> depthsList = new List<double>();
//            List<double> damagesList = new List<double>();
//            for (int i = 0; i < sdf.GetNumRows(); i++)
//            {
//                depthsList.Add(depths[i]);
//                damagesList.Add(damages[i]);
//            }
//            //always use linear. This is the only option in Old Fda.
//            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(depthsList, damagesList, InterpolationEnum.Linear);
//            //IFdaFunction stageDamage = IFdaFunctionFactory.Factory( IParameterEnum.InteriorStageDamage, (IFunction)func);

//            //there should only ever be 0 or 1 impact area elements
//            List<ImpactAreaElement> impactAreaElements = StudyCache.GetChildElementsOfType<ImpactAreaElement>();
//            if (impactAreaElements.Count > 0)
//            {
//                ObservableCollection<ImpactAreaRowItem> impactAreaRows = ((ImpactAreaElement)impactAreaElements[0]).ImpactAreaRows;
//                ImpactAreaRowItem selectedRow = null;

//                //does this curve's damage reach equal an existing impact area?
//                bool impactAreaMatches = false;
//                foreach (ImpactAreaRowItem row in impactAreaRows)
//                {
//                    //the damage reach name needs to match an existing impact area to be included.
//                    //message user if it does not.
//                    if (row.Name.Equals(damageReachName))
//                    {
//                        impactAreaMatches = true;
//                        curve = new StageDamageCurve(row, damCat, func);
//                        break;
//                    }
//                }
//                if(!impactAreaMatches)
//                {
//                    string msg = "The stage damage curve with damage reach of '" + damageReachName + "' could not be imported because it does not match any existing impact area names.";
//                    MessageBox.Show(msg, "Could Not Import", MessageBoxButton.OK, MessageBoxImage.Information);
//                }
//            }

//            return curve;
//        }

//    }
//}
