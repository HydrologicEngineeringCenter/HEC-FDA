using Functions;
using Functions.Ordinates;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunctionsView.ViewModel
{
    public class CoordinatesFunctionEditorVM : INotifyPropertyChanged
    {
        //public delegate void NewDistributionSelected(CurveGeneratorEventArgs e);
        //public static event NewDistributionSelected NewDistributionEventTriggered;
        public event PropertyChangedEventHandler PropertyChanged;

        private List<CoordinatesFunctionRowItem> _Rows = new List<CoordinatesFunctionRowItem>();
        private string _selectedDistType;
        private string _type;

        public ICoordinatesFunction Function { get; }

        public List<CoordinatesFunctionRowItem> RowItems
        {
            get { return _Rows; }
            set
            {
                _Rows = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RowItems"));
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

                    // NewDistributionEventTriggered?.Invoke( new CurveGeneratorEventArgs());
                }

            }
        }

        public CoordinatesFunctionEditorVM(ICoordinatesFunction function, string type)
        {
            _type = type;
            Function = function;
            CreateRowItems();
        }

        //todo actually, i think this will need to return an icoordinates function so that it can be combined
        //with other funcs to make a linked function. The convertion to IFdaFunction is easy and could be done
        //by the parent control.
        public ICoordinatesFunction GenerateFunction()
        {
            //can we assume that all rows will have the same distribution? i think so.
            //turn each row into a coordinate
            List<ICoordinate> coords = new List<ICoordinate>();
            foreach (CoordinatesFunctionRowItem ri in RowItems)
            {
                coords.Add(CreateCoordinateFromRowItem(ri));
            }
            ICoordinatesFunction func = ICoordinatesFunctionsFactory.Factory(coords, Functions.CoordinatesFunctions.InterpolationEnum.Linear);
            return func;
        }

        private void CreateRowItems()
        {
            List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
            foreach (ICoordinate coord in Function.Coordinates)
            {
                rows.Add(CreateRowItemFromCoordinate(coord));

            }
            RowItems = rows;
        }

        private ICoordinate CreateCoordinateFromRowItem(CoordinatesFunctionRowItem rowItem)
        {
            double x = rowItem.X;
            switch (rowItem.SelectedDistributionType)
            {
                case "None":
                    {
                        return ICoordinateFactory.Factory(x, rowItem.Y);
                    }
                case "Normal":
                    {
                        double mean = rowItem.Mean;
                        double stDev = rowItem.Distribution.StandardDeviation;
                        int sampleSize = rowItem.Distribution.SampleSize;
                        IDistributedValue dist = DistributedValueFactory.FactoryNormal(mean, stDev, sampleSize);
                        return ICoordinateFactory.Factory(x, dist);

                    }
            }
            //todo delete me
            return null;
        }
        /// <summary>
        /// An individual row will send an event when it changes distribution types.
        /// This method will send an event up to the parent that this table has a row that
        /// changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void RowChangedDistributionType(object sender, EventArgs e)
        //{
        //    CoordinatesFunctionRowItem rowThatChanged = (CoordinatesFunctionRowItem)sender;
        //    List<CoordinatesFunctionRowItem> rowsBelow = GetRowsBelowRow(rowThatChanged);
        //    List<CoordinatesFunctionRowItem> rowsAbove = GetRowsAboveRow(rowThatChanged);

        //    NewDistributionEventTriggered?.Invoke(new CurveGeneratorEventArgs(rowThatChanged, rowsBelow, rowsAbove));
        //}

        private List<CoordinatesFunctionRowItem> GetRowsAboveRow(CoordinatesFunctionRowItem row)
        {
            List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
            int i;
            for (i = 0; i < RowItems.Count; i++)
            {
                if (RowItems[i] == row)
                {
                    break;
                }
                else
                {
                    rows.Add(RowItems[i]);
                }
            }
            return rows;
        }
        private List<CoordinatesFunctionRowItem> GetRowsBelowRow(CoordinatesFunctionRowItem row)
        {
            int i;
            for (i = 0; i < RowItems.Count; i++)
            {
                if (RowItems[i] == row)
                {
                    break;
                }
            }
            List<CoordinatesFunctionRowItem> rows = new List<CoordinatesFunctionRowItem>();
            i++;
            for (int j = i; j < RowItems.Count; j++)
            {
                rows.Add(RowItems[j]);
            }
            return rows;
        }

        private CoordinatesFunctionRowItem CreateRowItemFromCoordinate(ICoordinate coord)
        {
            //we know that x is always constant
            double x = coord.X.Value();
            //todo if we add a dist type for "None" then i can get rid of the "else" here and 
            //just include it in the switch.
            if (coord.Y.GetType().Equals(typeof(Distribution)))
            {
                IDistributedValue dist = ((Distribution)coord.Y).GetDistribution;
                DistributionType type = dist.Type;
                switch (type)
                {
                    case DistributionType.Normal:
                        {

                            CoordinatesFunctionRowItem row = new RowItemBuilder(x)
                                .WithNormalDist(dist.Skewness, dist.StandardDeviation)
                                .Build();
                            row.SelectedInterpolationType = "Linear";
                            //row.ChangedDistributionType += RowChangedDistributionType;
                            return row;
                        }
                    default:
                        {
                            throw new ArgumentException("The coordinate has a distributed y value that is not an allowed distribution type.");
                        }
                }

            }
            else
            {
                //then y is a constant
                double y = coord.Y.Value();
                CoordinatesFunctionRowItem row = new CoordinatesFunctionRowItem(x, y);
                //row.ChangedDistributionType += RowChangedDistributionType;
                return row;
            }
        }


        private class RowItemBuilder
        {
            private double _X;
            private double _Y;
            private DistributionType _distType;
            private double _skew;
            private double _standDev;

            internal RowItemBuilder(double x)
            {
                _X = x;
                _distType = DistributionType.NotSupported;
            }

            internal RowItemBuilder WithNormalDist(double skew, double standardDeviation)
            {
                _distType = DistributionType.Normal;
                _skew = skew;
                _standDev = standardDeviation;
                return this;
            }

            internal RowItemBuilder WithNoneDist(double y)
            {
                _Y = y;
                //todo john, i think we need to add a "None" option to the dist enums.
                return this;
            }

            internal CoordinatesFunctionRowItem Build()
            {
                return new CoordinatesFunctionRowItem(_X, _Y);
            }

        }

        //public class CurveGeneratorRow
        //{
        //    private string _selectedDistType = "None";

        //    public string SelectedDistributionType
        //    {
        //        get { return _selectedDistType; }
        //        set
        //        {
        //            _selectedDistType = value;
        //        }
        //    }
        //    public double X { get; set; }
        //    public double Y { get; set; }

        //    public List<Functions.DistributionType> DistributionTypes
        //    {
        //        get
        //        {
        //            List<Functions.DistributionType> types = new List<Functions.DistributionType>();
        //            types.Add(Functions.DistributionType.Normal);
        //            types.Add(Functions.DistributionType.Triangular);
        //            types.Add(Functions.DistributionType.Uniform);
        //            return types;
        //        }
        //    }

        //    public CurveGeneratorRow(double x, double y)
        //    {
        //        X = x;
        //        Y = y;
        //    }
        //}

    }
}
