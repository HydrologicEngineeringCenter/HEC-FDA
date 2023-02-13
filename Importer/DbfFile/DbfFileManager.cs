using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using static System.Console;
using System.Linq;

namespace Importer
{
    //==============================================================================
    // DbfFileManager.h
    //==============================================================================
    /*
    ================================================================================
      Handles information for dbf files and opens, closes, reads and writes records:
        1) create field & tag information includeing dataDescriptor
        2) Read existing dbf file for field and tag information
        3) Read / Write data from/to dbf file

      The record navigation is broken down into chile classes (07Dec09).
        DbfFileFdDataManager
        DbfFileFdLookupManager
        DbfFileSingleRecord
        DbfFileGeneric

      Uses linked lists to that one field is defined as an element of a linked list.
        the DbfFieldInfo is a LINK4 object. The DbfFileManager object contains a
        LIST4 control object for positioning the LINK4 object at a desired location.
    ================================================================================
    */
    //==============================================================================
    public class DbfFileManager : FileNameManager
    {
        // This is the file header for a DBF. We do this special layout with everything
        // packed so we can read straight from disk into the structure to populate it
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct DBFHeader
        {
            public byte version;
            public byte updateYear;
            public byte updateMonth;
            public byte updateDay;
            public Int32 numRecords;
            public Int16 headerLen;
            public Int16 recordLen;
            public Int16 reserved1;
            public byte incompleteTrans;
            public byte encryptionFlag;
            public Int32 reserved2;
            public Int64 reserved3;
            public byte MDX;
            public byte language;
            public Int16 reserved4;
        }

