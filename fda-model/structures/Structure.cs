using RasMapperLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace structures
{
    public class Structure
    {
        private int _fdid;
        private PointM _point;
        private double _foundationHeightMean;
        private double _StructureValue;
        private double _ContentValue;
        private double _OtherValue;
        private double _StructureValueFromInput;
        private double _ContentValueFromInput;
        private double _OtherValueFromInput;
        private string _occtype_name;
        private string _damcat_name;
        private int _pop2amu65;
        private int _pop2amo65;
        private int _pop2pmu65;
        private int _pop2pmo65;
        private int _impactAreaID;
        private string _cbdID;
        private int _yearInService;
        private List<double> _computeStages;

        public PointM XYPoint
        {
            get { return _point; }
        }
        public string ImpactAreaID { get; set; }

        //This parameter list lines up with columnsOfInterest in the Inventory 
        public Structure(int name, PointM point, double foundationHeightMean, double structureValue, double contentValue, double vehicleValue, string damCat, string occtype, int pop2amu65, int pop2amo65, int pop2pmu65, int pop2pm065, int impactAreaID, string censusBlockID)
        {
            _fdid = name;
            _point = point;
            _foundationHeightMean = foundationHeightMean;
            _StructureValueFromInput = structureValue;
            _ContentValueFromInput = contentValue;
            _OtherValueFromInput = vehicleValue;
            _occtype_name = occtype;
            _damcat_name = damCat;
            _pop2amu65 = pop2amu65;
            _pop2amo65 = pop2amo65;
            _pop2pmu65 = pop2pmu65;
            _pop2pmo65 = pop2pm065;
            _impactAreaID = impactAreaID;
            _cbdID = censusBlockID;
        }


        public string DamCatName { get { return _damcat_name; } }
        public string OccTypeName { get { return _occtype_name; } }
        public DeterministicStructure Sample(int seed, DeterministicOccupancyType occtype)
        {
            Random random = new Random(seed);
            double foundHeightSample = _foundationHeightMean + (_foundationHeightMean * occtype.FoundationHeightError);
            double structValueSample = _StructureValue;
            //load up the deterministic structure
            return new DeterministicStructure(_fdid, structValueSample, foundHeightSample);

        }



    }
}