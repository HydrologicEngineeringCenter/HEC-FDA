using System;
using System.Collections.Generic;
using FdaModel.Utilities.Attributes;
using FdaModel.Utilities.Messager;

namespace FdaModel.Functions.FrequencyFunctions
{
    [Author("John Kucharski", "10/10/2016")]
    public class Graphical : FrequencyFunction
    {
        #region Notes
        /* 1. The Constructor and Sample Function abstract methods have not been implemented, only added to reduce errors.
           2. Ordered events methods need to be added.*/
        #endregion

        #region Constructors 
        public Graphical(Statistics.ContinuousDistribution functionPassedByChildren, FunctionTypes functionType) : base(functionPassedByChildren, functionType)
        {
        }
        #endregion

        #region Functions
        public override void Validate()
        {
            throw new NotImplementedException();
        }

        public override BaseFunction SampleFunction(Random randomNumberGenerator)
        {
            throw new NotImplementedException();
        }

        public override double GetXfromY(double Y, ref List<ErrorMessage> errors)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
