using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using System.IO;
namespace DbMemory
{
    #region enums
    public enum ErrorType { NONE, NORMAL, LOGNORMAL, TRIANGULAR, UNIFORM };
    public enum OccTypeStrucComponent { FFLOOR, STRUCTURE, CONTENT, OTHER, AUTO};
    public enum WspDataType { UNKNOWN, STAGE_FREQUENCY, DISCHARGE_FREQUENCY };
    public enum AggDamgFuncComponent { STRUCTURE, CONTENT, OTHER, AUTO, TOTAL};
    #endregion

    public class Study
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields
        static public double badNumber = -901.0;
        static public char PATH_SEPARATOR = '\\';
        public enum FdaSizes { NAME_SIZE = 32, DESC_SIZE = 64 };
        //public enum NAME_SIZE {32};
        static public StreamWriter _StreamWriter;
        public string _FilenameTrace;
        public int _TraceConvertLevel{ get; set; }

        private StudyData _StudyData = new StudyData();
        private NextIdMgr _NextIdMgr = new NextIdMgr();
        private EconGlobalParameters _EconGlobalParameters = new EconGlobalParameters();
        private EconPYList _EconPyList = new EconPYList();
        private PlanList _PlanList = new PlanList();
        private YearList _YearList = new YearList();
        private StreamList _StreamList = new StreamList();
        private DamageReachList _ReachList = new DamageReachList();
        private DamageCategoryList _CategoryList = new DamageCategoryList();
        private StructureModuleList _StructureModuleList = new StructureModuleList();
        private OccupancyTypeList _OccupancyTypeList = new OccupancyTypeList();
        private StructureList _StructureList = new StructureList();
        private ProbabilityFunctionList _ProbabilityFunctionList = new ProbabilityFunctionList();
        private RatingFunctionList _RatingFunctionList = new RatingFunctionList();
        private LeveeList _LeveeList = new LeveeList();
        private WaterSurfaceProfileList _WspList = new WaterSurfaceProfileList();
        private AggregateDamageFunctionList _AggDamgFuncList = new AggregateDamageFunctionList();
        private EadResultList _EadResultList = new EadResultList();
        private EquivBenefitsList _EquivBenefitsList = new EquivBenefitsList();

        private StrucGroupLookupList _StrucGroupLookupList = new StrucGroupLookupList();
        private WspLookupList _WspLookupList = new WspLookupList();
        private ProbLookupList _ProbLookupList = new ProbLookupList();
        private RateLookupList _RateLookupList = new RateLookupList();
        private LeveeLookupList _LeveeLookupList = new LeveeLookupList();
        private AggDamgLookupList _AggDamgLookupList = new AggDamgLookupList();
        private EadResultsLookupList _EadResultsLookupList = new EadResultsLookupList();
        #endregion
        #region Properties
        #endregion
        #region Constructors
        public Study()
        {

        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        public bool Open(string pathDbf, string filenameSty)
        {
            /*
            TODO: Need to supply study name from first argument. It can be fully
                qualified. Break out the subdirectory name, base filename, and
                filename for .sty file.
                */

            bool isOpen = true;
            string _FilenameTrace = pathDbf + "\\Fda_TraceConversion.txt";

            /*

            using (var fileStreamConvertTrace = new FileStream(_FilenameTrace, FileMode.Create, FileAccess.Write))
            {
                using (_StreamWriter = new StreamWriter(fileStreamConvertTrace))
                {
                    _StreamWriter.WriteLine($"Writing to the file: {_FilenameTrace}");
                }
            }
            */
            //rdc critical;26Nov2018;TODO;Need checks in here like above but global access.
            var fileStreamConvertTrace = new FileStream(_FilenameTrace, FileMode.Create, FileAccess.Write);
            _StreamWriter = new StreamWriter(fileStreamConvertTrace);
            _StreamWriter.WriteLine($"Writing to the file: {_FilenameTrace}");

            _TraceConvertLevel = 1;
            return isOpen;
        }
        public void Close()
        {
            _StreamWriter.WriteLine($"\n\nClosing the file.");
            _StreamWriter.Close();
            _StreamWriter.Dispose();
        }

        public StudyData GetStudyData()
        { return _StudyData; }
        public string DetermineVersionDate()
        {
            return "05Nov2018 Version 2.0";
        }
        public NextIdMgr GetNextIdMgr()
        { return _NextIdMgr; }
        public EconGlobalParameters getEconGlobalParameters()
        { return _EconGlobalParameters; }
        public EconPYList GetEconPyList()
        { return _EconPyList; }
        public PlanList GetPlanList()
        { return _PlanList; }
        public YearList GetYearList()
        { return _YearList; }
        public StreamList GetStreamList()
        { return _StreamList; }
        public DamageReachList GetDamageReachList()
        { return _ReachList; }
        public DamageCategoryList GetDamageCategoryList()
        { return _CategoryList; }
        public StructureModuleList GetStructureModuleList()
        { return _StructureModuleList; }
        public OccupancyTypeList GetOccupancyTypeList()
        { return _OccupancyTypeList; }
        public StructureList GetStructureList()
        { return _StructureList; }
        public ProbabilityFunctionList GetProbabilityFuncList()
        { return _ProbabilityFunctionList; }
        public RatingFunctionList GetRatingFunctionList()
        { return _RatingFunctionList; }
        public LeveeList GetLeveeList()
        { return _LeveeList; }
        public WaterSurfaceProfileList GetWspList()
        { return _WspList; }
        public AggregateDamageFunctionList GetAggDamgFuncList()
        { return _AggDamgFuncList; }
        public EadResultList GetEadResultList()
        { return _EadResultList; }
        public EquivBenefitsList GetEquivBenefitsList()
        { return _EquivBenefitsList; }
        public StrucGroupLookupList GetStrucGroupLookupList()
        { return _StrucGroupLookupList; }
        public WspLookupList GetWspLookupList()
        { return _WspLookupList; }
        public ProbLookupList GetProbLookupList()
        { return _ProbLookupList; }
        public RateLookupList GetRateLookupList()
        { return _RateLookupList; }
        public LeveeLookupList GetLeveeLookupList()
        { return _LeveeLookupList; }
        public AggDamgLookupList GetAggDamgLookupList()
        { return _AggDamgLookupList; }
        public EadResultsLookupList GetEadResultsLookupList()
        {
            return _EadResultsLookupList;
        }




        static public bool IsBadNumber(double theNumber)
        {
            if (Math.Abs(theNumber - badNumber) < 1.0e-07)
                return true;
            else
                return false;
        }
        static public void PrintTable(int numRows, int numCols, string name, double[,] values)
        {
            WriteLine($"\n\nTable {name}");
            for(int irow = 0; irow < numRows; irow++)
            {
                Write($"{(irow + 1).ToString().PadLeft(6)}");
                for(int icol = 0; icol < numCols; icol++)
                {
                    int m = irow * numCols + icol;
                    //Write($" {values[m].ToString().PadLeft(12)}");
                    Write($" {values[irow, icol].ToString().PadLeft(12)}");
                }
                Write("\n");
            }
        }
        #endregion
    }
}
