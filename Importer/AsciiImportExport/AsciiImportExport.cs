using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer
{
    #region enum_definitions
    public enum StructureErrorValue { FIRSTFLOOR, VALUE, CONTENTVALUERATIO, OTHERVALUERATIO };
    public enum StructureValueType { STRUCTURE, CONTENT, OTHER, CAR, TOTAL };
    public enum StreamBank { LEFT, RIGHT, BOTH}; 
    #endregion


    public class AsciiImportExport
    {
        #region Notes
        // Created By: q0hecrdc
        // Created Date: Nov2017
        #endregion
        #region Fields

        #region enums

        public enum TheImportCode
        {
            NO_KEY = 0,
            PLAN_NAME,
            YEAR_NAME,
            STRM_NME,
            RCH_NAME,
            CAT_NAME,
            OCC_NAME,
            MOD_NAME,
            STRUC_NAME,
            WSP_NAME,
            FREQ_NAME,
            RATE_NAME,
            LEVEE_NAME,
            SD_NAME,
            SM_PLAN
        };
        public enum TheDamageFunctionType
        {
            ERROR,
            STAGE,
            S,
            SN,
            SL,
            STU,
            STL,
            C,
            CN,
            CL,
            CTU,
            CTL,
            O,
            ON,
            OL,
            OTU,
            OTL,
            A,
            AN,
            AL,
            ATU,
            ATL
        };
        protected enum ProbFuncCodeType
        {
            UNKNOWN,    // 0
            LP3,        // 1
            SYN,        // 2
            PROB,       // 3
            FLOW,       // 4
            STAGE,      // 5
            UDFN,       // 6
            UDFL,       // 7
            UDFTL,      // 8
            UDFTU,      // 9
            QIN,        //10
            QOUT,       //11
            QN,         //12   
            QL,         //13
            QTU,        //14
            QTL         //15
        };
        public enum RatingFuncCodeType
        {
            UNKNOWN,    // 0
            Q,          // 1
            S,          // 2
            SN,         // 3
            SL,         // 4
            STL,        // 5
            STU         // 6
        };
        public enum LeveeCodeType
        {
            UNKNOWN,    // 0
            SE,         // 1
            SI,         // 2
            GSE,        // 3
            GPF         // 4
        }
        public enum AggDamgFuncCodeType
        {
            UNKNOWN,    // 0
            SSTAGE,     // 2
            S,          // 3
            SN,         // 4
            CSTAGE,     // 5
            C,          // 6
            CN,         // 7
            OSTAGE,     // 8
            O,          // 9
            ON,         //10
            ASTAGE,     //11
            A,          //12
            AN,         //13
            TSTAGE,     //14
            T,          //15
            TN          //16
        }
        #endregion
        protected TheImportCode _TheImportCode;
        protected TheImportCode _CurrentImportDataCode;
        protected string _InpString = "";
        protected string _InpStringCap = "";
        protected string[] _InputStringParsed;
        protected string[] _InputStringCapParsed;
        protected int _NumFieldsInputString;

        //The fields for each data type
        static public string[] FieldsPlan = { "PLAN_NAME", "PLAN_DESC" };
        protected enum FieldsPlanEnum { PLAN_NAME, PLAN_DESC };
        static public string[] FieldsYear = { "YEAR_NAME" };
        static public string[] FieldsStream = { "STRM_NME", "STRM_DESC" };
        static public string[] FieldsReach = { "RCH_NAME", "RCH_DESC", "STREAM_NME", "BEG_STA", "END_STA", "BANK", "INDEX_STA" };
        static public string[] FieldsCategory = { "CAT_NAME", "CAT_DESCRIPTION", "COST_FACTOR" };
        static public string[] FieldsOcctype = { "OCC_NAME", "OCC_DESCRIPTION", "CAT_NAME", "PARAMETER", "START_DATA" };
        static public string[] FieldsModule = { "MOD_NAME", "MOD_DESC" };
        static public string[] FieldsStructure =
        {
            "STRUC_NAME",	// 1
            "STRUC_DESC",	// 2
            "DATE",			// 3
            "CAT_NAME",		// 4
            "STREAM_NAME",	// 5
            "OCC_NAME",		// 6
            "STATION",		// 7
            "BANK",			// 8
            "YEAR",			// 9
            "STRUC_VAL",	//10
            "CONT_VAL",		//11
            "OTHER_VAL",	//12
            "CAR_VAL",		//13
            "USE_FF",       //14
            "1F_STAGE",		//15
            "GRND_STAGE",	//16
            "FOUND_HT",		//17
            "BEGIN_DAMG",	//18
            "AUTO_DIFF",	//19
            "STREET",		//20
            "CITY",			//21
            "STATE",		//22
            "ZIP",			//23
            "NORTH",		//24
            "EAST",			//25
            "ZONE",			//26
            "NUM_STRUCT",	//27
            "NOTES",		//28
            "MOD_NAME",		//29
            "SID_RCH",		//30
            "IMAGEFNAME",	//31
            "NUM_CARS",	    //32
            "PARCEL_NUM"    //33
        };
        static public string[] FieldsWsp =
        {
            "WSP_NAME",       // 1
            "DESCRIPTION",    // 2
            "PLAN",           // 3
            "YEAR",           // 4
            "STREAM",         // 5
            "TYPE",           // 6
            "NOTES"           // 7
        };
        static public string[] FieldsProbFunc =
        {   "FREQ_NAME",        // 1
            "PLAN",             // 2
            "YEAR",             // 3
            "STREAM",           // 4
            "REACH",            // 5
            "YRS",              // 6
            "FFTYPE",           // 7
            "DATATYPE",         // 8
            "UNCERTTYPE",       // 9
            "DESC"              //10
        };
        static public string[] FieldsRatingFunc =
        {
            "RATE_NAME",        // 1
            "PLAN",             // 2
            "YEAR",             // 3
            "STREAM",           // 4
            "REACH",            // 5
            "ERRTYPE",          // 6
            "STAGEBASE",        // 7
            "BASESD",           // 8
            "BASESDL",          // 9
            "BASELO",           //10
            "BASEHI",           //11
            "DESC"              //12
        };
        static public string[] FieldsLevee =
        {
            "LEVEE_NAME",       // 1
            "PLAN",             // 2
            "YEAR",             // 3
            "STREAM",           // 4
            "REACH",            // 5
            "ELTOP",            // 6
            "DESC"              // 7
        };
        static public string[] FieldsAggDamgFunc =
        {
            "SD_NAME",      // 1
            "PLAN",         // 2
            "YEAR",         // 3
            "STREAM",       // 4
            "REACH",        // 5
            "CATAGORY",     // 6
            "DESC"          // 7
        };

        protected SingleDamageFunction[] _SingleDamageFunction;

        protected Plan _Plan = new Plan();
        protected Year _Year = new Year();
        protected Stream _Stream = new Stream();
        protected DamageReach _DamageReach = new DamageReach();
        protected DamageCategory _DamageCategory = new DamageCategory();
        protected OccupancyType _OccupancyType = new OccupancyType();
        protected StructureModule _StructureModule = new StructureModule();
        protected Structure _Structure = new Structure();
        protected ProbabilityFunction _FrequencyFunction = new ProbabilityFunction();
        protected RatingFunction _RatingFunction = new RatingFunction();
        protected Levee _Levee = new Levee();
        protected WaterSurfaceProfile _WaterSurfaceProfile = new WaterSurfaceProfile();
        protected AggregateDamageFunction _AggregateDamageFunction = new AggregateDamageFunction();

        #endregion
        #region Properties
        #endregion
        #region Constructors
        public AsciiImportExport()
        {
            TheImportCode t = TheImportCode.NO_KEY;
        }
        #endregion
        #region Voids
        #endregion
        #region Functions
        #endregion
    }
}
