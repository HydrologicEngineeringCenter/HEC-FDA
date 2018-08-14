using System;
using FdaModel.Utilities.Attributes;

namespace FdaModel.Inputs.Functions.FrequencyFunctions
{
    [Author("John Kucharski", "10/10/2016")]
    public class Graphical : FrequencyFunction
    {
        #region Notes
        /* 1. The Constructor and Sample Function abstract methods have not been implemented, only added to reduce errors.
           2. Ordered events methods need to be added.*/
        #endregion

        #region Constructors 
        public Graphical(ComputationPoint computationPoint, Statistics.ContinuousDistribution functionPassedByChildren, FunctionTypes functionType) : base(computationPoint, functionPassedByChildren, functionType)
        {
        }
        #endregion

        #region Functions
        public override bool Validate()
        {
            throw new NotImplementedException();
        }

        public override BaseFunction SampleFunction(Random randomNumberGenerator)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
