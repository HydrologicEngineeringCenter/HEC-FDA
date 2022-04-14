using Statistics;
using System.Collections.Generic;
using Statistics.Distributions;
using HEC.FDA.ViewModel.TableWithPlot.Rows.Attributes;
using HEC.MVVMFramework.ViewModel.Validation;
using HEC.MVVMFramework.Base.Enumerations;


namespace HEC.FDA.ViewModel.TableWithPlot.Rows
{
    internal class TriangularRow : SequentialRow
    {
        [DisplayAsColumn("Min")]
        [DisplayAsLine("Min", Enumerables.ColorEnum.Blue, true)]
        public double Min
        {
            get
            {
                return ((Triangular)Y).Min;
            }
            set
            {
                ((Triangular)Y).Min = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Max));
                NotifyPropertyChanged(nameof(MostLikely));
                ((TriangularRow)PreviousRow)?.NotifyPropertyChanged(nameof(Min));
                ((TriangularRow)NextRow)?.NotifyPropertyChanged(nameof(Min));
            }
        }

        [DisplayAsColumn("Most Likely")]
        [DisplayAsLine("Most Likely", Enumerables.ColorEnum.Red)]
        public double MostLikely
        {
            get
            {
                return ((Triangular)Y).MostLikely;
            }

            set
            {
                ((Triangular)Y).MostLikely = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(Max));
                NotifyPropertyChanged(nameof(Min));
                ((TriangularRow)PreviousRow)?.NotifyPropertyChanged(nameof(MostLikely));
                ((TriangularRow)NextRow)?.NotifyPropertyChanged(nameof(MostLikely));
            }
        }

        [DisplayAsColumn("Max")]
        [DisplayAsLine("Max", Enumerables.ColorEnum.Blue, true)]
        public double Max
        {
            get
            {
                return ((Triangular)Y).Max;
            }
            set
            {
                ((Triangular)Y).Max = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(MostLikely));
                NotifyPropertyChanged(nameof(Min));
                ((TriangularRow)PreviousRow)?.NotifyPropertyChanged(nameof(Max));
                ((TriangularRow)NextRow)?.NotifyPropertyChanged(nameof(Max));
            }
        }

        protected override List<string> YMinProperties
        {
            get
            {
                return new List<string>() { nameof(Min), nameof(MostLikely)};
            }
        }
        protected override List<string> YMaxProperties
        {
            get
            {
                return new List<string>() { nameof(Max), nameof(MostLikely) };
            }
        }

        public TriangularRow(double x, IDistribution y) : base(x, y)
        {
            AddSinglePropertyRule(nameof(Min), new Rule(() => { if (PreviousRow == null) return true; return Min > ((Triangular)PreviousRow.Y).Min; }, "Min values are not increasing.",ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Min), new Rule(() => { if (NextRow == null) return true; return Min < ((Triangular)NextRow.Y).Min; }, "Min values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(MostLikely), new Rule(() => { if (PreviousRow == null) return true; return MostLikely > ((Triangular)PreviousRow.Y).MostLikely; }, "Most likely values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(MostLikely), new Rule(() => { if (NextRow == null) return true; return MostLikely < ((Triangular)NextRow.Y).MostLikely; }, "Most likely values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Max), new Rule(() => { if (PreviousRow == null) return true; return Max > ((Triangular)PreviousRow.Y).Max; }, "Max values are not increasing.", ErrorLevel.Severe));
            AddSinglePropertyRule(nameof(Max), new Rule(() => { if (NextRow == null) return true; return Max < ((Triangular)NextRow.Y).Max; }, "Max values are not increasing.", ErrorLevel.Severe));
            AddMultiPropertyRule(new List<string> { "Min", "MostLikely", "Max" }, new Rule(() => { return ((Min < MostLikely) && (MostLikely < Max)); }, "Min must be less than most likely, which must be less than Max", ErrorLevel.Severe));
        }

        public override void UpdateRow(int col, double value)
        {
            switch (col)
            {
                case 0:
                    X = value;
                    break;
                case 1:
                    Min = value;
                    break;
                case 2:
                    MostLikely = value;
                    break;
                case 3:
                    Max = value;
                    break;
            }
        }
    }
}
