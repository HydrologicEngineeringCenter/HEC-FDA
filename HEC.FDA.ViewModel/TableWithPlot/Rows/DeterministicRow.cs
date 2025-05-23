﻿using Statistics.Distributions;
using System.Collections.Generic;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.MVVMFramework.Base.Enumerations;
using HEC.MVVMFramework.Base.Interfaces;
using HEC.MVVMFramework.Base.Implementations;

namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    public class DeterministicRow : SequentialRow
    {
        private double _x;
        [DisplayAsColumn("X Value")]
        public override double X
        {
            get { return _x; }
            set
            {
                _x = value;
                NotifyPropertyChanged();
                ((DeterministicRow)PreviousRow)?.NotifyPropertyChanged(nameof(X));
                ((DeterministicRow)NextRow)?.NotifyPropertyChanged(nameof(X));
            }

        }

        [DisplayAsColumn("Y Value")]
        [DisplayAsLine("Y Value", Enumerables.ColorEnum.Red)]
        public double Value
        {
            get
            {
                return ((Deterministic)Y).Value;
            }
            set
            {
                Y = new Deterministic(value);
                NotifyPropertyChanged();
                ((DeterministicRow)PreviousRow)?.NotifyPropertyChanged(nameof(Value));
                ((DeterministicRow)NextRow)?.NotifyPropertyChanged(nameof(Value));
            }
        }
        protected override List<string> YMinProperties { 
            get
            {
                return new List<string>() { nameof(Value)};
            } 
        }
        protected override List<string> YMaxProperties
        {
            get
            {
                return new List<string>() { nameof(Value) };
            }
        }

        public DeterministicRow(double x, double y, bool isStrictMonotonic, bool xIsDecreasing = false) : base(x, new Deterministic(y), xIsDecreasing: xIsDecreasing)
        {
            AddSinglePropertyRule(nameof(Value), CreateValuesIncreasingPreviousRowRule(isStrictMonotonic));
            AddSinglePropertyRule(nameof(Value), CreateValuesIncreasingNextRowRule(isStrictMonotonic));
        }

        private Rule CreateValuesIncreasingPreviousRowRule(bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (PreviousRow == null)
                {
                    return true;
                }
                else
                {
                    double prevValue = ((Deterministic)PreviousRow.Y).Value;
                    if (isStrictMonotonic)
                    {
                        return Value > prevValue;
                    }
                    else
                    {
                        return Value >= prevValue;
                    }
                }
            },
                "Y values are not increasing.", ErrorLevel.Severe);
        }

        private Rule CreateValuesIncreasingNextRowRule(bool isStrictMonotonic)
        {
            return new Rule(() =>
            {
                if (NextRow == null)
                {
                    return true;
                }
                else
                {
                    double nextValue = ((Deterministic)NextRow.Y).Value;
                    if (isStrictMonotonic)
                    {
                        return Value < nextValue;
                    }
                    else
                    {
                        return Value <= nextValue;
                    }
                }
            },
                "Y values are not increasing.", ErrorLevel.Severe);
        }

        public override void UpdateRow(int col, double value)
        {
            switch (col)
            {
                case 0:
                    X = value;
                    break;
                case 1:
                    Value = value;
                    break;
            }
        }
    }
}


