//using FdaViewModel.Inventory.DepthDamage;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace FdaViewModel.Inventory.OccupancyTypes
//{
//    public class OccTypeReader
//    {
//        private string _filePath;
//        private List<DepthDamageCurve> _SingleDamageFunction;
//        private double[] _TheStageGlobal;
//        //_TheData gets loaded in AsciiImport. Proccess OccTypeFields
//        private double[] _TheData;
//        //the error type should be IOrdinateEnum

//        private bool UsesDollar;

//        public OccTypeReader(string occtypeFilePath)
//        {
//            _filePath = occtypeFilePath;
//        }
//        public  OccupancyTypeGroup Read(string occtypeFilePath)
//        {
//            //OccupancyTypeGroup group = new OccupancyTypeGroup()

//            ImportAsciiData(occtypeFilePath);
//            return null;
//        }

//        protected bool _PrevKeyRecord = false;


//        public void ImportAsciiData(string theImportFilename)
//        {
//            char delimiterChar = '\t';

//            FileStream fileStreamImport = File.OpenRead(theImportFilename);
//            StreamReader reader = new StreamReader(fileStreamImport);

//            _InpString = "";
//            _InpStringCap = "";
//            UsesDollar = false;

//            while (_InpString != null)
//            {
//                _InpString = reader.ReadLine();

//                if (_InpString == null)
//                {
//                    //Check for flushing 
//                    FlushOccupancyType("", "", "");
//                    continue;
//                }
//                else if (_InpString.Length < 1)
//                {
//                    continue;
//                }
//               // WriteLine($"Input Line->{_InpString}");
//                string inpStringTrimmed = _InpString.TrimEnd();
//                _InpStringCap = inpStringTrimmed.ToUpper();


//                if (inpStringTrimmed.Length < 1)
//                    continue;

//                //Parse the input line
//                _InputStringParsed = inpStringTrimmed.Split(delimiterChar);
//                _NumFieldsInputString = _InputStringParsed.Length;
//                _InputStringCapParsed = _InpStringCap.Split(delimiterChar);

//                //WriteLine($"Number of fields: {_NumFieldsInputString}");
//                //for (int i = 0; i < _NumFieldsInputString; i++)
//                //{
//                //    WriteLine($"Field: {i},\tContent: {_InputStringParsed[i]}");
//                //}

//                //Check for Keyword, Flush if necessary
//                if (IsKeyRecord(_InputStringParsed[0].ToUpper()))
//                {
//                    //Flush any existing data
//                    //!ToDo flushOccupancyType(,,) Do these need to be flushed? No, flushed if Occ Name changes

//                    FindFields();
//                    _PrevKeyRecord = true;
//                }
//                //Process Data Fields
//                else
//                {
//                    ProcessOcctypeFields();
//                    _PrevKeyRecord = false;
//                }
//            }
//            reader.Close();
         
//        }

//        void FindFields()
//        {
//            switch (_TheImportCode)
//            {
                
//                case TheImportCode.OCC_NAME:
//                    FindOcctypeNameFields();
//                    break;         
//                default:
//                    break;
//            }
//        }

//        public void FlushOccupancyType(string name, string description, string nameCategory)
//        {

//            //if (_FlushOccType)
//            //{
//            //    _OccupancyType.UsesDollar = UsesDollar;  //!ToDo need to check single damage functions but it's global
//            //    _OccupancyType.SetSingleDamageFunction(StructureValueType.STRUCTURE, this._SingleDamageFunction[(int)StructureValueType.STRUCTURE]);
//            //    _OccupancyType.SetSingleDamageFunction(StructureValueType.CONTENT, this._SingleDamageFunction[(int)StructureValueType.CONTENT]);
//            //    _OccupancyType.SetSingleDamageFunction(StructureValueType.OTHER, this._SingleDamageFunction[(int)StructureValueType.OTHER]);
//            //    _OccupancyType.SetSingleDamageFunction(StructureValueType.CAR, this._SingleDamageFunction[(int)StructureValueType.CAR]);

//            //    if (_OccupancyType.Name != "")
//            //    {
//            //        GlobalVariables.mp_fdaStudy.GetOccupancyTypeList().Add(_OccupancyType);

//            //    }
//            //}

//            //_OcctypePrevName = name.ToUpper();

//            ////Reset
//            //UsesDollar = false;
//            //_OccupancyType.Reset();
//            //for (int i = 0; i < 5; i++) _SingleDamageFunction[i].Reset();
//            //_OccupancyType.Description = description;
//            //_OccupancyType.CategoryName = nameCategory;
//        }


