

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

        public int NumberOfRows
        {
            get { return _NRows; }
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

        private object ConvertCellValueToProperType(string CellValue, Type ColumnType)
        {
            switch (ColumnType.GetTypeInfo().Name.ToUpper())
            {
                case "INT32":
                    {
                        int.TryParse(CellValue, out int i);
                        return i;
                    }

                case "DOUBLE":
                    {
                        double.TryParse(CellValue, out double i);
                        return i;
                    }
                case "STRING":
                    {
                        return CellValue.Trim(System.Convert.ToChar(" "));
                    }
                case "BOOL":
                    {
                        switch (CellValue.ToUpper())
                        {
                            case "0":
                            case "F":
                            case "N":
                                {
                                    return false;
                                }

                            case "1":
                            case "T":
                            case "Y":
                                {
                                    return true;
                                }

                            default:
                                {
                                    return false;
                                }
                        }
                    }
                default:
                    {
                        throw new Exception("Column Type " + ColumnType.GetTypeInfo().Name + " not supported");
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

        private string ReadRawCellSafe(int col, int row)
        {
            if (_DataBaseOpen == false)
                Open();
            // much slower for some reason _dbfreader.BaseStream.Position = _FirstDataRecordIndex + 1 + (row * _RecordLength) + _Positions(col)
            // _dbfreader.BaseStream.Seek(_FirstDataRecordIndex + 1 + (row * _RecordLength) + _Positions(col), SeekOrigin.Begin)
            _dbfreader.BaseStream.Position = _RecordStartPositions[row] + _Positions[col];
            return System.Text.Encoding.UTF8.GetString(_dbfreader.ReadBytes(_Lengths[col]), 0, _Lengths[col]).Trim(Convert.ToChar(0));
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
                _ColumnNames[col - 1] = System.Text.Encoding.ASCII.GetString(bytes, 0, 10).Trim(Convert.ToChar(0));
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
