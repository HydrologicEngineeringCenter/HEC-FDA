using System;

namespace HEC.MVVMFramework.Base.Enumerations
{
    [Flags]
    public enum ErrorLevel : byte
    {
        Unassigned = 0X00,
        Info = 0x01,
        Minor = 0x02,
        Major = 0x04,
        Fatal = 0x20,
        Severe = 0x40,

    }
}
