using Functions;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FdaViewModel.Utilities
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
            foreach(ICoordinate coord in function.Function.Coordinates)
            {
                Console.WriteLine(coord.X.Value() + "," + coord.Y.Value());
            }
        }

    }
}
