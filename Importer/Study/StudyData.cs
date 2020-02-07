using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.IO;

namespace Importer
{
    public class StudyData
    {
        #region Properties
        public string VersionDatabase
        { get; set; }
        public string StudyName
        { get; set; }
        public string StudyDescription
        { get; set; }
        public string StudyNotes
        { get; set; }
        public int StudySystemUnits
        { get; set; }
        public string StudyMonetaryUnits
        { get; set; }
        public int StudyProjectLife
        { get; set; }
        public double StudyDiscountRate
        { get; set; }
        public int StudyPriceYear
        { get; set; }
        public double StudyPriceFactor
        { get; set; }
        public int StudyBaseYear
        { get; set; }
        public double StudyBasePriceFactor
        { get; set; }
        public int StudyIdWithoutPlan
        { get; set; }
        #endregion
        #region Voids
        public void Print()
        {
            //Basic Information
            WriteLine($"\n\nStudy Data:");
            WriteLine($"\tDescription: {this.VersionDatabase}");
            WriteLine($"\tStudy Name: {this.StudyName}");
            WriteLine($"\tStudy Description: {StudyDescription}");
            WriteLine($"\tStudy Notes: {StudyNotes}");
            WriteLine($"\tStudy System Units: {StudySystemUnits}");
            WriteLine($"\tStudy Monetary Units: {StudyMonetaryUnits}");
            WriteLine($"\tStudy Discount Rate: {StudyDiscountRate}");
            WriteLine($"\tStudy Price Year: {StudyPriceYear}");
            WriteLine($"\tStudy Price Factor: {StudyPriceFactor}");
            WriteLine($"\tStudy Base Year: {StudyBaseYear}");
            WriteLine($"\tStudy Base Price Factor: {StudyBasePriceFactor}");
            WriteLine($"\tStudy ID of Without Plan: {StudyIdWithoutPlan}");
            Write("\n");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            //Basic Information
            wr.WriteLine($"\n\nStudy Data:");
            wr.WriteLine($"\tDescription: {this.VersionDatabase}");
            wr.WriteLine($"\tStudy Name: {this.StudyName}");
            wr.WriteLine($"\tStudy Description: {StudyDescription}");
            wr.WriteLine($"\tStudy Notes: {StudyNotes}");
            wr.WriteLine($"\tStudy System Units: {StudySystemUnits}");
            wr.WriteLine($"\tStudy Monetary Units: {StudyMonetaryUnits}");
            wr.WriteLine($"\tStudy Discount Rate: {StudyDiscountRate}");
            wr.WriteLine($"\tStudy Price Year: {StudyPriceYear}");
            wr.WriteLine($"\tStudy Price Factor: {StudyPriceFactor}");
            wr.WriteLine($"\tStudy Base Year: {StudyBaseYear}");
            wr.WriteLine($"\tStudy Base Price Factor: {StudyBasePriceFactor}");
            wr.WriteLine($"\tStudy ID of Without Plan: {StudyIdWithoutPlan}");
            Write("\n");
        }
        #endregion


    }
}
