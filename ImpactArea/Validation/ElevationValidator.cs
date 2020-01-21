using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace ImpactArea.Validation
{
    internal class ElevationValidator: IValidator<Elevation>
    {
        private static readonly IRange<double> _GroundRange = IRangeFactory.Factory(-999, 40000);
        private static readonly IRange<double> _HeightRange = IRangeFactory.Factory(-10, 10);
        private static readonly IRange<double> _AssetRange = IRangeFactory.Factory(_GroundRange.Min + _HeightRange.Min, _GroundRange.Max + _HeightRange.Max);
        
        public bool IsValid(Elevation obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max() < IMessageLevels.Error;
        }
        public IEnumerable<IMessage> ReportErrors(Elevation obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            if (obj.IsNull()) throw new ArgumentNullException("The elevation cannot be validated because it is null.");
            if (obj.Type == ElevationEnum.NotSet) msgs.Add(IMessageFactory.Factory(IMessageLevels.Message, $"The elevation type (e.g. ground elevation, asset height, asset elevation) is not set and cannot be used in computation."));
            if (obj.Type == ElevationEnum.Ground && !_GroundRange.IsOnRange(obj.Height.Value())) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The {obj.Type} elevation value: {obj.Height} is not on the valid range: {_GroundRange.Print(true)}."));
            if (obj.Type == ElevationEnum.Asset && !_AssetRange.IsOnRange(obj.Height.Value())) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The {obj.Type} elevation value: {obj.Height} is not on the valid range: {_AssetRange.Print(true)}."));
            if (obj.Type == ElevationEnum.Height && !_HeightRange.IsOnRange(obj.Height.Value())) msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, $"The {obj.Type} value: {obj.Height} is not on the valid range: {_HeightRange.Print(true)}."));
            return msgs;
        }

        public static bool IsConstructable(IOrdinate height, out string msg)
        {
            msg = ReportFatalError(height);
            return msg.Length == 0;
        }
        private static string ReportFatalError(IOrdinate height)
        {
            string msg = "";
            if (height.IsNull()) return "The IElevation object cannot be created because it is null.";
            if (!height.IsValid) msg += $"The height ordinate is invalid it contains the following messages: {Utilities.Validate.PrintTabbedListOfMessages(height.Messages)}";
            return msg;
        }
    }
}
