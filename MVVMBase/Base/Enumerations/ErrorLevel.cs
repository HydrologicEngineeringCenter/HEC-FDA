using System;

namespace Base.Enumerations
{
    [Flags]
    public enum ErrorLevel: byte
    {
        Unassigned = 0X00,
        ErrorFree = 0x01,
        Info = 0x02,
        Minor = 0x04,
        Major = 0x08,
        Fatal = 0x10,
        
    }
}
