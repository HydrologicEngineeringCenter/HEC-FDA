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
        internal static IRange<int> DollarsEnumRange => IRangeFactory.Factory(51, 59);

        /// <summary>
        /// Checks if a <see cref="UnitsEnum"/> represents an accepted imperial unit of measurement.
        /// </summary>
        /// <param name="unit"> The <see cref="UnitsEnum"/> to be evaluated. </param>
        /// <returns> <see langword="true"/> if the unit of measure is an imperial unit of measurement, <see langword="false"/> otherwise. </returns>
        /// <remarks> The seconds, hours, days, months and years are accepted units of measurement but only seconds are part of the imperial system. All time units return <see langword="true"/>. </remarks>
        public static bool IsImperialUnit(this UnitsEnum unit)
        {
            /* Imperial Units:
             *  1. Length, Area, Volume and Flow imperial units have 1 to 5 in ones place.
             *  2. Time all units are imperial (and SI)
             *  3. Currency no units are imperial (or SI) 
             */
            if (unit.IsLength() || unit.IsArea() || unit.IsVolume() || unit.IsFlow()
                && (int)unit % 10 < 5
                || unit.IsTime()) return true;
            else return false;
        }
        /// <summary>
        /// Checks if the <see cref="UnitsEnum"/> represents an International System (aka SI) unit of measurement.
        /// </summary>
        /// <param name="unit"> The <see cref="UnitsEnum"/> to be evaluated. </param>
        /// <returns> <see langword="true"/> if the unit of measurement is an SI unit, <see langword="false"/> otherwise. </returns>
        /// <remarks> The seconds, hours, days, months and years are accepted units of measurement but only seconds are part of the SI. All time units return <see langword="true"/>. </remarks>
        public static bool IsInternationalStandardUnit(this UnitsEnum unit)
        {
            /* SI Units:
             *  1. Length, Area, Volume and Flow SI units have 5 to 9 in ones place.
             *  2. Time all units are SI (and imperial)
             *  3. Currency no units are SI (or imperial) 
             */
            if (unit.IsLength() || unit.IsArea() || unit.IsVolume() || unit.IsFlow()
                && (int)unit % 10 >= 5) return true;
            else return false;
        }

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
        /// Tests if the <paramref name="units"/> represent unit of dollars. 
        /// </summary>
        /// <param name="units"> The unit type to be tested. </param>
        /// <returns> <see langword="true"/> if the <paramref name="units"/> is a unit of measurement of dollars, <see langword="false"/> otherwise. </returns>
        public static bool IsDollars(this UnitsEnum units) => DollarsEnumRange.IsOnRange((int)units);
        
        /// <summary>
        /// Converts between supported length units.
        /// </summary>
        /// <param name="value"> The value to convert. </param>
        /// <param name="fromUnits"> The units of the <paramref name="value"/> parameter. </param>
        /// <param name="toUnits"> The desired units for the <paramref name="value"/> parameter after conversion. </param>
        /// <returns> The provided <paramref name="value"/> as a <see cref="double"/> precision value in the requested <paramref name="toUnits"/>. </returns>
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
        /// <returns> The provided <paramref name="value"/> as a <see cref="double"/> precision value in the requested <paramref name="toUnits"/>. </returns>
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
        /// <returns> The provided <paramref name="value"/> as a <see cref="double"/> precision value in the requested <paramref name="toUnits"/>. </returns>
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
        /// <returns> The provided <paramref name="value"/> as a <see cref="double"/> precision value in the requested <paramref name="toUnits"/>. </returns>
        public static double ConvertFlows(this double value, UnitsEnum fromUnits, UnitsEnum toUnits)
        {
            if (IsSameUnitsType(fromUnits, toUnits)) return UnitsNet.UnitConverter.Convert(value, ConvertFlowUnitEnum(fromUnits), ConvertFlowUnitEnum(toUnits));
            else throw new ArgumentException($"One or more the unit types in the requested flow unit conversion from {fromUnits.ToString()} to {toUnits.ToString()} are not supported flow unit types.");
        }
        /// <summary>
        /// Converts between supported measurements of dollars (orders of magnitude measurements)
        /// </summary>
        /// <param name="value"> The value to be converted. </param>
        /// <param name="fromUnits"> The units of the <paramref name="value"/> parameter. </param>
        /// <param name="toUnits"> The desired units for the <paramref name="value"/> parameter after conversion. </param>
        /// <returns> The provided <paramref name="value"/> as a <see cref="double"/> precision value in the requested <paramref name="toUnits"/>. </returns>
        public static double ConvertDollars(this double value, UnitsEnum fromUnits, UnitsEnum toUnits)
        {
            if (IsSameUnitsType(fromUnits, toUnits))
            {
                /* Shifts between order of magnitude dollar units
                 *  Examples...
                 *      1. Convert $1,000 (ones) to $1 thousand
                 *          - fromUnits: 51, toUnits: 54 
                 *          - therefore nOrder: (51 - 54) = -3 
                 *          - return: 1,000 * 10^-3 = 1
                 *      2. Convert $1 thousand to $1,000
                 *          - fromUnits: 54, toUnits: 51
                 *          - therefore nOrder: (54 - 51) = +3
                 *          - return: 1 * 10^3 = 1,000
                 */
                int nOrders = (int)fromUnits - (int)fromUnits;
                return value * Math.Pow(10, nOrders);
            }
            else throw new ArgumentException($"One or more the unit types in the requested dollar unit conversion from {fromUnits.ToString()} to {toUnits.ToString()} are not supported dollar unit types.");
        }
        
        /// <summary>
        /// Converts between supported length units.
        /// </summary>
        /// <param name="range"> The <see cref="IRange{T}"/> to convert. </param>
        /// <param name="fromUnits"> The units of the <paramref name="range"/> parameter. </param>
        /// <param name="toUnits"> The desired units for the <paramref name="range"/> parameter after conversion. </param>
        /// <returns> The provided <paramref name="range"/> as a new <see cref="IRange{T}"/> in the requested <paramref name="toUnits"/>. </returns>
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
        /// <summary>
        /// Converts between supported area units.
        /// </summary>
        /// <param name="range"> The <see cref="IRange{T}"/> to convert. </param>
        /// <param name="fromUnits"> The units of the <paramref name="range"/> parameter. </param>
        /// <param name="toUnits"> The desired units for the <paramref name="range"/> parameter after conversion. </param>
        /// <returns> The provided <paramref name="range"/> as a new <see cref="IRange{T}"/> in the requested <paramref name="toUnits"/>. </returns>
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
        /// <summary>
        /// Converts between supported length units.
        /// </summary>
        /// <param name="range"> The <see cref="IRange{T}"/> to convert. </param>
        /// <param name="fromUnits"> The units of the <paramref name="range"/> parameter. </param>
        /// <param name="toUnits"> The desired units for the <paramref name="range"/> parameter after conversion. </param>
        /// <returns> The provided <paramref name="range"/> as a new <see cref="IRange{T}"/> in the requested <paramref name="toUnits"/>. </returns>
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
        /// <summary>
        /// Convert between supported dollar units.
        /// </summary>
        /// <param name="range"> The <see cref="IRange{T}"/> to convert. </param>
        /// <param name="fromUnits"> The units of the <paramref name="range"/> parameter. </param>
        /// <param name="toUnits"> The desired units for the <paramref name="range"/> parameter after conversion. </param>
        /// <returns> The provided <paramref name="range"/> as a new <see cref="IRange{T}"/> in the requested <paramref name="toUnits"/>. </returns>
        public static IRange<double> ConvertDollars(this IRange<double> range, UnitsEnum fromUnits, UnitsEnum toUnits)
        {
            if (IsSameUnitsType(fromUnits, toUnits))
            {
                return IRangeFactory.Factory(
                    ConvertDollars(range.Min, fromUnits, toUnits),
                    ConvertDollars(range.Max, fromUnits, toUnits));
            }
            else throw new ArgumentException($"One or more the unit types in the requested dollar unit conversion from {fromUnits.ToString()} to {toUnits.ToString()} are not supported dollar unit types.");
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
                case UnitsEnum.TenDollars:
                    if (abbreviate) return "USD ('0)";
                    else return "tens of US dollars";
                case UnitsEnum.HundredDollars:
                    if (abbreviate) return "USD ('00)";
                    else return "hundreds of US dollars";
                case UnitsEnum.ThousandDollars:
                    if (abbreviate) return "USD ('000)";
                    else return "thousands of US dollars";
                case UnitsEnum.TenThousandDollars:
                    if (abbreviate) return "USD ('0,000)";
                    else return "tens of thousands of US dollars";
                case UnitsEnum.HundredThousandDollars:
                    if (abbreviate) return "USD ('00,000)";
                    else return "hundreds of thousands of US dollars";
                case UnitsEnum.MillionDollars:
                    if (abbreviate) return "USD ('000,000)";
                    else return "millions of US dollars";
                case UnitsEnum.TenMillionDollars:
                    if (abbreviate) return "USD (0,000,000)";
                    else return "tens of millions of US dollars";
                case UnitsEnum.HundredMillionDollars:
                    if (abbreviate) return "USD (00,000,000)";
                    else return "hundreds of millions of US dollars";
                
                case UnitsEnum.Unitless:
                    return "unit less";
                default:
                    throw new NotImplementedException($"The {units.ToString()} is not a supported length unit.");
            }
        }
    }
}
