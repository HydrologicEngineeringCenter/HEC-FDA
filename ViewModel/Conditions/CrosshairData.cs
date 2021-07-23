using Functions;
using HEC.Plotting.Core;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Conditions
{
    public class CrosshairData
    {
        public event EventHandler<ModulatorEventArgs> UpdateModulator;
        public event EventHandler<ModulatorEventArgs> UpdateHorizontalModulator;
        public event EventHandler<ModulatorEventArgs> UpdateHorizontalFailureFunction;

        public bool IsPlot1Modulator { get; set; }
        public bool IsPlot5Modulator { get; set; }
        public bool IsPlotFailureFunction { get; set; }

        //the next control's crosshairdata, 
        public SharedAxisCrosshairData Next { get; set; }
        public SharedAxisCrosshairData Previous { get; set; }

        public event Action PreviousUpdated;
        public event Action NextUpdated;

        public IComparable XValue
        {
            get { return _xModulator.Invoke(_x); }
        }

        public IComparable YValue
        {
            get { return _yModulator.Invoke(_y); }
        }

        public IFdaFunction Function { get;  }

        private IComparable _x;
        private IComparable _y;
        private readonly Func<IComparable, IComparable> _xModulator;
        private readonly Func<IComparable, IComparable> _yModulator;

        public CrosshairData(IFdaFunction function, Func<IComparable, IComparable> xModulator = null,
            Func<IComparable, IComparable> yModulator = null)
        {
            Function = function;
            if (xModulator == null)
            {
                xModulator = (i) => i;
            }

            if (yModulator == null)
            {
                yModulator = (i) => i;
            }

            _xModulator = xModulator;
            _yModulator = yModulator;
        }

        public IComparable GetPreviousValue()
        {
            return GetValueFromSharedChart(Previous);
        }

        public IComparable GetNextValue()
        {
            return GetValueFromSharedChart(Next);
        }

        private IComparable GetValueFromSharedChart(SharedAxisCrosshairData other)
        {
            IComparable output = null;
            if (other != null)
            {
                switch (other.OtherAxis)
                {
                    case Axis.X:
                        output = other.OtherCrosshairData.XValue;
                        break;
                    case Axis.Y:
                        output = other.OtherCrosshairData.YValue;
                        break;
                }
            }

            return output;
        }

        public void UpdateValuesInSharedPlots(IComparable x, IComparable y)
        {
            _x = x;
            _y = y;
            Next?.OtherCrosshairData.UpdateFromPrevious(x, y);
            Previous?.OtherCrosshairData.UpdateFromNext(x, y);
        }

        public void UpdateNextValues(IComparable x, IComparable y)
        {
            _x = x;
            _y = y;

            if(IsPlot1Modulator)
            {
                //tell the ind linked plot control to update the modulator lines
                UpdateModulator?.Invoke(this, new ModulatorEventArgs((double)_x, (double)_y));
            }
            if(IsPlot5Modulator)
            {
                UpdateHorizontalModulator?.Invoke(this, new ModulatorEventArgs((double)_x, (double)_y));
            }
            if(IsPlotFailureFunction)
            {
                UpdateHorizontalFailureFunction?.Invoke(this, new ModulatorEventArgs((double)_x, (double)_y));
            }
            Next?.OtherCrosshairData.UpdateFromPrevious(x, y);
        }

        public void UpdatePreviousValues(IComparable x, IComparable y)
        {
            _x = x;
            _y = y;

            if (IsPlot1Modulator)
            {
                //tell the ind linked plot control to update the modulator lines
                UpdateModulator?.Invoke(this, new ModulatorEventArgs((double)_x, (double)_y));
            }
            if (IsPlot5Modulator)
            {
                UpdateHorizontalModulator?.Invoke(this, new ModulatorEventArgs((double)_x, (double)_y));
            }
            if (IsPlotFailureFunction)
            {
                UpdateHorizontalFailureFunction?.Invoke(this, new ModulatorEventArgs((double)_x, (double)_y));
            }

            Previous?.OtherCrosshairData.UpdateFromNext(x, y);
        }

        private void UpdateFromPrevious(IComparable prevX, IComparable prevY)
        {
            if ((double)prevX == double.NaN || (double)prevY == double.NaN)
            {
                return;
            }
            else
            {
                Update(prevX, prevY, Previous, PreviousUpdated);
            }
        }

        /// <summary>
        /// This gets called when a previous or next chart mouse moves.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="data"></param>
        /// <param name="action"></param>
        private void Update(IComparable x, IComparable y, SharedAxisCrosshairData data, Action action)
        {
            //if the axes are the same...
            if (data.OtherAxis == data.CurrentAxis)
            {
                switch (data.OtherAxis)
                {
                    case Axis.X:
                        _x = x;
                        break;
                    case Axis.Y:
                        _y = y;
                        break;
                }
            }
            else
            {
                //the two axes are different, so mix and match.
                switch (data.OtherAxis)
                {
                    case Axis.X:
                        _y = x;
                        break;
                    case Axis.Y:
                        _x = y;
                        break;
                }
            }

            action?.Invoke();
        }

        private void UpdateFromNext(IComparable nextX, IComparable nextY)
        {
            if ((double)nextX != double.NaN && (double)nextY != double.NaN)
            {
                Update(nextX, nextY, Next, NextUpdated);
            }
        }
    }

    public class ModulatorEventArgs : EventArgs
    {
        public double X { get; set; }
        public double Y { get; set; }
        public ModulatorEventArgs(double x, double y)
        {
            X = x;
            Y = y;
        }

    }

}
