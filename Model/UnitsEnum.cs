using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    /// <summary>
    /// Enumerates supports <see cref="Model"/> units of measurement.
    /// </summary>
    public enum UnitsEnum
    {
        /*
         * Unit Types:
         *  - Unit less values (-1 to -9)
         *  - Not Set - 0
         *  - Length, Distance or Height (+1 to +9)
         *  - Area (+11 to +19)
         *  - Volume (+21 to +29)
         *  - Time (+31 to +39)
         *  - Flow (+41 to + 49)
         *  - Dollars (+51 to +59)
         */
        /// <summary>
        /// No units.
        /// </summary>
        Unitless = -2,
        /// <summary>
        /// Probability [0, 1] scale. Default unit less assignment.
        /// </summary>
        Probability = -1,
        /// <summary>
        /// Default value, usually automatically reset during parameter construction.
        /// </summary>
        NotSet = 0,
        /// <summary>
        /// Feet. Default length unit.
        /// </summary>
        Foot = 1,
        /// <summary>
        /// Meters.
        /// </summary>
        Meters = 5,
        /// <summary>
        /// Square Foot. Default area unit.
        /// </summary>
        SquareFoot = 11,
        /// <summary>
        /// Square Meter.
        /// </summary>
        SquareMeter = 15,
        /// <summary>
        /// Cubic Foot. Default volume unit.
        /// </summary>
        CubicFoot = 21,
        /// <summary>
        /// Cubic Meter.
        /// </summary>
        CubicMeter = 25,
        /// <summary>
        /// Seconds. Default time unit
        /// </summary>
        Second = 31,
        /// <summary>
        /// Minutes.
        /// </summary>
        Minute = 32,
        /// <summary>
        /// Hours.
        /// </summary>
        Hour = 33,
        /// <summary>
        /// Days.
        /// </summary>
        Day = 34,
        /// <summary>
        /// Months.
        /// </summary>
        Month = 35,
        /// <summary>
        /// Years.
        /// </summary>
        Year = 36,
        /// <summary>
        /// Cubic feet per second. Default flow unit.
        /// </summary>
        CubicFootPerSecond = 41,
        /// <summary>
        /// Cubic meters per second.
        /// </summary>
        CubicMeterPerSecond = 45,
        /// <summary>
        /// US Dollars.
        /// </summary>
        Dollars = 51,
        /// <summary>
        /// 10s of US Dollars
        /// </summary>
        TenDollars = 52,
        /// <summary>
        /// 100s of US Dollars
        /// </summary>
        HundredDollars = 53,
        /// <summary>
        /// 1,000 of US Dollars
        /// </summary>
        ThousandDollars = 54,
        /// <summary>
        /// 10,000 of US Dollars
        /// </summary>
        TenThousandDollars = 55,
        /// <summary>
        /// 100,000 of US Dollars
        /// </summary>
        HundredThousandDollars = 56,
        /// <summary>
        /// 1,000,000 of US Dollars
        /// </summary>
        MillionDollars = 57,
        /// <summary>
        /// 10,000,000 of US Dollars
        /// </summary>
        TenMillionDollars = 58,
        /// <summary>
        /// 100,000,000 of US Dollars
        /// </summary>
        HundredMillionDollars = 59
    }
}
