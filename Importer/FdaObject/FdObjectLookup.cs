using System;
using System.Collections.Generic;
using System.Text;

namespace Importer
{
    public class FdObjectLookup
    {
        public long IdPlan { get; set; }
        public long IdYear { get; set; }
        public long IdStream { get; set; }
        public long IdReach { get; set; }
        public long IdCategory { get; set; }
        public long IdFunc { get; set; }

        public string NamePlan { get; set; }
        public string NameYear { get; set; }
        public string NameStream { get; set; }
        public string NameReach { get; set; }
        public string NameCategory { get; set; }
        public string NameFunc { get; set; }
        public string Name { get; set; }

        //friend class DbfFileFdLookupManager;
        //friend class DbfFileFdAssignedManager;
        public FdObjectLookup()
        {
            Reset();
        }

        public void Reset()
        {
            ResetNames();
            ResetIds();
        }
        public void ResetNames()
        {
            NamePlan = "";
            NameYear = "";
            NameStream = "";
            NameReach = "";
            NameCategory = "";
            NameFunc = "";
            Name = " ";
        }
        public void ResetIds()
        {
            IdPlan = IdYear = IdStream = IdReach = IdCategory = IdFunc = -1L;
        }
        public void FindPysrcNamesInLists()
        {
            ResetNames();

            NamePlan = GlobalVariables.mp_fdaStudy.GetPlanList().getName(IdPlan);
            NameYear = GlobalVariables.mp_fdaStudy.GetYearList().getName(IdYear);
            NameStream = GlobalVariables.mp_fdaStudy.GetStreamList().getName(IdStream);
            NameReach = GlobalVariables.mp_fdaStudy.GetDamageReachList().getName(IdReach);
            NameCategory = GlobalVariables.mp_fdaStudy.GetDamageCategoryList().getName(IdCategory);
        }
        public void findPysrcIdsInLists()
        {
            IdPlan = GlobalVariables.mp_fdaStudy.GetPlanList().GetId(NamePlan);
            IdYear = GlobalVariables.mp_fdaStudy.GetYearList().GetId(NameYear);
            IdStream = GlobalVariables.mp_fdaStudy.GetStreamList().GetId(NameStream);
            IdReach = GlobalVariables.mp_fdaStudy.GetDamageReachList().GetId(NameReach);
            IdCategory = GlobalVariables.mp_fdaStudy.GetDamageCategoryList().GetId(NameCategory);
        }

        /*
         * TODO
        public void setPYSRC(NameManager* nameManager)
        {
            long idPlan = -1L;
            long idYear = -1L;
            long idStream = -1L;
            long idReach = -1L;
            long idCategory = -1L;

            idPlan = nameManager->getPlanId();
            idYear = nameManager->getYearId();
            idStream = nameManager->getStreamId();
            idReach = nameManager->getReachId();
            idCategory = nameManager->getCategoryId();

            this->setIds(idPlan, idYear, idStream, idReach, idCategory);

        }
        */

        public void setIds(long idPlan = -1L,
                    long idYear = -1L,
                    long idStream = -1L,
                    long idReach = -1L,
                    long idCategory = -1L)
        {
            IdPlan = idPlan;
            IdYear = idYear;
            IdStream = idStream;
            IdReach = idReach;
            IdCategory = idCategory;
        }

        void setPYSRCIds(long planId)
        {
            IdPlan = planId;
        }
        void setPYSRCIds(long planId,
                         long yearId)
        {
            IdPlan = planId;
            IdYear = yearId;
        }
        void setPYSRCIds(long planId,
                       long yearId,
                       long streamId)
        {
            IdPlan = planId;
            IdYear = yearId;
            IdStream = streamId;
        }
        void setPYSRCIds(long planId,
                       long yearId,
                       long streamId,
                       long reachId)
        {
            IdPlan = planId;
            IdYear = yearId;
            IdStream = streamId;
            IdReach = reachId;
        }
        void setPYSRCIds(long planId,
                       long yearId,
                       long streamId,
                       long reachId,
                       long categoryId)
        {
            IdPlan = planId;
            IdYear = yearId;
            IdStream = streamId;
            IdReach = reachId;
            IdCategory = categoryId;
        }

        long getKeyId() { return IdFunc; }

        void setNames(string planName = "",
                    string yearName = "",
                    string streamName = "",
                    string reachName = "",
                    string categoryName = "")
        {
            NamePlan = planName;
            NameYear = yearName;
            NameStream = streamName;
            NameReach = reachName;
            NameCategory = categoryName;
        }

        void setPYSRCNames(string planName)
        {
            NamePlan = planName;
        }

        void setPYSRCNames(string planName,
                         string yearName)
        {
            NamePlan = planName;
            NameYear = yearName;
        }

        void setPYSRCNames(string planName,
                         string yearName,
                         string streamName)
        {
            NamePlan = planName;
            NameYear = yearName;
            NameStream = streamName;
        }

        void setPYSRCNames(string planName,
                         string yearName,
                         string streamName,
                         string reachName)
        {
            NamePlan = planName;
            NameYear = yearName;
            NameStream = streamName;
            NameReach = reachName;
        }

        void setPYSRCNames(string planName,
                         string yearName,
                         string streamName,
                         string reachName,
                         string categoryName)
        {
            NamePlan = planName;
            NameYear = yearName;
            NameStream = streamName;
            NameReach = reachName;
            NameCategory = categoryName;

        }
    }
}
