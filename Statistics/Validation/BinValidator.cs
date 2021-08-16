using System;
using System.Collections.Generic;
using System.Linq;

using Utilities;

using Statistics.Histograms;

namespace Statistics.Validation
{
    internal class BinValidator: IValidator<IBin>
    {
        internal BinValidator()
        {
        }
        public IMessageLevels IsValid(IBin obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }
        public IEnumerable<IMessage> ReportErrors(IBin obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj), "The Bin could not be validated because it is null.");
            if (obj.Range.Messages.Max() > IMessageLevels.Message || obj.Count < 0) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"{Resources.InvalidParameterizationNotice(obj.Print(true))} {Bin.Requirements(false)}."));
            if (obj.Range.Messages.Any()) msgs.Add(IMessageFactory.Factory(obj.Range.Messages.Max(), $"The {obj.Print(true)} contains the following messages: \r\n{obj.Range.Messages.PrintTabbedListOfMessages()}"));
            if (!obj.MidPoint.IsFinite()) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The histogram {obj.Print(true)} is invalid because its midpoint value: {obj.MidPoint.Print()} defined by the equations: (BinMax: {obj.Range.Max} - BinMin: {obj.Range.Min}) / 2 + BinMin: {obj.Range.Min}, is not a finite numerical value."));
            return msgs;
        }

        internal static bool IsConstructable(double min, double max, int n, out string msg)
        {
            msg = ReportFatalErrors(min, max, n);
            return msg.Length == 0;
        }
        internal static bool IsConstructable(IBin oldBin, int addN, out string msg)
        {
            msg = ReportFatalErrors(oldBin, addN);
            return msg.Length == 0;
        }
        internal static string ReportFatalErrors(double min, double max, int n)
        {
            string msg = "";
            if (!ValidationExtensions.IsRange(min, max, true, true)) msg += $"{Resources.FatalParameterizationNotice(Bin.Print(min, max, n))} {Bin.Requirements(true)}";
            if (n < 0) msg += $"The specified number of bin observations {n} is invalid because it is not a positive number.";
            return msg;
        }
        internal static string ReportFatalErrors(IBin oldBin, int addN)
        {
            string msg = "";
            if (oldBin.IsNull()) msg += "Observations cannot be added the bin because it is null";
            else
            {
                if (addN < 0 || ((long)oldBin.Count + (long)addN > (long)int.MaxValue)) msg += $"{addN.Print()} observations cannot be added to the {oldBin.Print(true)} because {addN.Print()} is a negative number or the sum of the observations: ({addN.Print()} + {oldBin.Count.Print()}) exceeds the maximum integer: {int.MaxValue.Print()} value.";
            }
            return msg;
        }
    }
}
