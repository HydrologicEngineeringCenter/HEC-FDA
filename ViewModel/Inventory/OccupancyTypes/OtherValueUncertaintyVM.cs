using HEC.FDA.ViewModel.Inventory.OccupancyTypes;
using Statistics;
using Statistics.Distributions;
using System;
using System.Xml.Linq;

namespace ViewModel.Inventory.OccupancyTypes
{
    public class OtherValueUncertaintyVM : ValueUncertaintyVM
    {
        public OtherValueUncertaintyVM(ContinuousDistribution valueUncertaintyOrdinate) : base(valueUncertaintyOrdinate)
        {
        }

        private void LoadDefaultDerministicControl()
        {
            DeterministicControlVM = new DeterministicControlVM(.5);
            DeterministicControlVM.WasModified += ControlWasModified;
        }

        private void LoadDefaultNormalControl()
        {
            NormalControlVM = new NormalControlVM(0, 0, "%", true);
            NormalControlVM.WasModified += ControlWasModified;
        }

        private void LoadDefaultLogNormalControl()
        {
            LogNormalControlVM = new LogNormalControlVM(0, 0, "", true);
            LogNormalControlVM.WasModified += ControlWasModified;
        }

        private void LoadDefaultTriangularControl()
        {
            TriangularControlVM = new TriangularControlVM(1, 0, 2, "%", "%", true);
            TriangularControlVM.WasModified += ControlWasModified;
        }

        private void LoadDefaultUniformControl()
        {
            UniformControlVM = new UniformControlVM(0, 1, "%", "%", true);
            UniformControlVM.WasModified += ControlWasModified;
        }

        private void SetSelectedDistribution(IDistribution ordinate)
        {
            switch (ordinate.Type)
            {
                case IDistributionEnum.Deterministic:
                    DeterministicControlVM.WasModified -= ControlWasModified;
                    DeterministicControlVM = new DeterministicControlVM(((Deterministic)ordinate).Value);
                    DeterministicControlVM.WasModified += ControlWasModified;
                    break;
                case IDistributionEnum.Normal:
                    NormalControlVM.WasModified -= ControlWasModified;
                    double normalMean = ((Normal)ordinate).Mean;
                    double normalStDev = ((Normal)ordinate).StandardDeviation;
                    NormalControlVM = new NormalControlVM(normalMean, normalStDev, "%", true);
                    NormalControlVM.WasModified += ControlWasModified;
                    break;
                case IDistributionEnum.LogNormal:
                    LogNormalControlVM.WasModified -= ControlWasModified;
                    double logNormalMean = ((LogNormal)ordinate).Mean;
                    double logNormalStDev = ((LogNormal)ordinate).StandardDeviation;
                    LogNormalControlVM = new LogNormalControlVM(logNormalMean, logNormalStDev, "", true);
                    LogNormalControlVM.WasModified += ControlWasModified;
                    break;
                case IDistributionEnum.Triangular:
                    //TriangularControlVM.WasModified -= ControlWasModified;
                    //double triMostLikely = ((Triangular)ordinate).MostLikely;
                    //double triMin = ((Triangular)ordinate).Min;
                    //double triMax = ((Triangular)ordinate).Max;
                    //TriangularControlVM = new TriangularControlVM(triMostLikely, triMin, triMax, "%", "%", true);
                    //TriangularControlVM.WasModified += ControlWasModified;
                    TriangularControlVM.UpdateValues((Triangular)ordinate);
                    break;
                case IDistributionEnum.Uniform:
                    UniformControlVM.WasModified -= ControlWasModified;
                    double uniMin = ((Uniform)ordinate).Min;
                    double uniMax = ((Uniform)ordinate).Max;
                    UniformControlVM = new UniformControlVM(uniMin, uniMax, "%", "%", true);
                    UniformControlVM.WasModified += ControlWasModified;
                    break;
            }
        }

        public override void LoadControlVMs(IDistribution ordinate)
        {
            LoadDefaultDerministicControl();
            LoadDefaultNormalControl();
            LoadDefaultLogNormalControl();
            LoadDefaultTriangularControl();
            LoadDefaultUniformControl();

            SetSelectedDistribution(ordinate);
        }

        public override XElement ToXML()
        {
            //todo:
            throw new NotImplementedException();
        }
    }
}
