using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using Functions;
using FdaViewModel.GeoTech;
using FdaViewModel.StageTransforms;
using Model;

namespace Importer
{
    [Serializable]
    public class Levee : FdObjectDataLook, ISaveToSqlite
    {
        #region Notes
        // Created By: $username$
        // Created Date: $time$
        #endregion
        #region Fields
        private List<Pair_xy> _IntExt = new List<Pair_xy>();
        private List<Pair_xy> _GeoTech = new List<Pair_xy>();
        #endregion
        #region Properties
        public List<Pair_xy> ExteriorInteriorPairs
        {
            get { return _IntExt; }
        }
        public List<Pair_xy> FailureFunctionPairs
        {
            get { return _GeoTech; }
        }
        public double ElevationTopOfLevee
        { get; set; }
        public int NumOrdsIntExt { get; set; }
        public int NumOrdsGeoTech { get; set; }
        public int NumOrdsWave { get; set; }
        public int NumOrdsShape { get; set; }
        public int NumOrdsInt { get; set; }
        #endregion
        #region Constructors
        public Levee()
        {
            _IntExt = new List<Pair_xy>();
            _GeoTech = new List<Pair_xy>();
        }
        #endregion
        #region Voids
        public new void Reset()
        {
            base.Reset();
            ElevationTopOfLevee = Study.badNumber;
            _IntExt.Clear();
            _GeoTech.Clear();
        }
        public void SetIntExtExterior(int numPoints, double[] elevExt)
        {
            //Get Interior
            double[] interior = null;
            if (this._IntExt.Count > 0)
            {
                interior = new double[this._IntExt.Count];
                for (int i = 0; i < this._IntExt.Count; i++)
                    interior[i] = this._IntExt.ElementAt(i).GetY();
            }
            //Interior doesn't exist
            else
            {
                interior = new double[numPoints];
                for (int i = 0; i < numPoints; i++)
                    interior[i] = Study.badNumber;
            }
            //Transfer Exterior and add to interior-exterior function
            this._IntExt.Clear();
            for (int i = 0; i < numPoints; i++)
            {
                Pair_xy xy = new Pair_xy(elevExt[i], interior[i]);
                this._IntExt.Add(xy);
            }
            return;
        }
        public void SetIntExtInterior(int numPoints, double[] elevInt)
        {
            //Get Exterior
            double[] exterior = null;
            if (this._IntExt.Count > 0)
            {
                exterior = new double[this._IntExt.Count];
                for (int i = 0; i < this._IntExt.Count; i++)
                    exterior[i] = this._IntExt.ElementAt(i).GetX();
            }
            //Interior doesn't exist
            else
            {
                exterior = new double[numPoints];
                for (int i = 0; i < numPoints; i++)
                    exterior[i] = Study.badNumber;
            }
            //Transfer Exterior and add to interior-exterior function
            this._IntExt.Clear();
            for (int i = 0; i < numPoints; i++)
            {
                Pair_xy xy = new Pair_xy(exterior[i], elevInt[i]);
                this._IntExt.Add(xy);
            }
            return;
        }
        public void SetIntExt(int numPoints, double[] elevExt, double[] elevInt)
        {
            _IntExt.Clear();
            for(int i = 0; i < numPoints; i++)
            {
                Pair_xy xy = new Pair_xy(elevExt[i], elevInt[i]);
                _IntExt.Add(xy);
            }
            return;
        }
        public void SetGeoTechElev(int numPoints, double[] elev)
        {
            //Get Probability
            double[] probability = null;
            if (this._GeoTech.Count > 0)
            {
                probability = new double[this._GeoTech.Count];
                for (int i = 0; i < this._GeoTech.Count; i++)
                    probability[i] = this._GeoTech.ElementAt(i).GetY();
            }
            //Probability doesn't exist
            else
            {
                probability = new double[numPoints];
                for (int i = 0; i < numPoints; i++)
                    probability[i] = Study.badNumber;
            }
            //Transfer elevation and add to Geotechnical Function function
            this._GeoTech.Clear();
            for (int i = 0; i < numPoints; i++)
            {
                Pair_xy xy = new Pair_xy(elev[i], probability[i]);
                this._GeoTech.Add(xy);
            }
            return;
        }
        public void SetGeoTechProb(int numPoints, double[] prob)
        {
            //Get Elevation
            double[] elevation = null;
            if (this._GeoTech.Count > 0)
            {
                elevation = new double[this._GeoTech.Count];
                for (int i = 0; i < this._GeoTech.Count; i++)
                    elevation[i] = this._GeoTech.ElementAt(i).GetX();
            }
            //Elevation doesn't exist
            else
            {
                elevation = new double[numPoints];
                for (int i = 0; i < numPoints; i++)
                    elevation[i] = Study.badNumber;
            }
            //Transfer Probability and add to Geotechnical function
            this._GeoTech.Clear();
            for (int i = 0; i < numPoints; i++)
            {
                Pair_xy xy = new Pair_xy(elevation[i], prob[i]);
                this._GeoTech.Add(xy);
            }
            return;
        }
        public void SetGeoTech(int numPoints, double[] elevation, double[] probability)
        {
            _GeoTech.Clear();
            for(int i = 0; i < numPoints; i++)
            {
                Pair_xy xy = new Pair_xy(elevation[i], probability[i]);
                _GeoTech.Add(xy);
            }
            return;
        }
        public void Print()
        {
            //Basic Information
            WriteLine($"\n\nLevee Name: {Name}");
            WriteLine($"\tDescription: {Description}");
            WriteLine($"\tPlan: {PlanName}");
            WriteLine($"\tYear: {YearName}");
            WriteLine($"\tStream: {StreamName}");
            WriteLine($"\tReach: {DamageReachName}");
            WriteLine($"\tTop of Levee: {ElevationTopOfLevee}");

            //Interior-Exterior Function
            if (_IntExt.Count > 0)
            {
                WriteLine($"\n\tInterior-Exterior Function, Number of Points {_IntExt.Count}");
                Write("\t\tExterior Elev: ");
                for (int i = 0; i < _IntExt.Count; i++)
                    Write($"\t{_IntExt.ElementAt(i).GetX()}");
                Write("\n\t\tInterior Elev: ");
                for (int i = 0; i < _IntExt.Count; i++)
                    Write($"\t{_IntExt.ElementAt(i).GetY()}");
                Write("\n");
            }

            //Geotechnical Function
            if (_GeoTech.Count > 0)
            {
                WriteLine($"\n\tGeotechnical Function, Number of Points {_GeoTech.Count}");
                Write("\t\tExterior Elev: ");
                for (int i = 0; i < _GeoTech.Count; i++)
                    Write($"\t{_GeoTech.ElementAt(i).GetX()}");
                Write("\n\t\tFailure Probability: ");
                for (int i = 0; i < _GeoTech.Count; i++)
                    Write($"\t{_GeoTech.ElementAt(i).GetY()}");
                Write("\n");
            }

            return;
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            //Basic Information
            wr.WriteLine($"\n\nLevee Name: {Name}");
            wr.WriteLine($"\tDescription: {Description}");
            wr.WriteLine($"\tPlan: {PlanName}");
            wr.WriteLine($"\tYear: {YearName}");
            wr.WriteLine($"\tStream: {StreamName}");
            wr.WriteLine($"\tReach: {DamageReachName}");
            wr.WriteLine($"\tTop of Levee: {ElevationTopOfLevee}");

            //Interior-Exterior Function
            if (_IntExt.Count > 0)
            {
                wr.WriteLine($"\n\tInterior-Exterior Function, Number of Points {_IntExt.Count}");
                wr.Write("\t\tExterior Elev: ");
                for (int i = 0; i < _IntExt.Count; i++)
                    wr.Write($"\t{_IntExt.ElementAt(i).GetX()}");
                wr.Write("\n\t\tInterior Elev: ");
                for (int i = 0; i < _IntExt.Count; i++)
                    wr.Write($"\t{_IntExt.ElementAt(i).GetY()}");
                wr.Write("\n");
            }

            //Geotechnical Function
            if (_GeoTech.Count > 0)
            {
                wr.WriteLine($"\n\tGeotechnical Function, Number of Points {_GeoTech.Count}");
                wr.Write("\t\tExterior Elev: ");
                for (int i = 0; i < _GeoTech.Count; i++)
                    wr.Write($"\t{_GeoTech.ElementAt(i).GetX()}");
                wr.Write("\n\t\tFailure Probability: ");
                for (int i = 0; i < _GeoTech.Count; i++)
                    wr.Write($"\t{_GeoTech.ElementAt(i).GetY()}");
                wr.Write("\n");
            }

            return;
        }
        public void Export(StreamWriter wr, char delimt)
        {
            ExportHeader(wr, delimt);
            ExportName(wr, delimt);
            ExportData(wr, delimt);
            return;
        }
        protected void ExportHeader(StreamWriter wr, char delimt)
        {
            for (int i = 0; i < AsciiImportExport.FieldsLevee.Length; i++)
                wr.Write($"{AsciiImportExport.FieldsLevee[i]}{delimt}{delimt}");
            wr.Write("\n");
            return;
        }
        protected void ExportName(StreamWriter wr, char delimt)
        {
            wr.Write($"{this.Name}{delimt}{delimt}");
            wr.Write($"{this.PlanName}{delimt}{delimt}");
            wr.Write($"{this.YearName}{delimt}{delimt}");
            wr.Write($"{this.StreamName}{delimt}{delimt}");
            wr.Write($"{this.DamageReachName}{delimt}{delimt}");
            wr.Write($"{this.ElevationTopOfLevee}{delimt}{delimt}");
            wr.Write($"{this.Description}");
            wr.Write("\n");
            return;
        }
        protected void ExportData(StreamWriter wr, char delimt)
        {
            if (_IntExt.Count > 0)
            {
                wr.Write($"{delimt}{delimt}SE");
                for (int i = 0; i < _IntExt.Count; i++)
                    wr.Write($"{delimt}{_IntExt.ElementAt(i).GetX()}");
                wr.Write($"\n{delimt}{delimt}SI");
                for (int i = 0; i < _IntExt.Count; i++)
                    wr.Write($"{delimt}{_IntExt.ElementAt(i).GetY()}");
                wr.Write("\n");

            }
            if (_GeoTech.Count > 0)
            {
                wr.Write($"{delimt}{delimt}GSE");
                for (int i = 0; i < _GeoTech.Count; i++)
                    wr.Write($"{delimt}{_GeoTech.ElementAt(i).GetX()}");
                wr.Write($"\n{delimt}{delimt}GPF");
                for (int i = 0; i < _GeoTech.Count; i++)
                    wr.Write($"{delimt}{_GeoTech.ElementAt(i).GetY()}");
                wr.Write("\n");
            }
            return;
        }

