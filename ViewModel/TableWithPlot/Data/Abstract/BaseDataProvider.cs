using HEC.FDA.ViewModel.TableWithPlot.Data.Interfaces;
using HEC.FDA.ViewModel.TableWithPlot.Rows;
using paireddata;
using Statistics;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace HEC.FDA.ViewModel.TableWithPlot.Data.Abstract
{
    public abstract class BaseDataProvider:IDataProvider
    {
        public string Name { get; set; } 
        public ObservableCollection<object> Data { get; } = new ObservableCollection<object>();
        public double xMax { get; set; } = double.MaxValue;
        public double yMax { get; set; } = double.MaxValue;
        public double xMin { get; set; } = double.MinValue;
        public double yMin { get; set; } = double.MinValue;

        public virtual void RemoveRows(List<int> rowIndices)
        {
            for (int i = rowIndices.Count() - 1; i >= 0; i--)
            {
                Data.RemoveAt(rowIndices[i]);
            }
            LinkList();
        }
        public void LinkList()
        {
            //Nothing to link if the list isn't more than 1.
            if (Data.Count < 2) { return; }

            //Clear previous links
            foreach (SequentialRow item in Data)
            {
                item.PreviousRow = null;
                item.NextRow = null;
            }

            //Set first row
            SequentialRow firstCurrent = Data[0] as SequentialRow;
            SequentialRow firstNext = Data[1] as SequentialRow;
            firstCurrent.NextRow = firstNext;

            //Set middle row
            for (int i = 1; i < Data.Count - 1; i++)
            {
                SequentialRow prev = Data[i - 1] as SequentialRow;
                SequentialRow current = Data[i] as SequentialRow;
                SequentialRow next = Data[i + 1] as SequentialRow;
                current.PreviousRow = prev;
                current.NextRow = next;
            }

            //Set last row
            SequentialRow lastPrev = Data[Data.Count - 2] as SequentialRow;
            SequentialRow lastCurrent = Data[Data.Count - 1] as SequentialRow;
            lastCurrent.PreviousRow = lastPrev;
        }
        public UncertainPairedData ToUncertainPairedData(string xlabel, string ylabel, string name, string description, string category)
        {
            double[] xs = new double[Data.Count];
            IDistribution[] ys = new IDistribution[Data.Count];
            for (int i = 0; i < Data.Count; i++)
            {
                xs[i] = ((SequentialRow)Data[i]).X;
                ys[i] = ((SequentialRow)Data[i]).Y;
            }
            return new UncertainPairedData(xs, ys, xlabel, ylabel, name, description, category);
        }
        public void UpdateFromUncertainPairedData(UncertainPairedData upd)
        {
            Data.Clear();
            double[] x = upd.Xvals;
            IDistribution[] y = upd.Yvals;
            for (int i = 0;i < x.Length; i++)
            {
                if(i >= Data.Count())
                {
                    AddUnlinkedRow(i);
                }
                SequentialRow sr = Data[i] as SequentialRow;
                sr.X = x[i];
                sr.Y = y[i];
            }
            LinkList();
        }

        public void AddRow(int i)
        {
            AddUnlinkedRow(i);
            LinkList();
            setGlobalMaxAndMin(i);
        }

        public void setGlobalMaxAndMin()
        {
            foreach(SequentialRow sr in Data)
            {
                sr.SetGlobalMaxRules(xMax, yMax, xMin, yMin);
                sr.Validate();
            }
        }
        public void setGlobalMaxAndMin( int i)
        {
            ((SequentialRow)Data[i]).SetGlobalMaxRules(xMax, yMax, xMin, yMin);
        }
        public abstract void AddUnlinkedRow(int i);
    }
}
