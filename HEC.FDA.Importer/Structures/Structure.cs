using System;
using System.Collections.Generic;
using System.IO;
using static System.Console;

namespace Importer
{
    [Serializable]
    public class Structure : FdObjectData
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region enums
        public enum ElevationValue { FIRST_FLOOR, GROUND, DELTAG, DELTAZ, AUTODIFF };
        #endregion
        #region Fields
        public double[] ElevationsStructure = new double[5];
        #endregion
        #region Properties
        public string YearInService
        { get; set; }
        public int YearInServiceInt { get; set; }
        public string CategoryName
        { get; set; }
        public int CategoryId { get; set; }
        public string StreamName
        { get; set; }
        public int StreamId { get; set; }
        public StreamBank BankOfStream
        { get; set; }
        public double StationAtStructure
        { get; set; }
        public string SidReachName
        { get; set; }
        public int SidReachId { get; set; }
        public int CalculatedReachId { get; set; }
        public string StructureModuleName
        { get; set; }
        public int StructureModuleId { get; set; }
        public bool UsesFirstFloorElev
        { get; set; }
        public string DamageFunctionName
        { get; set; }
        public int DamageFunctionId { get; set; }
        public double ValueOfStructure
        { get; set; }
        public double ValueOfContent
        { get; set; }
        public double ValueOfOther
        { get; set; }
        public double ValueOfCar
        { get; set; }
        public string StreetName
        { get; set; }
        public string CityName
        { get; set; }
        public string StateCode
        { get; set; }
        public string ZipCode
        { get; set; }
        public double NorthingCoordinate
        { get; set; }
        public double EastingCoordinate
        { get; set; }
        public double ZoneCoordinate
        { get; set; }
        public int NumberOfStructures
        { get; set; }
        public string Notes   //use meta data instead
        { get; set; }
        public string ImageFilename
        { get; set; }
        public int NumberOfCars
        { get; set; }
        public string ParcelNumber
        { get; set; }
        #endregion
        #region Constructors
        public Structure()
        {
            Reset();
        }
        #endregion
        #region Voids
        public new void Reset()
        {
            base.Reset();
            YearInService = "1900";
            CategoryName = "";
            StreamName = "";
            BankOfStream = StreamBank.LEFT;
            StationAtStructure = Study.badNumber;
            SidReachName = "";
            StructureModuleName = "BASE";
            UsesFirstFloorElev = true;
            for (int i = 0; i < 5; i++)
                ElevationsStructure[i] = Study.badNumber;
            DamageFunctionName = "";
            ValueOfStructure = Study.badNumber;
            ValueOfContent = Study.badNumber;
            ValueOfOther = Study.badNumber;
            ValueOfCar = Study.badNumber;
            StreetName = "";
            CityName = "";
            StateCode = "";
            ZipCode = "";
            NorthingCoordinate = Study.badNumber;
            EastingCoordinate = Study.badNumber;
            ZoneCoordinate = Study.badNumber;
            NumberOfStructures = 1;
            Notes = "";
            ImageFilename = "";
            NumberOfCars = 0;
            ParcelNumber = "";
        }
        public void Print()
        {
            WriteLine($"\nStructure Name: {this.Name}");
            WriteLine($"\tDescription: {this.Description}");
            WriteLine($"\tIn service year: {YearInService}");
            WriteLine($"\tDamage Category: {CategoryName}");
            WriteLine($"\tStream Name: {StreamName}");
            WriteLine($"\tStream Bank: {BankOfStream}");
            WriteLine($"\tStream Station: {StationAtStructure}");
            WriteLine($"\tSID Reach: {SidReachName}");
            WriteLine($"\tStructure Module: {StructureModuleName}");
            WriteLine($"\tUses First Floor Elevation: {UsesFirstFloorElev}");
            WriteLine($"\tFirst Floor Elev: {ElevationsStructure[(int)ElevationValue.FIRST_FLOOR]}");
            WriteLine($"\tGround Elev: {ElevationsStructure[(int)ElevationValue.GROUND]}");
            WriteLine($"\tFoundation Height: {ElevationsStructure[(int)ElevationValue.DELTAG]}");
            WriteLine($"\tBeginning Damage Depth: {ElevationsStructure[(int)ElevationValue.DELTAZ]}");
            WriteLine($"\tAutomobile Depth: {ElevationsStructure[(int)ElevationValue.AUTODIFF]}");
            WriteLine($"\tDamage Function: {DamageFunctionName}");
            WriteLine($"\tStructure Value: {ValueOfStructure}");
            WriteLine($"\tContent Value: {ValueOfContent}");
            WriteLine($"\tOther Value: {ValueOfOther}");
            WriteLine($"\tAutomobile Value: {ValueOfCar}");
            WriteLine($"\tStreet: {StreetName}");
            WriteLine($"\tCity: {CityName}");
            WriteLine($"\tState: {StateCode}");
            WriteLine($"\tZip: {ZipCode}");
            WriteLine($"\tNorthing: {NorthingCoordinate}");
            WriteLine($"\tEasting: {EastingCoordinate}");
            WriteLine($"\tZone: {ZoneCoordinate}");
            WriteLine($"\tNumber of Structures: {NumberOfStructures}");
            WriteLine($"\tNotes: {Notes}");
            WriteLine($"\tImage: {ImageFilename}");
            WriteLine($"\tNumber of Automobiles: {NumberOfCars}");
            WriteLine($"\tParcel Number: {ParcelNumber}");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;


            wr.WriteLine($"\nStructure Name: {this.Name}");
            wr.WriteLine($"\tDescription: {this.Description}");
            wr.WriteLine($"\tIn service year: {YearInService}");
            //wr.WriteLine($"\tDamage Category: {CategoryName}");
            wr.WriteLine($"\tDamage Category: {GlobalVariables.mp_fdaStudy.GetDamageCategoryList().getName(CategoryId)}");
            //wr.WriteLine($"\tStream Name: {StreamName}");
            wr.WriteLine($"\tStream Name: {GlobalVariables.mp_fdaStudy.GetStreamList().getName(StreamId)}");
            wr.WriteLine($"\tStream Bank: {BankOfStream}");
            wr.WriteLine($"\tStream Station: {StationAtStructure}");
            //wr.WriteLine($"\tSID Reach: {SidReachName}");
            wr.WriteLine($"\tSID Reach: {GlobalVariables.mp_fdaStudy.GetDamageReachList().getName(SidReachId)}");
            //wr.WriteLine($"\tStructure Module: {StructureModuleName}");
            wr.WriteLine($"\tStructure Module: {GlobalVariables.mp_fdaStudy.GetStructureModuleList().getName(StructureModuleId)}");
            wr.WriteLine($"\tUses First Floor Elevation: {UsesFirstFloorElev}");
            wr.WriteLine($"\tFirst Floor Elev: {ElevationsStructure[(int)ElevationValue.FIRST_FLOOR]}");
            wr.WriteLine($"\tGround Elev: {ElevationsStructure[(int)ElevationValue.GROUND]}");
            wr.WriteLine($"\tFoundation Height: {ElevationsStructure[(int)ElevationValue.DELTAG]}");
            wr.WriteLine($"\tBeginning Damage Depth: {ElevationsStructure[(int)ElevationValue.DELTAZ]}");
            wr.WriteLine($"\tAutomobile Depth: {ElevationsStructure[(int)ElevationValue.AUTODIFF]}");
            wr.WriteLine($"\tDamage Function: {DamageFunctionName}");
            wr.WriteLine($"\tStructure Value: {ValueOfStructure}");
            wr.WriteLine($"\tContent Value: {ValueOfContent}");
            wr.WriteLine($"\tOther Value: {ValueOfOther}");
            wr.WriteLine($"\tAutomobile Value: {ValueOfCar}");
            wr.WriteLine($"\tStreet: {StreetName}");
            wr.WriteLine($"\tCity: {CityName}");
            wr.WriteLine($"\tState: {StateCode}");
            wr.WriteLine($"\tZip: {ZipCode}");
            wr.WriteLine($"\tNorthing: {NorthingCoordinate}");
            wr.WriteLine($"\tEasting: {EastingCoordinate}");
            wr.WriteLine($"\tZone: {ZoneCoordinate}");
            wr.WriteLine($"\tNumber of Structures: {NumberOfStructures}");
            wr.WriteLine($"\tNotes: {Notes}");
            wr.WriteLine($"\tImage: {ImageFilename}");
            wr.WriteLine($"\tNumber of Automobiles: {NumberOfCars}");
            wr.WriteLine($"\tParcel Number: {ParcelNumber}");
        }

