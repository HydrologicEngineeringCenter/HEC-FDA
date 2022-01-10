using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Inventory.OccupancyTypes
{
    public class FoundationValueUncertaintyVM : ValueUncertaintyVM
    {
        public FoundationValueUncertaintyVM(IOrdinate valueUncertaintyOrdinate) : base(valueUncertaintyOrdinate)
        {
        }

        public override void LoadControlVMs(IOrdinate ordinate)
        {
            IOrdinateEnum ordType = ordinate.Type;
            //create constant option
            if (ordType == IOrdinateEnum.Constant)
            {
                //todo: i don't really know how to handle constant right now
                ControlWasModified(this, new EventArgs());
            }

            //create normal option
            double normalMean = 0;
            double normalStDev = 0;
            if (ordType == IOrdinateEnum.Normal)
            {
                normalMean = ((IDistributedOrdinate)ordinate).Mean;
                normalStDev = ((IDistributedOrdinate)ordinate).StandardDeviation;
            }
            _NormalControlVM = new NormalControlVM(normalMean, normalStDev, "");
            _NormalControlVM.WasModified += ControlWasModified;

            //create log normal option
            //create normal option
            double logNormalMean = 0;
            double logNormalStDev = 0;
            if (ordType == IOrdinateEnum.LogNormal)
            {
                logNormalMean = ((IDistributedOrdinate)ordinate).Mean;
                logNormalStDev = ((IDistributedOrdinate)ordinate).StandardDeviation;
            }
            _LogNormalControlVM = new LogNormalControlVM(logNormalMean, logNormalStDev, "");
            _LogNormalControlVM.WasModified += ControlWasModified;

            //create the triangular option
            double triMostLikely = 1;
            double triMin = 0;
            double triMax = 2;
            if (ordType == IOrdinateEnum.Triangular)
            {
                triMostLikely = ((IDistributedOrdinate)ordinate).MostLikely;
                triMin = ((IDistributedOrdinate)ordinate).Range.Min;
                triMax = ((IDistributedOrdinate)ordinate).Range.Max;
            }
            _TriangularControlVM = new TriangularControlVM(triMostLikely, triMin, triMax, "units below most likely", "units above most likely");
            _TriangularControlVM.WasModified += ControlWasModified;

            //create the uniform option
            double uniMin = 0;
            double uniMax = 1;
            if (ordType == IOrdinateEnum.Uniform)
            {
                uniMin = ((IDistributedOrdinate)ordinate).Range.Min;
                uniMax = ((IDistributedOrdinate)ordinate).Range.Max;
            }
            _UniformControlVM = new UniformControlVM(uniMin, uniMax, "from expected value");
            _UniformControlVM.WasModified += ControlWasModified;

        }
    }
}
