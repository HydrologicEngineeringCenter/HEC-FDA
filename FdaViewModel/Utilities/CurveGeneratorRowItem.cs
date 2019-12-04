using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
{
    public class CurveGeneratorRowItem
    {
        public event EventHandler ChangedDistributionType;

        private string _selectedDistType = "None";


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
                return new List<string>() {"None", "Linear", "Piecewise" };
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
        public double X { get; set; }
        public double Y { get; set; }

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

        public CurveGeneratorRowItem(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