//        private string _ParamOcctype;
//        private string _ParamOcctypeCap;
        
//        private int[] _InputColumnMapping = new int[5];
//        static public string[] _columnHeaders = { "OCC_NAME", "OCC_DESCRIPTION", "CAT_NAME", "PARAMETER", "START_DATA" };
//        protected int _NumFieldsInputString;
//        private string[] _InputStringCapParsed;
//        protected string[] _InputStringParsed;
//        private int _IParameterOcctype = -1;
//        private int _IStartOcctype = -1;
//        private string _OcctypePrevName = "NO PREV NAME";
//        private OccupancyType _OccupancyType = new OccupancyType();
//        private bool _FlushOccType = false;
//        private int _NumValTheData = 0;
//        private string _InpString;
//        private string _InpStringCap;

//        void ProcessOcctypeFields()
//        {
//            //fieldsOcctype = {"OCC_NAME", "OCC_DESCRIPTION", "CAT_NAME", "PARAMETER", "START_DATA" }
//            _ParamOcctype = "";
//            _ParamOcctypeCap = "";
//            string name = "";
//            string description = "";
//            string nameCategory = "";

//            //we don't know that the column headers being read in will be in the same order

//            for (int colIndex = 0; colIndex < _columnHeaders.Length; colIndex++)
//            {
//                //we are mapping the known column index (colIndex) to the input column
//                //index that has the same name (inputColIndex)
//                int inputColIndex = _InputColumnMapping[colIndex];
//                if (inputColIndex > -1 & inputColIndex < _NumFieldsInputString)
//                {
//                    string inputString = _InputStringParsed[inputColIndex];
//                    int ncInp = inputString.Length;

//                    switch (colIndex)
//                    {
//                        case 0:
//                            name = _InputStringParsed[inputColIndex];
//                            break;
//                        case 1:
//                            if (ncInp > 0) description = inputString;
//                            break;
//                        case 2:
//                            if (ncInp > 0) nameCategory = inputString;
//                            break;
//                        case 3:
//                            _IParameterOcctype = inputColIndex;
//                            _ParamOcctype = inputString;
//                            _ParamOcctypeCap = inputString.ToUpper();
//                            //check for valid entry
//                            break;
//                        case 4:
//                            _IStartOcctype = inputColIndex;
//                            //process remaining fields and store data
//                            break;
//                        default:
//                            break;
//                    }
//                }
//            }
//            //Occupancy Type Changes, Flush existing occupancy type
//            if (name.ToUpper() != _OcctypePrevName)
//            {
//                FlushOccupancyType(name, description, nameCategory);
//            }
//            else
//            {
//            }
//            _OccupancyType.Name = name;

//            //Decipher Parameter Code and Read Data
//            if (_ParamOcctypeCap == "STRUCT")
//            {
//                //Process fields for structure information (Error in values, FF stage error)
//                //todo: uncomment
//                DetermineOcctypeStructureParameters();
//            }
//            //Anything else is data
//            else
//            {
//                //Read the data
//                int numOrds = _NumFieldsInputString - _IStartOcctype;
//                if (numOrds > 0) _FlushOccType = true;

//                //WriteLine($"numOrds: {numOrds}\tnumFieldsInputString: {_NumFieldsInputString}\tiStartOcctype: {_IStartOcctype}");
//                _TheData = new double[numOrds];
//                _NumValTheData = numOrds;
//                for (int jfl = _IStartOcctype; jfl < _NumFieldsInputString; jfl++)
//                {
//                    //WriteLine($"jfl {jfl},   Data: {_InputStringParsed[jfl]}");
//                    int jdata = jfl - _IStartOcctype;
//                    _TheData[jdata] = Convert.ToDouble(_InputStringParsed[jfl]);
//                }
//                //Decipher Parameter Code
//                int i = 0;
//                //M_usesDollar = false;
//                char[] paramCode = new char[5];
//                for (i = 0; i < 5; i++) paramCode[i] = ' ';
//                int numParamCode = _ParamOcctypeCap.Length;
//                for (i = 0; i < numParamCode; i++)
//                {
//                    paramCode[i] = _ParamOcctypeCap[i];
//                }
//                if (paramCode[1] == '$') UsesDollar = true; //Set to dollar function
//                //todo: uncomment
//                _OccupancyType.UsesDollar = UsesDollar;

//                //Determine the type of damage function data (S, SN, 
//                TheDamageFunctionType type = TheDamageFunctionType.ERROR;
//                //todo: uncomment
//                type = DetermineOcctypeFunctionType(paramCode);