        public void SaveToSqlite()
        {
            if (FailureFunctionPairs.Count > 0)
            {
                SaveFailureFunction();
            }
            if (ExteriorInteriorPairs.Count > 0)
            {
                SaveExtIntFunction();
            }
        }

        private void SaveExtIntFunction()
        {
            List<ICoordinate> extIntCoords = new List<ICoordinate>();
            foreach (Pair_xy xy in ExteriorInteriorPairs)
            {
                double x = xy.GetX();
                double y = xy.GetY();
                extIntCoords.Add(ICoordinateFactory.Factory(x, y));
            }
            ICoordinatesFunction coordsFunction = ICoordinatesFunctionsFactory.Factory(extIntCoords, InterpolationEnum.Linear);
            IFunction function = IFunctionFactory.Factory(coordsFunction.Coordinates, coordsFunction.Interpolator);
            Model.IFdaFunction func = IFdaFunctionFactory.Factory( IParameterEnum.ExteriorInteriorStage, function);
            string editDate = DateTime.Now.ToString("G");
            ExteriorInteriorElement elem = new ExteriorInteriorElement(Name, editDate, Description, func);
            FdaViewModel.Saving.PersistenceFactory.GetExteriorInteriorManager().SaveNewElement(elem);
        }

        private void SaveFailureFunction()
        {
            List<ICoordinate> failureCoords = new List<ICoordinate>();
            foreach (Pair_xy xy in FailureFunctionPairs)
            {
                double x = xy.GetX();
                double y = xy.GetY();
                failureCoords.Add(ICoordinateFactory.Factory(x, y));
            }

            ICoordinatesFunction coordsFunction = ICoordinatesFunctionsFactory.Factory(failureCoords, InterpolationEnum.Linear);
            IFunction function = IFunctionFactory.Factory(coordsFunction.Coordinates, coordsFunction.Interpolator);
            IFdaFunction func = IFdaFunctionFactory.Factory( IParameterEnum.LateralStructureFailure, function);
            string editDate = DateTime.Now.ToString("G");
           // FailureFunctionElement elem = new FailureFunctionElement(Name, editDate, Description, func, leveeFeatureElement);
            //FdaViewModel.Saving.PersistenceFactory.GetFailureFunctionManager().SaveNewElement(elem);
            LeveeFeatureElement leveeFeatureElement = new LeveeFeatureElement(Name,editDate, Description, ElevationTopOfLevee, false,  func);
        }

        #endregion
        #region Functions
        #endregion
    }
}
