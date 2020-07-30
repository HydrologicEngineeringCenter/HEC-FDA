using System;
using System.Collections.Generic;
using System.Text;

using Functions;

using Utilities;

namespace Model
{
    /// <summary>
    /// Utilities to assist in unit assignments and conversions.
    /// </summary>
    public static class UnitsUtilities
    {
        internal static IRange<int> ProbabilityRange => IRangeFactory.Factory(-1, -1);
        internal static IRange<int> LengthEnumRange => IRangeFactory.Factory(1, 9);
        internal static IRange<int> AreaEnumRange => IRangeFactory.Factory(11, 19);
        internal static IRange<int> VolumeEnumRange => IRangeFactory.Factory(21, 29);
        internal static IRange<int> TimeEnumRange => IRangeFactory.Factory(31, 39);
        internal static IRange<int> FlowEnumRange => IRangeFactory.Factory(41, 49);

        /// <summary>
        /// Tests if the <paramref name="units"/> represent unit of probability. 
        /// </summary>
        /// <param name="units"> The unit type to be tested. </param>
        /// <returns> <see langword="true"/> if the <paramref name="units"/> is a unit of measurement for probability, <see langword="false"/> otherwise. </returns>
        public static bool IsProbability(this UnitsEnum units) => (int)units == -1;
        /// <summary>
        /// Tests if the <paramref name="units"/> represent unit of length. 
        /// </summary>
        /// <param name="units"> The unit type to be tested. </param>
        /// <returns> <see langword="true"/> if the <paramref name="units"/> is a unit of measurement for length, <see langword="false"/> otherwise. </returns>
        public static bool IsLength(this UnitsEnum units) => LengthEnumRange.IsOnRange((int)units);
        /// <summary>
        /// Tests if the <paramref name="units"/> represent unit of area. 
        /// </summary>
        /// <param name="units"> The unit type to be tested. </param>
        /// <returns> <see langword="true"/> if the <paramref name="units"/> is a unit of measurement for area, <see langword="false"/> otherwise. </returns>
        public static bool IsArea(this UnitsEnum units) => AreaEnumRange.IsOnRange((int)units);
        /// <summary>
        /// Tests if the <paramref name="units"/> represent unit of volume. 
        /// </summary>
        /// <param name="units"> The unit type to be tested. </param>
        /// <returns> <see langword="true"/> if the <paramref name="units"/> is a unit of measurement for volume, <see langword="false"/> otherwise. </returns>
        public static bool IsVolume(this UnitsEnum units) => VolumeEnumRange.IsOnRange((int)units);
        /// <summary>
        /// Tests if the <paramref name="units"/> represent unit of time. 
        /// </summary>
        /// <param name="units"> The unit type to be tested. </param>
        /// <returns> <see langword="true"/> if the <paramref name="units"/> is a unit of measurement for time, <see langword="false"/> otherwise. </returns>
        public static bool IsTime(this UnitsEnum units) => TimeEnumRange.IsOnRange((int)units);
        /// <summary>
        /// Tests if the <paramref name="units"/> represent unit of flow. 
        /// </summary>
        /// <param name="units"> The unit type to be tested. </param>
        /// <returns> <see langword="true"/> if the <paramref name="units"/> is a unit of measurement for flow, <see langword="false"/> otherwise. </returns>
        public static bool IsFlow(this UnitsEnum units) => FlowEnumRange.IsOnRange((int)units);
        
