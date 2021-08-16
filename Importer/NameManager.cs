using System;
using System.Collections.Generic;
using System.Text;

namespace Importer
{
    class NameManager
    {
        static int NUM_CH_SHORT_PLAN;
        static int NUM_CH_SHORT_YEAR;
        static int NUM_CH_SHORT_STREAM;
        static int NUM_CH_SHORT_REACH;
        static int NUM_CH_SHORT_CATEGORY;

        char m_typeName;

        PlanList mp_planList;
        YearList mp_yearList;
        StreamList mp_streamList;
        DamageReachList mp_reachList;
        DamageCategoryList mp_categoryList;

        int m_numPlans;
        int m_numYears;
        int m_numStreams;
        int m_numReaches;
        int m_numCategories;

        Plan m_plan;
        Year m_year;
        Stream m_stream;
        DamageReach m_reach;
        DamageCategory m_damageCategory;

        long m_idPlan;
        long m_idYear;
        long m_idStream;
        long m_idReach;
        long m_idCategory;

        string m_namePlan;
        string m_nameYear;
        string m_nameStream;
        string m_nameReach;
        string m_nameCategory;

        //BobsGlob m_bobsGlob;

        int m_ncNewName;
        int m_ncNewNameAlloc;
        string mp_newName;

        /*
        int                 NUM_CH_SHORT_PLAN;
        int                 NUM_CH_SHORT_YEAR;
        int                 NUM_CH_SHORT_STREAM;
        int                 NUM_CH_SHORT_REACH;
        int                 NUM_CH_SHORT_CATEGORY;
        */

        string m_nameShort;
        string m_nameShortPlan;
        string m_nameShortYear;
        string m_nameShortStream;
        string m_nameShortReach;
        string m_nameShortCategory;

        NameManager()
        {
            mp_planList = null;
            mp_yearList = null;
            mp_streamList = null;
            mp_reachList = null;
            mp_categoryList = null;

            m_typeName = 'C';

            mp_newName = null;
            m_ncNewName = 0;
            m_ncNewNameAlloc = 0;

            m_nameShort = string.Empty;

            resetNewName(101);

            /*
            NUM_CH_SHORT_PLAN      = 7;
            NUM_CH_SHORT_YEAR      = 4;
            NUM_CH_SHORT_STREAM    = 0;
            NUM_CH_SHORT_REACH     = 7;
            NUM_CH_SHORT_CATEGORY  = 5;
            */

            resetBasic();
        }
        void resetBasic()
        {
            m_numPlans = 0;
            m_numYears = 0;
            m_numStreams = 0;
            m_numReaches = 0;
            m_numCategories = 0;

            m_idPlan = -1L;
            m_idYear = -1L;
            m_idStream = -1L;
            m_idReach = -1L;
            m_idCategory = -1L;

            m_namePlan = string.Empty;
            m_nameYear = string.Empty;
            m_nameStream = string.Empty;
            m_nameReach = string.Empty;
            m_nameCategory = string.Empty;

            m_nameShort = string.Empty;
            m_nameShortPlan = string.Empty;
            m_nameShortYear = string.Empty;
            m_nameShortStream = string.Empty;
            m_nameShortReach = string.Empty;
            m_nameShortCategory = string.Empty;
        }
        public void setNumCharShort(int ncPlan,
                            int ncYear,
                            int ncStream,
                            int ncReach,
                            int ncCategory)
        {
            int[] nc = new int[5];

            nc[0] = ncPlan;
            nc[1] = ncYear;
            nc[2] = ncStream;
            nc[3] = ncReach;
            nc[4] = ncCategory;

            setNumCharShortPlan(nc[0]);
            setNumCharShortYear(nc[1]);
            setNumCharShortStream(nc[2]);
            setNumCharShortReach(nc[3]);
            setNumCharShortCategory(nc[4]);
        }
        int setNumCharShortPlan( int numChar)
        {
            NUM_CH_SHORT_PLAN = numChar;
            return NUM_CH_SHORT_PLAN;
        }

        int setNumCharShortYear( int numChar)
        {
            NUM_CH_SHORT_YEAR = numChar;
            return NUM_CH_SHORT_YEAR;
        }

        int setNumCharShortStream( int numChar)
        {
            NUM_CH_SHORT_STREAM = numChar;
            return NUM_CH_SHORT_STREAM;
        }

