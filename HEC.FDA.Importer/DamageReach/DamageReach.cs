using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace Importer
{
    [Serializable]
    public class DamageReach : FdObjectData
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        #endregion
        #region Properties
        public int StreamId
        { get; set; }
        public string StreamName
        { get; set; }
        public StreamBank BankStream
        { get; set; }
        public double StationBegin
        { get; set; }
        public double StationIndex
        { get; set; }
        public double StationEnd
        { get; set; }
        #endregion
        #region Constructors
        public DamageReach()
        {
            Reset();
        }
        #endregion
        #region Voids
        public new void Reset()
        {
            base.Reset();
            StreamId = -1;
            StreamName = "";
            BankStream = StreamBank.BOTH;
            StationBegin = StationIndex = StationEnd = Study.badNumber;
        }
        public void Print()
        {
            WriteLine($"Reach Name: {this.Name}");
            WriteLine($"\tReach ID: {this.Id}");
            WriteLine($"\tDescription: {this.Description}");
            WriteLine($"\tDate: {this.CalculationDate}");
            WriteLine($"\tMeta Data: {this.MetaData}");
            WriteLine($"\tStream Name: {StreamName}");
            WriteLine($"\tStream ID: {StreamId}");
            WriteLine($"\tBank: {BankStream}");
            WriteLine($"\tIndex Station: {StationIndex}");
            WriteLine($"\tBeginning Station: {StationBegin}");
            WriteLine($"\tEnding Station: {StationEnd}");
        }
        public void PrintToFile()
        {
            StreamWriter wr = Study._StreamWriter;

            wr.WriteLine($"Reach Name: {this.Name}");
            wr.WriteLine($"\tReach ID: {this.Id}");
            wr.WriteLine($"\tDescription: {this.Description}");
            wr.WriteLine($"\tDate: {this.CalculationDate}");
            wr.WriteLine($"\tMeta Data: {this.MetaData}");
            wr.WriteLine($"\tStream Name: {StreamName}");
            wr.WriteLine($"\tStream ID: {StreamId}");
            wr.WriteLine($"\tBank: {BankStream}");
            wr.WriteLine($"\tIndex Station: {StationIndex}");
            wr.WriteLine($"\tBeginning Station: {StationBegin}");
            wr.WriteLine($"\tEnding Station: {StationEnd}");
        }
        public void ExportHeader(StreamWriter wr, char delimt)
        {
            //fieldsReach = { "RCH_NAME", "RCH_DESC", "STREAM_NME", "BEG_STA", "END_STA", "BANK", "INDEX_STA" };

            for (int i = 0; i < AsciiImportExport.FieldsReach.Length; i++)
            {
                wr.Write($"{ AsciiImportExport.FieldsReach[i]}{ delimt}");
            }
            wr.Write("\n");
        }
        public void Export(StreamWriter wr, char delimt)
        {
            wr.Write($"{this.Name}{delimt}");
            wr.Write($"{this.Description}{delimt}");
            wr.Write($"{this.StreamName}{delimt}");
            wr.Write($"{this.StationBegin}{delimt}");
            wr.Write($"{this.StationEnd}{delimt}");
            wr.Write($"{this.BankStream}{delimt}");
            wr.Write($"{this.StationIndex}{delimt}");
            wr.Write("\n");
        }
        #endregion
        #region Functions

        #endregion
    }
}
