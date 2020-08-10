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
            if (obj.IsNull()) throw new ArgumentNullException(nameof(obj));
            if (!obj.ParameterType.IsElevation())
                msgs.Add(IMessageFactory.Factory(IMessageLevels.Error,
                        $"A elevation parameter was expected but a {obj.ParameterType.Print()} parameter was found causing an error."));
            if (!obj.Units.IsLength())
                msgs.Add(IMessageFactory.Factory(IMessageLevels.Error,
                        $"The specified units: {obj.Units.Print()} are not a valid measurement of elevation/length. " +
                        $"The default unit of measurement for this {obj.ParameterType.Print()} parameter are: {obj.ParameterType.DefaultUnits().Print()}."));
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
            return $"The specified {obj.ParameterType} range: {obj.Range.Print(true)} exceeds the allowable approximate range of {AllowableRangeMessage(obj)}.";
        }
        private string AllowableRangeMessage(ElevationBase obj)
        {
            switch (obj.ParameterType)
            {
                case IParameterEnum.GroundElevation:
                case IParameterEnum.ExteriorElevation:
                case IParameterEnum.InteriorElevation:
                    return $"ground elevations on Earth: {IRangeFactory.Factory(_LowestGroundElevation, _HighestGroundElevation).ConvertLenghts(obj.ParameterType.DefaultUnits(), obj.Units).Print(true)} {obj.Units.Print(true)}.";
                case IParameterEnum.AssetHeight:
                    return $"reasonable height of assets (such as buildings) above the ground elevation: {IRangeFactory.Factory(_LowestAssetHeight, _HighestAssetHeight).ConvertLenghts(obj.ParameterType.DefaultUnits(), obj.Units).Print(true)} {obj.Units.Print(true)}.";
                case IParameterEnum.AssetElevation:
                    return $"reasonable elevation of assets (such as buildings) on earth. This is based on the maximum and minimum elevation on earth: {IRangeFactory.Factory(_LowestGroundElevation, _HighestGroundElevation).ConvertLenghts(obj.ParameterType.DefaultUnits(), obj.Units).Print(true)} {obj.Units.Print(true)} plus a range of reasonable asset heights (above the ground elevation): {IRangeFactory.Factory(_LowestAssetHeight, _HighestAssetHeight).ConvertLenghts(obj.ParameterType.DefaultUnits(), obj.Units).Print(true)} {obj.Units.Print(true)}.";
                case IParameterEnum.LateralStructureElevation:
                    return $"ground elevations on earth plus a reasonable lateral structure height. This is based on the maximum and minimum elevation on earth: {IRangeFactory.Factory(_LowestGroundElevation, _HighestGroundElevation).ConvertLenghts(obj.ParameterType.DefaultUnits(), obj.Units).Print(true)} {obj.Units.Print(true)} plus a range of lateral structure heights (above the ground elevation): {IRangeFactory.Factory(0, _TallestDamOrLevee).ConvertLenghts(obj.ParameterType.DefaultUnits(), obj.Units).Print(true)} {obj.Units.Print(true)} with the maximum lateral structure elevation based on the Jinping-I arch dam located in China.";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
