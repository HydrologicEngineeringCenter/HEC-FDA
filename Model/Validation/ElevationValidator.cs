using Functions;
using Model.Parameters.Elevations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Model.Validation
{

    internal class ElevationValidator: IValidator<ElevationBase>
    {
        internal const int _MinElevationParameterEnum = 20;
        internal const int _MaxElevationParameterEnum = 29;
        internal const double _LowestGroundElevation = -1365;
        internal const double _HighestGroundElevation = 29035;
        internal const double _TallestDamOrLevee = 1001;
        internal const double _LowestAssetHeight = 0;
        internal const double _HighestAssetHeight = 100;

        public IMessageLevels IsValid(ElevationBase obj, out IEnumerable<IMessage> msgs)
        {
            msgs = ReportErrors(obj);
            return msgs.Max();
        }
        public IEnumerable<IMessage> ReportErrors(ElevationBase obj)
        {
            List<IMessage> msgs = new List<IMessage>();
            switch (obj.ParameterType)
            {
                case IParameterEnum.GroundElevation:
                    if (obj.Range.Min < _LowestGroundElevation || 
                        obj.Range.Max > _HighestGroundElevation)
                    {
                        msgs.Add(IMessageFactory.Factory(IMessageLevels.Error,
                            NoticeMessage(obj),
                            ExtendedMessage(obj, _LowestGroundElevation, _HighestGroundElevation, obj.Units)));    
                    }
                    break;
                case IParameterEnum.AssetHeight:
                    if (obj.Range.Min < _LowestAssetHeight || 
                        obj.Range.Max > _HighestAssetHeight)
                    {
                        msgs.Add(IMessageFactory.Factory(IMessageLevels.Error,
                            NoticeMessage(obj),
                            ExtendedMessage(obj, _LowestAssetHeight, _HighestAssetHeight, obj.Units)));
                    }
                    break;
                case IParameterEnum.AssetElevation:
                    if (obj.Range.Min < _LowestGroundElevation + _LowestAssetHeight || 
                        obj.Range.Max > _HighestGroundElevation + _HighestAssetHeight)
                    {
                        msgs.Add(IMessageFactory.Factory(IMessageLevels.Error,
                            NoticeMessage(obj),
                            ExtendedMessage(obj, _LowestGroundElevation + _LowestAssetHeight, _HighestGroundElevation + _HighestAssetHeight, obj.Units)));      
                    }
                    break;
                case IParameterEnum.LateralStructureElevation:
                    if (obj.Range.Min < _LowestGroundElevation || 
                        obj.Range.Max > _HighestGroundElevation + _TallestDamOrLevee)
                    {
                        msgs.Add(IMessageFactory.Factory(IMessageLevels.Error,
                            NoticeMessage(obj),
                            ExtendedMessage(obj, _LowestGroundElevation, _HighestGroundElevation + _TallestDamOrLevee, obj.Units))); 
                    }
                    break;
                default:
                    msgs.Add(IMessageFactory.Factory(IMessageLevels.Error, "The elevation is invalid. It is not set to an acceptable type."));
                    break;
            }
            return msgs;
        }
        private string NoticeMessage(ElevationBase obj) => $"The specified {obj.Print(true, true)} is outside the allowable range.";
        private string ExtendedMessage(ElevationBase obj, double min, double max, UnitsEnum units)
        {
            return $"The specified {obj.ParameterType} range: {obj.Range.Print(true)} exceeds the allowable approximate range of {AllowableRangeMessage(obj.ParameterType)}: " +
                $"[{UnitsUtilities.Print(UnitsUtilities.ConvertLengths(_LowestGroundElevation, UnitsEnum.Foot, units), units, true, false)}," +
                $" {UnitsUtilities.Print(UnitsUtilities.ConvertLengths(_HighestGroundElevation, UnitsEnum.Foot, units), units, true, false)}].";
        }
        private string AllowableRangeMessage(IParameterEnum type)
        {
            switch (type)
            {
                case IParameterEnum.GroundElevation:
                case IParameterEnum.ExteriorElevation:
                case IParameterEnum.InteriorElevation:
                    return "ground elevations on Earth";
                case IParameterEnum.AssetHeight:
                    return "reasonable height of assets above the ground";
                case IParameterEnum.AssetElevation:
                    return "reasonable elevation of assets (including ground elevations) on earth";
                case IParameterEnum.LateralStructureElevation:
                    return "ground elevations on earth plus a reasonable lateral structure height";
                default:
                    throw new NotSupportedException();
            }
        }

        public static bool IsConstructable(IParameterEnum type, IOrdinate value, UnitsEnum units, out string msg)
        {
            msg = "";
            if (!IParameterUtilities.IsElevation(type)) msg += $"The {nameof(ElevationOrdinate)} parameter cannot be constructed because it is marked as a {type} parameter, not a elevation parameter.";
            if (!UnitsUtilities.IsLength(units)) msg += $"The specified unit of measurement {units.ToString()} is invalid because it is not a supported measurement of length.";
            if (value.IsNull()) msg += $"The {nameof(ElevationOrdinate)} parameter cannot be constructed because the elevation {nameof(value)} input parameter is null.";
            if (value.Value().IsFinite()) msg += $"The {nameof(ElevationOrdinate)} parameter cannot be constructed because the elevation value is not a finite numerical value.";
            return msg.Length == 0;
        }
    }
}
