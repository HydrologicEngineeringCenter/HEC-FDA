using Functions;
using Functions.CoordinatesFunctions;
using Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsView.ViewModel
{
    public class CoordinatesFunctionRowItem
    {
        //If any property changes then throw the row modified event. The table that the row is in will
        //be listening to it. It will create a new ICoordinatesFunction and pass it on up to the parent.
        //public event EventHandler DataChanged;
        public event EventHandler RowIsLeavingTable;
        //public event EventHandler ChangedInterpolationType;
        private readonly List<DistributionType> SupportedDistributionTypes = new List<DistributionType>() { DistributionType.Constant, DistributionType.Normal, 
            DistributionType.Triangular, DistributionType.Uniform, DistributionType.TruncatedNormal, DistributionType.Beta4Parameters };
        
        private DistributionType _selectedDistType = DistributionType.Constant;
        private InterpolationEnum _selectedInterpolationType = InterpolationEnum.Linear;
        private double _X;
        private double _Y;
        private double _StandardDeviation;
        private double _Min;
        private double _Max;
        private double _MostLikely;
        private double _Mean;


        #region Properties
        public double X
        {
            get { return _X; }
            set { _X = value;  }
        }
        public double Y
        {
            get { return _Y; }
            set { _Y = value;  }
        }
        public double StandardDeviation
        {
            get { return _StandardDeviation; }
            set { _StandardDeviation = value; }
        }
        public double Min
        {
            get { return _Min; }
            set { _Min = value;  }
        }
        public double Max
        {
            get { return _Max; }
            set { _Max = value;  }
        }
        public double MostLikely
        {
            get { return _MostLikely; }
            set { _MostLikely = value;  }
        }
        public double Mean
        {
            get { return _Mean; }
            set { _Mean = value;  }
        }

        //public DistributionType DistributionType
        //{
        //    get { return _selectedDistType; }
        //    set { _selectedDistType = value; StructureChanged?.Invoke(this, new EventArgs()); }
        //}
        //public InterpolationEnum InterpolationType
        //{
        //    get { return _selectedInterpolationType; }
        //    set { _selectedInterpolationType = value; StructureChanged?.Invoke(this, new EventArgs()); }
        //}

        public CoordinatesFunctionRowItem Row
        {
            get { return this; }
        }
        //public Statistics.IDistribution Distribution
        //{
        //    get;
        //    set;
        //}  

        public IEnumerable<DistributionType> DistributionTypes
        {
            get
            {

                //return Enum.GetNames(typeof(DistributionType)).ToList();
                return SupportedDistributionTypes; //Enum.GetValues(typeof(DistributionType)).Cast<DistributionType>();
            }

        }

        public IEnumerable<InterpolationEnum> InterpolationTypes
        {
            get
            {
                //return new List<string>() { "None", "Linear", "Piecewise" };
                return Enum.GetValues(typeof(InterpolationEnum)).Cast<InterpolationEnum>();

            }
        }
        public InterpolationEnum SelectedInterpolationType
        {
            get { return _selectedInterpolationType; }
            set
            {
                if (!value.Equals(_selectedInterpolationType))
                {
                    _selectedInterpolationType = value;
                    RowIsLeavingTable?.Invoke(this, new EventArgs());
                }
            }
        }

        public DistributionType SelectedDistributionType
        {
            get { return _selectedDistType; }
            set
            {
                if (!value.Equals(_selectedDistType))
                {
                    _selectedDistType = value;
                    RowIsLeavingTable?.Invoke(this, new EventArgs());
                }
            }
        }
        

        //public List<Functions.DistributionType> DistributionTypes
        //{
        //    get
        //    {
        //        List<Functions.DistributionType> types = new List<Functions.DistributionType>();
        //        types.Add(Functions.DistributionType.Normal);
        //        types.Add(Functions.DistributionType.Triangular);
        //        types.Add(Functions.DistributionType.Uniform);
        //        return types;
        //    }
        //}
        #endregion
        //public CoordinatesFunctionRowItem(double x, double y, string distType, string interpType)
        //{
        //    X = x;
        //    Y = y;
        //    SelectedDistributionType = distType;
        //    SelectedInterpolationType = interpType;
        //}
        /// <summary>
        /// An empty constructor is required by the CopyPasteDataGrid to add empty rows. 
        /// </summary>
        public CoordinatesFunctionRowItem()
        {
            X = 0;
            Y = 0;
            SelectedDistributionType = DistributionType.Constant;
            SelectedInterpolationType = InterpolationEnum.Linear;
        }
        /// <summary>
        /// You should never have to call this directly. Use the RowItemBuilder to build this object.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="standDev"></param>
        /// <param name="mean"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="mostLikely"></param>
        /// <param name="distType"></param>
        /// <param name="interpType"></param>
        public CoordinatesFunctionRowItem(double x, double y, double standDev,double mean, double min, double max, double mostLikely, 
            DistributionType distType,InterpolationEnum interpType)
        {
            X = x;
            Y = y;
            StandardDeviation = standDev;
            Mean = mean;
            Min = min;
            Max = max;
            MostLikely = mostLikely;
            SelectedDistributionType = distType;
            SelectedInterpolationType = interpType;

            
        }

        public CoordinatesFunctionRowItem Clone()
        {
            return new CoordinatesFunctionRowItem(X, Y, StandardDeviation, Mean, Min, Max, MostLikely, SelectedDistributionType, SelectedInterpolationType);
        }
        //public CoordinatesFunctionRowItem(double x, IDistribution dist)
        //{
        //    X = x;
        //    Distribution = dist;
        //}
    }
}
