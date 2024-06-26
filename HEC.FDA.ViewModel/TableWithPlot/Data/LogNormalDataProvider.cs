﻿using HEC.FDA.ViewModel.TableWithPlot.Rows;
using Statistics.Distributions;
using HEC.FDA.ViewModel.TableWithPlot.Data.ExtensionMethods;
using HEC.FDA.ViewModel.TableWithPlot.Data.Abstract;
using HEC.FDA.Model.paireddata;
using HEC.FDA.ViewModel.Utilities;
using Statistics;

namespace HEC.FDA.ViewModel.TableWithPlot.Data
{
    internal class LogNormalDataProvider : BaseDataProvider
    {
        public LogNormalDataProvider()
        {
            Name = "LogNormal";
            UncertainPairedData uncertainPairedData = DefaultData.GeneralUseDefaultCurve(IDistributionEnum.LogNormal);
            for (int i = 0; i < uncertainPairedData.Xvals.Length; i++)
            {
                Data.Add(new LogNormalRow(uncertainPairedData.Xvals[i], (LogNormal)uncertainPairedData.Yvals[i], IsStrictMonotonic));
            }
            LinkList();
        }
        public LogNormalDataProvider(bool isStrictMonotonic)
        {
            IsStrictMonotonic = isStrictMonotonic;
            Name = "LogNormal";
            UncertainPairedData uncertainPairedData = DefaultData.GeneralUseDefaultCurve(IDistributionEnum.LogNormal);
            for (int i = 0; i < uncertainPairedData.Xvals.Length; i++)
            {
                Data.Add(new LogNormalRow(uncertainPairedData.Xvals[i], (LogNormal)uncertainPairedData.Yvals[i], IsStrictMonotonic));
            }
            LinkList();
        }
        public LogNormalDataProvider(UncertainPairedData uncertainPairedData, bool isStrictMonotonic)
        {
            IsStrictMonotonic = isStrictMonotonic;
            Name = "LogNormal";
            for (int i = 0; i < uncertainPairedData.Xvals.Length; i++)
            {
                Data.Add(new LogNormalRow(uncertainPairedData.Xvals[i], (LogNormal)uncertainPairedData.Yvals[i], IsStrictMonotonic));
            }
            LinkList();
        }
        override public void AddUnlinkedRow(int i)
        {
            double mean0 = 0.0;
            double stdev0 = 0.0;
            double x0 = 0.0;
            if (i == 0)
            {
                if (Data.Count != 0)
                {
                    x0 = ((LogNormalRow)Data[i]).X;
                    mean0 = ((LogNormal)((LogNormalRow)Data[i]).Y).Mean;
                    stdev0 = ((LogNormal)((LogNormalRow)Data[i]).Y).StandardDeviation;
                }
                DataProviderExtensions.AddRow(this, i, new LogNormalRow(x0, new LogNormal(mean0, stdev0), IsStrictMonotonic));
                return;
            }
            if (Data.Count > i)
            {
                x0 = ((LogNormalRow)Data[i - 1]).X;
                mean0 = ((LogNormal)((LogNormalRow)Data[i - 1]).Y).Mean;
                stdev0 = ((LogNormal)((LogNormalRow)Data[i - 1]).Y).StandardDeviation;
                double x1 = 0.0;
                double mean1 = 0.0;
                double stdev1 = 0.0;
                x1 = ((LogNormalRow)Data[i]).X;
                mean1 = ((LogNormal)((LogNormalRow)Data[i]).Y).Mean;
                stdev1 = ((LogNormal)((LogNormalRow)Data[i]).Y).StandardDeviation;
                double x = x0 + ((x1 - x0) / 2.0);
                double mean = mean0 + ((mean1 - mean0) / 2.0);
                double stdev = stdev0 + ((stdev1 - stdev0) / 2.0);
                DataProviderExtensions.AddRow(this, i, new LogNormalRow(x, new LogNormal(mean, stdev), IsStrictMonotonic));
            }
            else
            {
                x0 = ((LogNormalRow)Data[Data.Count - 1]).X;
                mean0 = ((LogNormal)((LogNormalRow)Data[Data.Count - 1]).Y).Mean;
                stdev0 = ((LogNormal)((LogNormalRow)Data[Data.Count - 1]).Y).StandardDeviation;
                DataProviderExtensions.AddRow(this, i, new LogNormalRow(x0, new LogNormal(mean0, stdev0), IsStrictMonotonic));
                return;
            }
        }
    }
}
