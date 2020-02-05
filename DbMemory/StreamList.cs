using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;

namespace DbMemory
{
    public class StreamList
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private Stream _Stream;
        private SortedList<string, Stream> _StreamListSort = new SortedList<string, Stream>();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public StreamList()
        {
        }
        #endregion
        #region Voids
        public void Add(Stream theStream)
        {
            Stream aStream = ObjectCopier.Clone(theStream);

            _StreamListSort.Add(aStream.Name.Trim(), aStream);
            WriteLine($"Add StreamObj to SortList.  {aStream.Name}");
            if(GlobalVariables.mp_fdaStudy._TraceConvertLevel > 19) aStream.Print();
        }
        public void Print()
        {
            Stream aStream;
            WriteLine($"Number of Streams {_StreamListSort.Count}");
            for (int i = 0; i < _StreamListSort.Count; i++)
            {
                aStream = _StreamListSort.ElementAt(i).Value;
                aStream.Print();
            }
        }
        public void PrintToFile()
        {
            Stream aStream;
            WriteLine($"Number of Streams {_StreamListSort.Count}");
            for (int i = 0; i < _StreamListSort.Count; i++)
            {
                aStream = _StreamListSort.ElementAt(i).Value;
                aStream.PrintToFile();
            }
        }
        public void Export(StreamWriter wr, char delimt)
        {
            Stream aStream = new Stream();
            for (int i = 0; i < _StreamListSort.Count; i++)
            {
                aStream = _StreamListSort.ElementAt(i).Value;
                if (i == 0) aStream.ExportHeader(wr, delimt);
                aStream.Export(wr, delimt);
            }
        }
        #endregion
        #region Functions
        public long GetId(string name)
        {
            long id = -1L;
            int ix = _StreamListSort.IndexOfKey(name);
            if (ix > -1)
            {
                _Stream = _StreamListSort.ElementAt(ix).Value;
                id = _Stream.Id;
            }
            else
            {
                id = -1L;
            }
            return id;
        }
        public string getName(long id)
        {
            bool found = false;
            string name = "";
            for (int i = 0; i < _StreamListSort.Count && !found; i++)
            {
                _Stream = _StreamListSort.ElementAt(i).Value;
                if (id == _Stream.Id)
                {
                    found = true;
                    name = _Stream.Name;
                }
            }
            return name;
        }

        public Stream GetStream(string nameStream)
        {
            int ixOfStream = _StreamListSort.IndexOfKey(nameStream);
            _Stream = _StreamListSort.ElementAt(ixOfStream).Value;
            WriteLine($"Did I find the {nameStream} StreamObj, name = {_Stream.Name}");
            return _Stream;
        }
        #endregion
    }
}
