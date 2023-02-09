using System;
using System.Collections.Generic;
using System.IO;
using static Importer.ProbabilityFunction;
using static System.Console;

namespace Importer
{
    [Serializable]
    public class AsciiImport : AsciiImportExport
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        private int _IParameterOcctype = -1;
        private int _IStartOcctype = -1;
        private string _ParamOcctype = "";
        private string _ParamOcctypeCap = "";

        private int _IxWspInvert = -1;
        private int _IxWspStage = -1;
        private int _IxWspQ = -1;

        private bool _MustFlushProbFunc = false;
        private bool _MustFlushRatingFunc = false;
        private bool _MustFlushLevee = false;
        private bool _MustFlushWsp = false;
        private bool _PrevRecWspProb = false;
        private bool _MustFlushAggDamgFunc = false;

        private int[] _IxFieldPlan = new int[2];
        private int[] _IxFieldYear = new int[2];
        private int[] _IxFieldStream = new int[2];
        private int[] _IxFieldReach = new int[7];
        private int[] _IxFieldCategory = new int[3];
        private int[] _IxFieldOcctype = new int[5];
        private int[] _IxFieldModule = new int[2];
        private int[] _IxFieldStructure = new int[33];
        private int[] _IxFieldProbFunc = new int[10];
        private int[] _IxFieldRatingFunc = new int[12];
        private int[] _IxFieldLevee = new int[7];
        private int[] _IxFieldWsp = new int[7];
        private int[] _IxFieldAgDamgFunc = new int[7];

        private double[] _TheData;
        private int _NumValTheData = 0;
        private int _NumValAllocTheData = 0;
        private double[] _TheStageGlobal;

        private string _OcctypePrevName = "NO PREV NAME";
        private bool _FlushOccType = false;
        private WspSectionData _WspSectData = null;
        protected bool _PrevKeyRecord = false;

        public enum ImportOptions
        {
            ImportEverything,
            ImportOcctypes,
            ImportRatings,
            ImportInflowOutflow,
            ImportStageDamages,
            ImportExteriorInterior,
            ImportLevees,
            ImportFrequency,
        }

        #endregion
        #region Properties
        private readonly AsyncLogger _Logger;
        public bool UsesDollar
        { get; set; }
        #endregion
        #region Constructors
        public AsciiImport(AsyncLogger logger)
        {
            _Logger = logger;
            //clear the fda study. Because it is public static it hangs around forever.
            //This might not be the first time we are importing during an open session, so the study should be cleared.
            GlobalVariables.mp_fdaStudy = new Study();
            _SingleDamageFunction = new SingleDamageFunction[5];
            for (int i = 0; i < 5; i++) _SingleDamageFunction[i] = new SingleDamageFunction();
        }
        #endregion
        #region Voids

        public void ImportAsciiData(string theImportFilename, ImportOptions importOptions)
        {
            char delimiterChar = '\t';

            FileStream fileStreamImport = File.OpenRead(theImportFilename);
            StreamReader reader = new StreamReader(fileStreamImport);

            _InpString = "";
            _InpStringCap = "";
            UsesDollar = false;

            while (_InpString != null)
            {
                _InpString = reader.ReadLine();

                if (_InpString == null)
                {
                    //Check for flushing 
                    FlushOccupancyType("", "", "");
                    FlushWspData();
                    FlushProbFunction();
                    FlushRatingFunction();
                    FlushLevee();
                    FlushAggDamgFunc();
                    continue;
                }
                else if (_InpString.Length < 1)
                {
                    continue;
                }
                WriteLine($"Input Line->{_InpString}");
                string inpStringTrimmed = _InpString.TrimEnd();
                _InpStringCap = inpStringTrimmed.ToUpper();


                if (inpStringTrimmed.Length < 1)
                    continue;

                //Parse the input line
                _InputStringParsed = inpStringTrimmed.Split(delimiterChar);
                _NumFieldsInputString = _InputStringParsed.Length;
                _InputStringCapParsed = _InpStringCap.Split(delimiterChar);

                WriteLine($"Number of fields: {_NumFieldsInputString}");
                for (int i = 0; i < _NumFieldsInputString; i++)
                {
                    WriteLine($"Field: {i},\tContent: {_InputStringParsed[i]}");
                }

                //Check for Keyword, Flush if necessary
                if (IsKeyRecord(_InputStringParsed[0].ToUpper()))
                {
                    //Flush any existing data
                    //!ToDo flushOccupancyType(,,) Do these need to be flushed? No, flushed if Occ Name changes
                    FlushWspData();
                    FlushProbFunction();
                    FlushRatingFunction();
                    FlushLevee();
                    FlushAggDamgFunc();

                    FindFields();
                    _PrevKeyRecord = true;
                }
                //Process Data Fields
                else
                {
                    ProcessFields();
                    _PrevKeyRecord = false;
                }
            }
            reader.Close();

            switch (importOptions)
            {
                case ImportOptions.ImportEverything:
                    PrintEverything();
                    break;
                case ImportOptions.ImportOcctypes:
                    PrintOcctypes();
                    break;
                case ImportOptions.ImportFrequency:
                    PrintFrequencyFunction();
                    break;
                case ImportOptions.ImportInflowOutflow:
                    PrintInflowOutflowFunction();
                    break;
                case ImportOptions.ImportRatings:
                    PrintRatings();
                    break;
                case ImportOptions.ImportExteriorInterior:
                    PrintExteriorInteriorFunctions();
                    break;
                case ImportOptions.ImportLevees:
                    PrintLevees();
                    break;
                case ImportOptions.ImportStageDamages:
                    PrintStageDamage();
                    break;
            }

            
        }

        #region print statements

        private void PrintFrequencyFunction()
        {
            ProbabilityFunctionList probFuncs = GlobalVariables.mp_fdaStudy.GetProbabilityFuncList();
            foreach (KeyValuePair<string, ProbabilityFunction> kvp in probFuncs.ProbabilityFunctions)
            {
                ProbabilityFunction pf = kvp.Value;
                FrequencyFunctionType typeID = pf.ProbabilityFunctionTypeId;
                if (typeID == FrequencyFunctionType.ANALYTICAL || typeID == FrequencyFunctionType.GRAPHICAL)
                {
                    pf.Print(_Logger);
                }
            }
        }

        private void PrintInflowOutflowFunction()
        {
            ProbabilityFunctionList probFuncs = GlobalVariables.mp_fdaStudy.GetProbabilityFuncList();
            foreach (KeyValuePair<string, ProbabilityFunction> kvp in probFuncs.ProbabilityFunctions)
            {
                ProbabilityFunction pf = kvp.Value;
                FrequencyFunctionType typeID = pf.ProbabilityFunctionTypeId;
                if (pf.NumberOfTransFlowPoints > 0)
                {
                    pf.Print(_Logger, ImportOptions.ImportInflowOutflow);
                }
            }
        }

        private void PrintRatings()
        {
            GlobalVariables.mp_fdaStudy.GetRatingFunctionList().Print(_Logger);
        }

        private void PrintExteriorInteriorFunctions()
        {
            LeveeList leveeList = GlobalVariables.mp_fdaStudy.GetLeveeList();
            foreach (KeyValuePair<string, Levee> kvp in leveeList.Levees)
            {
                Levee lev = kvp.Value;
                if (lev.ExteriorInteriorPairs.Count > 0)
                {
                    lev.Print(_Logger, ImportOptions.ImportExteriorInterior);
                }
            }
        }

        private void PrintLevees()
        {
            LeveeList leveeList = GlobalVariables.mp_fdaStudy.GetLeveeList();
            foreach (KeyValuePair<string, Levee> kvp in leveeList.Levees)
            {
                Levee lev = kvp.Value;
                lev.Print(_Logger, ImportOptions.ImportLevees);              
            }
        }

        private void PrintStageDamage()
        {
            GlobalVariables.mp_fdaStudy.GetAggDamgFuncList().Print(_Logger);
        }
        private void PrintOcctypes()
        {
            GlobalVariables.mp_fdaStudy.GetOccupancyTypeList().Print(_Logger);
        } 

        private void PrintEverything()
        {
            PrintFrequencyFunction();
            PrintInflowOutflowFunction();
            PrintRatings();
            PrintExteriorInteriorFunctions();
            PrintLevees();
            PrintOcctypes();
        }
        #endregion

