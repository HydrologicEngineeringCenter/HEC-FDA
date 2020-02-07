using System;
using System.Collections.Generic;
using System.Text;
using static System.Console;
using System.IO;

namespace Importer
{
    public class EconGlobalParameters
    {
        public long NumOfSimulations { get; set; }
        public int NumOfStageDamageOrdsMinimum { get; set; }
        public int NumOfStageDamageOrdsMaximum { get; set; }
        public int TraceLevelStageDamage { get; set; }
        public int TraceStageDamageAppend { get; set; }
        public int TraceStrucDetail { get; set; }
        public int TraceStrucDetailAppend { get; set; }
        public int TraceStdDev { get; set; }
        public int TraceStdDevAppend { get; set; }
        public int Trace { get; set; }
        public int TraceSTD { get; set; }
        public string Separators { get; set; }
        public int UpdateImport { get; set; }
        public int FL_ADDUSTR { get; set; }
        public int ExportAll { get; set; }
        public int ExportPlan { get; set; }
        public int ExportYear { get; set; }
        public int ExportStream { get; set; }
        public int ExportReach { get; set; }
        public int ExportCategory { get; set; }
        public int ExportOccType { get; set; }
        public int ExportModule { get; set; }
        public int ExportStructure { get; set; }
        public int ExportWsp { get; set; }
        public int ExportProbFunc { get; set; }
        public int ExportRateFunc { get; set; }
        public int ExportLevee { get; set; }
        public int ExportStageDamage { get; set; }
        public int TraceLevelEad { get; set; }
        public int NumEadSimulationsMax { get; set; }
        public int NumEadSimulationsMin { get; set; }
        public int UseNewOrderStats { get; set; }
        public int UseMatchPoint { get; set; }
        public double ProbMatchLow { get; set; }
        public double ProbMatchHigh { get; set; }
        public int ComputePopulation { get; set; }
        public int UpdateIAID { get; set; }
        public int STRCSD { get; set; }
        public int STRCPD { get; set; }
        public int TRUNCLV { get; set; }
        public int AUTOSN { get; set; }
        public int StoEadResults { get; set; }
        public int StoQfResults { get; set; }
        public int StoQsResults { get; set; }
        public int StoSdResults { get; set; }
        public int StoFdResults { get; set; }
        public int StoSfResults { get; set; }
        public int StoNoDSS { get; set; }
        public int StoLimcl { get; set; }
        public int StoOrder { get; set; }
        public int StoInputFunc { get; set; }
        public int StoEadDistribution { get; set; }
        public int StoEadCategory { get; set; }
        public int StoAepDistribution { get; set; }
        public int StoEadInput { get; set; }
        public int StoEventDistribution { get; set; }
        public int EadCalcLevel { get; set; }
        public int NumRepitionsToStor { get; set; }
        public void Print()
        {
            WriteLine($"\n\tEconomic Global Parameters:");
            WriteLine($"\tNumber of Stage-Damage Simulations: {NumOfSimulations}");
            WriteLine($"\tMinimum number of Stage-Damage Ordinates: {NumOfStageDamageOrdsMinimum}");
            WriteLine($"\tMaximum number of Stage-Damage Ordinates: {NumOfStageDamageOrdsMaximum}");
            WriteLine($"\tTrace Level Stage-Damage: {TraceLevelStageDamage}");
            WriteLine($"\tAppend Trace: {TraceStageDamageAppend}");
            WriteLine($"\tPrint Structure Detail Out: {TraceStrucDetail}");
            WriteLine($"\tAppend Structure Detail Out: {TraceStrucDetailAppend}");
            WriteLine($"\tPrint Standard Deviation Output: {TraceStdDev}");
            WriteLine($"\tAppend Standard Deviation Output: {TraceStdDevAppend}");
            WriteLine($"\tTrace: {Trace}");
            WriteLine($"\tSTD Trace: {TraceSTD}");
            WriteLine($"\tSeparators: {Separators}");
            WriteLine($"\tUpdate Structures during import: {UpdateImport}");

            WriteLine($"\tExport All: {ExportAll}");
            WriteLine($"\tExport Plan: {ExportPlan}");
            WriteLine($"\tExport Year: {ExportYear}");
            WriteLine($"\tExport Stream: {ExportStream}");
            WriteLine($"\tExport Reach:  {ExportReach}");
            WriteLine($"\tExport Category: {ExportCategory}");
            WriteLine($"\tExport Occupancy Type: {ExportOccType}");
            WriteLine($"\tExport Structure Modules: {ExportModule}");
            WriteLine($"\tExport Structures: {ExportStructure}");
            WriteLine($"\tExport Water Surface Profiles: {ExportWsp}");
            WriteLine($"\tExport Probability Function: {ExportProbFunc}");
            WriteLine($"\tExport Stage-Discharge Rating Function: {ExportRateFunc}");
            WriteLine($"\tExport Levee Data: {ExportLevee}");
            WriteLine($"\tExport Stage-Damage Function: {ExportStageDamage}");

            WriteLine($"\tTrace level of EAD calculations: {TraceLevelEad}");
            WriteLine($"\tMaximum number of EAD simulations: {NumEadSimulationsMax}");
            WriteLine($"\tMinimum number of EAD simulations: {NumEadSimulationsMin}");
            WriteLine($"\tUse New Order Statistics: {UseNewOrderStats}");
            WriteLine($"\tUse match point: {UseMatchPoint}");
            WriteLine($"\tProbability at low end Match: {ProbMatchLow}");
            WriteLine($"\tProbability at high end Match: {ProbMatchHigh}");

            //WriteLine($"\t xxxxxx {xxx}");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\nEconomic Global Parameters:");
            wr.WriteLine($"\tNumber of Stage-Damage Simulations: {NumOfSimulations}");
            wr.WriteLine($"\tMinimum number of Stage-Damage Ordinates: {NumOfStageDamageOrdsMinimum}");
            wr.WriteLine($"\tMaximum number of Stage-Damage Ordinates: {NumOfStageDamageOrdsMaximum}");
            wr.WriteLine($"\tTrace Level Stage-Damage: {TraceLevelStageDamage}");
            wr.WriteLine($"\tAppend Trace: {TraceStageDamageAppend}");
            wr.WriteLine($"\tPrint Structure Detail Out: {TraceStrucDetail}");
            wr.WriteLine($"\tAppend Structure Detail Out: {TraceStrucDetailAppend}");
            wr.WriteLine($"\tPrint Standard Deviation Output: {TraceStdDev}");
            wr.WriteLine($"\tAppend Standard Deviation Output: {TraceStdDevAppend}");
            wr.WriteLine($"\tTrace: {Trace}");
            wr.WriteLine($"\tSTD Trace: {TraceSTD}");
            wr.WriteLine($"\tSeparators: {Separators}");
            wr.WriteLine($"\tUpdate Structures during import: {UpdateImport}");

            wr.WriteLine($"\tExport All: {ExportAll}");
            wr.WriteLine($"\tExport Plan: {ExportPlan}");
            wr.WriteLine($"\tExport Year: {ExportYear}");
            wr.WriteLine($"\tExport Stream: {ExportStream}");
            wr.WriteLine($"\tExport Reach:  {ExportReach}");
            wr.WriteLine($"\tExport Category: {ExportCategory}");
            wr.WriteLine($"\tExport Occupancy Type: {ExportOccType}");
            wr.WriteLine($"\tExport Structure Modules: {ExportModule}");
            wr.WriteLine($"\tExport Structures: {ExportStructure}");
            wr.WriteLine($"\tExport Water Surface Profiles: {ExportWsp}");
            wr.WriteLine($"\tExport Probability Function: {ExportProbFunc}");
            wr.WriteLine($"\tExport Stage-Discharge Rating Function: {ExportRateFunc}");
            wr.WriteLine($"\tExport Levee Data: {ExportLevee}");
            wr.WriteLine($"\tExport Stage-Damage Function: {ExportStageDamage}");

            wr.WriteLine($"\tTrace level of EAD calculations: {TraceLevelEad}");
            wr.WriteLine($"\tMaximum number of EAD simulations: {NumEadSimulationsMax}");
            wr.WriteLine($"\tMinimum number of EAD simulations: {NumEadSimulationsMin}");
            wr.WriteLine($"\tUse New Order Statistics: {UseNewOrderStats}");
            wr.WriteLine($"\tUse match point: {UseMatchPoint}");
            wr.WriteLine($"\tProbability at low end Match: {ProbMatchLow}");
            wr.WriteLine($"\tProbability at high end Match: {ProbMatchHigh}");

            //WriteLine($"\t xxxxxx {xxx}");
        }
    }
}
