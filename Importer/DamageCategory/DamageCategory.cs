using System;
using System.IO;
using static System.Console;

namespace Importer
{
    [Serializable]
    public class DamageCategory : FdObjectData
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        #endregion
        #region Properties
        public double CostFactor
        { get; set; }
        #endregion
        #region Constructors
        public DamageCategory()
        {
            Reset();
        }
        #endregion
        #region Voids
        public new void Reset()
        {
            base.Reset();
            CostFactor = Study.badNumber;
        }
        public void Print()
        {
            WriteLine($"Category Name: {this.Name}");
            WriteLine($"\tCategory ID: {this.Id}");
            WriteLine($"\tDescription: {this.Description}");
            WriteLine($"\tDate: {this.CalculationDate}");
            WriteLine($"\tMeta Data: {this.MetaData}");
            WriteLine($"\tCost Factor: {CostFactor}");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"Category Name: {this.Name}");
            wr.WriteLine($"\tCategory ID: {this.Id}");
            wr.WriteLine($"\tDescription: {this.Description}");
            wr.WriteLine($"\tDate: {this.CalculationDate}");
            wr.WriteLine($"\tMeta Data: {this.MetaData}");
            wr.WriteLine($"\tCost Factor: {CostFactor}");
        }
        public void ExportHeader(StreamWriter wr, char delimt)
        {
            //fieldsCategory = { "CAT_NAME", "CAT_DESCRIPTION", "COST_FACTOR" };

            for (int i = 0; i < AsciiImportExport.FieldsCategory.Length; i++)
            {
                wr.Write($"{ AsciiImportExport.FieldsCategory[i]}{ delimt}");
            }
            wr.Write("\n");
        }
        public void Export(StreamWriter wr, char delimt)
        {
            wr.Write($"{this.Name}{delimt}");
            wr.Write($"{this.Description}{delimt}");
            wr.Write($"{this.CostFactor}{delimt}");
            wr.Write("\n");
        }
        #endregion

    }
}
