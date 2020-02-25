using Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Utilities;

namespace Model.Validation
{
    internal class ElevationValidator: IValidator<IElevation>
    {
        public bool IsValid(IElevation obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max() < IMessageLevels.Error;
        }
        public IEnumerable<IMessage> ReportErrors(IElevation obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            switch (obj.Type)
            {
                case IElevationEnum.Ground:
                    if (obj.Height.Range.Min < -1355d || obj.Height.Range.Max > 29035)
                    {
                        msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The specified ground elevation range: {obj.Height.Range.Print(true)} exceeds the approximate range of ground elevations on earth: [-1355, 29035] in feet. This is likely to produce undesirable results."));
                    }
                    break;
                case IElevationEnum.AssetHeight:
                    if (obj.Height.Range.Min < -10d || obj.Height.Range.Max > 100)
                    {
                        msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The specified asset height (relative to the land) range: {obj.Height.Range.Print(true)} exceeds the expected maximum and minimum range: [-10, 100] in feet. This is likely to produce undesirable results."));
                    }
                    break;
                case IElevationEnum.Asset:
                    if (obj.Height.Range.Min < -1365d || obj.Height.Range.Max > 29135)
                    {
                        msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The specified asset elevation range: {obj.Height.Range.Print(true)} exceeds the expected range (based on maximum ground elevations on earth, plus the expected extremes in asset heights): [-1365, 29135] in feet. This is likely to produce undesirable results."));
                    }
                    break;
                case IElevationEnum.Levee:
                    if (!obj.IsConstant) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The levee elevation is invalid because it is not constant.")); 
                    if (obj.Height.Value() < -1355d || obj.Height.Value() > 29135)
                    {
                        msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The specified levee elevation: {obj.Height.Value().Print()} exceeds the approximate range of ground elevations on earth plus a reasonable levee height: [-1355, 29035] in feet. This is likely to produce undesirable results."));
                    }
                    break;
                default:
                    msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, "The elevation is invalid. It is not set to an acceptable type."));
                    break;
            }
            if (obj.Height.Messages.Any()) msgs.Add(IMessageFactory.Factory(obj.Height.Messages.Max(), $"The elevation height contains the following messages: \r\n {obj.Height.Messages.PrintTabbedListOfMessages()}"));
            return msgs;
        }
        public static bool IsConstructable(IElevationEnum t, IOrdinate value, out string msg)
        {
            msg = "";
            if (t.IsNull() || value.IsNull()) msg += $"The {nameof(IElevation)} cannot be constructed because the {nameof(IElevation.Type)} or {nameof(IElevation.Height)} is null.";
            if (t == IElevationEnum.NotSet) msg += $"The {nameof(IElevation)} cannot be constructed because the {nameof(IElevation.Type)} is not set.";
            if (value.Value().IsFinite()) msg += $"The {nameof(IElevation)} cannot be constructed because the height is not a finite numerical value.";
            return msg.Length == 0;
        }
    }
}