        #region findFields
        void FindFields()
        {
            switch (_TheImportCode)
            {
                case TheImportCode.NO_KEY:
                    break;
                case TheImportCode.PLAN_NAME:
                    FindPlanNameFields();
                    break;
                case TheImportCode.YEAR_NAME:
                    FindYearNameFields();
                    break;
                case TheImportCode.STRM_NME:
                    FindStreamNameFields();
                    break;
                case TheImportCode.RCH_NAME:
                    FindReachNameFields();
                    break;
                case TheImportCode.CAT_NAME:
                    FindCategoryNameFields();
                    break;
                case TheImportCode.OCC_NAME:
                    FindOcctypeNameFields();
                    break;
                case TheImportCode.MOD_NAME:
                    FindModuleNameFields();
                    break;
                case TheImportCode.STRUC_NAME:
                    FindStructureNameFields();
                    break;
                case TheImportCode.WSP_NAME:
                    FindWspNameFields();
                    break;
                case TheImportCode.FREQ_NAME:
                    FindProbFuncNameFields();
                    break;
                case TheImportCode.RATE_NAME:
                    FindRatingFuncNameFields();
                    break;
                case TheImportCode.LEVEE_NAME:
                    FindLeveeNameFields();
                    break;
                case TheImportCode.SD_NAME:
                    FindAggDamgFuncNameFields();
                    break;
                case TheImportCode.SM_PLAN:
                    break;
                default:
                    break;
            }
        }
        void FindPlanNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsPlan.Length; ip++)
            {
                _IxFieldPlan[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsPlan[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldPlan[ip] = jf;
                    }
                }
            }
        }
        void FindYearNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsYear.Length; ip++)
            {
                _IxFieldYear[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsYear[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldYear[ip] = jf;
                    }
                }
            }
        }
        void FindStreamNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsStream.Length; ip++)
            {
                _IxFieldStream[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsStream[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldStream[ip] = jf;
                    }
                }
            }
        }
        void FindReachNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsReach.Length; ip++)
            {
                _IxFieldReach[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsReach[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldReach[ip] = jf;
                    }
                }
            }
        }
        void FindCategoryNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsCategory.Length; ip++)
            {
                _IxFieldCategory[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsCategory[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldCategory[ip] = jf;
                    }
                }
            }
        }
        void FindOcctypeNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsOcctype.Length; ip++)
            {
                _IxFieldOcctype[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsOcctype[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldOcctype[ip] = jf;
                    }
                }
            }
        }
        void FindModuleNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsModule.Length; ip++)
            {
                _IxFieldModule[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsModule[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldModule[ip] = jf;
                    }
                }
            }
        }
        void FindStructureNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsStructure.Length; ip++)
            {
                _IxFieldStructure[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsStructure[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldStructure[ip] = jf;
                    }
                }
            }
        }
        void FindWspNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsWsp.Length; ip++)
            {
                _IxFieldWsp[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsWsp[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldWsp[ip] = jf;
                    }
                }
            }
            return;
        }
        void FindProbFuncNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsProbFunc.Length; ip++)
            {
                _IxFieldProbFunc[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsProbFunc[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldProbFunc[ip] = jf;
                    }
                }
            }
            return;
        }
        void FindRatingFuncNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsRatingFunc.Length; ip++)
            {
                _IxFieldRatingFunc[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsRatingFunc[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldRatingFunc[ip] = jf;
                    }
                }
            }
            return;
        }
        void FindLeveeNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsLevee.Length; ip++)
            {
                _IxFieldLevee[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsLevee[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldLevee[ip] = jf;
                    }
                }
            }
            return;
        }
        void FindAggDamgFuncNameFields()
        {
            //Process all valid fields to match valid fields to input fields
            for (int ip = 0; ip < FieldsAggDamgFunc.Length; ip++)
            {
                _IxFieldAgDamgFunc[ip] = -1;
                //Find match of input field with parameter fields
                bool match = false;
                for (int jf = 0; jf < _NumFieldsInputString && !match; jf++)
                {
                    if (FieldsAggDamgFunc[ip].Equals(_InputStringCapParsed[jf]))
                    {
                        match = true;
                        _IxFieldAgDamgFunc[ip] = jf;
                    }
                }
            }
            return;
        }
        #endregion
        #region processFields
        void ProcessFields()
        {
            switch (_CurrentImportDataCode)
            {
                case TheImportCode.NO_KEY:
                    break;
                case TheImportCode.PLAN_NAME:
                    ProcessPlanFields();
                    break;
                case TheImportCode.YEAR_NAME:
                    ProcessYearFields();
                    break;
                case TheImportCode.STRM_NME:
                    ProcessStreamFields();
                    break;
                case TheImportCode.RCH_NAME:
                    ProcessReachFields();
                    break;
                case TheImportCode.CAT_NAME:
                    ProcessCategoryFields();
                    break;
                case TheImportCode.OCC_NAME:
                    ProcessOcctypeFields();
                    break;
                case TheImportCode.MOD_NAME:
                    ProcessModuleFields();
                    break;
                case TheImportCode.STRUC_NAME:
                    ProcessStructureFields();
                    break;
                case TheImportCode.WSP_NAME:
                    ProcessWspNameFields();
                    break;
                case TheImportCode.FREQ_NAME:
                    ProcessProbFuncNameFields();
                    break;
                case TheImportCode.RATE_NAME:
                    ProcessRatingFuncNameFields();
                    break;
                case TheImportCode.LEVEE_NAME:
                    ProcessLeveeNameFields();
                    break;
                case TheImportCode.SD_NAME:
                    ProcessAggDamgFuncFields();
                    break;
                case TheImportCode.SM_PLAN:
                    break;
                default:
                    break;
            }
        }
        void ProcessPlanFields()
        {
            for (int ip = 0; ip < FieldsPlan.Length; ip++)
            {
                int ixField = _IxFieldPlan[ip];
                if (ixField > -1 & ixField < _NumFieldsInputString)
                {
                    switch (ip)
                    {
                        case 0:
                            _Plan.Name = _InputStringParsed[ixField];
                            break;
                        case 1:
                            _Plan.Description = _InputStringParsed[ixField];
                            break;
                        default:
                            break;
                    }
                }
            }
            //Store Plan in Database
            PlanList planList = GlobalVariables.mp_fdaStudy.GetPlanList();
            planList.Add(_Plan);
            //Reset Plan
            _Plan.Reset();
        }
        void ProcessYearFields()
        {
            for (int ip = 0; ip < FieldsYear.Length; ip++)
            {
                int ixField = _IxFieldYear[ip];
                if (ixField > -1 & ixField < _NumFieldsInputString)
                {
                    switch (ip)
                    {
                        case 0:
                            _Year.Name = _InputStringParsed[ixField];
                            break;
                        case 1:
                            _Year.Description = _InputStringParsed[ixField];
                            break;
                        default:
                            break;
                    }
                }
            }
            //Store Year in Database
            YearList yearList = GlobalVariables.mp_fdaStudy.GetYearList();
            yearList.Add(_Year);
            //Reset Year
            _Year.Reset();
        }
        void ProcessStreamFields()
        {
            for (int ip = 0; ip < FieldsStream.Length; ip++)
            {
                int ixField = _IxFieldStream[ip];
                if (ixField > -1 & ixField < _NumFieldsInputString)
                {
                    switch (ip)
                    {
                        case 0:
                            _Stream.Name = _InputStringParsed[ixField];
                            break;
                        case 1:
                            _Stream.Description = _InputStringParsed[ixField];
                            break;
                        default:
                            break;
                    }
                }
            }
            //Store StreamObj in Database
            GlobalVariables.mp_fdaStudy.GetStreamList().Add(_Stream);
            //Reset StreamObj
            _Stream.Reset();
        }
        void ProcessReachFields()
        {
            //fieldsReach = { "RCH_NAME", "RCH_DESC", "STREAM_NME", "BEG_STA", "END_STA", "BANK", "INDEX_STA" };

            for (int ip = 0; ip < FieldsReach.Length; ip++)
            {
                int ixField = _IxFieldReach[ip];
                if (ixField > -1 & ixField < _NumFieldsInputString)
                {
                    string field = _InputStringParsed[ixField];
                    int ncField = field.Length;

                    switch (ip)
                    {
                        case 0:
                            _DamageReach.Name = _InputStringParsed[ixField];
                            break;
                        case 1:
                            _DamageReach.Description = _InputStringParsed[ixField];
                            break;
                        case 2:
                            _DamageReach.StreamName = _InputStringParsed[ixField];
                            break;
                        case 3:
                            //double staBegin = Double.Parse(inputStringParsed[ixField]);
                            _DamageReach.StationBegin = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 4:
                            _DamageReach.StationEnd = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 5:
                            if (_InputStringCapParsed[ixField] == "LEFT" || field.ToUpper() == "L")
                                _DamageReach.BankStream = StreamBank.LEFT;
                            else if (_InputStringCapParsed[ixField] == "RIGHT" || field.ToUpper() == "R")
                                _DamageReach.BankStream = StreamBank.RIGHT;
                            else if (_InputStringCapParsed[ixField] == "BOTH" || field.ToUpper() == "B")
                                _DamageReach.BankStream = StreamBank.BOTH;
                            else
                                _DamageReach.BankStream = StreamBank.LEFT;
                            break;
                        case 6:
                            _DamageReach.StationIndex = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        default:
                            break;
                    }
                }
            }
            //Store Reach in Database
            GlobalVariables.mp_fdaStudy.GetDamageReachList().Add(_DamageReach);
            //Reset Reach
            _DamageReach.Reset();
        }
        void ProcessCategoryFields()
        {
            for (int ip = 0; ip < FieldsCategory.Length; ip++)
            {
                int ixField = _IxFieldCategory[ip];
                if (ixField > -1)
                {
                    switch (ip)
                    {
                        case 0:
                            _DamageCategory.Name = _InputStringParsed[ixField];
                            break;
                        case 1:
                            _DamageCategory.Description = _InputStringParsed[ixField];
                            break;
                        case 2:
                            _DamageCategory.CostFactor = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        default:
                            break;
                    }
                }
            }
            //Store Category in Database
            GlobalVariables.mp_fdaStudy.GetDamageCategoryList().Add(_DamageCategory);
            //Reset Category
            _DamageCategory.Reset();
        }
        void ProcessOcctypeFields()
        {
            //fieldsOcctype = {"OCC_NAME", "OCC_DESCRIPTION", "CAT_NAME", "PARAMETER", "START_DATA" }
            _ParamOcctype = "";
            _ParamOcctypeCap = "";
            string name = "";
            string description = "";
            string nameCategory = "";

            for (int ip = 0; ip < FieldsOcctype.Length; ip++)
            {
                int ixField = _IxFieldOcctype[ip];
                if (ixField > -1 & ixField < _NumFieldsInputString)
                {
                    string inp = _InputStringParsed[ixField];
                    int ncInp = inp.Length;

                    switch (ip)
                    {
                        case 0:
                            name = _InputStringParsed[ixField];
                            break;
                        case 1:
                            if (ncInp > 0) description = inp;
                            break;
                        case 2:
                            if (ncInp > 0) nameCategory = inp;
                            break;
                        case 3:
                            _IParameterOcctype = ixField;
                            _ParamOcctype = inp;
                            _ParamOcctypeCap = inp.ToUpper();
                            //check for valid entry
                            break;
                        case 4:
                            _IStartOcctype = ixField;
                            //process remaining fields and store data
                            break;
                        default:
                            break;
                    }
                }
            }
            //Occupancy Type Changes, Flush existing occupancy type
            if (name.ToUpper() != _OcctypePrevName)
            {
                FlushOccupancyType(name, description, nameCategory);
            }
            else
            {
            }
            _OccupancyType.Name = name;

            //Decipher Parameter Code and Read Data
            if (_ParamOcctypeCap == "STRUCT")
            {
                //Process fields for structure information (Error in values, FF stage error)
                DetermineOcctypeStructureParameters();
            }
            //Anything else is data
            else
            {
                //Read the data
                int numOrds = _NumFieldsInputString - _IStartOcctype;
                if (numOrds > 0) _FlushOccType = true;

                WriteLine($"numOrds: {numOrds}\tnumFieldsInputString: {_NumFieldsInputString}\tiStartOcctype: {_IStartOcctype}");
                _TheData = new double[numOrds];
                _NumValTheData = numOrds;
                for (int jfl = _IStartOcctype; jfl < _NumFieldsInputString; jfl++)
                {
                    WriteLine($"jfl {jfl},   Data: {_InputStringParsed[jfl]}");
                    int jdata = jfl - _IStartOcctype;
                    _TheData[jdata] = Convert.ToDouble(_InputStringParsed[jfl]);
                }
                //Decipher Parameter Code
                int i = 0;
                //M_usesDollar = false;
                char[] paramCode = new char[5];
                for (i = 0; i < 5; i++) paramCode[i] = ' ';
                int numParamCode = _ParamOcctypeCap.Length;
                for (i = 0; i < numParamCode; i++)
                {
                    paramCode[i] = _ParamOcctypeCap[i];
                }
                if (paramCode[1] == '$') UsesDollar = true; //Set to dollar function
                _OccupancyType.UsesDollar = UsesDollar;

                //Determine the type of damage function data (S, SN, 
                TheDamageFunctionType type = TheDamageFunctionType.ERROR;
                type = DetermineOcctypeFunctionType(paramCode);

                //Read (or copy from theData) the function part
                TransferDataToFunction(type);

            }
        }
        void DetermineOcctypeStructureParameters()
        {
            //Determines the parameters associated with the structure (first floor, S, C, O, A)
            for (int jfl = _IStartOcctype; jfl < _NumFieldsInputString; jfl++)
            {
                if (_InputStringParsed[jfl].Length < 1)
                    continue;

                int iKeyStructData = jfl - _IStartOcctype;
                string fieldCap = _InputStringCapParsed[jfl];

                switch (iKeyStructData)
                {
                    //First Floor Information
                    case 0:     //Distribution Type
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.FFLOOR).SetErrorType(fieldCap);
                        break;
                    case 1:     //Median value (not entered for FF and S
                        break;
                    case 2:     //Std. Dev for N or L; Lower for T
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.FFLOOR).SetStdDev(Convert.ToDouble(fieldCap));
                        break;
                    case 3:     //Upper for T
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.FFLOOR).SetUpper(Convert.ToDouble(fieldCap));
                        break;
                    //Structure Value 
                    case 4:     //Distribution type
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.STRUCTURE).SetErrorType(fieldCap);
                        break;
                    case 5:     //Median value (not entered for Structure Value
                        break;
                    case 6:     //Std. Dev for N or L, Lower for T
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.STRUCTURE).SetStdDev(Convert.ToDouble(fieldCap));
                        break;
                    case 7:     //Upper for T
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.STRUCTURE).SetUpper(Convert.ToDouble(fieldCap));
                        break;
                    //Content Ratio Value
                    case 8:     //Distribution type
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.CONTENT).SetErrorType(fieldCap);
                        break;
                    case 9:     //Content to structure value ratio
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.CONTENT).SetCentralValue(Convert.ToDouble(fieldCap));
                        break;
                    case 10:    //Std. Dev for N or L, Lower for T
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.CONTENT).SetStdDev(Convert.ToDouble(fieldCap));
                        break;
                    case 11:    //Upper for T
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.CONTENT).SetUpper(Convert.ToDouble(fieldCap));
                        break;
                    //Other Ratio Value
                    case 12:    //Distribution Type
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.OTHER).SetErrorType(fieldCap);
                        break;
                    case 13:    //Other to structure value ratio
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.OTHER).SetCentralValue(Convert.ToDouble(fieldCap));
                        break;
                    case 14:    //Std. Dev for N or L, Lower for T
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.OTHER).SetStdDev(Convert.ToDouble(fieldCap));
                        break;
                    case 15:    //Upper for T
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.OTHER).SetUpper(Convert.ToDouble(fieldCap));
                        break;
                    //Automobile Value Ratio
                    case 16:    //Distribution Type
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.AUTO).SetErrorType(fieldCap);
                        break;
                    case 17:    //Automobile to structure value ratio
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.AUTO).SetCentralValue(Convert.ToDouble(fieldCap));
                        break;
                    case 18:    //Std. Dev for N or L, Lower for T
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.AUTO).SetStdDev(Convert.ToDouble(fieldCap));
                        break;
                    case 19:    //Upper for T
                        _OccupancyType.GetErrorDistribution(OccTypeStrucComponent.AUTO).SetUpper(Convert.ToDouble(fieldCap));
                        break;
                    default:
                        break;
                }
            }

        }
        TheDamageFunctionType DetermineOcctypeFunctionType(char[] paramCode)
        {
            TheDamageFunctionType type = TheDamageFunctionType.ERROR;
            //Check for stage
            if (_ParamOcctypeCap == "STAGE")
            {
                type = TheDamageFunctionType.STAGE;
            }

            //First Character is type (S,C,O,A) structure, content, other, automobile
            else if (paramCode[0] == 'S')
            {
                type = TheDamageFunctionType.S;
                if (paramCode[1] == 'N')
                    type = TheDamageFunctionType.SN;
                else if (paramCode[1] == 'L')
                    type = TheDamageFunctionType.SL;
                else if (paramCode[1] == 'T')
                {
                    if (paramCode[2] == 'U')
                        type = TheDamageFunctionType.STU;
                    else if (paramCode[2] == 'L')
                        type = TheDamageFunctionType.STL;
                }
            }
            else if (paramCode[0] == 'C')
            {
                type = TheDamageFunctionType.C;
                if (paramCode[1] == 'N')
                    type = TheDamageFunctionType.CN;
                else if (paramCode[1] == 'L')
                    type = TheDamageFunctionType.CL;
                else if (paramCode[1] == 'T')
                {
                    if (paramCode[2] == 'U')
                        type = TheDamageFunctionType.CTU;
                    else if (paramCode[2] == 'L')
                        type = TheDamageFunctionType.CTL;
                }
            }
            else if (paramCode[0] == 'O')
            {
                type = TheDamageFunctionType.O;
                if (paramCode[1] == 'N')
                    type = TheDamageFunctionType.ON;
                else if (paramCode[1] == 'L')
                    type = TheDamageFunctionType.OL;
                else if (paramCode[1] == 'T')
                {
                    if (paramCode[2] == 'U')
                        type = TheDamageFunctionType.OTU;
                    else if (paramCode[2] == 'L')
                        type = TheDamageFunctionType.OTL;
                }
            }
            else if (paramCode[0] == 'A')
            {
                type = TheDamageFunctionType.A;
                if (paramCode[1] == 'N')
                    type = TheDamageFunctionType.AN;
                else if (paramCode[1] == 'L')
                    type = TheDamageFunctionType.AL;
                else if (paramCode[1] == 'T')
                {
                    if (paramCode[2] == 'U')
                        type = TheDamageFunctionType.ATU;
                    else if (paramCode[2] == 'L')
                        type = TheDamageFunctionType.ATL;
                }
            }
            else
            {
                //There is an error
                type = TheDamageFunctionType.ERROR;
            }
            return type;
        }
        void TransferDataToFunction(TheDamageFunctionType type)
        {
            //Transfer data from temporary array theData into the specific
            //  array in the single damage function. 
            //Important, the stage (depth) values must be entered before the damage and
            //  error distribution values.
            switch (type)
            {
                case TheDamageFunctionType.STAGE:
                    _TheStageGlobal = new double[_TheData.Length];
                    for (int i = 0; i < _TheData.Length; i++) _TheStageGlobal[i] = _TheData[i];
                    break;
                case TheDamageFunctionType.S:
                    _SingleDamageFunction[0].SetNumRows(_TheData.Length, ErrorType.NONE);
                    _SingleDamageFunction[0].SetDamage(_TheData);
                    _SingleDamageFunction[0].SetDepth(_TheStageGlobal);
                    if (UsesDollar) _SingleDamageFunction[0].DirectDollar = UsesDollar;
                    break;
                case TheDamageFunctionType.SN:
                    _SingleDamageFunction[0].SetNumRows(_TheData.Length, ErrorType.NORMAL);
                    _SingleDamageFunction[0].SetStdDev(_TheData);
                    _SingleDamageFunction[0].SetDepth(_TheStageGlobal);
                    break;
                case TheDamageFunctionType.SL:
                    _SingleDamageFunction[0].SetNumRows(_TheData.Length, ErrorType.LOGNORMAL);
                    _SingleDamageFunction[0].SetStdDev(_TheData);
                    _SingleDamageFunction[0].SetDepth(_TheStageGlobal);
                    break;
                case TheDamageFunctionType.STL:
                    _SingleDamageFunction[0].SetNumRows(_TheData.Length, ErrorType.TRIANGULAR);
                    _SingleDamageFunction[0].SetStdDev(_TheData);
                    _SingleDamageFunction[0].SetDepth(_TheStageGlobal);
                    break;
                case TheDamageFunctionType.STU:
                    _SingleDamageFunction[0].SetNumRows(_TheData.Length, ErrorType.TRIANGULAR);
                    _SingleDamageFunction[0].SetTriangularUpper(_TheData);
                    _SingleDamageFunction[0].SetDepth(_TheStageGlobal);
                    break;
                case TheDamageFunctionType.C:
                    _SingleDamageFunction[1].SetNumRows(_TheData.Length, ErrorType.NONE);
                    _SingleDamageFunction[1].SetDamage(_TheData);
                    _SingleDamageFunction[1].SetDepth(_TheStageGlobal);
                    if (UsesDollar) _SingleDamageFunction[1].DirectDollar = UsesDollar;
                    break;
                case TheDamageFunctionType.CN:
                case TheDamageFunctionType.CL:
                case TheDamageFunctionType.CTL:
                    _SingleDamageFunction[1].SetStdDev(_TheData);
                    _SingleDamageFunction[1].SetDepth(_TheStageGlobal);
                    switch (type)
                    {
                        case TheDamageFunctionType.CN:
                            _SingleDamageFunction[1].SetNumRows(_TheData.Length, ErrorType.NORMAL);
                            break;
                        case TheDamageFunctionType.CL:
                            _SingleDamageFunction[1].SetNumRows(_TheData.Length, ErrorType.LOGNORMAL);
                            break;
                        case TheDamageFunctionType.CTL:
                            _SingleDamageFunction[1].SetNumRows(_TheData.Length, ErrorType.TRIANGULAR);
                            break;
                    }

                    break;
                case TheDamageFunctionType.CTU:
                    _SingleDamageFunction[1].SetTriangularUpper(_TheData);
                    _SingleDamageFunction[1].SetDepth(_TheStageGlobal);
                    break;
                case TheDamageFunctionType.O:
                    _SingleDamageFunction[2].SetNumRows(_TheData.Length, ErrorType.NONE);
                    _SingleDamageFunction[2].SetDamage(_TheData);
                    _SingleDamageFunction[2].SetDepth(_TheStageGlobal);
                    if (UsesDollar) _SingleDamageFunction[2].DirectDollar = UsesDollar;
                    break;
                case TheDamageFunctionType.ON:
                case TheDamageFunctionType.OL:
                case TheDamageFunctionType.OTL:
                    _SingleDamageFunction[2].SetStdDev(_TheData);
                    _SingleDamageFunction[2].SetDepth(_TheStageGlobal);
                    switch (type)
                    {
                        case TheDamageFunctionType.ON:
                            _SingleDamageFunction[2].SetNumRows(_TheData.Length, ErrorType.NORMAL);
                            break;
                        case TheDamageFunctionType.OL:
                            _SingleDamageFunction[2].SetNumRows(_TheData.Length, ErrorType.LOGNORMAL);
                            break;
                        case TheDamageFunctionType.OTL:
                            _SingleDamageFunction[2].SetNumRows(_TheData.Length, ErrorType.TRIANGULAR);
                            break;
                        default:
                            break;
                    }
                    break;
                case TheDamageFunctionType.OTU:
                    _SingleDamageFunction[2].SetTriangularUpper(_TheData);
                    _SingleDamageFunction[2].SetDepth(_TheStageGlobal);
                    break;
                case TheDamageFunctionType.A:
                    _SingleDamageFunction[3].SetNumRows(_TheData.Length, ErrorType.NONE);
                    _SingleDamageFunction[3].SetDamage(_TheData);
                    _SingleDamageFunction[3].SetDepth(_TheStageGlobal);
                    if (UsesDollar) _SingleDamageFunction[3].DirectDollar = UsesDollar;
                    break;
                case TheDamageFunctionType.AN:
                case TheDamageFunctionType.AL:
                case TheDamageFunctionType.ATL:
                    _SingleDamageFunction[3].SetStdDev(_TheData);
                    _SingleDamageFunction[3].SetDepth(_TheStageGlobal);
                    switch (type)
                    {
                        case TheDamageFunctionType.AN:
                            _SingleDamageFunction[3].SetNumRows(_TheData.Length, ErrorType.NORMAL);
                            break;
                        case TheDamageFunctionType.AL:
                            _SingleDamageFunction[3].SetNumRows(_TheData.Length, ErrorType.LOGNORMAL);
                            break;
                        case TheDamageFunctionType.ATL:
                            _SingleDamageFunction[3].SetNumRows(_TheData.Length, ErrorType.TRIANGULAR);
                            break;
                        default:
                            break;
                    }
                    break;
                case TheDamageFunctionType.ATU:
                    _SingleDamageFunction[3].SetTriangularUpper(_TheData);
                    _SingleDamageFunction[3].SetDepth(_TheStageGlobal);
                    break;
                default:
                    break;
            }
        }
        public void FlushOccupancyType(string name, string description, string nameCategory)
        {
            //  Stores Occupancy Type in Memory from Import
            //  Will have to store Occupancy Type in the Database
            //      Do we do it here as it is imported (probably)
            //      or do we do it after all have been stored in memory
            if (_FlushOccType)
            {
                _OccupancyType.UsesDollar = UsesDollar;  //!ToDo need to check single damage functions but it's global
                _OccupancyType.SetSingleDamageFunction(StructureValueType.STRUCTURE, this._SingleDamageFunction[(int)StructureValueType.STRUCTURE]);
                _OccupancyType.SetSingleDamageFunction(StructureValueType.CONTENT, this._SingleDamageFunction[(int)StructureValueType.CONTENT]);
                _OccupancyType.SetSingleDamageFunction(StructureValueType.OTHER, this._SingleDamageFunction[(int)StructureValueType.OTHER]);
                _OccupancyType.SetSingleDamageFunction(StructureValueType.CAR, this._SingleDamageFunction[(int)StructureValueType.CAR]);

                if (_OccupancyType.Name != "")
                {
                    GlobalVariables.mp_fdaStudy.GetOccupancyTypeList().Add(_OccupancyType, _Logger);

                    ////rdc test;13Aug2018;rdc critical;Test conversion into Johns IEnumerator
                    //IFunction<double>[] test = new IFunction<double>[4];
                    //for (int i = 0; i < 4; i++)
                    //{
                    //    test[i] = null;
                    //    //Had to take IFunction out of class space in SDF because couldn't serialize nulls
                    //    test[i] = _SingleDamageFunction[i].ConvertToV2();
                    //}
                    ////Write to as-yet-to-be-written database software

                }
            }

            _OcctypePrevName = name.ToUpper();

            //Reset
            UsesDollar = false;
            _OccupancyType.Reset();
            for (int i = 0; i < 5; i++) _SingleDamageFunction[i].Reset();
            _OccupancyType.Description = description;
            _OccupancyType.CategoryName = nameCategory;
        }
        void ProcessModuleFields()
        {
            for (int ip = 0; ip < FieldsModule.Length; ip++)
            {
                int ixField = _IxFieldModule[ip];
                if (ixField > -1 & ixField < _NumFieldsInputString)
                {
                    switch (ip)
                    {
                        case 0:
                            _StructureModule.Name = _InputStringParsed[ixField];
                            break;
                        case 1:
                            _StructureModule.Description = _InputStringParsed[ixField];
                            break;
                        default:
                            break;
                    }
                }
            }
            //Store Module in Database
            GlobalVariables.mp_fdaStudy.GetStructureModuleList().Add(_StructureModule);
            //Reset Module
            _StructureModule.Reset();
        }
        void ProcessStructureFields()
        {
            string field = "";
            int ncField = 0;

            for (int ip = 0; ip < FieldsStructure.Length; ip++)
            {
                int ixField = _IxFieldStructure[ip];
                if (ixField > -1 && ixField < _NumFieldsInputString)
                {
                    field = _InputStringParsed[ixField];
                    ncField = field.Length;
                    if (ncField < 1)
                        continue;

                    switch (ip)
                    {
                        case 0:
                            _Structure.Name = _InputStringParsed[ixField];
                            break;
                        case 1:
                            _Structure.Description = _InputStringParsed[ixField];
                            break;
                        case 2:
                            _Structure.CalculationDate = _InputStringParsed[ixField];
                            break;
                        case 3:
                            _Structure.CategoryName = _InputStringParsed[ixField];
                            break;
                        case 4:
                            _Structure.StreamName = _InputStringParsed[ixField];
                            break;
                        case 5:
                            _Structure.DamageFunctionName = _InputStringParsed[ixField];
                            break;
                        case 6:
                            _Structure.StationAtStructure = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 7:
                            if (field.ToUpper() == "LEFT" || field.ToUpper() == "L")
                                _Structure.BankOfStream = StreamBank.LEFT;
                            else if (field.ToUpper() == "RIGHT" || field.ToUpper() == "R")
                                _Structure.BankOfStream = StreamBank.RIGHT;
                            else
                                _Structure.BankOfStream = StreamBank.LEFT;
                            break;
                        case 8:
                            _Structure.YearInServiceInt = Convert.ToInt32(_InputStringParsed[ixField]);
                            break;
                        case 9:
                            _Structure.ValueOfStructure = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 10:
                            _Structure.ValueOfContent = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 11:
                            _Structure.ValueOfOther = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 12:
                            _Structure.ValueOfCar = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 13:
                            int uses = Convert.ToInt32(_InputStringParsed[ixField]);
                            if (uses == 0)
                                _Structure.UsesFirstFloorElev = false;
                            else
                                _Structure.UsesFirstFloorElev = true;
                            break;
                        case 14:
                            _Structure.ElevationsStructure[(int)Structure.ElevationValue.FIRST_FLOOR] = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 15:
                            _Structure.ElevationsStructure[(int)Structure.ElevationValue.GROUND] = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 16:
                            _Structure.ElevationsStructure[(int)Structure.ElevationValue.DELTAG] = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 17:
                            _Structure.ElevationsStructure[(int)Structure.ElevationValue.DELTAZ] = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 18:
                            _Structure.ElevationsStructure[(int)Structure.ElevationValue.AUTODIFF] = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 19:
                            _Structure.StreetName = _InputStringParsed[ixField];
                            break;
                        case 20:
                            _Structure.CityName = _InputStringParsed[ixField];
                            break;
                        case 21:
                            _Structure.StateCode = _InputStringParsed[ixField];
                            break;
                        case 22:
                            _Structure.ZipCode = _InputStringParsed[ixField];
                            break;
                        case 23:
                            _Structure.NorthingCoordinate = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 24:
                            _Structure.EastingCoordinate = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 25:
                            _Structure.ZoneCoordinate = Convert.ToDouble(_InputStringParsed[ixField]);
                            break;
                        case 26:
                            _Structure.NumberOfStructures = Convert.ToInt32(_InputStringParsed[ixField]);
                            break;
                        case 27:
                            _Structure.Notes = _InputStringParsed[ixField];
                            break;
                        case 28:
                            _Structure.StructureModuleName = _InputStringParsed[ixField];
                            break;
                        case 29:
                            _Structure.SidReachName = _InputStringParsed[ixField];
                            break;
                        case 30:
                            _Structure.ImageFilename = _InputStringParsed[ixField];
                            break;
                        case 31:
                            _Structure.NumberOfCars = Convert.ToInt32(_InputStringParsed[ixField]);
                            break;
                        case 32:
                            _Structure.ParcelNumber = _InputStringParsed[ixField];
                            break;
                        default:
                            break;
                    }
                }
            }
            //Store the structure
            GlobalVariables.mp_fdaStudy.GetStructureList().Add(_Structure);
            //Reset the structure
            _Structure.Reset();
        }
        void ProcessWspNameFields()
        {
            //Previous Record was a key record containing the PYSR etc named field; Now read the data for that
            if (_PrevKeyRecord)
            {
                string name = "";
                string description = "";
                string plan = "";
                string year = "";
                string stream = "";
                WspDataType wspDataType = WspDataType.DISCHARGE_FREQUENCY;
                string notes = "";

                for (int ip = 0; ip < FieldsWsp.Length; ip++)
                {
                    int ixField = _IxFieldWsp[ip];
                    if (ixField > -1 && ixField < _NumFieldsInputString)
                    {
                        string inp = _InputStringParsed[ixField];
                        int ncInp = inp.Length;
                        if (ncInp < 1)
                            continue;
                        switch (ip)
                        {
                            case 0:
                                name = inp;
                                break;
                            case 1:
                                description = inp;
                                break;
                            case 2:
                                plan = inp;
                                break;
                            case 3:
                                year = inp;
                                break;
                            case 4:
                                stream = inp;
                                break;
                            case 5:
                                if (inp.ToUpper() == "ASSIGN")
                                    continue;
                                else if (inp.ToUpper() == "DISCHARGE_FREQ")
                                {
                                    wspDataType = WspDataType.DISCHARGE_FREQUENCY;
                                    break;
                                }
                                else if (inp.ToUpper() == "STAGE_FREQUENCY")
                                {
                                    wspDataType = WspDataType.STAGE_FREQUENCY;
                                    break;
                                }
                                else
                                {
                                    wspDataType = WspDataType.UNKNOWN;
                                }
                                break;
                            case 6:
                                notes = inp;
                                break;
                            default:
                                break;
                        }
                    }
                }
                _WaterSurfaceProfile.Name = name;
                _WaterSurfaceProfile.Description = description;
                _WaterSurfaceProfile.PlanName = plan;
                _WaterSurfaceProfile.YearName = year;
                _WaterSurfaceProfile.StreamName = stream;
                _WaterSurfaceProfile.WspDataTypeId = wspDataType;
                _WaterSurfaceProfile.Notes = notes;
                _PrevRecWspProb = false;
            }
            //This is data, store it in current Levee
            //WSP_PROBABILITY key word
            else if (_InputStringCapParsed[0] == "WSP_PROBABILITY")
            {
                _PrevRecWspProb = true;
            }
            //Get the probability
            else if (_PrevRecWspProb)
            {
                ProcessWspProbabilities();
                _PrevRecWspProb = false;
            }
            //Header for Wsp Data
            else if (_InputStringCapParsed[0] == "WSP_STATION")
            {
                ProcessWspDataHeader();
                _PrevRecWspProb = false;
            }
            //Wsp data
            else
            {
                ProcessWspData();
                _MustFlushWsp = true;
            }
            return;
        }
        protected int ProcessWspProbabilities()
        {
            //Find first non-blank field and process until blank or probability rises
            int numProbs = 0;
            double prevProb = 2.0;
            int icycle = 1;
            double[] probabilities = new double[100];

            for (int i = 0; i < _NumFieldsInputString && icycle == 1; i++)
            {
                string inp = _InputStringParsed[i];
                int ncInp = inp.Length;
                if (ncInp < 1)
                    continue;
                else
                {
                    double p = Convert.ToDouble(inp);
                    if (p > prevProb)
                        icycle++;
                    else
                    {
                        probabilities[numProbs] = p;
                        prevProb = p;
                        numProbs++;
                    }
                }
            }
            _WaterSurfaceProfile.SetProbabilities(numProbs, probabilities);
            return numProbs;
        }
        protected void ProcessWspDataHeader()
        {
            //Find undex fields for INVERT, 1st for STAGE and 1st for Q
            _IxWspInvert = -1;
            _IxWspStage = -1;
            _IxWspQ = -1;

            //for(int i = 0; i < numFieldsInputString || iWspStage < 0 || iWspQ < 0; i++)
            for (int i = 0; i < _NumFieldsInputString; i++)
            {
                string inp = _InputStringCapParsed[i];
                int ncInp = inp.Length;
                if (ncInp > 0)
                {
                    if (inp == "INVERT" && _IxWspInvert < 0)
                        _IxWspInvert = i;
                    else if (inp == "STAGE" && _IxWspStage < 0)
                        _IxWspStage = i;
                    else if (inp == "Q" && _IxWspQ < 0)
                        _IxWspQ = i;
                }
            }
            return;
        }
        protected void ProcessWspData()
        {
            _WspSectData = new WspSectionData(_WaterSurfaceProfile.NumberOfProfiles);

            //Assume station is always first
            _WspSectData.SetPoint(Convert.ToDouble(_InputStringCapParsed[0]),
                                 Convert.ToDouble(_InputStringParsed[_IxWspInvert]));

            for (int i = 0; i < _WaterSurfaceProfile.NumberOfProfiles; i++)
            {
                double stage = Convert.ToDouble(_InputStringParsed[_IxWspStage + i]);
                double flow = Study.badNumber;
                if (_IxWspQ > -1)
                    flow = Convert.ToDouble(_InputStringParsed[_IxWspQ + i]);
                _WspSectData.SetPoint(i, stage, flow);
            }
            _WaterSurfaceProfile.AddWspSectionData(_WspSectData);
            return;
        }
        public void FlushWspData()
        {
            if (_MustFlushWsp)
            {
                //Store Water Surface Profile in global list. Probably should write to disk 
                GlobalVariables.mp_fdaStudy.GetWspList().Add(_WaterSurfaceProfile);
            }
            _MustFlushWsp = false;
            _WaterSurfaceProfile.Reset();
        }
        void ProcessProbFuncNameFields()
        {
            //Previous Record was a key record containing the PYSR etc named field; Now read the data for that
            if (_PrevKeyRecord)
            {
                string name = "";
                string plan = "";
                string year = "";
                string stream = "";
                string reach = "";
                string equivLen = "";
                int equivLenAsInt = 1900;
                ProbabilityFunction.FrequencyFunctionType funcType = ProbabilityFunction.FrequencyFunctionType.FF_UNKNOWN;
                ProbabilityFunction.ProbabilityDataType dataType = ProbabilityFunction.ProbabilityDataType.DISCHARGE_FREQUENCY;
                ProbabilityFunction.UncertaintyTypeSpecification uncertType = ProbabilityFunction.UncertaintyTypeSpecification.EQUIV_REC_LENGTH;

                string description = "";

                for (int ip = 0; ip < FieldsProbFunc.Length; ip++)
                {
                    int ixField = _IxFieldProbFunc[ip];
                    if (ixField > -1 && ixField < _NumFieldsInputString)
                    {
                        string inp = _InputStringParsed[ixField];
                        int ncInp = inp.Length;
                        if (ncInp < 1)
                            continue;

                        switch (ip)
                        {
                            case 0:
                                name = inp;
                                break;
                            case 1:
                                plan = inp;
                                break;
                            case 2:
                                year = inp;
                                break;
                            case 3:
                                stream = inp;
                                break;
                            case 4:
                                reach = inp;
                                break;
                            case 5:
                                equivLen = inp;
                                if (inp.ToUpper() == "ASSIGN")
                                    continue;
                                else
                                    equivLenAsInt = Convert.ToInt32(inp);
                                break;
                            case 6:
                                if (inp.ToUpper() == "G")
                                    funcType = ProbabilityFunction.FrequencyFunctionType.GRAPHICAL;
                                else if (inp.ToUpper() == "L")
                                    funcType = ProbabilityFunction.FrequencyFunctionType.ANALYTICAL;
                                else if (inp.ToUpper() == "S")
                                    funcType = ProbabilityFunction.FrequencyFunctionType.ANALYTICAL;
                                else
                                    funcType = ProbabilityFunction.FrequencyFunctionType.FF_UNKNOWN;
                                break;
                            case 7:
                                if (inp.ToUpper() == "Q")
                                    dataType = ProbabilityFunction.ProbabilityDataType.DISCHARGE_FREQUENCY;
                                else
                                    dataType = ProbabilityFunction.ProbabilityDataType.STAGE_FREQUENCY;
                                break;
                            case 8:
                                int uncertTypeAsInt = Convert.ToInt32(inp);
                                if (uncertTypeAsInt == -1)
                                    uncertType = ProbabilityFunction.UncertaintyTypeSpecification.EQUIV_REC_LENGTH;
                                else if (uncertTypeAsInt == 0)
                                    uncertType = ProbabilityFunction.UncertaintyTypeSpecification.NONE;
                                else if (uncertTypeAsInt == 1)
                                    uncertType = ProbabilityFunction.UncertaintyTypeSpecification.NORMAL;
                                else if (uncertTypeAsInt == 2)
                                    uncertType = ProbabilityFunction.UncertaintyTypeSpecification.LOG_NORMAL;
                                else if (uncertTypeAsInt == 3)
                                    uncertType = ProbabilityFunction.UncertaintyTypeSpecification.TRIANGULAR;
                                else
                                    uncertType = ProbabilityFunction.UncertaintyTypeSpecification.EQUIV_REC_LENGTH;
                                break;
                            case 9:
                                description = inp;
                                break;
                            default:
                                break;
                        }
                    }
                }
                //Set the parameters; should just use the frequency function above 
                _FrequencyFunction.Name = name;
                _FrequencyFunction.PlanName = plan;
                _FrequencyFunction.YearName = year;
                _FrequencyFunction.StreamName = stream;
                _FrequencyFunction.DamageReachName = reach;
                _FrequencyFunction.EquivalentLengthOfRecord = equivLenAsInt;
                _FrequencyFunction.ProbabilityFunctionTypeId = funcType;
                _FrequencyFunction.ProbabilityDataTypeId = dataType;
                _FrequencyFunction.UncertTypeSpecification = uncertType;
                _FrequencyFunction.Description = description;
            }
            //This is data, store it in current Probability Function
            else
            {
                //LP3, SYN, PROB, FLOW, STAGE, UDFN, UDFL, UDFTL, UDFTU, QIN, QOUT, QN, QL, QTU, QTL
                //SearchOption for this code in first non-blank field, then process data accordingly.
                int ixFieldCode = -1;
                ProbFuncCodeType probFuncCodeType = ProbFuncCodeType.UNKNOWN;
                int numPoints = 0;

                //Search to find first nonblank field that contains a code.
                for (int i = 0; i < _NumFieldsInputString && probFuncCodeType == ProbFuncCodeType.UNKNOWN; i++)
                {
                    if (_InputStringCapParsed[i].Length > 0)
                    {
                        ixFieldCode = i;
                        string field = _InputStringCapParsed[i];
                        if (field == "LP3")
                            probFuncCodeType = ProbFuncCodeType.LP3;
                        else if (field == "SYN")
                            probFuncCodeType = ProbFuncCodeType.SYN;
                        else if (field == "PROB")
                            probFuncCodeType = ProbFuncCodeType.PROB;
                        else if (field == "FLOW")
                            probFuncCodeType = ProbFuncCodeType.FLOW;
                        else if (field == "STAGE")
                            probFuncCodeType = ProbFuncCodeType.STAGE;
                        else if (field == "UDFN")
                            probFuncCodeType = ProbFuncCodeType.UDFN;
                        else if (field == "UDFL")
                            probFuncCodeType = ProbFuncCodeType.UDFL;
                        else if (field == "UDFTL")
                            probFuncCodeType = ProbFuncCodeType.UDFTL;
                        else if (field == "UDFTU")
                            probFuncCodeType = ProbFuncCodeType.UDFTU;
                        else if (field == "QIN")
                            probFuncCodeType = ProbFuncCodeType.QIN;
                        else if (field == "QOUT")
                            probFuncCodeType = ProbFuncCodeType.QOUT;
                        else if (field == "QN")
                            probFuncCodeType = ProbFuncCodeType.QN;
                        else if (field == "QL")
                            probFuncCodeType = ProbFuncCodeType.QL;
                        else if (field == "QTU")
                            probFuncCodeType = ProbFuncCodeType.QTU;
                        else if (field == "QTL")
                            probFuncCodeType = ProbFuncCodeType.QTL;
                        else
                            ixFieldCode = -1;
                    }
                }
                //Process Data
                switch (probFuncCodeType)
                {
                    case ProbFuncCodeType.LP3:
                        ProcessProbFuncLP3(ixFieldCode);
                        break;
                    case ProbFuncCodeType.SYN:
                        ProcessProbFuncSyn(ixFieldCode);
                        break;
                    case ProbFuncCodeType.PROB:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.NumberOfGraphicalPoints = numPoints;
                        _FrequencyFunction.ReallocateGraphicalWithCheckAndSave(numPoints);
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.ExceedanceProbability);
                        break;
                    case ProbFuncCodeType.FLOW:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.NumberOfGraphicalPoints = numPoints;
                        _FrequencyFunction.ReallocateGraphicalWithCheckAndSave(numPoints);
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.Discharge);
                        break;
                    case ProbFuncCodeType.STAGE:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.NumberOfGraphicalPoints = numPoints;
                        _FrequencyFunction.ReallocateGraphicalWithCheckAndSave(numPoints);
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.Stage);
                        break;
                    case ProbFuncCodeType.UDFN:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.NumberOfGraphicalPoints = numPoints;
                        _FrequencyFunction.ReallocateGraphicalWithCheckAndSave(numPoints);
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.StdDevNormalUserDef);
                        break;
                    case ProbFuncCodeType.UDFL:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.NumberOfGraphicalPoints = numPoints;
                        _FrequencyFunction.ReallocateGraphicalWithCheckAndSave(numPoints);
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.StdDevLogUserDef);
                        break;
                    case ProbFuncCodeType.UDFTL:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.NumberOfGraphicalPoints = numPoints;
                        _FrequencyFunction.ReallocateGraphicalWithCheckAndSave(numPoints);
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.StdDevLowerUserDef);
                        break;
                    case ProbFuncCodeType.UDFTU:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.NumberOfGraphicalPoints = numPoints;
                        _FrequencyFunction.ReallocateGraphicalWithCheckAndSave(numPoints);
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.StdDevUpperUserDef);
                        break;
                    case ProbFuncCodeType.QIN:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.ReallocateTransformFlowWithCheckAndSave(numPoints);
                        _FrequencyFunction.NumberOfTransFlowPoints = numPoints;
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.TransFlowInflow);
                        break;
                    case ProbFuncCodeType.QOUT:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.ReallocateTransformFlowWithCheckAndSave(numPoints);
                        _FrequencyFunction.NumberOfTransFlowPoints = numPoints;
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.TransFlowOutflow);
                        break;
                    case ProbFuncCodeType.QN:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.ReallocateTransformFlowWithCheckAndSave(numPoints);
                        _FrequencyFunction.NumberOfTransFlowPoints = numPoints;
                        _FrequencyFunction.ErrorTypeTransformFlow = ErrorType.NORMAL;
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.TransFlowStdDev);
                        break;
                    case ProbFuncCodeType.QL:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.ReallocateTransformFlowWithCheckAndSave(numPoints);
                        _FrequencyFunction.NumberOfTransFlowPoints = numPoints;
                        _FrequencyFunction.ErrorTypeTransformFlow = ErrorType.LOGNORMAL;
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.TransFlowLogStdDev);
                        break;
                    case ProbFuncCodeType.QTU:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.ReallocateTransformFlowWithCheckAndSave(numPoints);
                        _FrequencyFunction.NumberOfTransFlowPoints = numPoints;
                        _FrequencyFunction.ErrorTypeTransformFlow = ErrorType.TRIANGULAR;
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.TransFlowUpper);
                        break;
                    case ProbFuncCodeType.QTL:
                        numPoints = ProcessProbFuncNumPoints(ixFieldCode);
                        _FrequencyFunction.ReallocateTransformFlowWithCheckAndSave(numPoints);
                        _FrequencyFunction.NumberOfTransFlowPoints = numPoints;
                        _FrequencyFunction.ErrorTypeTransformFlow = ErrorType.TRIANGULAR;
                        ProcessProbFuncPoints(ixFieldCode, _FrequencyFunction.TransFlowLower);
                        break;
                    default:
                        break;
                }
            }
            return;
        }
        void ProcessProbFuncLP3(int ixFieldCode)
        {
            _FrequencyFunction.UncertTypeSpecification = ProbabilityFunction.UncertaintyTypeSpecification.EQUIV_REC_LENGTH;
            _FrequencyFunction.ProbabilityFunctionTypeId = ProbabilityFunction.FrequencyFunctionType.ANALYTICAL;
            _FrequencyFunction.SourceOfStatisticsId = ProbabilityFunction.SourceOfStatistics.ENTERED;

            for (int i = ixFieldCode + 1; i < _NumFieldsInputString; i++)
            {
                string field = _InputStringParsed[i];
                int icase = i - ixFieldCode - 1;
                switch (icase)
                {
                    case 0:
                        _FrequencyFunction.MomentsLp3[0] = Convert.ToDouble(field);
                        _MustFlushProbFunc = true;
                        break;
                    case 1:
                        _FrequencyFunction.MomentsLp3[1] = Convert.ToDouble(field);
                        break;
                    case 2:
                        _FrequencyFunction.MomentsLp3[2] = Convert.ToDouble(field);
                        break;
                    default:
                        break;
                }
            }
            return;
        }
        void ProcessProbFuncSyn(int ixFieldCode)
        {
            _FrequencyFunction.UncertTypeSpecification = ProbabilityFunction.UncertaintyTypeSpecification.EQUIV_REC_LENGTH;
            _FrequencyFunction.ProbabilityFunctionTypeId = ProbabilityFunction.FrequencyFunctionType.ANALYTICAL;
            _FrequencyFunction.SourceOfStatisticsId = ProbabilityFunction.SourceOfStatistics.CALCULATED; ;

            for (int i = ixFieldCode + 1; i < _NumFieldsInputString; i++)
            {
                string field = _InputStringParsed[i];
                int icase = i - ixFieldCode - 1;
                switch (icase)
                {
                    case 0:
                        _FrequencyFunction.PointsSynthetic[0] = Convert.ToDouble(field);
                        _MustFlushProbFunc = true;
                        break;
                    case 1:
                        _FrequencyFunction.PointsSynthetic[1] = Convert.ToDouble(field);
                        break;
                    case 2:
                        _FrequencyFunction.PointsSynthetic[2] = Convert.ToDouble(field);
                        break;
                    default:
                        break;
                }
            }
            return;
        }
        protected int ProcessProbFuncNumPoints(int ixFieldCode)
        {
            int numPoints = _NumFieldsInputString - ixFieldCode - 1;
            return numPoints;
        }
        protected void ProcessProbFuncPoints(int ixFieldCode, double[] points)
        {
            int numPoints = _NumFieldsInputString - ixFieldCode - 1;
            if (numPoints > 0) _MustFlushProbFunc = true;
            for (int i = ixFieldCode + 1; i < _NumFieldsInputString; i++)
            {
                points[i - ixFieldCode - 1] = Convert.ToDouble(_InputStringParsed[i]);
            }
            return;
        }
        protected void FlushProbFunction()
        {
            if (_MustFlushProbFunc)
            {
                GlobalVariables.mp_fdaStudy.GetProbabilityFuncList().Add(_FrequencyFunction, _Logger);
            }
            _MustFlushProbFunc = false;
            _FrequencyFunction.Reset();
        }
        void ProcessRatingFuncNameFields()
        {
            //char* codeData[] = { "Q", "S", "SN", "SL", "STL", "STU" };

            //Previous Record was a key record containing the PYSR etc named field; Now read the data for that
            if (_PrevKeyRecord)
            {
                string name = "";
                string plan = "";
                string year = "";
                string stream = "";
                string reach = "";
                ErrorType errType = ErrorType.NONE;
                bool usesGlobal = false;
                double stageBase = Study.badNumber;
                double stageSd = 0.0;
                double stageSdL = Study.badNumber;
                double stageLo = Study.badNumber;
                double stageHi = Study.badNumber;

                string description = "";

                for (int ip = 0; ip < FieldsRatingFunc.Length; ip++)
                {
                    int ixField = _IxFieldRatingFunc[ip];
                    if (ixField > -1 && ixField < _NumFieldsInputString)
                    {
                        string inp = _InputStringParsed[ixField];
                        int ncInp = inp.Length;
                        if (ncInp < 1)
                            continue;

                        switch (ip)
                        {
                            case 0:
                                name = inp;
                                break;
                            case 1:
                                plan = inp;
                                break;
                            case 2:
                                year = inp;
                                break;
                            case 3:
                                stream = inp;
                                break;
                            case 4:
                                reach = inp;
                                break;
                            case 5:
                                if (inp.ToUpper() == "ASSIGN")
                                    continue;
                                else if (inp.ToUpper() == "")
                                    errType = ErrorType.NONE;
                                else if (inp.ToUpper() == "N")
                                    errType = ErrorType.NORMAL;
                                else if (inp.ToUpper() == "L")
                                    errType = ErrorType.LOGNORMAL;
                                else if (inp.ToUpper() == "T")
                                    errType = ErrorType.TRIANGULAR;
                                else
                                    errType = ErrorType.NONE;
                                break;
                            case 6:
                                stageBase = Convert.ToDouble(inp);
                                usesGlobal = true;
                                break;
                            case 7:
                                stageSd = Convert.ToDouble(inp);
                                break;
                            case 8:
                                stageSdL = Convert.ToDouble(inp);
                                break;
                            case 9:
                                stageLo = Convert.ToDouble(inp);
                                break;
                            case 10:
                                stageHi = Convert.ToDouble(inp);
                                break;
                            case 11:
                                description = inp;
                                break;
                            default:
                                break;
                        }
                    }
                }
                //Set the parameters; should just use the rating function above 
                _RatingFunction.Name = name;
                _RatingFunction.PlanName = plan;
                _RatingFunction.YearName = year;
                _RatingFunction.StreamName = stream;
                _RatingFunction.DamageReachName = reach;

                _RatingFunction.ErrorTypesId = errType;
                _RatingFunction.UsesGlobalError = usesGlobal;
                _RatingFunction.BaseStage = stageBase;
                _RatingFunction.GlobalStdDev = stageSd;
                _RatingFunction.GlobalStdDevLog = stageSdL;
                _RatingFunction.GlobalStdDevLow = stageLo;
                _RatingFunction.GlobalStdDevHigh = stageHi;

                _RatingFunction.Description = description;
            }
            //This is data, store it in current Rating Function
            else
            {
                //"Q", "S", "SN", "SL", "STL", "STU" };
                int ixFieldCode = -1;
                RatingFuncCodeType ratingFuncCodeType = RatingFuncCodeType.UNKNOWN;
                int numPoints = 0;

                //Search to find first nonblank field that contains a code.
                for (int i = 0; i < _NumFieldsInputString && ratingFuncCodeType == RatingFuncCodeType.UNKNOWN; i++)
                {
                    if (_InputStringCapParsed[i].Length > 0)
                    {
                        ixFieldCode = i;
                        string field = _InputStringCapParsed[i];
                        if (field == "Q")
                            ratingFuncCodeType = RatingFuncCodeType.Q;
                        else if (field == "S")
                            ratingFuncCodeType = RatingFuncCodeType.S;
                        else if (field == "SN")
                            ratingFuncCodeType = RatingFuncCodeType.SN;
                        else if (field == "SL")
                            ratingFuncCodeType = RatingFuncCodeType.SL;
                        else if (field == "STL")
                            ratingFuncCodeType = RatingFuncCodeType.STL;
                        else if (field == "STU")
                            ratingFuncCodeType = RatingFuncCodeType.STU;
                        else
                        {
                            ratingFuncCodeType = RatingFuncCodeType.UNKNOWN;
                            ixFieldCode = -1;
                        }
                    }
                }
                //Process Data
                switch (ratingFuncCodeType)
                {
                    case RatingFuncCodeType.Q:
                        numPoints = ProcessRatingFuncNumPoints(ixFieldCode);
                        ProcessRatingFuncPoints(ixFieldCode, _TheData);
                        _RatingFunction.SetDischarge(numPoints, _TheData);
                        break;
                    case RatingFuncCodeType.S:
                        numPoints = ProcessRatingFuncNumPoints(ixFieldCode);
                        ProcessRatingFuncPoints(ixFieldCode, _TheData);
                        _RatingFunction.SetStage(numPoints, _TheData);
                        break;
                    case RatingFuncCodeType.SN:
                        numPoints = ProcessRatingFuncNumPoints(ixFieldCode);
                        ProcessRatingFuncPoints(ixFieldCode, _TheData);
                        _RatingFunction.SetStdDev(numPoints, _TheData);
                        break;
                    case RatingFuncCodeType.SL:
                        numPoints = NewMethod(ixFieldCode);
                        ProcessRatingFuncPoints(ixFieldCode, _TheData);
                        _RatingFunction.SetStdDevLog(numPoints, _TheData);
                        break;
                    case RatingFuncCodeType.STL:
                        numPoints = ProcessRatingFuncNumPoints(ixFieldCode);
                        ProcessRatingFuncPoints(ixFieldCode, _TheData);
                        _RatingFunction.SetStdDevLow(numPoints, _TheData);
                        break;
                    case RatingFuncCodeType.STU:
                        numPoints = ProcessRatingFuncNumPoints(ixFieldCode);
                        ProcessRatingFuncPoints(ixFieldCode, _TheData);
                        _RatingFunction.SetStdDevHigh(numPoints, _TheData);
                        break;
                    case RatingFuncCodeType.UNKNOWN:
                        break;
                    default:
                        break;
                }
            }
            return;
        }

        private int NewMethod(int ixFieldCode)
        {
            return ProcessRatingFuncNumPoints(ixFieldCode);
        }

        protected int ProcessRatingFuncNumPoints(int ixFieldCode)
        {
            int numPoints = _NumFieldsInputString - ixFieldCode - 1;
            if(numPoints > _NumValAllocTheData)
            {
                _NumValAllocTheData = numPoints;
                _TheData = new double[_NumValAllocTheData];
                for (int j = 0; j < _NumValAllocTheData; j++)
                    _TheData[j] = Study.badNumber;
            }
            return numPoints;
        }
        protected void ProcessRatingFuncPoints(int ixFieldCode, double[] points)
        {
            int numPoints = _NumFieldsInputString - ixFieldCode - 1;
            if (numPoints > 0) _MustFlushRatingFunc = true;
            for (int i = ixFieldCode + 1; i < _NumFieldsInputString; i++)
            {
                points[i - ixFieldCode - 1] = Convert.ToDouble(_InputStringParsed[i]);
            }
            return;
        }
        protected void FlushRatingFunction()
        {
            if (_MustFlushRatingFunc)
            {
               // _RatingFunction.SaveToSqlite();
                GlobalVariables.mp_fdaStudy.GetRatingFunctionList().Add(_RatingFunction, _Logger);
            }
            _MustFlushRatingFunc = false;
            _RatingFunction.Reset();
        }
        void ProcessLeveeNameFields()
        {
            //Previous Record was a key record containing the PYSR etc named field; Now read the data for that
            if (_PrevKeyRecord)
            {
                string name = "";
                string plan = "";
                string year = "";
                string stream = "";
                string reach = "";
                double elevTopLevee = Study.badNumber;
                string description = "";

                for (int ip = 0; ip < FieldsLevee.Length; ip++)
                {
                    int ixField = _IxFieldLevee[ip];
                    if (ixField > -1 && ixField < _NumFieldsInputString)
                    {
                        string inp = _InputStringParsed[ixField];
                        int ncInp = inp.Length;
                        if (ncInp < 1)
                            continue;
                        switch (ip)
                        {
                            case 0:
                                name = inp;
                                break;
                            case 1:
                                plan = inp;
                                break;
                            case 2:
                                year = inp;
                                break;
                            case 3:
                                stream = inp;
                                break;
                            case 4:
                                reach = inp;
                                break;
                            case 5:
                                if (inp.ToUpper() == "ASSIGN")
                                    continue;
                                else
                                {
                                    elevTopLevee = Convert.ToDouble(inp);
                                    this._MustFlushLevee = true;
                                }
                                break;
                            case 6:
                                description = inp;
                                break;
                            default:
                                break;
                        }
                    }
                }
                _Levee.Name = name;
                _Levee.PlanName = plan;
                _Levee.YearName = year;
                _Levee.StreamName = stream;
                _Levee.DamageReachName = reach;
                _Levee.ElevationTopOfLevee = elevTopLevee;
                _Levee.Description = description;

            }
            //This is data, store it in current Levee
            else
            {
                //UNKNOWN, SE, SI, GSE,GPF
                int ixFieldCode = -1;
                LeveeCodeType leveeCodeType = LeveeCodeType.UNKNOWN;
                int numPoints = 0;

                //Search to find first nonblank field that contains a code.
                for (int i = 0; i < _NumFieldsInputString && leveeCodeType == LeveeCodeType.UNKNOWN; i++)
                {
                    if (_InputStringCapParsed[i].Length > 0)
                    {
                        ixFieldCode = i;
                        string field = _InputStringCapParsed[i];
                        if (field == "SE")
                            leveeCodeType = LeveeCodeType.SE;
                        else if (field == "SI")
                            leveeCodeType = LeveeCodeType.SI;
                        else if (field == "GSE")
                            leveeCodeType = LeveeCodeType.GSE;
                        else if (field == "GPF")
                            leveeCodeType = LeveeCodeType.GPF;
                        else
                            ixFieldCode = -1;
                    }
                }
                //Process Data
                double[] points = null;
                numPoints = ProcessLeveeNumPoints(ixFieldCode);
                points = new double[numPoints];
                ProcessLeveePoints(ixFieldCode, points);

                switch (leveeCodeType)
                {
                    case LeveeCodeType.SE:
                        _Levee.SetIntExtExterior(numPoints, points);
                        break;
                    case LeveeCodeType.SI:
                        _Levee.SetIntExtInterior(numPoints, points);
                        break;
                    case LeveeCodeType.GSE:
                        _Levee.SetGeoTechElev(numPoints, points);
                        break;
                    case LeveeCodeType.GPF:
                        _Levee.SetGeoTechProb(numPoints, points);
                        break;
                    default:
                        break;
                }
            }
            return;
        }
        protected int ProcessLeveeNumPoints(int ixFieldCode)
        {
            int numPoints = _NumFieldsInputString - ixFieldCode - 1;
            return numPoints;
        }
        protected void ProcessLeveePoints(int ixFieldCode, double[] points)
        {
            int numPoints = _NumFieldsInputString - ixFieldCode - 1;
            if (numPoints > 0) _MustFlushLevee = true;
            for (int i = ixFieldCode + 1; i < _NumFieldsInputString; i++)
            {
                if (_InputStringParsed[i].Trim().Length < 1)
                    points[i - ixFieldCode - 1] = Study.badNumber;
                else if(ixFieldCode < 0)
                    points[i - ixFieldCode - 1] = Study.badNumber;
                else
                {
                    try
                    {
                        points[i - ixFieldCode - 1] = Convert.ToDouble(_InputStringParsed[i]);
                    }
                    catch (ArgumentException e)
                    {
                        WriteLine($"Failure to convert to double _InputStringParsed[i], i: {_InputStringParsed[i]}");
                    }
                }
            }
            return;
        }
        protected void FlushLevee()
        {
            if (_MustFlushLevee)
            {
                GlobalVariables.mp_fdaStudy.GetLeveeList().Add(_Levee, _Logger, ImportOptions.ImportEverything);
            }
            _MustFlushLevee = false;
            _Levee.Reset();
        }

        protected void ProcessAggDamgFuncFields()
        {
            //"SD_NAME", "PLAN", "YEAR", "STREAM", "REACH", "RunsOn", "DESC"

            //Previous Record was a key record containing the PYSRC etc name field; Now read the deata for that
            if (_PrevKeyRecord)
            {
                string name = "";
                string plan = "";
                string year = "";
                string stream = "";
                string reach = "";
                string category = "";
                string description = "";

                for (int ip = 0; ip < FieldsAggDamgFunc.Length; ip++)
                {
                    int ixField = _IxFieldAgDamgFunc[ip];
                    if (ixField > -1 && ixField < _NumFieldsInputString)
                    {
                        string inp = _InputStringParsed[ixField];
                        int ncInp = inp.Length;

                        if (ncInp < 1)
                            continue;

                        switch (ip)
                        {
                            case 0:
                                name = inp;
                                break;
                            case 1:
                                plan = inp;
                                break;
                            case 2:
                                year = inp;
                                break;
                            case 3:
                                stream = inp;
                                break;
                            case 4:
                                reach = inp;
                                break;
                            case 5:
                                category = inp;
                                break;
                            case 6:
                                description = inp;
                                break;
                            default:
                                break;
                        }
                    }
                }
                //Set the parameters; should just use the aggregate damage function above
                _AggregateDamageFunction.Name = name;
                _AggregateDamageFunction.PlanName = plan;
                _AggregateDamageFunction.YearName = year;
                _AggregateDamageFunction.StreamName = stream;
                _AggregateDamageFunction.DamageReachName = reach;
                _AggregateDamageFunction.CategoryName = category;
                _AggregateDamageFunction.Description = description;
            }
            //This is data, store it in current Aggregated damage function
            else
            {
                //"SSTAGE","S","SN","CSTAGE","C","CN","OSTAGE","O","ON","ASTAGE","A","AN","TSTAGE","T","TN"
                int ixFieldCode = -1;
                AggDamgFuncCodeType aggDamgFuncCodeType = AggDamgFuncCodeType.UNKNOWN;
                int[] numPoints = new int[5];
                for (int i = 0; i < 5; i++) numPoints[i] = 0;

                //Search to find first nonblank field that contains a code.
                for (int i = 0; i < _NumFieldsInputString && aggDamgFuncCodeType == AggDamgFuncCodeType.UNKNOWN; i++)
                {
                    if (_InputStringCapParsed[i].Length > 0)
                    {
                        ixFieldCode = i;
                        string inp = _InputStringCapParsed[i];

                        if (inp == "SSTAGE")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.SSTAGE;
                        else if (inp == "S")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.S;
                        else if (inp == "SN")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.SN;
                        else if (inp == "CSTAGE")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.CSTAGE;
                        else if (inp == "C")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.C;
                        else if (inp == "CN")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.CN;
                        else if (inp == "OSTAGE")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.OSTAGE;
                        else if (inp == "O")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.O;
                        else if (inp == "ON")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.ON;
                        else if (inp == "ASTAGE")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.ASTAGE;
                        else if (inp == "A")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.A;
                        else if (inp == "AN")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.AN;
                        else if (inp == "TSTAGE")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.TSTAGE;
                        else if (inp == "T")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.T;
                        else if (inp == "TN")
                            aggDamgFuncCodeType = AggDamgFuncCodeType.TN;
                        else
                            ixFieldCode = -1;

                    }
                }
                //Process the Data
                SingleDamageFunction singleDamgFunc = null;

                switch (aggDamgFuncCodeType)
                {
                    //Structure stage-damage
                    case AggDamgFuncCodeType.SSTAGE:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.STRUCTURE);
                        numPoints[0] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[0], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.Depth);
                        break;
                    case AggDamgFuncCodeType.S:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.STRUCTURE);
                        numPoints[0] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[0], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.Damage);
                        break;
                    case AggDamgFuncCodeType.SN:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.STRUCTURE);
                        numPoints[0] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[0], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.StdDev);
                        break;
                    //Content stage-damage
                    case AggDamgFuncCodeType.CSTAGE:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.CONTENT);
                        numPoints[1] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[1], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.Depth);
                        break;
                    case AggDamgFuncCodeType.C:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.CONTENT);
                        numPoints[1] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[1], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.Damage);
                        break;
                    case AggDamgFuncCodeType.CN:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.CONTENT);
                        numPoints[1] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[1], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.StdDev);
                        break;
                    //Other stage-damage
                    case AggDamgFuncCodeType.OSTAGE:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.OTHER);
                        numPoints[2] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[2], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.Depth);
                        break;
                    case AggDamgFuncCodeType.O:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.OTHER);
                        numPoints[2] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[2], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.Damage);
                        break;
                    case AggDamgFuncCodeType.ON:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.OTHER);
                        numPoints[2] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[2], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.StdDev);
                        break;
                    //Automobile stage-damage
                    case AggDamgFuncCodeType.ASTAGE:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.CAR);
                        numPoints[3] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[3], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.Depth);
                        break;
                    case AggDamgFuncCodeType.A:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.CAR);
                        numPoints[3] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[3], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.Damage);
                        break;
                    case AggDamgFuncCodeType.AN:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.CAR);
                        numPoints[3] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[3], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.StdDev);
                        break;
                    //Total stage-damage
                    case AggDamgFuncCodeType.TSTAGE:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.TOTAL);
                        numPoints[4] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[4], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.Depth);
                        break;
                    case AggDamgFuncCodeType.T:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.TOTAL);
                        numPoints[4] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[4], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.Damage);
                        break;
                    case AggDamgFuncCodeType.TN:
                        singleDamgFunc = _AggregateDamageFunction.GetSingleDamageFunction(StructureValueType.TOTAL);
                        numPoints[4] = ProcessAggDamgFuncNumPoints(ixFieldCode);
                        singleDamgFunc.SetNumRows(numPoints[4], ErrorType.NORMAL);
                        ProcessAggDamgFuncPoints(ixFieldCode, singleDamgFunc.StdDev);
                        break;
                }
            }
        }
        protected int ProcessAggDamgFuncNumPoints(int ixFieldCode)
        {
            int numPoints = _NumFieldsInputString - ixFieldCode - 1;
            return numPoints;
        }
        protected void ProcessAggDamgFuncPoints(int ixFieldCode, double[] points)
        {
            int numPoints = _NumFieldsInputString - ixFieldCode - 1;
            if (numPoints > 0) _MustFlushAggDamgFunc = true;
            for (int i = ixFieldCode + 1; i < _NumFieldsInputString; i++)
            {
                points[i - ixFieldCode - 1] = Convert.ToDouble(_InputStringParsed[i]);
            }
            return;
        }
        protected void FlushAggDamgFunc()
        {
            if (_MustFlushAggDamgFunc)
            {
                GlobalVariables.mp_fdaStudy.GetAggDamgFuncList().Add(_AggregateDamageFunction, _Logger);
            }
            _MustFlushAggDamgFunc = false;
            _AggregateDamageFunction.Reset();
        }

        #endregion
        #endregion
        #region Functions
        bool IsKeyRecord(string theField)
        {
            bool itisKeyRecord = false;
            _TheImportCode = TheImportCode.NO_KEY;

            if (theField == "PLAN_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.PLAN_NAME;
            }
            else if (theField == "YEAR_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.YEAR_NAME;
            }
            else if (theField == "STRM_NME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.STRM_NME;
            }
            else if (theField == "RCH_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.RCH_NAME;
            }
            else if (theField == "CAT_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.CAT_NAME;
            }
            else if (theField == "OCC_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.OCC_NAME;
            }
            else if (theField == "MOD_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.MOD_NAME;
            }
            else if (theField == "STRUC_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.STRUC_NAME;
            }
            else if (theField == "WSP_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.WSP_NAME;
            }
            else if (theField == "FREQ_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.FREQ_NAME;
            }
            else if (theField == "RATE_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.RATE_NAME;
            }
            else if (theField == "LEVEE_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.LEVEE_NAME;
            }
            else if (theField == "SD_NAME")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.SD_NAME;
            }
            else if (theField == "SM_PLAN")
            {
                itisKeyRecord = true;
                _TheImportCode = TheImportCode.SM_PLAN;
            }
            else
            {
                itisKeyRecord = false;
                _TheImportCode = TheImportCode.NO_KEY;
            }
            //Key Record, Flush Records and set code
            if (itisKeyRecord)
            {
                //Flush
                //Set current operation
                _CurrentImportDataCode = _TheImportCode;
            }
            return itisKeyRecord;
        }
        #endregion
    }
}