        int setNumCharShortReach( int numChar)
        {
            NUM_CH_SHORT_REACH = numChar;
            return NUM_CH_SHORT_REACH;
        }

        int setNumCharShortCategory( int numChar)
        {
            NUM_CH_SHORT_CATEGORY = numChar;
            return NUM_CH_SHORT_CATEGORY;
        }
        void resetNewName(int numCharAlloc)
        {
            //Reallocate name to numCharAlloc
            if (numCharAlloc > 0)
            {
                mp_newName = "";
            }
            else
            {
                mp_newName = string.Empty;
            }
        }
        public void assignLists()
        {
            mp_planList = GlobalVariables.mp_fdaStudy.GetPlanList();
            mp_yearList = GlobalVariables.mp_fdaStudy.GetYearList();
            mp_streamList = GlobalVariables.mp_fdaStudy.GetStreamList();
            mp_reachList = GlobalVariables.mp_fdaStudy.GetDamageReachList();
            mp_categoryList = GlobalVariables.mp_fdaStudy.GetDamageCategoryList();

            return;
        }
        public void setPlan(string namePlan) 
        {
            m_idPlan = mp_planList.GetId(namePlan);
            m_namePlan = namePlan.PadRight((int)(Study.FdaSizes.NAME_SIZE));
        }
        public void setPlan(long idPlan)
        {
            m_idPlan = idPlan;
            m_namePlan = GlobalVariables.mp_fdaStudy.GetPlanList().getName(idPlan).Trim().PadRight((int)(Study.FdaSizes.NAME_SIZE));
        }
        public void setYear(string nameYear)
        {
            m_idYear = GlobalVariables.mp_fdaStudy.GetYearList().GetId(nameYear);
            m_nameYear = nameYear.PadRight(4);
        }
        public void setYear(long idYear)
        {
            m_idYear = idYear;
            m_nameYear = GlobalVariables.mp_fdaStudy.GetYearList().getName(idYear);
        }
        public void setStream(string nameStream)
        {
            m_idStream = GlobalVariables.mp_fdaStudy.GetStreamList().GetId(nameStream);
            m_nameStream = nameStream.PadRight((int)(Study.FdaSizes.NAME_SIZE));
        }
        public void setStream(long idStream)
        {
            m_idStream = idStream;
            m_nameStream = GlobalVariables.mp_fdaStudy.GetStreamList().getName(idStream).Trim().PadRight((int)Study.FdaSizes.NAME_SIZE);
        }
        public void setReach(string nameReach)
        { 
            m_idReach = GlobalVariables.mp_fdaStudy.GetDamageReachList().GetId(nameReach);
            m_nameReach = nameReach.PadRight((int)(Study.FdaSizes.NAME_SIZE));
        }
        public void setReach(long idReach)
        {
            m_idReach = idReach;
            m_nameReach = GlobalVariables.mp_fdaStudy.GetDamageReachList().getName(idReach).Trim().PadRight((int)(Study.FdaSizes.NAME_SIZE));
        }
        void setCategory(string  nameCategory)
        {
            m_idCategory =GlobalVariables.mp_fdaStudy.GetDamageCategoryList().GetId(nameCategory);
            m_nameCategory = nameCategory.PadRight((int)(Study.FdaSizes.NAME_SIZE));
        }
        void setCategory(long idCategory)
        {
            m_idCategory = idCategory;
            m_nameCategory = GlobalVariables.mp_fdaStudy.GetDamageCategoryList().getName(idCategory).Trim().PadRight((int)(Study.FdaSizes.NAME_SIZE));
        }
        public void setPysrcNames(string namePlan,
                                    string nameYear = "@@@@@@",
                                    string nameStream = "@@@@@@",
                                    string nameReach = "@@@@@@",
                                    string nameCategory = "@@@@@@")
        {
            string c;
            string[] p = new string[5];

            p[0] = namePlan;
            p[1] = nameYear;
            p[2] = nameStream;
            p[3] = nameReach;
            p[4] = nameCategory;

            for (int i = 0; i < 5; i++)
            {
                if (p[i] == "@@@@@@")
                {
                    //don't set it
                }
                else
                {
                    c = p[i];

                    switch (i)
                    {
                        case 0:
                            setPlan(c);
                            break;
                        case 1:
                            setYear(c);
                            break;
                        case 2:
                            setStream(c);
                            break;
                        case 3:
                            setReach(c);
                            break;
                        case 4:
                            setCategory(c);
                            break;
                        default:
                            //nothing right now
                            break;
                    }
                }
            }
        }
        void setPysrcNames(long idPlan,
                            long idYear = -10,
                            long idStream = -10,
                            long idReach = -10,
                            long idCategory = -10)
        {
            long idP = idPlan;
            long idY = idYear;
            long idS = idStream;
            long idR = idReach;
            long idC = idCategory;

            if (idP > 0) setPlan(idP);
            if (idY > 0) setYear(idY);
            if (idS > 0) setStream(idS);
            if (idR > 0) setReach(idR);
            if (idC > 0) setCategory(idC);
        }
        //----------------------------------------------------------------------------------------
        //  Generate the Name
        //----------------------------------------------------------------------------------------
        public string genName(string plan,
                               string year,
                               string stream,
                               string reach,
                               string category,
                               long idNextObj,
                               int maxCharNewName = (int)(Study.FdaSizes.NAME_SIZE))
        { 
            string c = string.Empty;

            int ncIdNext = 9; //hardwire for now
            int ic = 0, nc = 0;

            int numCharTotal = NUM_CH_SHORT_PLAN +
                               NUM_CH_SHORT_YEAR +
                               NUM_CH_SHORT_STREAM +
                               NUM_CH_SHORT_REACH +
                               NUM_CH_SHORT_CATEGORY +
                               //numChrIdNextObj;
                               ncIdNext;

            assignLists();

            m_idPlan      = GlobalVariables.mp_fdaStudy.GetPlanList().GetId(plan);
            m_idYear      = GlobalVariables.mp_fdaStudy.GetYearList().GetId(year);
            m_idStream    = GlobalVariables.mp_fdaStudy.GetStreamList().GetId(stream);
            m_idReach     = GlobalVariables.mp_fdaStudy.GetDamageReachList().GetId(reach);
            m_idCategory  = GlobalVariables.mp_fdaStudy.GetDamageCategoryList().GetId(category);

            mp_newName = string.Empty;

            for(int i = 0; i< 6; i++)
            {
                c = string.Empty;
                switch(i)
                {
                    case 0:
                        nc = Math.Min(NUM_CH_SHORT_PLAN, plan.TrimEnd().Length);
                        if (NUM_CH_SHORT_PLAN > 0) c = plan.Substring(0, nc).Trim().PadRight(NUM_CH_SHORT_PLAN);
                        break;
                    case 1:
                        nc = Math.Min(NUM_CH_SHORT_YEAR, year.TrimEnd().Length);
                        if (NUM_CH_SHORT_YEAR > 0) c = year.Substring(0, nc).Trim().PadRight(NUM_CH_SHORT_YEAR);
                            break;
                    case 2:
                        nc = Math.Min(NUM_CH_SHORT_STREAM, stream.TrimEnd().Length);
                        if (NUM_CH_SHORT_STREAM > 0) c = stream.Substring(0, nc).Trim().PadRight(NUM_CH_SHORT_STREAM);
                            break;
                    case 3:
                        nc = Math.Min(NUM_CH_SHORT_REACH, reach.TrimEnd().Length);
                        if (NUM_CH_SHORT_REACH > 0) c = reach.Substring(0, nc).Trim().PadRight(NUM_CH_SHORT_REACH);
                        break;
                    case 4:
                        nc = Math.Min(NUM_CH_SHORT_CATEGORY, category.TrimEnd().Length);
                        if (NUM_CH_SHORT_CATEGORY > 0) c = category.Substring(0, nc).Trim().PadRight(NUM_CH_SHORT_CATEGORY);
                        break;
                    case 5:
                        c = idNextObj.ToString("000000000");
                        break;
                    default:
                        break;
                }
                mp_newName += c;
            }
            return mp_newName;
        }
        public string genName(long idNextObj, int maxCharNewName = (int)(Study.FdaSizes.NAME_SIZE))
        {
            string c = string.Empty;
            int ncIdNext = 9;//HARDWIRE FOR NOW
            int ic = 0, nc = 0;


            int numCharTotal = NUM_CH_SHORT_PLAN +
                               NUM_CH_SHORT_YEAR +
                               NUM_CH_SHORT_STREAM +
                               NUM_CH_SHORT_REACH +
                               NUM_CH_SHORT_CATEGORY +
                               //numChrIdNextObj;
                               ncIdNext;

            for(int i = 0; i< 6; i++)
            {
                c = string.Empty;

                switch(i)
                {
                    case 0:
                        nc = Math.Min(NUM_CH_SHORT_PLAN, m_namePlan.TrimEnd().Length);
                        if (NUM_CH_SHORT_PLAN > 0) c = m_namePlan.Substring(0, nc).Trim().PadRight(NUM_CH_SHORT_PLAN);
                        break;
                    case 1:
                        nc = Math.Min(NUM_CH_SHORT_YEAR, m_nameYear.TrimEnd().Length);
                        if (NUM_CH_SHORT_YEAR > 0) c = m_nameYear.Substring(0, nc).Trim().PadRight(NUM_CH_SHORT_YEAR);
                      break;
                    case 2:
                        nc = Math.Min(NUM_CH_SHORT_STREAM, m_nameStream.TrimEnd().Length);
                        if (NUM_CH_SHORT_STREAM > 0) c = m_nameStream.Substring(0, nc).Trim().PadRight(NUM_CH_SHORT_STREAM);
                        break;
                    case 3:
                        nc = Math.Min(NUM_CH_SHORT_REACH, m_nameReach.TrimEnd().Length);
                        if (NUM_CH_SHORT_REACH > 0) c = m_nameReach.Substring(0, nc).Trim().PadRight(NUM_CH_SHORT_REACH);
                        break;
                    case 4:
                        nc = Math.Min(NUM_CH_SHORT_CATEGORY, m_nameCategory.TrimEnd().Length);
                        if (NUM_CH_SHORT_CATEGORY > 0) c = m_nameCategory.Substring(0, nc).Trim().PadRight(NUM_CH_SHORT_CATEGORY);
                        break;
                    case 5:
                        c = idNextObj.ToString("000000000");
                        break;
                    default:
                        break;
                }
                mp_newName += c;
            }
            return mp_newName;
        }
        public string genNameShort( int numCharMax,  string name)
        {
            int i = 0, nc = 0;
            m_nameShort = string.Empty;
            nc = name.Length;

            if (numCharMax > 0)
            {

                //non-blank name, use first numCharMax characters
                if (nc > 0)
                {
                    int ncMax = Math.Min(numCharMax, nc);
                    m_nameShort = name.Substring(0, ncMax).TrimEnd().PadRight(numCharMax);
                }

                //name blank, set first numCharMax characters to blank.
                else
                {
                    m_nameShort = "".PadRight(numCharMax);
                }
            }
            return m_nameShort;
        }
        public string genNameShortPlan( string name = "")
        {
            int ncName = name.Length;
            if (ncName > 0)
                m_nameShortPlan = genNameShort(NUM_CH_SHORT_PLAN, name);
            else
                m_nameShortPlan = genNameShort(NUM_CH_SHORT_PLAN, m_namePlan);
            return m_nameShortPlan;
        }
        public string genNameShortYear(string name = "")
        {
            int ncName = name.Length;
            if (ncName > 0)
            {
                m_nameShortYear = genNameShort(NUM_CH_SHORT_YEAR, name);
            }
            else
            {
                m_nameShortYear = genNameShort(NUM_CH_SHORT_YEAR, m_nameYear);
            }
            return m_nameShortYear;
        }
        public string genNameShortStream( string name = "")
        {
            int ncName = name.Length;

            if (ncName > 0)
                name=m_nameShortStream = genNameShort(NUM_CH_SHORT_STREAM, name);
            else
                m_nameShortStream = genNameShort(NUM_CH_SHORT_STREAM, m_nameStream);
            return m_nameShortStream;
        }
        public string genNameShortReach( string name = "")
        {
            int ncName = name.Length;

            if (ncName > 0)
                m_nameShortReach = genNameShort(NUM_CH_SHORT_REACH, name);
            else
                m_nameShortReach = genNameShort(NUM_CH_SHORT_REACH, m_nameReach);
            return m_nameShortReach;
        }
        public string genNameShortCategory( string name = "")
        {
            int ncName = name.Length;

            if (ncName > 0)
                m_nameShortCategory = genNameShort(NUM_CH_SHORT_CATEGORY, name);
            else
                m_nameShortCategory = genNameShort(NUM_CH_SHORT_CATEGORY, m_nameCategory);
            return m_nameShortCategory;
        }
    }
}