        // This is the field descriptor structure. 
        // There will be one of these for each column in the table.
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct FieldDescriptor
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
            public string fieldName;
            public char fieldType;
            public Int32 address;
            public byte fieldLen;
            public byte count;
            public Int16 reserved1;
            public byte workArea;
            public Int16 reserved2;
            public byte flag;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public byte[] reserved3;
            public byte indexFlag;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct DBTHeader
        {
            public Int32 nextBlockID;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] reserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
            public string fileName;
            public byte version; // 0x03 = Version III, 0x00 = Version IV
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] reserved3;
            public Int16 blockLength;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 490)]
            public byte[] reserved4;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
        private struct MemoHeader
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
            public byte[] reserved;
            public Int16 startPosition;
            public Int32 fieldLength;
        }
        public ArrayList _fields;

        BinaryReader _br;
        byte[] _buffer;
        GCHandle _handle;
        DBFHeader _header;
        Dictionary<int, String> _memoLookup;
        Dictionary<int, double[]> _memoLookupDouble;

        public void openDbf()
        {
            _br = new BinaryReader(File.OpenRead(mp_fileNameFull));
        }
        public void readDbfHeader()
        {
            /*
             * Must have opened the Dbf file
            */
            // Read the header into a buffer
            _buffer = _br.ReadBytes(Marshal.SizeOf(typeof(DBFHeader)));

            // Marshall the header into a DBFHeader structure
            _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
            _header = (DBFHeader)Marshal.PtrToStructure(
                                _handle.AddrOfPinnedObject(), typeof(DBFHeader));
            _handle.Free();
        }
        public void ReadFieldDescriptors()
        {
            // Read in all the field descriptors. 
            // Per the spec, 13 (0D) marks the end of the field descriptors
            _fields = new ArrayList();
            while ((13 != _br.PeekChar()))
            {
                _buffer = _br.ReadBytes(Marshal.SizeOf(typeof(FieldDescriptor)));
                _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
                _fields.Add((FieldDescriptor)Marshal.PtrToStructure(
                            _handle.AddrOfPinnedObject(), typeof(FieldDescriptor)));
                _handle.Free();
            }
        }
        public void CreateColunns(DataTable dataTable)
        {
            DataColumn col = null;
            
            foreach (FieldDescriptor field in _fields)
            {
                int countField = field.count;       //Might be number of digits to right of decimal. It is!
                int lenField = field.fieldLen;  //Length of field (number of bytes in field / column width)
                Type fieldType = field.GetType();
                string fieldString = field.ToString();

                switch (field.fieldType)
                {
                    case 'N':
                        col = new DataColumn(field.fieldName, typeof(Int32));
                        //doesn't work, only for strings;  col.MaxLength = 10; //rdc temp;Would be field dependent
                        break;
                    case 'F':
                        col = new DataColumn(field.fieldName, typeof(double));
                        break;
                    case 'C':
                        col = new DataColumn(field.fieldName, typeof(string));
                        break;
                    case 'D':
                        col = new DataColumn(field.fieldName, typeof(DateTime));
                        break;
                    case 'L':
                        col = new DataColumn(field.fieldName, typeof(bool));
                        break;
                    case 'M':
                        col = new DataColumn(field.fieldName, typeof(string));
                        break;
                    default:
                        Console.WriteLine($"Failure to add field {field.fieldName}");
                        break;
                }
                dataTable.Columns.Add(col);
            }
        }
        public void ProcessTheDbfFile(DataTable dataTable)
        {
            //SetTheDbfFile(theDbfFileName);
            openDbf();
            readDbfHeader();
            ReadFieldDescriptors();
            CreateColunns(dataTable);
            ReadAllDbfRecords(dataTable);
            ProcessDataTable(dataTable);
        }


        public void ReadAllDbfRecords(DataTable dataTable)
        {
            string reachName = "";

            _memoLookupDouble = new Dictionary<int, double[]>();

            //Bring in the Memo information
            _memoLookup = ReadDBT(ref _memoLookupDouble);

            _buffer = _br.ReadBytes(1); //rdc temp;Test to see if this strips only cr character


            // Read in all the records
            for (int counter = 0; counter <= _header.numRecords - 1; counter++)
            {
                int numberOfXSect = 0;
                int numberOfProfiles = 0;

                // First we'll read the entire record into a buffer and then read each 
                // field from the buffer. This helps account for any extra space at the 
                // end of each record and probably performs better.
                _buffer = _br.ReadBytes(_header.recordLen);
                BinaryReader recReader = new BinaryReader(new MemoryStream(_buffer));

                // Loop through each field in a record
                DataRow row = dataTable.NewRow();

                //string carriageReturn = Encoding.ASCII.GetString(recReader.ReadBytes(1));
                string flagDeleteRec = Encoding.ASCII.GetString(recReader.ReadBytes(1));
                int fieldIndex = -1;

                foreach (FieldDescriptor field in _fields)
                {
                    int numCharInField = field.fieldLen;
                    string theFieldName = field.fieldName.ToUpper();
                    fieldIndex++;

                    switch (field.fieldType)
                    {
                        case 'N':
                            int ID = -1;
                            string theStringInt = Encoding.ASCII.GetString(recReader.ReadBytes(numCharInField));
                            try
                            {
                                ID = Convert.ToInt32(theStringInt);
                            }
                            catch
                            {
                                ID = -1;
                            }
                            row[fieldIndex] = ID;

                            if (theFieldName == "NU_XSECT")
                                numberOfXSect = ID;
                            else if (theFieldName == "NU_PROFILE")
                                numberOfProfiles = ID;
                            break;
                        case 'F':
                            double theDouble = -901.0;
                            string theStringFloat = Encoding.ASCII.GetString(recReader.ReadBytes(numCharInField));
                            try
                            {
                                theDouble = Convert.ToDouble(theStringFloat);
                            }
                            catch
                            {
                                theDouble = -901.0;
                            }
                            row[fieldIndex] = theDouble;
                            break;
                        case 'C':
                            string theString = Encoding.ASCII.GetString(recReader.ReadBytes(numCharInField));
                            row[fieldIndex] = theString;
                            if (fieldIndex == 1) reachName = theString;
                            break;
                        case 'M':
                            //Problem, how do we know how many characters to read?
                            {
                                int numRows = 0;
                                int numCols = 0;
                                double[,] valuesInMemoField = null;
                                int intRef;
                                String strRef = Encoding.ASCII.GetString(recReader.ReadBytes(field.fieldLen)).Trim();
                                if (Int32.TryParse(strRef, out intRef))
                                {
                                    //This dbf field has data in the dbt file 
                                    if (_memoLookup.ContainsKey(intRef))
                                    {
                                        WriteLine($"Block number (intRef): {intRef}\t Field: {theFieldName}\t Row[0]: {row[0]}");
                                        //For certain Fields (WSP fields), Process Legacy Memo Fields (binary data in dbt)
                                        if (theFieldName == "WSP_STAGES" ||
                                            theFieldName == "WSP_STATN" ||
                                            theFieldName == "WSP_INVERT" ||
                                            theFieldName == "WSP_DISCH")
                                        {
                                            if (theFieldName == "WSP_STATN") numCols = 1;
                                            else if (theFieldName == "WSP_INVERT") numCols = 1;
                                            else numCols = numberOfProfiles;
                                            numRows = numberOfXSect;
                                            valuesInMemoField = new double[numRows, numCols];

                                            //Get the data from the memo field
                                            double[] theValuesDouble = _memoLookupDouble[intRef];

                                            int m = -1;

                                            for (int ixsec = 0; ixsec < numRows; ixsec++)
                                            {
                                                for (int iprof = 0; iprof < numCols; iprof++)
                                                {
                                                    m = ixsec * numCols + iprof;
                                                    valuesInMemoField[ixsec, iprof] = theValuesDouble[m];
                                                }
                                            }
                                            string theMemoFieldFromDouble = null;
                                            MemoDataField.ProcessDoubleToMemoString(numRows, numCols, valuesInMemoField, ref theMemoFieldFromDouble);
                                            row[fieldIndex] = theMemoFieldFromDouble;
                                        }
                                        else
                                        {
                                            row[fieldIndex] = _memoLookup[intRef];
                                            string theMemoFieldStr = row[fieldIndex].ToString();
                                            MemoDataField.ProcessMemoDataField(theMemoFieldStr, ref numRows, ref numCols, ref valuesInMemoField);
                                            //WriteLine($"Tab Delimited Data Field\n {theMemoFieldStr}");
                                        }
                                    }
                                    else
                                    {
                                        row[fieldIndex] = String.Empty;
                                        WriteLine($"Block number (intRef): {intRef}\t Field: {theFieldName}\t Row[0]: {row[0]}");
                                        WriteLine($"Can't find the block.");
                                    }
                                }
                                else
                                    row[fieldIndex] = String.Empty;
                                break;
                            }
                        case 'D': // Date (YYYYMMDD)
                            string year = Encoding.ASCII.GetString(recReader.ReadBytes(4));
                            string month = Encoding.ASCII.GetString(recReader.ReadBytes(2));
                            string day = Encoding.ASCII.GetString(recReader.ReadBytes(2));
                            row[field.fieldName] = System.DBNull.Value;
                            try
                            {
                                if ((Int32.Parse(year) > 1900))
                                {
                                    row[field.fieldName] = new DateTime(Int32.Parse(year),
                                                               Int32.Parse(month), Int32.Parse(day));
                                }
                            }
                            catch
                            { }

                            break;
                        case 'L':
                            bool theBool = false;
                            string theStringLogical = Encoding.ASCII.GetString(recReader.ReadBytes(numCharInField));
                            try
                            {
                                theBool = Convert.ToBoolean(theStringLogical);
                            }
                            catch
                            {
                                theBool = false;
                            }
                            row[field.fieldName] = theBool;
                            break;
                        default:
                            string theStringDefault = Encoding.ASCII.GetString(recReader.ReadBytes(numCharInField));
                            row[fieldIndex] = theStringDefault;
                            break;
                    }
                }

                recReader.Close();
                dataTable.Rows.Add(row);
            }
            _br.Close();
        }
        private Dictionary<int, string> ReadDBT(ref Dictionary<int, double[]> resultDouble)
        {
            StreamWriter wr = Study._StreamWriter;
            wr.WriteLine($"\n\nBegin Processing DBT file.");

            Dictionary<int, string> result = new Dictionary<int, string>();

            //------------------------------------------------------------------------
            //rdc temp;rdc critical
            long numValDouble = 0;
            Array theArray = null;
            long numDoubleValues = 10000;
            int bytesToRead = 0;
            int bytesToReadMax = 64000;
            //------------------------------------------------------------------------


            string dbtFile =
                Path.GetDirectoryName(mp_fileNameFull) + @"\" + Path.GetFileNameWithoutExtension(mp_fileNameFull) + ".dbt";

            if (!File.Exists(dbtFile))
                return result;

            BinaryReader br = null;
            try
            {
                br = new BinaryReader(File.OpenRead(dbtFile));

                // Read the header into a buffer
                byte[] buffer = br.ReadBytes(Marshal.SizeOf(typeof(DBTHeader)));

                // Marshall the header into a DBTHeader structure
                GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                DBTHeader header = (DBTHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(DBTHeader));
                handle.Free();

                int currentBlock = 1;
                int currentBlockWrite = 0;


                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    // Position reader at beginning of current block
                    br.BaseStream.Position = header.blockLength * currentBlock;

                    // Read the memo field header into a buffer
                    buffer = br.ReadBytes(Marshal.SizeOf(typeof(MemoHeader)));

                    // Marshall the header into a MemoHeader structure
                    handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                    MemoHeader memHeader = (MemoHeader)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(MemoHeader));
                    handle.Free();

                    if (memHeader.startPosition != 8)
                    {
                        WriteLine($"Failure to get good start \t Start position: {memHeader.startPosition}\t Current Block: {currentBlock}");
                        wr.WriteLine($"Failure to get good start \t Start position: {memHeader.startPosition}\t Current Block: {currentBlock}");
                        currentBlock++;
                        continue;
                    }


                    bytesToRead = memHeader.fieldLength - memHeader.startPosition;
                    //String value = Encoding.ASCII.GetString(br.ReadBytes(bytesToRead));

                    /*
                    Write($"\tCurrent block: {currentBlock.ToString().PadLeft(10)}");       //rdc temp
                    Write($"\tmemHeader.startPosition ({memHeader.startPosition})");   //rdc temp
                    Write($"\tmemHeader.fieldLength ({memHeader.fieldLength})");   //rdc temp
                    WriteLine($"\tBytes to Read: { bytesToRead.ToString().PadLeft(12)}");   //rdc temp
                    */


                    if (bytesToRead > int.MaxValue)
                    {
                        WriteLine($"\nError, number of bytes to read ({bytesToRead}) exceeds the size of integer ({int.MaxValue})");
                        wr.WriteLine($"\nError, number of bytes to read ({bytesToRead}) exceeds the size of integer ({int.MaxValue})");
                    }
                    else if (bytesToRead > bytesToReadMax)
                    {
                        WriteLine($"\nError, number of bytes to read ({bytesToRead}) exceeds {bytesToReadMax}.");
                        WriteLine($"memHeader.startPosition ({memHeader.startPosition})\tmemheader.fieldLength {memHeader.fieldLength}).");
                        wr.WriteLine($"\nError, number of bytes to read ({bytesToRead}) exceeds {bytesToReadMax}.");
                        wr.WriteLine($"memHeader.startPosition ({memHeader.startPosition})\tmemheader.fieldLength {memHeader.fieldLength}).");
                    }

                    //--------------------------------------------------------------------
                    //rdc temp;rdc critical;24Oct2018; Try to convert to doubles for WSP

                    if (bytesToRead < 30000)
                    {

                        byte[] theBytes = br.ReadBytes(bytesToRead);

                        theArray = new double[numDoubleValues];
                        String value = Encoding.ASCII.GetString(theBytes);
                        String theValueByteString = theBytes.ToString();

                        //WriteLine($"\tThe string: {value}");   //rdc temp;rdc critical

                        //Buffer.BlockCopy(br.ReadBytes(bytesToRead), 0, theArray, 0, bytesToRead);
                        Buffer.BlockCopy(theBytes, 0, theArray, 0, bytesToRead);

                        //theDoubleArray[0] = (double)theArray[0];
                        //theDoubleArray = Convert.ToDouble(theArray);
                        numValDouble = bytesToRead / sizeof(double);
                        double[] theDoubleArray = new double[numValDouble];

                        for (int j = 0; j < numValDouble; j++)
                            theDoubleArray[j] = (double)theArray.GetValue(j);
                        //--------------------------------------------------------------------





                        currentBlockWrite++;
                        //result.Add(currentBlockWrite, value);
                        result.Add(currentBlock, value);
                        resultDouble.Add(currentBlock, theDoubleArray);
                        wr.WriteLine($"Current Block: {currentBlock}\t Bytes Read: {bytesToRead}");
                        wr.WriteLine($"Values\n {value}");
                    }

                    // Find next block
                    while (br.BaseStream.Position > (header.blockLength * currentBlock))
                        currentBlock++;
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n\nFailure to convert memofield.");
                WriteLine($"Number of required double values: {numValDouble}");
                WriteLine($"Bytes to read: {bytesToRead}");
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                if (null != br)
                    br.Close();
            }
            return result;
        }
        public void PrintDataTableVertical( DataTable dataTable)
        {
            //  Print each field in the current row of the DataTable
            int numCols = dataTable.Columns.Count;
            int numRows = dataTable.Rows.Count;
            for (int irow = 0; irow < numRows; irow++)
            {
                Console.WriteLine($"\nRecord: {irow + 1}");
                for (int icol = 0; icol < numCols; icol++)
                {
                    Console.Write($"{Convert.ToString(icol + 1).PadLeft(5)}   ");
                    Console.Write($"{dataTable.Columns[icol].ColumnName.PadRight(12)}");
                    Object cellValue = dataTable.Rows[irow][icol];
                    Console.Write($"  {cellValue}");
                    Console.WriteLine("");
                }
            }
        }
        public void ProcessDataTable(DataTable dataTable)
        {
            //  The DataTable contains all the records in the dbf file
            //  This routine checks the file name and then calls the proper method for
            //      for the datatype determined from the filename
            if (mp_fileNameBase == "STUDY")
            {
                StudyDataTable sdt = new StudyDataTable();
                sdt.ProcessStudyDataTable(dataTable);
            }
            else if (mp_fileNameBase == "NEXTID")
            {
                NextObjIdDataTable nxObjId = new NextObjIdDataTable();
                nxObjId.ProcessNextObjIdTable(dataTable);
            }
            else if (mp_fileNameBase == "ECONPARM")
            {
                EconGlobalParametersDataTable econParamDT = new EconGlobalParametersDataTable();
                econParamDT.ProcessEconGlobalParametersTable(dataTable);
            }
            else if (mp_fileNameBase == "PLAN")
            {
                PlanDataTable planDT = new PlanDataTable();
                planDT.ProcessPlanDataTable(dataTable);
            }
            else if (mp_fileNameBase == "YEAR")
            {
                YearDataTable yearDT = new YearDataTable();
                yearDT.ProcessYearDataTable(dataTable);
            }
            else if (mp_fileNameBase == "STREAM")
            {
                StreamDataTable streamDT = new StreamDataTable();
                streamDT.ProcessStreamDataTable(dataTable);
            }
            else if (mp_fileNameBase == "IMPAREA")
            {
                ReachDataTable reachDT = new ReachDataTable();
                reachDT.ProcessReachDataTable(dataTable);
            }
            else if (mp_fileNameBase == "RunsOn")
            {
                CategoryDataTable categoryDT = new CategoryDataTable();
                categoryDT.ProcessCategoryDataTable(dataTable);
            }
            else if (mp_fileNameBase == "ECONPY")
            {
                EconPYTable econPYDT = new EconPYTable();
                econPYDT.ProcessEconPYDataTable(dataTable);
            }
            else if (mp_fileNameBase == "SGRPDATA")
            {
                StructureModuleDataTable strucModuleDT = new StructureModuleDataTable();
                strucModuleDT.ProcessStructureModulesTable(dataTable);
            }
            else if (mp_fileNameBase == "OCCTYPE")
            {
                OccupancyTypeDataTable occtypeDT = new OccupancyTypeDataTable();
                occtypeDT.ProcessOccupancyTypeDataTable(dataTable);
            }
            else if (mp_fileNameBase == "WSPDATA")
            {
                WspDataTable wspDT = new WspDataTable();
                wspDT.ProcessWspDataTable(dataTable);
            }
            else if (mp_fileNameBase == "PROBDATA")
            {
                ProbabilityDataTable pdt = new ProbabilityDataTable();
                pdt.ProcessProbdataDataTable(dataTable);
            }
            else if (mp_fileNameBase == "RATDATA")
            {
                RatingDataTable rdt = new RatingDataTable();
                rdt.ProcessRatingDataTable(dataTable);
            }
            else if (mp_fileNameBase == "LEVDATA")
            {
                LeveeDataTable leveeDT = new LeveeDataTable();
                leveeDT.ProcessProbdataDataTable(dataTable);
            }
            else if (mp_fileNameBase == "STRUCT")
            {
                StructureDataTable strucDT = new StructureDataTable();
                strucDT.ProcessStructureDataTable(dataTable);
            }
            else if (mp_fileNameBase == "SDMGDATA")
            {
                StageDamageDataTable stageDamgDT = new StageDamageDataTable();
                stageDamgDT.ProcessStageDamageDataTable(dataTable);
            }
            else if (mp_fileNameBase == "EADDATA")
            {
                EadResultsDataTable eadResultsDT = new EadResultsDataTable();
                eadResultsDT.ProcessEadResultsDataTable(dataTable);
            }
            else if (mp_fileNameBase == "EQUIVBEN")
            {
                EquivBenefitDataTable equivBenDT = new EquivBenefitDataTable();
                equivBenDT.ProcessEquivBenefitDataTable(dataTable);
            }
            else if (mp_fileNameBase == "SGRPLOOK")
            {
                StrucGroupLookupDataTable strucGroupDT = new StrucGroupLookupDataTable();
                strucGroupDT.ProcessStrucGroupLookDataTable(dataTable);
            }

            else if (mp_fileNameBase == "WSPLOOK")
            {
                WspLookupDataTable wspLookupDT = new WspLookupDataTable();
                wspLookupDT.ProcessWspLookDataTable(dataTable); ;
            }
            else if (mp_fileNameBase == "PROBLOOK")
            {
                ProbLookupDataTable probLookupDT = new ProbLookupDataTable();
                probLookupDT.ProcessProbLookDataTable(dataTable);
            }
            else if (mp_fileNameBase == "RATLOOK")
            {
                RatingLookupDataTable rateLookDT = new RatingLookupDataTable();
                rateLookDT.ProcessRateLookDataTable(dataTable);
            }
            else if (mp_fileNameBase == "LEVLOOK")
            {
                LeveeLookupDataTable leveeLookDT = new LeveeLookupDataTable();
                leveeLookDT.ProcessLeveeLookDataTable(dataTable);
            }
            else if (mp_fileNameBase == "SDMGLOOK")
            {
                AggDamgLookupDataTable aggDmgLookDT = new AggDamgLookupDataTable();
                aggDmgLookDT.ProcessAggDamgLookDataTable(dataTable);
            }
            else if (mp_fileNameBase == "EADLOOK")
            {
                EadResultsLookupDataTable eadLookDT = new EadResultsLookupDataTable();
                eadLookDT.ProcessEadLookDataTable(dataTable);
            }
        }













        /*


        int m_recWidth;

        long m_savedRecordNumber;
        int m_append;

        //Type of data file (if applicable)
        char m_typeOfPysrc;


        //Data for maintaining linked lists of field4 and tag4 defninitions & data
        //............................................................................
        int m_numFields;
        int m_numTags;


        DbfFieldInfo mp_dbfFieldInfo;  //
        DbfTagInfo mp_dbfTagInfo;    //

        int m_traceMemoField;

        int m_sizeList;
        int m_sizeObj;


        //Methods
        public DbfFileManager()
        {
            //mp_data4 = NULL;
            constructInitialize();
        }
        public DbfFileManager(char typePysrc)
        {
            //mp_data4 = NULL;
            constructInitialize();
            m_typeOfPysrc = typePysrc;
        }
        public DbfFileManager(ref DbfFileManager theMgr)
        {
        }

        //ref DbfFileManager   operator=(ref DbfFileManager dbfMgr); //Outlaw copying





    Exception create()
    { }
    Exception create(char* subdirectory,
                            int numFields,
                            FIELD4INFO* fields,
                            int numTags,
                            TAG4INFO* sortTags);
Exception open();
Exception openNoMapping();
Exception mapMembers();
Exception close();
void flushNewData();
virtual Exception flushFile();
int flush();

//Wayne said that lock / unlock doesn't work
//Removed them but GUI calls them so reactivated them as a do nothing
Exception     lock()   {return NO_PROBLEM;}
Exception unlock() { return NO_PROBLEM; }

Exception indexBy(int);
Exception indexBy(const char* descriptor);

virtual Exception saveIndexSettings();
virtual Exception restoreIndexSettings();

void constructInitialize();

virtual void setFieldsAndTags() {; }
virtual void computeFieldIndices() {; }
virtual void mapFields1() {; }
void getFieldDefinitions();

//File Related
DATA4* getData4() { return mp_data4; }
void setData4(DATA4* d4) { mp_data4 = d4; }

char* getFileNameAlias() { return m_alias; }
int getRecordWidth() { return m_recWidth; }

//RecordNavigatorManager*     getRecordNavigatorManager() {return (RecordNavigatorManager*)mp_recNavMgrPtr;}
//RecordNavigatorDataManager* getRecordNavigatorDataManager() {return (RecordNavigatorDataManager*)mp_recNavMgrPtr;}

//RecordNavigatorManager*     setRecordNavigatorManager(RecordNavigatorManager& theRecNavDataMgr);
//RecordNavigatorDataManager* setRecordNavigatorManager(RecordNavigatorDataManager& theRecNavDataMgr);

void setTypeOfPysrc(char typeOfPysrc = ' ') { m_typeOfPysrc = (char)toupper(typeOfPysrc); }

char getTypeOfPysrc() { return m_typeOfPysrc; }

int buildDbfFieldInfo();
int buildDbfFieldInfoM();

int getFieldIndex(const char* dataDescriptor);
int getTagIndex(const char* tagDescriptor);

DbfFieldInfo* setField4Info(const int numFields, const FIELD4INFO_BOB* f4ibob);
DbfFieldInfo* setFieldDescriptors(const FIELD4INFO_BOB* f4ibob);

TAG4INFO* setTag4Info(const int numTags, const TAG4INFO_BOB* tag4info);
TAG4INFO* setTagDescriptors(const TAG4INFO_BOB* tag4info);

FIELD4INFO* buildField4InfoList();
TAG4INFO* buildTag4InfoList();

DbfFieldInfo* getDbfFieldInfo();
DbfFieldInfo* getDbfFieldInfo(int ixField);

FIELD4INFO* getField4Info();
FIELD4INFO* getField4Info(int ixField);
TAG4INFO* getTagInfo();
TAG4INFO getTagInfo(int ixField);

int getNumFieldsInfo();

DbfTagInfo* getDbfTagInfo() { return mp_dbfTagInfo; }
DbfTagInfo* getDbfTagInfo(int ixField) { return &mp_dbfTagInfo[ixField]; }

int getNumFields();
int getNumTags();

Exception selectTagForDescriptor(char* descriptor);
Exception selectTagForTagName(char* tagName);

//These get FIELD4INFO for current record
FIELD4INFO getFieldDefinition();

FIELD4INFO getFieldDefinition(const int ixField); //starts at one

FIELD4INFO getFieldDefinition(const int ixField,
                    DbfFieldInfo &fieldDef ); //starts at one

FIELD4INFO getFieldDefinition(const int ixField,
                               char* name,
                               char &type,
                               int &length,
                               int &decimal);

FIELD4INFO* getExistingDbfFields(CODE4* p_code4);
FIELD4INFO* getExistingDbfFields();

TAG4INFO* getExistingTagFields();
void printTagFields(ofstream &w);

void getDbfFieldData(const long ixRec = 1L);


FIELD4* getField4(const int ixField);
FIELD4* getField4(const char* fieldDescriptor);
TAG4* getTag4(const int ixField);
TAG4* getTag4(const char* tagDescriptor);

int lookupDataFieldDescriptor(char* descriptor);
int lookupTagDescriptor(char* descriptor);

void resetDbfFieldInfoData();

//............................................................................
//Finds if a record in the file matches the input ID (or name) for the
//descriptor field.
int usesID(long idInp, char* descriptorLookup);
int usesName(char* name, char* descriptor, int nameSize);

long getRecordNumber();
Exception deleteRecordWithoutPacking();
Exception deleteAllRecords();
Exception deleteRecord();
Exception deleteRecord(long& recordNumber);
virtual Exception pack();

//Navigation
Exception goTo(long recordNumber);

virtual Exception first();
virtual Exception first(long& newRecordNumber);

virtual Exception last();
virtual Exception last(long& newRecordNumber);

virtual Exception previous();
virtual Exception previous(long oldRecordRecordNumber, long& newRecordNumber);

virtual Exception next();
virtual Exception next(long oldRecordRecordNumber, long& newRecordNumber);

//Read / Write Records
Exception isMemoFieldDataBad(FIELD4* f4, int nrow, int ncol);
Exception readRecord();
Exception readRecord(const int traceMemo);
void setLegacyMemoRows(int ixNrow, int ixParam, int numColumns = 1);
Exception readRecordLegacyMemo();
Exception writeRecord(int append);

Exception mapFields();
Exception mapTags();

long getCount();        // of records in the file
long getNumberOfRecordsDb();
long getNumberOfRecordsFiltered();
long refreshTheList(const int forceRefresh = 0);
*/
    }
}
