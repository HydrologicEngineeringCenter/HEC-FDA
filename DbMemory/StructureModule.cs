using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace DbMemory
{
    [Serializable]
    public class StructureModule : FdObjectData
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        #endregion
        #region Properties
        public int NumReferences { get; set; }
        #endregion
        #region Constructors
        public StructureModule()
        {
            Reset();
        }
        #endregion
        #region Voids
        public void Print()
        {
            WriteLine($"\n\tStructure Module Name: {this.Name}");
            WriteLine($"\tStructure Module ID: {Id}");
            WriteLine($"\tDescription: {Description}");
            WriteLine($"\tNumber of References: {NumReferences}");
            WriteLine($"\tEntry Date: {CalculationDate}");
            WriteLine($"\tMeta Data: {MetaData}");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\nStructure Module Name: {this.Name}");
            wr.WriteLine($"\tStructure Module ID: {Id}");
            wr.WriteLine($"\tDescription: {Description}");
            wr.WriteLine($"\tNumber of References: {NumReferences}");
            wr.WriteLine($"\tEntry Date: {CalculationDate}");
            wr.WriteLine($"\tMeta Data: {MetaData}");
        }
        public void ExportHeader(StreamWriter wr, char delimt)
        {
            //fieldsModules = { "MOD_NAME", "MOD_DESC" };

            for (int i = 0; i < AsciiImportExport.FieldsModule.Length; i++)
            {
                wr.Write($"{ AsciiImportExport.FieldsModule[i]}{ delimt}");
            }
            wr.Write("\n");
        }
        public void Export(StreamWriter wr, char delimt)
        {
            wr.Write($"{this.Name}{delimt}");
            wr.Write($"{this.Description}{delimt}");
            wr.Write("\n");
        }
        #endregion
        #region Functions
        #endregion
    }
}
