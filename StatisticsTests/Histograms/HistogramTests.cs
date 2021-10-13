using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Statistics;
using Statistics.Histograms;
using Xunit;
using Utilities;

namespace StatisticsTests.Histograms
{


    [ExcludeFromCodeCoverage]
    public class HistogramTests
    {
        [Theory]
        [InlineData(1, 1)]
        public void HistogramStatistics_Minimum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.Min;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 0)]
        public void HistogramStatistics_AddedData_Minimum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double[] additionalData = new double[1] { 0 };
            IData additionalObs = new Data(additionalData);
            histogram.AddObservationsToHistogram(additionalObs);
            double actual = histogram.Min;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 6)]
        public void HistogramStatistics_Maximum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(1, 7)]
        public void HistogramStatistics_AddedData_Maximum(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double[] additionalData = new double[1] { 6 };
            IData additionalObs = new Data(additionalData);
            histogram.AddObservationsToHistogram(additionalObs);
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(1, 3.5)]
        public void HistogramStatistics_Mean(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.Mean;
            Assert.Equal(expected, actual);
        }


        [Theory]
        [InlineData(1, 1.58113883)]
        public void HistogramStatistics_StandardDeviation(double binWidth, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.StandardDeviation;
            double err = Math.Abs((expected - actual) / expected);
            double tol = 0.01;
            Assert.True(err < tol);
        }

        [Theory]
        [InlineData(1, 0.5, 3)]
        public void Histogram_InvCDF(double binWidth, double prob, double expected)
        {
            double[] data = new double[6] { 1, 2, 3, 4, 5, 6};
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.InverseCDF(prob);
            double err = Math.Abs((expected - actual) / expected);
            double tol = 0.01;
            Assert.True(err < tol);
        }

        [Theory]
        [InlineData(1, 2, .4)]
        public void Histogram_CDF(double binWidth, double val, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.CDF(val);
            double err = Math.Abs((expected - actual) / expected);
            double tol = 0.01;
            Assert.True(err < tol);
        }

        [Theory]
        [InlineData(1, 2, .2)]
        public void Histogram_PDF(double binWidth, double val, double expected)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double actual = histogram.PDF(val);
            double err = Math.Abs((expected - actual) / expected);
            double tol = 0.01;
            Assert.True(err < tol);
        }

        [Theory]
        [InlineData(1)]
        public void Fit_ExpandsHistogram_WithDataOutOfRange(double binWidth)
        {
            double[] data = new double[5] { 1, 2, 3, 4, 5 };
            IData obs = new Data(data);
            Histogram histogram = new Histogram(obs, binWidth);
            double[] newData = new double[2] { 7, 9 };
            IData newObs = new Data(newData);
            histogram.AddObservationsToHistogram(newObs);
            double expected = 10;
            double actual = histogram.Max;
            Assert.Equal(expected, actual);
        }



    }
}
