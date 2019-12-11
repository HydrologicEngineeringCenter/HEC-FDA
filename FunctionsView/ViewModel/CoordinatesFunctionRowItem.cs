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
        public event EventHandler ChangedDistributionType;
        public event EventHandler ChangedInterpolationType;

        private string _selectedDistType = "None";
        private string _selectedInterpolationType = "Linear";

        #region Properties
        public CoordinatesFunctionRowItem Row
        {
            get { return this; }
        }
        public Statistics.IDistribution Distribution
        {
            get;
            set;
        }

        public double Mean
        {
            get { return 99; }
            set { Mean = value; }
        }

        public List<string> DistributionTypes
        {
            get
            {
                List<string> types = new List<string>();
                types.Add("Normal");
                types.Add("Triangular");
                types.Add("None");

                return types;
            }

        }

        public List<String> InterpolationTypes
        {
            get
            {
                return new List<string>() { "None", "Linear", "Piecewise" };
            }
        }
        public string SelectedInterpolationType
        {
            get { return _selectedInterpolationType; }
            set
            {
                if (!value.Equals(_selectedInterpolationType))
                {
                    _selectedInterpolationType = value;
                    ChangedInterpolationType?.Invoke(this, new EventArgs());
                }
            }
        }

        public string SelectedDistributionType
        {
            get { return _selectedDistType; }
            set
            {
                if (!value.Equals(_selectedDistType))
                {
                    _selectedDistType = value;
                    ChangedDistributionType?.Invoke(this, new EventArgs());
                }
            }
        }
        public double X
        {
            get;
            set;
        }
        public double Y
        {
            get;
            set;
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
        public CoordinatesFunctionRowItem(double x, double y, string distType, string interpType)
        {
            X = x;
            Y = y;
            SelectedDistributionType = distType;
            SelectedInterpolationType = interpType;
        }
        public CoordinatesFunctionRowItem()
        {
            X = 0;
            Y = 0;
            SelectedDistributionType = "None";
            SelectedInterpolationType = "Linear";
        }
        public CoordinatesFunctionRowItem(double x, double y)
        {
            X = x;
            Y = y;
            
        }
        public CoordinatesFunctionRowItem(double x, IDistribution dist)
        {
            X = x;
            Distribution = dist;
        }
    }
}