        /// <summary>
        /// Converts between supported length units.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <param name="fromUnits"> The units of the <paramref name="value"/> parameter. </param>
        /// <param name="toUnits"> The desired units for the <paramref name="value"/> parameter after conversion. </param>
        /// <returns> The provided <paramref name="value"/> as a <see cref="double"/> precision in the requested <paramref name="toUnits"/>. </returns>
        public static double ConvertLengths(this double value, UnitsEnum fromUnits, UnitsEnum toUnits)
        {
            if (IsSameUnitsType(fromUnits, toUnits)) return UnitsNet.UnitConverter.Convert(value, ConvertLengthUnitEnum(fromUnits), ConvertLengthUnitEnum(toUnits));
            else throw new ArgumentException($"One or more the unit types in the requested length unit conversion from {fromUnits.ToString()} to {toUnits.ToString()} are not supported length unit types.");
        }
        /// <summary>
        /// Converts between supported area units.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <param name="fromUnits"> The units of the <paramref name="value"/> parameter. </param>
        /// <param name="toUnits"> The desired units for the <paramref name="value"/> parameter after conversion. </param>
        /// <returns> The provided <paramref name="value"/> as a <see cref="double"/> precision in the requested <paramref name="toUnits"/>. </returns>
        public static double ConvertAreas(this double value, UnitsEnum fromUnits, UnitsEnum toUnits)
        {
            if (IsSameUnitsType(fromUnits, toUnits)) return UnitsNet.UnitConverter.Convert(value, ConvertAreaUnitEnum(fromUnits), ConvertAreaUnitEnum(toUnits));
            else throw new ArgumentException($"One or more the unit types in the requested area unit conversion from {fromUnits.ToString()} to {toUnits.ToString()} are not supported area unit types.");
        }
        /// <summary>
        /// Converts between supported volume units.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <param name="fromUnits"> The units of the <paramref name="value"/> parameter. </param>
        /// <param name="toUnits"> The desired units for the <paramref name="value"/> parameter after conversion. </param>
        /// <returns> The provided <paramref name="value"/> as a <see cref="double"/> precision in the requested <paramref name="toUnits"/>. </returns>
        public static double ConvertVolumes(this double value, UnitsEnum fromUnits, UnitsEnum toUnits)
        {
            if (IsSameUnitsType(fromUnits, toUnits)) return UnitsNet.UnitConverter.Convert(value, ConvertVolumeUnitEnum(fromUnits), ConvertVolumeUnitEnum(toUnits));
            else throw new ArgumentException($"One or more the unit types in the requested volume unit conversion from {fromUnits.ToString()} to {toUnits.ToString()} are not supported volume unit types.");
        }
        /// <summary>
        /// Converts between supported flow units.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <param name="fromUnits"> The units of the <paramref name="value"/> parameter. </param>
        /// <param name="toUnits"> The desired units for the <paramref name="value"/> parameter after conversion. </param>
        /// <returns> The provided <paramref name="value"/> as a <see cref="double"/> precision in the requested <paramref name="toUnits"/>. </returns>
        public static double ConvertFlows(this double value, UnitsEnum fromUnits, UnitsEnum toUnits)
        {
            if (IsSameUnitsType(fromUnits, toUnits)) return UnitsNet.UnitConverter.Convert(value, ConvertFlowUnitEnum(fromUnits), ConvertFlowUnitEnum(toUnits));
            else throw new ArgumentException($"One or more the unit types in the requested flow unit conversion from {fromUnits.ToString()} to {toUnits.ToString()} are not supported flow unit types.");
        }
        public static IRange<double> ConvertLenghts(this IRange<double> range, UnitsEnum fromUnits, UnitsEnum toUnits)
        {
            if (IsSameUnitsType(fromUnits, toUnits))
            {
                return IRangeFactory.Factory(
                    UnitsNet.UnitConverter.Convert(range.Min, ConvertLengthUnitEnum(fromUnits), ConvertLengthUnitEnum(toUnits)),
                    UnitsNet.UnitConverter.Convert(range.Max, ConvertLengthUnitEnum(fromUnits), ConvertLengthUnitEnum(toUnits)));
            }
            else throw new ArgumentException($"One or more the unit types in the requested length unit conversion from {fromUnits.ToString()} to {toUnits.ToString()} are not supported length unit types.");
        }
        public static IRange<double> ConvertAreas(this IRange<double> range, UnitsEnum fromUnits, UnitsEnum toUnits)
        {
            if (IsSameUnitsType(fromUnits, toUnits))
            {
                return IRangeFactory.Factory(
                    UnitsNet.UnitConverter.Convert(range.Min, ConvertAreaUnitEnum(fromUnits), ConvertAreaUnitEnum(toUnits)),
                    UnitsNet.UnitConverter.Convert(range.Max, ConvertAreaUnitEnum(fromUnits), ConvertAreaUnitEnum(toUnits)));
            }
            else throw new ArgumentException($"One or more the unit types in the requested area unit conversion from {fromUnits.ToString()} to {toUnits.ToString()} are not supported area unit types.");
        }
        public static IRange<double> ConvertFlows(this IRange<double> range, UnitsEnum fromUnits, UnitsEnum toUnits)
        {
            if (IsSameUnitsType(fromUnits, toUnits))
            {
                return IRangeFactory.Factory(
                    UnitsNet.UnitConverter.Convert(range.Min, ConvertFlowUnitEnum(fromUnits), ConvertFlowUnitEnum(toUnits)),
                    UnitsNet.UnitConverter.Convert(range.Max, ConvertFlowUnitEnum(fromUnits), ConvertFlowUnitEnum(toUnits)));
            }
            else throw new ArgumentException($"One or more the unit types in the requested flow unit conversion from {fromUnits.ToString()} to {toUnits.ToString()} are not supported flow unit types.");
        }