        public static explicit operator Structure(KeyValuePair<string, Structure> v)
        {
            throw new NotImplementedException();
        }

        public void ExportHeader(StreamWriter wr, char delimt)
        {
            //fieldsStructures
            for (int i = 0; i < AsciiImportExport.FieldsStructure.Length; i++)
            {
                wr.Write($"{ AsciiImportExport.FieldsStructure[i]}{ delimt}");
            }
            wr.Write("\n");
        }
        public void Export(StreamWriter wr, char delimt)
        {
            wr.Write($"{this.Name}{delimt}");
            wr.Write($"{this.Description}{delimt}");
            wr.Write($"{this.CalculationDate}{delimt}");
            wr.Write($"{this.CategoryName}{delimt}");
            wr.Write($"{this.StreamName}{delimt}");
            wr.Write($"{this.DamageFunctionName}{delimt}");
            wr.Write($"{this.StationAtStructure}{delimt}");
            wr.Write($"{this.BankOfStream}{delimt}");
            wr.Write($"{this.YearInService}{delimt}");
            wr.Write($"{this.ValueOfStructure}{delimt}");
            wr.Write($"{this.ValueOfContent}{delimt}");
            wr.Write($"{this.ValueOfOther}{delimt}");
            wr.Write($"{this.ValueOfCar}{delimt}");
            if (this.UsesFirstFloorElev)
            {
                wr.Write($"1{delimt}");
            }
            else
            {
                wr.Write($"0{delimt}");
            }

            wr.Write($"{this.ElevationsStructure[(int)ElevationValue.FIRST_FLOOR]}{delimt}");
            wr.Write($"{this.ElevationsStructure[(int)ElevationValue.GROUND]}{delimt}");
            wr.Write($"{this.ElevationsStructure[(int)ElevationValue.DELTAG]}{delimt}");
            wr.Write($"{this.ElevationsStructure[(int)ElevationValue.DELTAZ]}{delimt}");
            wr.Write($"{this.ElevationsStructure[(int)ElevationValue.AUTODIFF]}{delimt}");
            wr.Write($"{this.StreetName}{delimt}");
            wr.Write($"{this.CityName}{delimt}");
            wr.Write($"{this.StateCode}{delimt}");
            wr.Write($"{this.ZipCode}{delimt}");
            wr.Write($"{this.NorthingCoordinate}{delimt}");
            wr.Write($"{this.EastingCoordinate}{delimt}");
            wr.Write($"{this.ZoneCoordinate}{delimt}");
            wr.Write($"{this.NumberOfStructures}{delimt}");
            wr.Write($"{this.Notes}{delimt}");
            wr.Write($"{this.StructureModuleName}{delimt}");
            wr.Write($"{this.SidReachName}{delimt}");
            wr.Write($"{this.ImageFilename}{delimt}");
            wr.Write($"{this.NumberOfCars}{delimt}");
            wr.Write($"{this.ParcelNumber}{delimt}");
            wr.Write("\n");
        }
        #endregion
    }
}
