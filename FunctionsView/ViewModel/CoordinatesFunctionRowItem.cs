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
        public event EventHandler RowIsLeavingTable;
        private readonly List<DistributionType> SupportedDistributionTypes = new List<DistributionType>() { DistributionType.Constant, DistributionType.Normal, 
            DistributionType.Triangular, DistributionType.Uniform, DistributionType.TruncatedNormal, DistributionType.Beta4Parameters };

        private DistributionType _selectedDistType = DistributionType.NotSupported;// = DistributionType.Constant;
        private InterpolationEnum _selectedInterpolationType;// = InterpolationEnum.Linear;

        #region Properties
        public double Alpha { get; set; }
        public double Beta { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double StandardDeviation { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public double MostLikely { get; set; }
        public double Mean { get; set; }

        /// <summary>
        /// I know that this is weird to have this property, but i needed a way for the 
        /// Distribution control and the InterpolationControl to know what row it belongs to.
        /// So the table xaml binds this row property to the controls so that they know.
        /// </summary>
        public CoordinatesFunctionRowItem Row
        {
            get { return this; }
        }

        public IEnumerable<DistributionType> DistributionTypes
        {
            get
            {
                return SupportedDistributionTypes; 
            }
        }

        public IEnumerable<InterpolationEnum> InterpolationTypes
        {
            get
            {
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
                    DistributionType originalType = _selectedDistType;
                    DistributionType newType = value;
                    //i don't want to clear the values if this is the first time values are getting set
                    //The first time the row will have a type of NotSupported
                    if (originalType != DistributionType.NotSupported)
                    {
                        TranslateValuesBetweenDistributionTypes(originalType, newType);
                    }
                    _selectedDistType = value;
                    RowIsLeavingTable?.Invoke(this, new EventArgs());
                }
            }
        }
        
        /// <summary>
        /// When switching distribution types there are some values that we might want to use for the new type. For example
        /// if a type has min and max and you are switching to another type that has min and max then we want to keep those values.
        /// All not needed properties will get cleared to 0.
        /// </summary>
        /// <param name="originalType"></param>
        /// <param name="newType"></param>
        private void TranslateValuesBetweenDistributionTypes( DistributionType originalType, DistributionType newType)
        {
            switch(originalType)
            {
                case DistributionType.Constant:
                    {
                        TranslateValuesFromConstantTo(newType);
                        break;
                    }
                case DistributionType.Normal:
                    {
                        TranslateValuesFromNormalTo(newType);
                        break;
                    }
                case DistributionType.Uniform:
                    {
                        TranslateValuesFromUniformTo(newType);
                        break;
                    }
                case DistributionType.Triangular:
                    {
                        TranslateValuesFromTriangularTo(newType);
                        break;
                    }
                case DistributionType.TruncatedNormal:
                    {
                        TranslateValuesFromTruncNormalTo(newType);
                        break;
                    }
                case DistributionType.Beta4Parameters:
                    {
                        TranslateValuesFromBetaTo(newType);
                        break;
                    }
            }
        }

        private void TranslateValuesFromBetaTo(DistributionType newDist)
        {
            double tempMin = Min;
            double tempMax = Max;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case DistributionType.TruncatedNormal:
                case DistributionType.Triangular:
                case DistributionType.Uniform:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
            }
        }

        private void TranslateValuesFromUniformTo(DistributionType newDist)
        {
            double tempMin = Min;
            double tempMax = Max;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case DistributionType.TruncatedNormal:
                case DistributionType.Triangular:
                case DistributionType.Beta4Parameters:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
            }
        }

        private void TranslateValuesFromTriangularTo(DistributionType newDist)
        {
            double tempMostLikely = MostLikely;
            double tempMin = Min;
            double tempMax = Max;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case DistributionType.Uniform:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
                case DistributionType.Constant:
                    {
                        Y = tempMostLikely;
                        break;
                    }
                case DistributionType.Normal:
                    {
                        Mean = tempMostLikely;
                        break;
                    }
                case DistributionType.TruncatedNormal:
                    {
                        Mean = tempMostLikely;
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
                case DistributionType.Triangular:
                    {
                        MostLikely = tempMostLikely;
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
                case DistributionType.Beta4Parameters:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
            }
        }

        private void TranslateValuesFromTruncNormalTo(DistributionType newDist)
        {
            double tempMean = Mean;
            double tempStDev = StandardDeviation;
            double tempMin = Min;
            double tempMax = Max;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case DistributionType.Uniform:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
                case DistributionType.Constant:
                    {
                        Y = tempMean;
                        break;
                    }
                case DistributionType.Normal:
                    {
                        Mean = tempMean;
                        StandardDeviation = tempStDev;
                        break;
                    }
                case DistributionType.Triangular:
                    {
                        MostLikely = tempMean;
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
                case DistributionType.Beta4Parameters:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
            }
        }

        private void TranslateValuesFromConstantTo(DistributionType newDist)
        {
            double tempY = Y;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case DistributionType.Normal:
                case DistributionType.TruncatedNormal:
                    {
                        Mean = tempY;
                        break;
                    }
                case DistributionType.Triangular:
                    {
                        MostLikely = tempY;
                        break;
                    }
            }
        }

        private void TranslateValuesFromNormalTo(DistributionType newDist)
        {
            double tempMean = Mean;
            double tempStDev = StandardDeviation;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case DistributionType.Constant:
                    {
                        Y = tempMean;
                        break;
                    }
                case DistributionType.TruncatedNormal:
                    {
                        Mean = tempMean;
                        StandardDeviation = tempStDev;
                        break;
                    }
                case DistributionType.Triangular:
                    {
                        MostLikely = tempMean;
                        break;
                    }
            }
        }

        private void ClearAllPropertiesExceptX()
        {
            Y = Mean = StandardDeviation = Min = Max = MostLikely = Alpha = Beta = 0;
        }
        
        #endregion
        
        /// <summary>
        /// An empty constructor.
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
            double alpha, double beta, DistributionType distType,InterpolationEnum interpType)
        {
            X = x;
            Y = y;
            StandardDeviation = standDev;
            Mean = mean;
            Min = min;
            Max = max;
            MostLikely = mostLikely;
            Alpha = alpha;
            Beta = beta;
            SelectedDistributionType = distType;
            SelectedInterpolationType = interpType;
            
        }

        public CoordinatesFunctionRowItem Clone()
        {
            return new CoordinatesFunctionRowItem(X, Y, StandardDeviation, Mean, Min, Max, MostLikely,Alpha,Beta, SelectedDistributionType, SelectedInterpolationType);
        }
       
        public ICoordinate CreateCoordinateFromRow()
        {
            double x = X;

            if(SelectedDistributionType == DistributionType.Constant)
            {
                return ICoordinateFactory.Factory(x, Y);
            }
            else
            {
                return ICoordinateFactory.Factory(x, CreateDistributedValueFromRow());
            }
        }

        private IDistributedValue CreateDistributedValueFromRow()
        {
            IDistributedValue retval = null;
            switch (SelectedDistributionType)
            {
                case DistributionType.Normal:
                    {
                        retval = DistributedValueFactory.FactoryNormal(Mean, StandardDeviation);
                        break;
                    }
                case DistributionType.TruncatedNormal:
                    {
                        retval = DistributedValueFactory.FactoryTruncatedNormal(Mean, StandardDeviation, Min, Max);
                        break;
                    }
                case DistributionType.Uniform:
                    {
                        retval = DistributedValueFactory.FactoryUniform(Min, Max);
                        break;
                    }
                case DistributionType.Beta4Parameters:
                    {
                        retval = DistributedValueFactory.FactoryBeta(Alpha, Beta, Min, Max);
                        break;
                    }
                case DistributionType.Triangular:
                    {
                        retval = DistributedValueFactory.FactoryTriangular(MostLikely, Min, Max);
                        break;
                    }
            }
            if(retval == null)
            {
                throw new Exception("Unable to create a distributed value from row. The selected distribution type may be unsupported.");
            }
            else
            {
                return retval;
            }
        }
    }
}
