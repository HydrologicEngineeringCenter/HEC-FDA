using Functions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model.Utilities
{
    public static class WriteToConsole
    {
        public static void WriteCoordinatesToConsole(IFdaFunction function, String name)
        {
            Console.WriteLine("Writing coordinates for function: " + name);
            WriteCoordinatesToConsole(function);
        }

        public static void WriteCoordinatesToConsole(IFdaFunction function)
        {
            foreach (ICoordinate coord in function.Function.Coordinates)
            {
                Console.WriteLine(coord.X.Value() + "," + coord.Y.Value());
            }
        }
    }
}
