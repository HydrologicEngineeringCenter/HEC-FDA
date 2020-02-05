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
    public class Year : FdObjectData
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public Year()
        {
            Reset();
        }
        #endregion
        #region Voids
        public void Print()
        {
            WriteLine($"\n\tYear Name: {this.Name}");
            WriteLine($"\tYear ID: {this.Id}");
            WriteLine($"\tDescription: {this.Description}");
            WriteLine($"\tDate: {this.CalculationDate}");
            WriteLine($"\tMeta Data: {this.MetaData}");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\nYear Name: {this.Name}");
            wr.WriteLine($"\tYear ID: {this.Id}");
            wr.WriteLine($"\tDescription: {this.Description}");
            wr.WriteLine($"\tDate: {this.CalculationDate}");
            wr.WriteLine($"\tMeta Data: {this.MetaData}");
        }
        public void ExportHeader(StreamWriter wr, char delimt)
        {
            for (int i = 0; i < AsciiImportExport.FieldsYear.Length; i++)
            {
                wr.Write($"{ AsciiImportExport.FieldsYear[i]}{ delimt}");
            }
            wr.Write("\n");
        }
        public void Export(StreamWriter wr, char delimt)
        {
            //wr.WriteLine($"{this.M_name}{delimt}{this.M_description}");
            wr.WriteLine($"{this.Name}");
        }
        #endregion
        #region Functions
        #endregion
    }
}
