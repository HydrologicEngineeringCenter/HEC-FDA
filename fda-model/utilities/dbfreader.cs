

namespace utilities
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    public class dbfreader
    {
        private FileStream _dbfstream;
        private BinaryReader _dbfreader;
        private bool _DataBaseOpen;
        private Int16 _FirstDataRecordIndex;
        private List<Int32> _RecordStartPositions = new List<Int32>();
        private Int32 _NumberOfRecords; // Some of them can have the deleted tag
        private Int16 _RecordLength;
        private byte[] _Lengths;
        private Int16[] _Positions;
        private string _filePath;
        private string[] _ColumnNames;
        private Type[] _ColumnTypes;
        private int _NRows;
        private int _NColumns;

        private struct dbfField
        {
            public string Name;
            public string TypeID;
            public Type Type;
            public int Length;
            public int NumDecimal;
            public bool WriteField;
        }

        public dbfreader(string filepath)
        {
            if (Path.GetExtension(filepath).ToLower() != ".dbf")
                throw new Exception("This is not a .dbf file");
            _filePath = filepath;
            Open();
            LoadAttributeInfo();
            Close();
        }
 
        public void Open()
        {
            _dbfstream = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            _dbfreader = new BinaryReader(_dbfstream);
            _DataBaseOpen = true;
        }
        public void Close()
        {
            _DataBaseOpen = false;
            _dbfreader.Close(); _dbfreader.Dispose();
            _dbfstream.Close(); _dbfstream.Dispose();
        }
        /// <summary>
        ///     ''' Gets the record index using a row index from only non-deleted rows
        ///     ''' </summary>
        ///     ''' <param name="RowIndex">Row index of only non-deleted rows</param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public Int32 GetRecordIndexFromRowIndex(Int32 RowIndex)
        {
            // _RecordStartPositions.Add(_FirstDataRecordIndex + 1 + ((i) * _RecordLength))
            return (int)((_RecordStartPositions[RowIndex] - _FirstDataRecordIndex - 1) / (double)_RecordLength);
        }
        /// <summary>
        ///     ''' Gets the appropriate record start position row index using a record index since records can have a deleted flag.
        ///     ''' </summary>
        ///     ''' <param name="RecordIndex">Record index of row which could be deleted and not included in the record start positions list.</param>
        ///     ''' <returns></returns>
        ///     ''' <remarks></remarks>
        public Int32 GetRowIndexFromRecordIndex(Int32 RecordIndex)
        {
            Int32 Position = _FirstDataRecordIndex + 1 + ((RecordIndex) * _RecordLength);
            return _RecordStartPositions.BinarySearch(Position);
        }
        public object[] GetUnique(string ColumnName)
        {
            if (_ColumnNames.Contains(ColumnName) == false)
                throw new Exception("Field does not exist");
            Int32 colindex = Array.IndexOf(_ColumnNames, ColumnName);
            return GetUnique(colindex);
        }
        public object[] GetUnique(Int32 ColumnIndex)
        {
            if (_ColumnNames.Length < ColumnIndex)
                throw new Exception("Column Index entered does not exist");
            bool WasOpen = _DataBaseOpen;
            if (_DataBaseOpen == false)
                Open();
            string[] UniqueData = GetColumnRaw(ColumnIndex).Distinct().ToArray();
            object[] Result = new object[UniqueData.Count() - 1 + 1];
            for (Int32 i = 0; i <= Result.Count() - 1; i++)
                Result[i] = ConvertCellValueToProperType(UniqueData[i], _ColumnTypes[ColumnIndex]);
            if (WasOpen == false)
                Close();
            return Result;
        }
        public string[] GetColumnRaw(string ColumnName)
        {
            if (_ColumnNames.Contains(ColumnName) == false)
                throw new Exception("Field does not exist");
            Int32 colindex = Array.IndexOf(_ColumnNames, ColumnName);
            return GetColumnRaw(colindex);
        }
        public string[] GetColumnRaw(Int32 ColumnIndex)
        {
            if (_ColumnNames.Length < ColumnIndex)
                throw new Exception("Column Index entered does not exist");
            bool WasOpen = _DataBaseOpen;
            if (_DataBaseOpen == false)
                Open();
            byte[] dbfbytes;
            List<string> result = new List<string>(_NRows); // (_NRows - 1) As String
            _dbfreader.BaseStream.Position = _FirstDataRecordIndex;
            for (Int32 i = 0; i <= _NumberOfRecords - 1; i++)
            {
                dbfbytes = _dbfreader.ReadBytes(_RecordLength);
                if (dbfbytes[0] == 32)
                    result.Add(System.Text.Encoding.UTF8.GetString(dbfbytes, _Positions[ColumnIndex] + 1, _Lengths[ColumnIndex]).Trim(Convert.ToChar(0)));
            }
            if (WasOpen == false)
                Close();
            return result.ToArray();
        }
        public object[] GetColumn(string ColumnName)
        {
            if (_ColumnNames.Contains(ColumnName) == false)
                throw new Exception("Field does not exist");
            bool WasOpen = _DataBaseOpen;
            if (_DataBaseOpen == false)
                Open();
            byte[] dbfbytes;
            List<object> result = new List<object>(_NRows); // (_NRows - 1) As Object
            Int32 colindex = Array.IndexOf(_ColumnNames, ColumnName);
            _dbfreader.BaseStream.Position = _FirstDataRecordIndex;
            string cellvalue;
            for (Int32 i = 0; i <= _NumberOfRecords - 1; i++)
            {
                dbfbytes = _dbfreader.ReadBytes(_RecordLength);
                if (dbfbytes[0] == 32)
                {
                    cellvalue = System.Text.Encoding.UTF8.GetString(dbfbytes, _Positions[colindex] + 1, _Lengths[colindex]).Trim(Convert.ToChar(0));
                    result.Add(ConvertCellValueToProperType(cellvalue, _ColumnTypes[colindex]));
                }
            }
            if (WasOpen == false)
                Close();
            return result.ToArray();
        }
        public object[] GetColumn(int ColumnIndex)
        {
            if (_ColumnNames.Length < ColumnIndex)
                throw new Exception("Column Index entered does not exist");
            bool WasOpen = _DataBaseOpen;
            if (_DataBaseOpen == false)
                Open();
            byte[] dbfbytes;
            List<object> result = new List<object>();
            _dbfreader.BaseStream.Position = _FirstDataRecordIndex;
            string cellvalue;
            for (Int32 i = 0; i <= _NumberOfRecords - 1; i++)
            {
                dbfbytes = _dbfreader.ReadBytes(_RecordLength);
                if (dbfbytes[0] == 32)
                {
                    cellvalue = System.Text.Encoding.UTF8.GetString(dbfbytes, _Positions[ColumnIndex] + 1, _Lengths[ColumnIndex]).Trim(Convert.ToChar(0));
                    result.Add(ConvertCellValueToProperType(cellvalue, _ColumnTypes[ColumnIndex]));
                }
            }
            if (WasOpen == false)
                Close();
            return result.ToArray();
        }
        private object ConvertCellValueToProperType(string CellValue, Type ColumnType)
        {
            switch (ColumnType.GetTypeInfo().Name)
            {
                case "Int32":
                    {
                        Int32 i;
                        int.TryParse(CellValue, out i);
                        return i;
                    }

                case "double":
                    {
                        double i;
                        double.TryParse(CellValue, out i);
                        return i;
                    }
                case "Double":
                    {
                        double i;
                        double.TryParse(CellValue, out i);
                        return i;
                    }
                case "string":
                    {
                        return CellValue.Trim(System.Convert.ToChar(" "));
                    }

                case "bool":
                    {
                        switch (CellValue)
                        {
                            case "0":
                            case "F":
                            case "f":
                            case "N":
                            case "n":
                                {
                                    return false;
                                }

                            case "1":
                            case "T":
                            case "t":
                            case "Y":
                            case "y":
                                {
                                    return true;
                                }

                            default:
                                {
                                    return false;
                                }
                        }

                        break;
                    }

                default:
                    {
                        throw new Exception("Column Type not supported");
                        return null;
                    }
            }
        }

        public object GetCell(int ColumnIndex, int RowIndex)
        {
            return ConvertCellValueToProperType(ReadRawCellSafe(ColumnIndex, RowIndex), _ColumnTypes[ColumnIndex]);
        }
        public object GetCell(string ColumnName, int RowIndex)
        {
            if (_ColumnNames.Contains(ColumnName) == false)
                throw new Exception("Column Name " + ColumnName + " does not exist.");
            return GetCell(Array.IndexOf(_ColumnNames, ColumnName), RowIndex);
        }
        public object[] GetCells(int[] CellLocations)
        {
            object[] Cells = new object[CellLocations.Count() - 1 + 1];
            Int32 RowIndex, ColumnIndex;
            for (Int32 i = 0; i <= CellLocations.Count() - 1; i++)
            {
                RowIndex = (int)Math.Floor((double)CellLocations[i] / (double)_NColumns);
                ColumnIndex = CellLocations[i] - RowIndex * _NColumns;
                Cells[i] = GetCell(ColumnIndex, RowIndex);
            }
            return Cells;
        }
        private string ReadRawCellSafe(int col, int row)
        {
            if (_DataBaseOpen == false)
                Open();
            // much slower for some reason _dbfreader.BaseStream.Position = _FirstDataRecordIndex + 1 + (row * _RecordLength) + _Positions(col)
            // _dbfreader.BaseStream.Seek(_FirstDataRecordIndex + 1 + (row * _RecordLength) + _Positions(col), SeekOrigin.Begin)
            _dbfreader.BaseStream.Position = _RecordStartPositions[row] + _Positions[col];
            return System.Text.Encoding.UTF8.GetString(_dbfreader.ReadBytes(_Lengths[col]), 0, _Lengths[col]).Trim(Convert.ToChar(0));
        }
        public string ReadRawCellUnsafe(Int32 Column, Int32 Row)
        {
            // much slower for some reason _dbfreader.BaseStream.Position = _FirstDataRecordIndex + 1 + (Row * _RecordLength) + _Positions(Column)
            // _dbfreader.BaseStream.Seek(_FirstDataRecordIndex + 1 + (Row * _RecordLength) + _Positions(Column), SeekOrigin.Begin)
            _dbfreader.BaseStream.Position = _RecordStartPositions[Row] + _Positions[Column];
            return System.Text.Encoding.UTF8.GetString(_dbfreader.ReadBytes(_Lengths[Column]), 0, _Lengths[Column]).Trim(Convert.ToChar(0));
        }
        public object[] GetRow(int RowIndex)
        {
            // _dbfreader.BaseStream.Seek(_FirstDataRecordIndex + 1 + ((RowIndex) * _RecordLength), SeekOrigin.Begin)
            _dbfreader.BaseStream.Position = _RecordStartPositions[RowIndex];
            return ParseRowBytes(_dbfreader.ReadBytes(_RecordLength));
        }
        public object[] GetRow(Int32 RowIndex, Int32[] ColumnIndices)
        {
            // _dbfreader.BaseStream.Seek(_FirstDataRecordIndex + 1 + ((RowIndex) * _RecordLength), SeekOrigin.Begin)
            _dbfreader.BaseStream.Seek(_RecordStartPositions[RowIndex], SeekOrigin.Begin);
            byte[] RowBytes = _dbfreader.ReadBytes(_RecordLength);
            string CellValue;
            object[] ReturnValues = new object[ColumnIndices.Count() - 1 + 1];
            for (var j = 0; j <= ColumnIndices.Count() - 1; j++)
            {
                CellValue = System.Text.Encoding.UTF8.GetString(RowBytes, _Positions[ColumnIndices[j]], _Lengths[ColumnIndices[j]]).Trim(Convert.ToChar(0));
                ReturnValues[j] = ConvertCellValueToProperType(CellValue, _ColumnTypes[ColumnIndices[j]]);
            }
            return ReturnValues;
        }
        public System.Collections.Generic.List<object[]> GetRows(int StartRowIndex, int EndRowIndex)
        {

            if (EndRowIndex > _NRows - 1)
                EndRowIndex = _NRows - 1;
            object[] Row = new object[_ColumnNames.Length - 1 + 1];
            List<object[]> Rows = new List<object[]>(EndRowIndex - StartRowIndex + 1);
            byte[] RowBytes;
            string CellValue;
            // _dbfreader.BaseStream.Seek(_FirstDataRecordIndex + 1 + ((StartRowIndex) * _RecordLength), SeekOrigin.Begin)
            for (Int32 i = StartRowIndex; i <= EndRowIndex; i++)
            {
                _dbfreader.BaseStream.Seek(_RecordStartPositions[i], SeekOrigin.Begin);
                RowBytes = _dbfreader.ReadBytes(_RecordLength);
                for (var j = 0; j <= _ColumnNames.Length - 1; j++)
                {
                    CellValue = System.Text.Encoding.UTF8.GetString(RowBytes, _Positions[j], _Lengths[j]).Trim(Convert.ToChar(0));
                    Row[j] = ConvertCellValueToProperType(CellValue, _ColumnTypes[j]);
                }

                // Rows.Add(ParseRowBytes(_dbfreader.ReadBytes(_RecordLength)))

                Rows.Add((object[])Row.Clone());
            }
            return Rows;
        }

        private object[] ParseRowBytes(byte[] RowBytes)
        {
            object[] Row = new object[_ColumnNames.Length - 1 + 1];
            string CellValue;
            for (var i = 0; i <= _ColumnNames.Length - 1; i++)
            {
                CellValue = System.Text.Encoding.UTF8.GetString(RowBytes, _Positions[i], _Lengths[i]).Trim(Convert.ToChar(0));
                Row[i] = ConvertCellValueToProperType(CellValue, _ColumnTypes[i]);
            }
            return Row;
        }
        public void LoadAttributeInfo()
        {
            if (_DataBaseOpen == false)
                Open();
            _dbfreader.BaseStream.Position = 0;
            // ''''''''''''''''''''''''''''''''''''''Table Header (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
            byte[] bytes = _dbfreader.ReadBytes(32);
            _NumberOfRecords = BitConverter.ToInt32(bytes, 4); // number of rows
            _FirstDataRecordIndex = BitConverter.ToInt16(bytes, 8); // always 32 + 32*number of fields + 1
            _RecordLength = BitConverter.ToInt16(bytes, 10);
            _NColumns = (_FirstDataRecordIndex - 32 - 1) / 32;
            // 
            _RecordStartPositions = new List<Int32>();
            for (Int32 i = 0; i <= _NumberOfRecords - 1; i++)
            {
                _dbfreader.BaseStream.Position = _FirstDataRecordIndex + ((i) * _RecordLength);
                if (_dbfreader.ReadByte() == 32)
                    _RecordStartPositions.Add(_FirstDataRecordIndex + 1 + ((i) * _RecordLength));
            }
            _NRows = _RecordStartPositions.Count;
            // ''''''''''''''''''''''''''''''''''''''Field Subrecords (always 32 bytes from 0 to 31)'''''''''''''''''''''''''''
            _dbfreader.BaseStream.Position = 32;
            // create the table columns
            _Lengths = new byte[_NColumns];
            _Positions = new short[_NColumns];
            _ColumnNames = new string[_NColumns];
            _ColumnTypes = new Type[_NColumns];
            for (int col = 1; col <= _NColumns; col++)
            {
                bytes = _dbfreader.ReadBytes(32);
                _ColumnNames[col - 1] = System.Text.Encoding.ASCII.GetString(bytes, 0, 10);
                // field type
                switch (System.Text.Encoding.UTF8.GetString(bytes, 11, 1))
                {
                    case "I":
                        {
                            _ColumnTypes[col - 1] = typeof(Int32);
                            break;
                        }

                    case "N":
                        {
                            if (bytes[17] > 0)
                                _ColumnTypes[col - 1] = typeof(double);
                            else
                                _ColumnTypes[col - 1] = typeof(Int32);
                            break;
                        }

                    case "F":
                    case "B":
                        {
                            _ColumnTypes[col - 1] = typeof(double);
                            break;
                        }

                    case "L":
                        {
                            _ColumnTypes[col - 1] = typeof(bool);
                            break;
                        }

                    case "C":
                        {
                            _ColumnTypes[col - 1] = typeof(string);
                            break;
                        }

                    default:
                        {
                            _ColumnTypes[col - 1] = typeof(string);
                            break;
                        }
                }

                // displacement of field
                int Displacement = BitConverter.ToInt32(bytes, 12); // unused beyond this is that right????
                                                                    // 
                                                                    // field length
                _Lengths[col - 1] = bytes[16];
                if (col == 1)
                    _Positions[col - 1] = 0;
                else
                    _Positions[col - 1] = (short)(_Positions[col - 2] + _Lengths[col - 2]);
            }
        }

    }

}
