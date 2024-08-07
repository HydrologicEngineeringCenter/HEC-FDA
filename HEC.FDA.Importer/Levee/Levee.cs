﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Importer.AsciiImport;

namespace Importer
{
    [Serializable]
    public class Levee : FdObjectDataLook
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
        public void Print(AsyncLogger logger, ImportOptions importOptions = ImportOptions.ImportEverything)
        {
            //Basic Information
            logger.Log("\n\nLevee Name: ", Name);
            logger.Log("\tDescription: ",Description);
            logger.Log("\tPlan: ", PlanName);
            logger.Log("\tYear: ",YearName);
            logger.Log("\tStream: ", StreamName);
            logger.Log("\tReach: ", DamageReachName);
            logger.Log("\tTop of Levee: ", ElevationTopOfLevee.ToString());

            //Interior-Exterior Function
            if (_IntExt.Count > 0 && (importOptions == ImportOptions.ImportEverything || importOptions == ImportOptions.ImportExteriorInterior))
            {
                logger.Log("\n\tInterior-Exterior Function, Number of Points ", _IntExt.Count.ToString());
                logger.Append("\t\tExterior Elev: ");
                for (int i = 0; i < _IntExt.Count; i++)
                    logger.Append($"\t{_IntExt.ElementAt(i).GetX()}");
                logger.Append("\n\t\tInterior Elev: ");
                for (int i = 0; i < _IntExt.Count; i++)
                    logger.Append($"\t{_IntExt.ElementAt(i).GetY()}");
                logger.Append("\n");
            }

            //Geotechnical Function
            if (_GeoTech.Count > 0 && (importOptions == ImportOptions.ImportEverything || importOptions == ImportOptions.ImportLevees))
            {
                logger.Log("\n\tGeotechnical Function, Number of Points ", _GeoTech.Count.ToString());
                logger.Append("\t\tExterior Elev: ");
                for (int i = 0; i < _GeoTech.Count; i++)
                    logger.Append($"\t{_GeoTech.ElementAt(i).GetX()}");
                logger.Append("\n\t\tFailure Probability: ");
                for (int i = 0; i < _GeoTech.Count; i++)
                    logger.Append($"\t{_GeoTech.ElementAt(i).GetY()}");
                logger.Append("\n");
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
        #endregion
    }
}
