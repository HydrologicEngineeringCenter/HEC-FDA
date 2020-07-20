using System;
using System.Collections.Generic;
using System.Text;
using Model.Parameters.Series;

using Functions;

using Utilities;

namespace Model.Validation
{
    internal sealed class FrequencySeriesValidator : IValidator<FrequencySeries>
    {
        internal static IRange<double> _ValidRange => IRangeFactory.Factory(0d, 1d); 
        
        public IMessageLevels IsValid(FrequencySeries entity, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(entity);
            return msgs.Max();
        }
        public IEnumerable<IMessage> ReportErrors(FrequencySeries series)
        {
            List<IMessage> msgs = new List<IMessage>();
            //if (!IsOnRange(series.Parameter)) msgs.Add(
            //    IMessageFactory.Factory(IMessageLevels.Error, 
            //    $"The {series.ParameterType.ToString()} contains one or more probability values outside of the acceptable range of [0, 1]."));
            return msgs;
        }
        private bool IsOnRange(IEnumerable<IOrdinate> series)
        {
            foreach (var x in series) if (x.Range.IsOnRange(_ValidRange)) return false;
            return true;
        }
        public static bool IsConstructable(IFdaFunction fx, IParameterEnum parameterType, out string msg)
        {
            msg = "";
            if (fx.IsNull()) 
                msg += $"The {typeof(FrequencySeries)} cannot be constructed because the specified {typeof(IFdaFunction)} containing the {typeof(IParameter)} is null. ";
            if (!(parameterType == IParameterEnum.ExceedanceProbability ||
                parameterType == IParameterEnum.NonExceedanceProbability ||
                parameterType == IParameterEnum.LateralStructureFailure)) 
                msg += $"The {typeof(FrequencySeries)} cannot be constructed from because the {typeof(IParameterEnum)} is set to {parameterType}. Valid {typeof(IParameterEnum)} values include: ({IParameterEnum.NonExceedanceProbability}, {IParameterEnum.ExceedanceProbability}, or {IParameterEnum.FailureProbability}). "; 
            return msg.Length == 0;
        }
    }
}
