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
    public class Stream : FdObjectData
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
        public Stream()
        {
            Reset();
        }
        #endregion
        #region Voids
        public void Print()
        {
            WriteLine($"\n\tStream Name: {this.Name}");
            WriteLine($"\tStream ID: {this.Id}");
            WriteLine($"\tDescription: {this.Description}");
            WriteLine($"\tDate: {this.CalculationDate}");
            WriteLine($"\tMeta Data: {this.MetaData}");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"\nStream Name: {this.Name}");
            wr.WriteLine($"\tStream ID: {this.Id}");
            wr.WriteLine($"\tDescription: {this.Description}");
            wr.WriteLine($"\tDate: {this.CalculationDate}");
            wr.WriteLine($"\tMeta Data: {this.MetaData}");
        }
        public void ExportHeader(StreamWriter wr, char delimt)
        {
            for (int i = 0; i < AsciiImportExport.FieldsStream.Length; i++)
            {
                wr.Write($"{ AsciiImportExport.FieldsStream[i]}{ delimt}");
            }
            wr.Write("\n");
        }
        public void Export(StreamWriter wr, char delimt)
        {
            wr.WriteLine($"{this.Name}{delimt}{this.Description}");
        }
        #endregion
        #region Functions
        #endregion
    }
}
