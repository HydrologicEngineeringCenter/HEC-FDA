using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Importer
{
    [Serializable]
    public class FdObjectData
    {
        //enum FdaSizes { NAME_SIZE = 33, DESC_SIZE = 65};
        #region Properties
        public long Id
        { get; set; }
        public string Name
        { get; set; }
        public string NameShort
        { get; set; }
        public string Description
        { get; set; }
        public long SortOrder
        { get; set; }
        public int SortClass
        { get; set; }
        public bool New
        { get; set; }
        public string CalculationDate
        { get; set; }
        public bool IsValid
        { get; set; }
        public bool IsOutOfDate
        { get; set; }
        public string MetaData
        { get; set; }
        #endregion
        #region Constructors
        public FdObjectData()
        {
            Reset();
        }
        public FdObjectData(FdObjectData theObject)
        {
            Id = theObject.Id;
            Name = theObject.Name;
            NameShort = theObject.NameShort;
            Description = theObject.Description;
            CalculationDate = theObject.CalculationDate;
            MetaData = theObject.MetaData;
        }
        #endregion
        #region Voids
        public void Reset()
        {
            Id = -1;
            Name = "";
            NameShort = "";
            Description = "";
            SortOrder = -1;
            SortClass = 0;
            New = true;
            CalculationDate = "";
            IsValid = true;
            IsOutOfDate = false;
            MetaData = "";
        }
        #endregion
        #region Functions
        public bool IsEqualTo(FdObjectData other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id == other.Id &&
                   Name == other.Name &&
                   NameShort == other.NameShort &&
                   Description == other.Description &&
                   SortOrder == other.SortOrder &&
                   SortClass == other.SortClass &&
                   New == other.New &&
                   CalculationDate == other.CalculationDate &&
                   IsValid == other.IsValid &&
                   IsOutOfDate == other.IsOutOfDate &&
                   MetaData == other.MetaData;
        }
        #endregion
    }
}
