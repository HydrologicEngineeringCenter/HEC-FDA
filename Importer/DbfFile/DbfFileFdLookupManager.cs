using System;
using System.Collections.Generic;
using System.Text;

namespace Importer
{
    [Serializable]
    public class DbfFileFdLookupManager : DbfFileManager
    {
        public long _IdPlan { get; set; }
        public long _IdYear { get; set; }
        public long _IdStream { get; set; }
        public long _IdReach { get; set; }
        public long _IdCategory { get; set; }
        public long _IdDataFunc { get; set; }
        public DbfFileFdLookupManager() : base()
        {
            reset();
        }
        public void reset()
        {
            _IdPlan = -1L;
            _IdYear = -1L;
            _IdStream = -1L;
            _IdReach = -1L;
            _IdCategory = -1L;
            _IdDataFunc = -1L;
        }

    }
}