        private static bool IsSameUnitsType(this UnitsEnum fromUnits, UnitsEnum toUnits)
        {
            int floor = (int)Math.Floor((int)fromUnits / 10.0) * 10;
            int ceiling = (int)Math.Ceiling((int)fromUnits / 10.0) * 10;
            return floor < (int)toUnits && (int)toUnits < ceiling;
        }
        private static UnitsNet.Units.LengthUnit ConvertLengthUnitEnum(UnitsEnum units)
        {
            switch (units)
            {
                case UnitsEnum.Foot:
                    return UnitsNet.Units.LengthUnit.Foot;
                case UnitsEnum.Meters:
                    return UnitsNet.Units.LengthUnit.Meter;
                default:
                    throw new NotImplementedException($"The {units.ToString()} is not a supported length unit.");
            }
        }
        private static UnitsNet.Units.AreaUnit ConvertAreaUnitEnum(UnitsEnum units)
        {
            switch (units)
            {
                case UnitsEnum.SquareFoot:
                    return UnitsNet.Units.AreaUnit.SquareFoot;
                case UnitsEnum.SquareMeter:
                    return UnitsNet.Units.AreaUnit.SquareMeter;
                default:
                    throw new NotImplementedException($"The {units.ToString()} is not a supported area unit.");
            }
        }
        private static UnitsNet.Units.VolumeUnit ConvertVolumeUnitEnum(UnitsEnum units)
        {
            switch (units)
            {
                case UnitsEnum.CubicFoot:
                    return UnitsNet.Units.VolumeUnit.CubicFoot;
                case UnitsEnum.SquareMeter:
                    return UnitsNet.Units.VolumeUnit.CubicMeter;
                default:
                    throw new NotImplementedException($"The {units.ToString()} is not a supported volume unit.");
            }
        }
        private static UnitsNet.Units.VolumeFlowUnit ConvertFlowUnitEnum(UnitsEnum units)
        {
            switch (units)
            {
                case UnitsEnum.CubicFootPerSecond:
                    return UnitsNet.Units.VolumeFlowUnit.CubicFootPerSecond;
                case UnitsEnum.SquareMeter:
                    return UnitsNet.Units.VolumeFlowUnit.CubicMeterPerSecond;
                default:
                    throw new NotImplementedException($"The {units.ToString()} is not a supported flow unit.");
            }
        }

