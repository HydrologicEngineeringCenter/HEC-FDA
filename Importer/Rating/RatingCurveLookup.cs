using System;
using System.Collections.Generic;
using System.Text;

namespace Importer
{
    public class RatingCurveLookup : DbfFileFdLookupManager
    {
        void setNextObjectID(long id)
        {
            GlobalVariables.mp_fdaStudy.GetNextIdMgr().setNextObjIdRatData(id);
        }
        long getNextObjectID()
        {
            return GlobalVariables.mp_fdaStudy.GetNextIdMgr().getCurrentObjIdRatData();
        }

    }
}
