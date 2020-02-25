using System;
using System.Collections.Generic;
using System.Text;
using Functions;
using Utilities;

namespace Model.Elevations
{
    internal class Elevation : IElevation
    {
        public bool IsConstant { get; }
        public IElevationEnum Type { get; }
        public IOrdinate Height { get; }
        
        public Elevation(IElevationEnum type, IOrdinate height)
        {
            if (!Validation.ElevationValidator.IsConstructable(type, height, out string msg)) throw new InvalidConstructorArgumentsException(msg);
            else
            {
                IsConstant = height.Type == IOrdinateEnum.Constant ? true : false;
                Type = type;
                Height = height;
            }
        }
    }
}