//                //Read (or copy from theData) the function part
//                //todo: uncomment
//                TransferDataToFunction(type);

//            }
//        }

//        void DetermineOcctypeStructureParameters()
//        {
//            //Determines the parameters associated with the structure (first floor, S, C, O, A)
//            for (int jfl = _IStartOcctype; jfl < _NumFieldsInputString; jfl++)
//            {
//                if (_InputStringParsed[jfl].Length < 1)
//                    continue;

//                int iKeyStructData = jfl - _IStartOcctype;
//                string fieldCap = _InputStringCapParsed[jfl];

//                switch (iKeyStructData)
//                {
//                    //First Floor Information
//                    case 0:     //Distribution Type
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.FFLOOR).SetErrorType(fieldCap);
//                        break;
//                    case 1:     //Median value (not entered for FF and S
//                        break;
//                    case 2:     //Std. Dev for N or L; Lower for T
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.FFLOOR).SetStdDev(Convert.ToDouble(fieldCap));
//                        break;
//                    case 3:     //Upper for T
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.FFLOOR).SetUpper(Convert.ToDouble(fieldCap));
//                        break;
//                    //Structure Value 
//                    case 4:     //Distribution type
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.STRUCTURE).SetErrorType(fieldCap);
//                        break;
//                    case 5:     //Median value (not entered for Structure Value
//                        break;
//                    case 6:     //Std. Dev for N or L, Lower for T
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.STRUCTURE).SetStdDev(Convert.ToDouble(fieldCap));
//                        break;
//                    case 7:     //Upper for T
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.STRUCTURE).SetUpper(Convert.ToDouble(fieldCap));
//                        break;
//                    //Content Ratio Value
//                    case 8:     //Distribution type
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.CONTENT).SetErrorType(fieldCap);
//                        break;
//                    case 9:     //Content to structure value ratio
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.CONTENT).SetCentralValue(Convert.ToDouble(fieldCap));
//                        break;
//                    case 10:    //Std. Dev for N or L, Lower for T
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.CONTENT).SetStdDev(Convert.ToDouble(fieldCap));
//                        break;
//                    case 11:    //Upper for T
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.CONTENT).SetUpper(Convert.ToDouble(fieldCap));
//                        break;
//                    //Other Ratio Value
//                    case 12:    //Distribution Type
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.OTHER).SetErrorType(fieldCap);
//                        break;
//                    case 13:    //Other to structure value ratio
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.OTHER).SetCentralValue(Convert.ToDouble(fieldCap));
//                        break;
//                    case 14:    //Std. Dev for N or L, Lower for T
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.OTHER).SetStdDev(Convert.ToDouble(fieldCap));
//                        break;
//                    case 15:    //Upper for T
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.OTHER).SetUpper(Convert.ToDouble(fieldCap));
//                        break;
//                    //Automobile Value Ratio
//                    case 16:    //Distribution Type
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.AUTO).SetErrorType(fieldCap);
//                        break;
//                    case 17:    //Automobile to structure value ratio
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.AUTO).SetCentralValue(Convert.ToDouble(fieldCap));
//                        break;
//                    case 18:    //Std. Dev for N or L, Lower for T
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.AUTO).SetStdDev(Convert.ToDouble(fieldCap));
//                        break;
//                    case 19:    //Upper for T
//                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.AUTO).SetUpper(Convert.ToDouble(fieldCap));
//                        break;
//                    default:
//                        break;
//                }
//            }

//        }

