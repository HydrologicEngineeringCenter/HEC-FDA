using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
using ViewModel.WaterSurfaceElevation;
using ViewModel.Saving.PersistenceManagers;

namespace Importer
{
    [Serializable]
    public class WaterSurfaceProfile : FdObjectDataLook, ISaveToSqlite
    {
        #region Notes
        // Created By: $username$
        // Created Date: $time$
        #endregion
        #region Fields
        private List<WspSectionData> _WspSectionDataList = new List<WspSectionData>();

        public double[] wspProbs = new double[100];
        #endregion
        #region Properties
        public string PlanName
        { get; set; }
        public string YearName
        { get; set; }
        public string StreamName
        { get; set; }
        public WspDataType WspDataTypeId = WspDataType.DISCHARGE_FREQUENCY;
        public string Notes
        { get; set; }
        public int NumberOfProfiles
        { get; set; }
        public int NumberOfCrossSections
        { get; set; }
        public int NumberOfProfilesAllocated
        { get; set; }
        public bool IsDischarge
        { get; set; }
        public double StationAtCrossSection
        { get; set; }
        public double ElevationOfInvert
        { get; set; }
        #endregion
        #region Constructors
        public WaterSurfaceProfile() : base()
        {
            NumberOfProfilesAllocated = 0;
            NumberOfProfiles = 0;
            IsDischarge = true;
            StationAtCrossSection = ElevationOfInvert = Study.badNumber;
            PlanName = "";
            YearName = "";
            StreamName = "";
            Notes = "";
            return;
        }
        #endregion
        #region Voids
        public new void Reset()
        {
            base.Reset();
            PlanName = YearName = StreamName = "";
            WspDataTypeId = WspDataType.UNKNOWN;
            NumberOfProfiles = NumberOfCrossSections = 0;
            StationAtCrossSection = ElevationOfInvert = Study.badNumber;
            _WspSectionDataList.Clear();
            return;
        }
        public void SetProbabilities(int numProbs, double[] probabilities)
        {
            NumberOfProfiles = numProbs;
            this.wspProbs = new double[numProbs];
            for (int i = 0; i < numProbs; i++)
                this.wspProbs[i] = probabilities[i];
            return;
        }
        public void Print()
        {
            //Basic Information
            WriteLine($"\n\nWater Surface Profile Name: {Name}");
            WriteLine($"\tDescription: {Description}");
            WriteLine($"\tPlan: {PlanName}");
            WriteLine($"\tYear: {YearName}");
            WriteLine($"\tStream: {StreamName}");
            WriteLine($"\tWSP Type: {WspDataTypeId}");
            WriteLine("\tProbabilities: \n\t");
            for (int i = 0; i < NumberOfProfiles; i++)
                Write($"\t{wspProbs[i]}");
            if (WspDataTypeId == WspDataType.DISCHARGE_FREQUENCY)
            {
                Write("\t");
                for (int i = 0; i < NumberOfProfiles; i++)
                    Write($"\t{wspProbs[i]}");
            }
            Write("\n");
            //Data Header
            Write($"WSP_STATION\tINVERT");
            for (int i = 0; i < NumberOfProfiles; i++)
                Write("\tSTAGE");
            if (WspDataTypeId == WspDataType.DISCHARGE_FREQUENCY)
            {
                Write("\t");
                for (int i = 0; i < NumberOfProfiles; i++)
                    Write("\tQ");
            }
            Write("\n");
            //Data
            for (int ix = 0; ix < this._WspSectionDataList.Count; ix++)
            {
                WspSectionData wspSectionData = _WspSectionDataList.ElementAt(ix);
                Write($"{wspSectionData.GetStation()}");
                Write($"\t{wspSectionData.GetInvert()}");
                for (int ip = 0; ip < NumberOfProfiles; ip++)
                    Write($"\t{wspSectionData.GetPointStage(ip)}");
                if (WspDataTypeId == WspDataType.DISCHARGE_FREQUENCY)
                {
                    Write("\t");
                    for (int ip = 0; ip < NumberOfProfiles; ip++)
                        Write($"\t{wspSectionData.GetPointFlow(ip)}");
                }
                Write("\n");
            }
            return;
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            //Basic Information
            wr.WriteLine($"\n\nWater Surface Profile Name: {Name}");
            wr.WriteLine($"\tDescription: {Description}");
            //wr.WriteLine($"\tPlan: {PlanName}");
            wr.WriteLine($"\tPlan: {GlobalVariables.mp_fdaStudy.GetPlanList().getName(IdPlan)}");
            //wr.WriteLine($"\tYear: {YearName}");
            wr.WriteLine($"\tYear: {GlobalVariables.mp_fdaStudy.GetYearList().getName(IdYear)}");
            //wr.WriteLine($"\tStream: {StreamName}");
            wr.WriteLine($"\tStream: {GlobalVariables.mp_fdaStudy.GetStreamList().getName(IdStream)}");
            wr.WriteLine($"\tWSP Type: {WspDataTypeId}");
            wr.WriteLine("\tProbabilities: \n\t");
            for (int i = 0; i < NumberOfProfiles; i++)
                wr.Write($"\t{wspProbs[i]}");
            if (WspDataTypeId == WspDataType.DISCHARGE_FREQUENCY)
            {
                wr.Write("\t");
                for (int i = 0; i < NumberOfProfiles; i++)
                    wr.Write($"\t{wspProbs[i]}");
            }
            wr.Write("\n");
            //Data Header
            wr.Write($"WSP_STATION\tINVERT");
            for (int i = 0; i < NumberOfProfiles; i++)
                wr.Write("\tSTAGE");
            if (WspDataTypeId == WspDataType.DISCHARGE_FREQUENCY)
            {
                wr.Write("\t");
                for (int i = 0; i < NumberOfProfiles; i++)
                    wr.Write("\tQ");
            }
            wr.Write("\n");
            //Data
            for (int ix = 0; ix < this._WspSectionDataList.Count; ix++)
            {
                WspSectionData wspSectionData = _WspSectionDataList.ElementAt(ix);
                wr.Write($"{wspSectionData.GetStation()}");
                wr.Write($"\t{wspSectionData.GetInvert()}");
                for (int ip = 0; ip < NumberOfProfiles; ip++)
                    wr.Write($"\t{wspSectionData.GetPointStage(ip)}");
                if (WspDataTypeId == WspDataType.DISCHARGE_FREQUENCY)
                {
                    wr.Write("\t");
                    for (int ip = 0; ip < NumberOfProfiles; ip++)
                        wr.Write($"\t{wspSectionData.GetPointFlow(ip)}");
                }
                wr.Write("\n");
            }
            return;
        }
        public void Export(StreamWriter wr, char delimt)
        {
            ExportHeader(wr, delimt);
            ExportName(wr, delimt);
            ExportProbability(wr, delimt);
            ExportData(wr, delimt);
            return;
        }
        protected void ExportHeader(StreamWriter wr, char delimt)
        {
            for (int i = 0; i < AsciiImportExport.FieldsWsp.Length; i++)
                wr.Write($"{AsciiImportExport.FieldsWsp[i]}{delimt}{delimt}");
            wr.Write("\n");
            return;

        }
        protected void ExportName(StreamWriter wr, char delimt)
        {
            wr.Write($"{this.Name}{delimt}{delimt}");
            wr.Write($"{this.Description}{delimt}{delimt}");
            wr.Write($"{this.PlanName}{delimt}{delimt}");
            wr.Write($"{this.YearName}{delimt}{delimt}");
            wr.Write($"{this.StreamName}{delimt}{delimt}");
            wr.Write($"{this.WspDataTypeId}{delimt}{delimt}");
            wr.Write($"{this.Notes}\n");
            return;
        }
        protected void ExportProbability(StreamWriter wr, char delimt)
        {
            wr.WriteLine("WSP_PROBABILITY");
            wr.Write($"{delimt}");
            for (int i = 0; i < NumberOfProfiles; i++)
                wr.Write($"{delimt}{wspProbs[i]}");
            if (WspDataTypeId == WspDataType.DISCHARGE_FREQUENCY)
            {
                wr.Write($"{delimt}");
                for (int i = 0; i < NumberOfProfiles; i++)
                    wr.Write($"{delimt}{wspProbs[i]}");
            }
            wr.Write("\n");
            return;
        }
        protected void ExportData(StreamWriter wr, char delimt)
        {
            //Data Header
            wr.Write($"WSP_STATION{delimt}INVERT");
            for (int i = 0; i < NumberOfProfiles; i++)
                wr.Write($"{delimt}STAGE");
            if (WspDataTypeId == WspDataType.DISCHARGE_FREQUENCY)
            {
                wr.Write($"{delimt}");
                for (int i = 0; i < NumberOfProfiles; i++)
                    wr.Write($"{delimt}Q");
            }
            wr.Write("\n");
            //Data
            for (int ix = 0; ix < this._WspSectionDataList.Count; ix++)
            {
                WspSectionData wspSectionData = _WspSectionDataList.ElementAt(ix);
                wr.Write($"{wspSectionData.GetStation()}");
                wr.Write($"{delimt}{wspSectionData.GetInvert()}");
                for (int ip = 0; ip < NumberOfProfiles; ip++)
                    wr.Write($"{delimt}{wspSectionData.GetPointStage(ip)}");
                if (WspDataTypeId == WspDataType.DISCHARGE_FREQUENCY)
                {
                    wr.Write($"{delimt}");
                    for (int ip = 0; ip < NumberOfProfiles; ip++)
                        wr.Write($"{delimt}{wspSectionData.GetPointFlow(ip)}");
                }
                wr.Write("\n");
            }
            return;
        }
        #endregion
        #region Functions
        public int AddWspSectionData(WspSectionData wspSectionData)
        {
            WspSectionData aWspSectData = ObjectCopier.Clone(wspSectionData);
            _WspSectionDataList.Add(wspSectionData);
            return _WspSectionDataList.Count;
        }
        public int AddWspSectionData(int numCrossSection, int numProfiles, WspDataType theType, double[,] theDataMatrix)
        {
            int js = 0;
            int jq = 0;

            _WspSectionDataList.Clear();

            WspSectionData aWspSectData = null;
            int numMatrixValues = 2 + numProfiles;
            if (theType == WspDataType.DISCHARGE_FREQUENCY)
                numMatrixValues = 2 + 2 * numProfiles;

            for(int ixSect = 0; ixSect < numCrossSection; ixSect++)
            {
                aWspSectData = new WspSectionData(numProfiles);

                for (int ixProfile = 0; ixProfile < numProfiles; ixProfile++)
                {

                    aWspSectData.SetStation(theDataMatrix[ixSect, 0]);
                    aWspSectData.SetInvert(theDataMatrix[ixSect, 1]);

                    js = 2 + ixProfile;
                    jq = 2 + numProfiles + ixProfile;

                    if (theType == WspDataType.DISCHARGE_FREQUENCY)
                        aWspSectData.SetPoint(ixProfile, theDataMatrix[ixSect, js], theDataMatrix[ixSect, jq]);
                    else
                        aWspSectData.SetPoint(ixProfile, theDataMatrix[ixSect, 2 + ixProfile], Study.badNumber);
                }
                _WspSectionDataList.Add(aWspSectData);
            }

            return _WspSectionDataList.Count;
        }

        public void SaveToSqlite()
        {
            //path and probability
            List<double> probs = new List<double>();
            for(int i = 0;i<NumberOfProfiles; i++)
            {
                probs.Add(wspProbs[i]);
            }

            WaterSurfaceElevationElement elem = new WaterSurfaceElevationElement(Name, Description, probs, true);
            WaterSurfaceAreaPersistenceManager manager = ViewModel.Saving.PersistenceFactory.GetWaterSurfaceManager();
            manager.SaveNew(elem);
        }

        public WaterSurfaceElevationElement ConvertToFDA2()
        {
            //path and probability
            List<double> probs = new List<double>();
            for (int i = 0; i < NumberOfProfiles; i++)
            {
                probs.Add(wspProbs[i]);
            }
            return new WaterSurfaceElevationElement(Name, Description, probs, false);
        }

        #endregion
    }
}