        /// <summary>
        /// Prints a string representation of the <see cref="IParameter"/> value and its units.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameter"/> to be printed. </param>
        /// <param name="round"> <see langword="true"/> if some values should be rounded or displayed in scientific notation to produce a more human readable result. </param>
        /// <param name="abbreviate"> <see langword="true"/> if the unit name should be printed using the <a href="http://www.ieee.org/">IEEE</a> recommended abbreviation, <see langword="false"/> otherwise. </param>
        /// <returns> A string in the form... <see cref="IParameter"/> <see cref="IParameter"/>, where <see cref="IParameter"/> is formated as a string by the <see cref="IOrdinate.Print(bool)"/> function. </returns>
        public static string Print(IParameter parameter, bool round = false, bool abbreviate = false) => parameter.Print(round) + " " + Print(parameter.Units, abbreviate);
        /// <summary>
        /// Prints a string representation of the <see cref="IParameter"/> value and its units.
        /// </summary>
        /// <param name="parameter"> The <see cref="IParameter"/> value to be printed. </param>
        /// <param name="units"> The <paramref name="parameter"/> units. </param>
        /// <param name="round"> <see langword="true"/> if some values should be rounded or displayed in scientific notation to produce a more human readable result. </param>
        /// <param name="abbreviate"> <see langword="true"/> if the unit name should be printed using the <a href="http://www.ieee.org/">IEEE</a> recommended abbreviation, <see langword="false"/> otherwise. </param>
        /// <returns> A string in the form... <see cref="IParameter"/> <see cref="IParameter.Units"/>, where <see cref="IParameter"/> is formated as a string by the <see cref="IOrdinate.Print(bool)"/> function. </returns>
        public static string Print(double parameter, UnitsEnum units, bool round = false, bool abbreviate = false) => round ? parameter.Print() + " " + Print(units, abbreviate) : parameter.ToString() + " " + Print(units, abbreviate);
        /// <summary>
        /// Prints a string representation of the unit of measurement.
        /// </summary>
        /// <param name="units"> The <see cref="UnitsEnum"/> to be printed. </param>
        /// <param name="abbreviate"> <see langword="true"/> if the unit of measure should be abbreviated (e.g. ft instead of foot), <see langword="false"/> otherwise. </param>
        /// <returns> A string describing the <see cref="UnitsEnum"/>. </returns>
        public static string Print(this UnitsEnum units, bool abbreviate = false)
        {
            switch (units)
            {
                case UnitsEnum.Probability:
                    return "probability";
                case UnitsEnum.Foot:
                    if (abbreviate) return "ft";
                    else return "feet";
                case UnitsEnum.Meters:
                    if (abbreviate) return "m";
                    else return units.ToString().ToLower();
                case UnitsEnum.SquareFoot:
                    if (abbreviate) return "sq ft";
                    else return "square feet";
                case UnitsEnum.SquareMeter:
                    if (abbreviate) return "sq m";
                    else return "square meters";
                case UnitsEnum.CubicFoot:
                    if (abbreviate) return "cubic ft";
                    else return "cubic feet";
                case UnitsEnum.CubicMeter:
                    if (abbreviate) return "cubic m";
                    else return "cubic meters";
                case UnitsEnum.Second:
                    if (abbreviate) return "s";
                    else return units.ToString().ToLower();
                case UnitsEnum.Minute:
                    if (abbreviate) return "min";
                    else return units.ToString().ToLower();
                case UnitsEnum.Hour:
                    if (abbreviate) return "hr";
                    else return units.ToString().ToLower();
                case UnitsEnum.Day:
                    if (abbreviate) return "day";
                    else return units.ToString().ToLower();
                case UnitsEnum.Month:
                    if (abbreviate) return "mo";
                    else return units.ToString().ToLower();
                case UnitsEnum.Year:
                    if (abbreviate) return "yr";
                    else return units.ToString().ToLower();
                case UnitsEnum.CubicFootPerSecond:
                    if (abbreviate) return "cfs";
                    else return "cubic feet per second";
                case UnitsEnum.CubicMeterPerSecond:
                    if (abbreviate) return "cms";
                    else return "cubic meters per second";
                case UnitsEnum.Dollars:
                    if (abbreviate) return "USD";
                    else return "US dollars";
                case UnitsEnum.Unitless:
                    return "unit less";
                default:
                    throw new NotImplementedException($"The {units.ToString()} is not a supported length unit.");
            }
        }
    }
}
