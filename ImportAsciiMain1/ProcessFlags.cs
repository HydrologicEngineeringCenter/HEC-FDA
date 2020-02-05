using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaCombinedMain
{
    public class ProcessFlags
    {
        #region Properties
        public bool ProcessStudy { get; set; }
        public bool ProcessNextId { get; set; }
        public bool ProcessEconParm { get; set; }
        public bool ProcessPlan { get; set; }
        public bool ProcessYear { get; set; }
        public bool ProcessStream { get; set; }
        public bool ProcessReach { get; set; }
        public bool ProcessCategory { get; set; }
        public bool ProcessOccType { get; set; }
        public bool ProcessStrucGroup { get; set; }
        public bool ProcessStructures { get; set; }
        public bool ProcessWsp { get; set; }
        public bool ProcessProb { get; set; }
        public bool ProcessRate { get; set; }
        public bool ProcessLevee { get; set; }
        public bool ProcessEconPY { get; set; }
        public bool ProcessStageDamage { get; set; }
        public bool ProcessEad { get; set; }
        public bool ProcessEquivAD { get; set; }
        public bool ProcessStrucGroupLook { get; set; }
        public bool ProcessWspLook { get; set; }
        public bool ProcessProbLook { get; set; }
        public bool ProcessRatLook { get; set; }
        public bool ProcessLevLook { get; set; }
        public bool ProcessAggDamgLook { get; set; }
        public bool ProcessEadLook { get; set; }
        #endregion
        #region Constructors
        public ProcessFlags()
        {
            ProcessStudy = false;
            ProcessNextId = false;
            ProcessEconParm = false;
            ProcessPlan = false;
            ProcessYear = false;
            ProcessStream = false;
            ProcessReach = false;
            ProcessCategory = false;
            ProcessOccType = false;
            ProcessStrucGroup = false;
            ProcessStructures = false;
            ProcessWsp = false;
            ProcessProb = false;
            ProcessRate = false;
            ProcessLevee = false;
            ProcessEconPY = false;
            ProcessStageDamage = false;
            ProcessEad = false;
            ProcessEquivAD = false;

            ProcessStrucGroupLook = false;
            ProcessWspLook = false;
            ProcessProbLook = false;
            ProcessRatLook = false;
            ProcessLevLook = false;
            ProcessAggDamgLook = false;
            ProcessEadLook = false;
        }
        #endregion



    }
}
