using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionsTests.TestData
{
    public static class DefaultFunctionData
    {

        public static List<double> BaseXs_1_10_By1s
        {
            get
            {
                return new List<double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            }
        }

        public static List<double> BaseYs_1_10_By1s
        {
            get
            {
                return new List<double>() { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            }
        }

        public static List<double> BaseXs_1_10
        {
            get
            {
                return new List<double>() { 1, 10 };
            }
        }

        public static List<double> BaseYs_1_10
        {
            get
            {
                return new List<double>() { 1, 10 };
            }
        }

        public static List<double> AboveBaseXs_20_29_By1s
        {
            get
            {
                return new List<double>() { 20,21,22,23,24,25,26,27,28,29 };
            }
        }

        public static List<double> AboveBaseYs_20_29_By1s
        {
            get
            {
                return new List<double>() { 20, 21, 22, 23, 24, 25, 26, 27, 28, 29 };
            }
        }

        public static List<double> AboveBaseXs_20_29
        {
            get
            {
                return new List<double>() { 20,  29 };
            }
        }

        public static List<double> AboveBaseYs_20_29
        {
            get
            {
                return new List<double>() { 20,  29 };
            }
        }

        public static List<double> BelowBaseXs_Neg29_Neg20_By1s
        {
            get
            {
                return new List<double>() { -29, -28, -27, -26, -25, -24, -23, -22, -21, -20 };
            }
        }

        public static List<double> BelowBaseYs_Neg29_Neg20_By1s
        {
            get
            {
                return new List<double>() { -29, -28, -27, -26, -25, -24, -23, -22, -21, -20 };
            }
        }

        public static List<double> BelowBaseXs_Neg29_Neg20
        {
            get
            {
                return new List<double>() { -29, -20 };
            }
        }

        public static List<double> BelowBaseYs_Neg29_Neg20
        {
            get
            {
                return new List<double>() { -29, -20 };
            }
        }


        public static List<double> InsideBaseXs_3_7_By1s
        {
            get
            {
                return new List<double>() { 3,4,5,6,7 };
            }
        }

        public static List<double> InsideBaseYs_3_7_By1s
        {
            get
            {
                return new List<double>() { 3, 4, 5, 6, 7 };
            }
        }

        public static List<double> InsideBaseXs_3_7
        {
            get
            {
                return new List<double>() { 3,  7 };
            }
        }

        public static List<double> InsideBaseYs_3_7
        {
            get
            {
                return new List<double>() { 3, 7 };
            }
        }

        public static List<double> ContainsBaseXs_Neg3_13_By1s
        {
            get
            {
                return new List<double>() { -3, -2, -1, 0,1,2,3, 4, 5, 6, 7,8,9,10,11,12,13 };
            }
        }
        public static List<double> ContainsBaseYs_Neg3_13_By1s
        {
            get
            {
                return new List<double>() { -3, -2, -1, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            }
        }

        public static List<double> ContainsBaseXs_Neg3_13
        {
            get
            {
                return new List<double>() { -3,  13 };
            }
        }
        public static List<double> ContainsBaseYs_Neg3_13
        {
            get
            {
                return new List<double>() { -3,  13 };
            }
        }

        public static List<double> AboveAndOverlappingBaseXs_7_15_By1s
        {
            get
            {
                return new List<double>() {  7, 8, 9, 10, 11, 12, 13,14,15 };
            }
        }

        public static List<double> AboveAndOverlappingBaseYs_7_15_By1s
        {
            get
            {
                return new List<double>() { 7, 8, 9, 10, 11, 12, 13, 14, 15 };
            }
        }

        public static List<double> AboveAndOverlappingBaseXs_7_15
        {
            get
            {
                return new List<double>() { 7, 15 };
            }
        }

        public static List<double> AboveAndOverlappingBaseYs_7_15
        {
            get
            {
                return new List<double>() { 7, 15 };
            }
        }

        public static List<double> BelowAndOverlappingBaseXs_Neg3_5_By1s
        {
            get
            {
                return new List<double>() { -3, -2, -1, 0, 1, 2, 3, 4, 5 };
            }
        }

        public static List<double> BelowAndOverlappingBaseYs_Neg3_5_By1s
        {
            get
            {
                return new List<double>() { -3, -2, -1, 0, 1, 2, 3, 4, 5 };
            }
        }

        public static List<double> BelowAndOverlappingBaseXs_Neg3_5
        {
            get
            {
                return new List<double>() { -3, 5 };
            }
        }

        public static List<double> BelowAndOverlappingBaseYs_Neg3_5
        {
            get
            {
                return new List<double>() { -3,  5 };
            }
        }

    }
}
