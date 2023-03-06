using System;
using System.Collections.Generic;
using System.Text;

namespace Importer
{
    [Serializable]
    public class FdObjectDataLook : FdObjectData
    {
        //Id, Name, & Description from FdObjectData
        public long IdPlan { get; set; }
        public long IdYear { get; set; }
        public long IdStream { get; set; }
        public long IdReach { get; set; }
        public long IdCategory { get; set; }
        public long NumRefs { get; set; }
        public string PlanName
        { get; set; }
        public string YearName
        { get; set; }
        public string StreamName
        { get; set; }
        public string DamageReachName
        { get; set; }
        public string CategoryName
        { get; set; }

        public FdObjectDataLook() : base()
        {
            initializeReferenceCount();
            resetIds();

        }
        public FdObjectDataLook(FdObjectDataLook theObject) : base((FdObjectData)theObject)
        {
            IdPlan = theObject.IdPlan;
            IdYear = theObject.IdYear;
            IdStream = theObject.IdStream;
            IdReach = theObject.IdReach;
            IdCategory = theObject.IdCategory;
            NumRefs = theObject.NumRefs;
        }
        public FdObjectDataLook(string name, string desc)
        {
            Name = name;
            Description = desc;
            initializeReferenceCount();
            resetIds();
        }
        public void resetIds()
        {
            IdPlan = -1L;
            IdYear = -1L;
            IdStream = -1L;
            IdReach = -1L;
            IdCategory = -1L;
        }
        public void reset()
        {
            base.Reset();
            resetIds();
            NumRefs = 0;
        }

        public void SetIds(long idPlan = -1L,
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

        public void SetNames(string planName,
                      string yearName = "",
                      string streamName = "",
                      string reachName = "",
                      string categoryName = "")
        {
            //TODO; Need supporting code to convert names to ID's
        }
        public long initializeReferenceCount(long initialValue = 0L)
        {
            NumRefs = initialValue;
            return NumRefs;
        }
        public long incrementReferenceCount()
        {
            NumRefs++;
            return NumRefs;
        }
        public long decrementReferenceCount()
        {
            NumRefs--;
            return NumRefs;
        }

    } 
}