//        void TransferDataToFunction(TheDamageFunctionType type)
//        {
//            //Transfer data from temporary array theData into the specific
//            //  array in the single damage function. 
//            //Important, the stage (depth) values must be entered before the damage and
//            //  error distribution values.
//            switch (type)
//            {
//                case TheDamageFunctionType.STAGE:
//                    _TheStageGlobal = new double[_TheData.Length];
//                    for (int i = 0; i < _TheData.Length; i++) _TheStageGlobal[i] = _TheData[i];
//                    break;
//                case TheDamageFunctionType.S:
//                    _SingleDamageFunction[0].SetNumRows(_TheData.Length, ErrorType.NONE);
//                    _SingleDamageFunction[0].SetDamage(_TheData);
//                    _SingleDamageFunction[0].SetDepth(_TheStageGlobal);
//                    if (UsesDollar) _SingleDamageFunction[0].DirectDollar = UsesDollar;
//                    break;
//                case TheDamageFunctionType.SN:
//                    _SingleDamageFunction[0].SetNumRows(_TheData.Length, ErrorType.NORMAL);
//                    _SingleDamageFunction[0].SetStdDev(_TheData);
//                    _SingleDamageFunction[0].SetDepth(_TheStageGlobal);
//                    break;
//                case TheDamageFunctionType.SL:
//                    _SingleDamageFunction[0].SetNumRows(_TheData.Length, ErrorType.LOGNORMAL);
//                    _SingleDamageFunction[0].SetStdDev(_TheData);
//                    _SingleDamageFunction[0].SetDepth(_TheStageGlobal);
//                    break;
//                case TheDamageFunctionType.STL:
//                    _SingleDamageFunction[0].SetNumRows(_TheData.Length, ErrorType.TRIANGULAR);
//                    _SingleDamageFunction[0].SetStdDev(_TheData);
//                    _SingleDamageFunction[0].SetDepth(_TheStageGlobal);
//                    break;
//                case TheDamageFunctionType.STU:
//                    _SingleDamageFunction[0].SetNumRows(_TheData.Length, ErrorType.TRIANGULAR);
//                    _SingleDamageFunction[0].SetTriangularUpper(_TheData);
//                    _SingleDamageFunction[0].SetDepth(_TheStageGlobal);
//                    break;
//                case TheDamageFunctionType.C:
//                    _SingleDamageFunction[1].SetNumRows(_TheData.Length, ErrorType.NONE);
//                    _SingleDamageFunction[1].SetDamage(_TheData);
//                    _SingleDamageFunction[1].SetDepth(_TheStageGlobal);
//                    if (UsesDollar) _SingleDamageFunction[1].DirectDollar = UsesDollar;
//                    break;
//                case TheDamageFunctionType.CN:
//                case TheDamageFunctionType.CL:
//                case TheDamageFunctionType.CTL:
//                    _SingleDamageFunction[1].SetStdDev(_TheData);
//                    _SingleDamageFunction[1].SetDepth(_TheStageGlobal);
//                    switch (type)
//                    {
//                        case TheDamageFunctionType.CN:
//                            _SingleDamageFunction[1].SetNumRows(_TheData.Length, ErrorType.NORMAL);
//                            break;
//                        case TheDamageFunctionType.CL:
//                            _SingleDamageFunction[1].SetNumRows(_TheData.Length, ErrorType.LOGNORMAL);
//                            break;
//                        case TheDamageFunctionType.CTL:
//                            _SingleDamageFunction[1].SetNumRows(_TheData.Length, ErrorType.TRIANGULAR);
//                            break;
//                    }

//                    break;
//                case TheDamageFunctionType.CTU:
//                    _SingleDamageFunction[1].SetTriangularUpper(_TheData);
//                    _SingleDamageFunction[1].SetDepth(_TheStageGlobal);
//                    break;
//                case TheDamageFunctionType.O:
//                    _SingleDamageFunction[2].SetNumRows(_TheData.Length, ErrorType.NONE);
//                    _SingleDamageFunction[2].SetDamage(_TheData);
//                    _SingleDamageFunction[2].SetDepth(_TheStageGlobal);
//                    if (UsesDollar) _SingleDamageFunction[2].DirectDollar = UsesDollar;
//                    break;
//                case TheDamageFunctionType.ON:
//                case TheDamageFunctionType.OL:
//                case TheDamageFunctionType.OTL:
//                    _SingleDamageFunction[2].SetStdDev(_TheData);
//                    _SingleDamageFunction[2].SetDepth(_TheStageGlobal);
//                    switch (type)
//                    {
//                        case TheDamageFunctionType.ON:
//                            _SingleDamageFunction[2].SetNumRows(_TheData.Length, ErrorType.NORMAL);
//                            break;
//                        case TheDamageFunctionType.OL:
//                            _SingleDamageFunction[2].SetNumRows(_TheData.Length, ErrorType.LOGNORMAL);
//                            break;
//                        case TheDamageFunctionType.OTL:
//                            _SingleDamageFunction[2].SetNumRows(_TheData.Length, ErrorType.TRIANGULAR);
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case TheDamageFunctionType.OTU:
//                    _SingleDamageFunction[2].SetTriangularUpper(_TheData);
//                    _SingleDamageFunction[2].SetDepth(_TheStageGlobal);
//                    break;
//                case TheDamageFunctionType.A:
//                    _SingleDamageFunction[3].SetNumRows(_TheData.Length, ErrorType.NONE);
//                    _SingleDamageFunction[3].SetDamage(_TheData);
//                    _SingleDamageFunction[3].SetDepth(_TheStageGlobal);
//                    if (UsesDollar) _SingleDamageFunction[3].DirectDollar = UsesDollar;
//                    break;
//                case TheDamageFunctionType.AN:
//                case TheDamageFunctionType.AL:
//                case TheDamageFunctionType.ATL:
//                    _SingleDamageFunction[3].SetStdDev(_TheData);
//                    _SingleDamageFunction[3].SetDepth(_TheStageGlobal);
//                    switch (type)
//                    {
//                        case TheDamageFunctionType.AN:
//                            _SingleDamageFunction[3].SetNumRows(_TheData.Length, ErrorType.NORMAL);
//                            break;
//                        case TheDamageFunctionType.AL:
//                            _SingleDamageFunction[3].SetNumRows(_TheData.Length, ErrorType.LOGNORMAL);
//                            break;
//                        case TheDamageFunctionType.ATL:
//                            _SingleDamageFunction[3].SetNumRows(_TheData.Length, ErrorType.TRIANGULAR);
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                case TheDamageFunctionType.ATU:
//                    _SingleDamageFunction[3].SetTriangularUpper(_TheData);
//                    _SingleDamageFunction[3].SetDepth(_TheStageGlobal);
//                    break;
//                default:
//                    break;
//            }
//        }
//        void FindOcctypeNameFields()
//        {
//            //Process all valid fields to match valid fields to input fields
//            for (int ip = 0; ip < _columnHeaders.Length; ip++)
//            {
//                _InputColumnMapping[ip] = -1;
//                //Find match of input field with parameter fields
//                bool match = false;
//                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
//                {
//                    if (_columnHeaders[ip].Equals(_InputStringCapParsed[jf]))
//                    {
//                        match = true;
//                        _InputColumnMapping[ip] = jf;
//                    }
//                }
//            }
//        }

