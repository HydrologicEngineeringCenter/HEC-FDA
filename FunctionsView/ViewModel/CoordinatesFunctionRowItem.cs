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
        private readonly List<IOrdinateEnum> SupportedDistributionTypes = new List<IOrdinateEnum>() { IOrdinateEnum.Constant, IOrdinateEnum.Normal, 
            IOrdinateEnum.Triangular, IOrdinateEnum.Uniform, IOrdinateEnum.TruncatedNormal, IOrdinateEnum.Beta4Parameters };

        private IOrdinateEnum _selectedDistType = IOrdinateEnum.NotSupported;// = DistributionType.Constant;
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

        public IEnumerable<IOrdinateEnum> DistributionTypes
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

        public IOrdinateEnum SelectedDistributionType
        {
            get { return _selectedDistType; }
            set
            {
                
                if (!value.Equals(_selectedDistType))
                {
                    IOrdinateEnum originalType = _selectedDistType;
                    IOrdinateEnum newType = value;
                    //i don't want to clear the values if this is the first time values are getting set
                    //The first time the row will have a type of NotSupported
                    if (originalType != IOrdinateEnum.NotSupported)
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
        private void TranslateValuesBetweenDistributionTypes( IOrdinateEnum originalType, IOrdinateEnum newType)
        {
            switch(originalType)
            {
                case IOrdinateEnum.Constant:
                    {
                        TranslateValuesFromConstantTo(newType);
                        break;
                    }
                case IOrdinateEnum.Normal:
                    {
                        TranslateValuesFromNormalTo(newType);
                        break;
                    }
                case IOrdinateEnum.Uniform:
                    {
                        TranslateValuesFromUniformTo(newType);
                        break;
                    }
                case IOrdinateEnum.Triangular:
                    {
                        TranslateValuesFromTriangularTo(newType);
                        break;
                    }
                case IOrdinateEnum.TruncatedNormal:
                    {
                        TranslateValuesFromTruncNormalTo(newType);
                        break;
                    }
                case IOrdinateEnum.Beta4Parameters:
                    {
                        TranslateValuesFromBetaTo(newType);
                        break;
                    }
            }
        }

        private void TranslateValuesFromBetaTo(IOrdinateEnum newDist)
        {
            double tempMin = Min;
            double tempMax = Max;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case IOrdinateEnum.TruncatedNormal:
                case IOrdinateEnum.Triangular:
                case IOrdinateEnum.Uniform:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
            }
        }

        private void TranslateValuesFromUniformTo(IOrdinateEnum newDist)
        {
            double tempMin = Min;
            double tempMax = Max;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case IOrdinateEnum.TruncatedNormal:
                case IOrdinateEnum.Triangular:
                case IOrdinateEnum.Beta4Parameters:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
            }
        }

        private void TranslateValuesFromTriangularTo(IOrdinateEnum newDist)
        {
            double tempMostLikely = MostLikely;
            double tempMin = Min;
            double tempMax = Max;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case IOrdinateEnum.Uniform:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
                case IOrdinateEnum.Constant:
                    {
                        Y = tempMostLikely;
                        break;
                    }
                case IOrdinateEnum.Normal:
                    {
                        Mean = tempMostLikely;
                        break;
                    }
                case IOrdinateEnum.TruncatedNormal:
                    {
                        Mean = tempMostLikely;
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
                case IOrdinateEnum.Triangular:
                    {
                        MostLikely = tempMostLikely;
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
                case IOrdinateEnum.Beta4Parameters:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
            }
        }

        private void TranslateValuesFromTruncNormalTo(IOrdinateEnum newDist)
        {
            double tempMean = Mean;
            double tempStDev = StandardDeviation;
            double tempMin = Min;
            double tempMax = Max;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case IOrdinateEnum.Uniform:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
                case IOrdinateEnum.Constant:
                    {
                        Y = tempMean;
                        break;
                    }
                case IOrdinateEnum.Normal:
                    {
                        Mean = tempMean;
                        StandardDeviation = tempStDev;
                        break;
                    }
                case IOrdinateEnum.Triangular:
                    {
                        MostLikely = tempMean;
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
                case IOrdinateEnum.Beta4Parameters:
                    {
                        Min = tempMin;
                        Max = tempMax;
                        break;
                    }
            }
        }

        private void TranslateValuesFromConstantTo(IOrdinateEnum newDist)
        {
            double tempY = Y;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case IOrdinateEnum.Normal:
                case IOrdinateEnum.TruncatedNormal:
                    {
                        Mean = tempY;
                        break;
                    }
                case IOrdinateEnum.Triangular:
                    {
                        MostLikely = tempY;
                        break;
                    }
            }
        }

        private void TranslateValuesFromNormalTo(IOrdinateEnum newDist)
        {
            double tempMean = Mean;
            double tempStDev = StandardDeviation;
            ClearAllPropertiesExceptX();
            switch (newDist)
            {
                case IOrdinateEnum.Constant:
                    {
                        Y = tempMean;
                        break;
                    }
                case IOrdinateEnum.TruncatedNormal:
                    {
                        Mean = tempMean;
                        StandardDeviation = tempStDev;
                        break;
                    }
                case IOrdinateEnum.Triangular:
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
            SelectedDistributionType = IOrdinateEnum.Constant;
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
            double alpha, double beta, IOrdinateEnum distType,InterpolationEnum interpType)
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

            if(SelectedDistributionType == IOrdinateEnum.Constant)
            {
                return ICoordinateFactory.Factory(x, Y);
            }
            else
            {
                return ICoordinateFactory.Factory(x, CreateDistributedValueFromRow());
            }
        }

        private IDistributedOrdinate CreateDistributedValueFromRow()
        {
            IDistributedOrdinate retval = null;
            switch (SelectedDistributionType)
            {
                case IOrdinateEnum.Normal:
                    {
                        retval = IDistributedOrdinateFactory.FactoryNormal(Mean, StandardDeviation);
                        break;
                    }
                case IOrdinateEnum.TruncatedNormal:
                    {
                        retval = IDistributedOrdinateFactory.FactoryTruncatedNormal(Mean, StandardDeviation, Min, Max);
                        break;
                    }
                case IOrdinateEnum.Uniform:
                    {
                        retval = IDistributedOrdinateFactory.FactoryUniform(Min, Max);
                        break;
                    }
                case IOrdinateEnum.Beta4Parameters:
                    {
                        retval = IDistributedOrdinateFactory.FactoryBeta(Alpha, Beta, Min, Max);
                        break;
                    }
                case IOrdinateEnum.Triangular:
                    {
                        retval = IDistributedOrdinateFactory.FactoryTriangular(MostLikely, Min, Max);
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
