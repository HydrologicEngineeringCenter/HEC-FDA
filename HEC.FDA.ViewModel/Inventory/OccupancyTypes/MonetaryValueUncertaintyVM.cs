﻿using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using Statistics;
using Statistics.Distributions;
using System;
using System.Xml.Linq;

namespace ViewModel.Inventory.OccupancyTypes
{
    public class MonetaryValueUncertaintyVM : ValueUncertaintyVM
    {

        public MonetaryValueUncertaintyVM(ContinuousDistribution valueUncertaintyOrdinate) : base(valueUncertaintyOrdinate)
        {
        }

        public override void LoadControlVMs(IDistribution ordinate)
        {
            IDistributionEnum ordType = ordinate.Type;
            //create constant option
            if (ordType == IDistributionEnum.Deterministic)
            {
                ControlWasModified(this, new EventArgs());
            }

            //create normal option
            double normalMean = 0;
            double normalStDev = 0;
            if (ordType == IDistributionEnum.Normal)
            {
                normalMean = ((Normal)ordinate).Mean;
                normalStDev = ((Normal)ordinate).StandardDeviation;
            }
            NormalControlVM = new NormalControlVM(normalMean, normalStDev, "% of inventory value");
            NormalControlVM.WasModified += ControlWasModified;

            //create log normal option
            double logNormalMean = 0;
            double logNormalStDev = 0;
            if (ordType == IDistributionEnum.LogNormal)
            {
                logNormalMean = ((LogNormal)ordinate).Mean;
                logNormalStDev = ((LogNormal)ordinate).StandardDeviation;
            }
            LogNormalControlVM = new LogNormalControlVM(logNormalMean, logNormalStDev, "");
            LogNormalControlVM.WasModified += ControlWasModified;

            //create the triangular option
            double triMostLikely = 1;
            double triMin = 0;
            double triMax = 2;
            if (ordType == IDistributionEnum.Triangular)
            {
                triMostLikely = ((Triangular)ordinate).MostLikely;
                triMin = ((Triangular)ordinate).Min;
                triMax = ((Triangular)ordinate).Max;
            }
            TriangularControlVM = new TriangularControlVM(triMostLikely, triMin, triMax, "% of inventory value", "% of inventory value");
            TriangularControlVM.WasModified += ControlWasModified;

            //create the uniform option
            double uniMin = 0;
            double uniMax = 1;
            if (ordType == IDistributionEnum.Uniform)
            {
                uniMin = ((Uniform)ordinate).Min;
                uniMax = ((Uniform)ordinate).Max;
            }
            UniformControlVM = new UniformControlVM(uniMin, uniMax, "% of inventory value", "% of inventory value");
            UniformControlVM.WasModified += ControlWasModified;
        }

        public override XElement ToXML()
        {
            //todo:
            throw new NotImplementedException();
        }
    }
}