//        protected TheImportCode _TheImportCode;
//        private TheImportCode _CurrentImportDataCode;

//        bool IsKeyRecord(string theField)
//        {
//            bool itisKeyRecord = false;
//            _TheImportCode = TheImportCode.NO_KEY;

//            if (theField == "PLAN_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.PLAN_NAME;
//            }
//            else if (theField == "YEAR_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.YEAR_NAME;
//            }
//            else if (theField == "STRM_NME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.STRM_NME;
//            }
//            else if (theField == "RCH_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.RCH_NAME;
//            }
//            else if (theField == "CAT_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.CAT_NAME;
//            }
//            else if (theField == "OCC_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.OCC_NAME;
//            }
//            else if (theField == "MOD_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.MOD_NAME;
//            }
//            else if (theField == "STRUC_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.STRUC_NAME;
//            }
//            else if (theField == "WSP_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.WSP_NAME;
//            }
//            else if (theField == "FREQ_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.FREQ_NAME;
//            }
//            else if (theField == "RATE_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.RATE_NAME;
//            }
//            else if (theField == "LEVEE_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.LEVEE_NAME;
//            }
//            else if (theField == "SD_NAME")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.SD_NAME;
//            }
//            else if (theField == "SM_PLAN")
//            {
//                itisKeyRecord = true;
//                _TheImportCode = TheImportCode.SM_PLAN;
//            }
//            else
//            {
//                itisKeyRecord = false;
//                _TheImportCode = TheImportCode.NO_KEY;
//            }
//            //Key Record, Flush Records and set code
//            if (itisKeyRecord)
//            {
//                //Flush
//                //Set current operation
//                _CurrentImportDataCode = _TheImportCode;
//            }
//            return itisKeyRecord;
//        }

//        public enum TheImportCode
//        {
//            NO_KEY = 0,
//            PLAN_NAME,
//            YEAR_NAME,
//            STRM_NME,
//            RCH_NAME,
//            CAT_NAME,
//            OCC_NAME,
//            MOD_NAME,
//            STRUC_NAME,
//            WSP_NAME,
//            FREQ_NAME,
//            RATE_NAME,
//            LEVEE_NAME,
//            SD_NAME,
//            SM_PLAN
//        };

//        public enum TheDamageFunctionType
//        {
//            ERROR,
//            STAGE,
//            S,
//            SN,
//            SL,
//            STU,
//            STL,
//            C,
//            CN,
//            CL,
//            CTU,
//            CTL,
//            O,
//            ON,
//            OL,
//            OTU,
//            OTL,
//            A,
//            AN,
//            AL,
//            ATU,
//            ATL
//        };

//    }
//}
