using Functions;
using Functions.Coordinates;
using Functions.CoordinatesFunctions;
using Statistics;
using Statistics.Distributions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Xunit;

namespace FunctionsTests.CoordinatesFunctions
{
    [ExcludeFromCodeCoverage]
    public class CoordinateFunctionsTestData
    {

        #region ConstantData
        #region GoodData
        public static TheoryData<List<ICoordinate>> GoodData_Constant =>
            new TheoryData<List<ICoordinate>>
            {
                { ConstantCoordinates(new double[]{0}, new double[]{0})},
                {  ConstantCoordinates(new double[]{0,1}, new double[]{0,0})},
                {  ConstantCoordinates(new double[]{Double.MaxValue,0}, new double[]{0,Double.MinValue})},
                {  ConstantCoordinates(new double[]{Double.MinValue,0}, new double[]{0,Double.MaxValue})},
            };
        #endregion
        #region BadData
        public static TheoryData<List<ICoordinate>> BadData_Constant_Nan =>
            new TheoryData<List<ICoordinate>>
            {
                { ConstantCoordinates(new double[]{double.NaN}, new double[]{0})},
            };
        public static TheoryData<List<ICoordinate>> BadData_Constant_PositiveInfinity =>
            new TheoryData<List<ICoordinate>>
            {
                { ConstantCoordinates(new double[]{double.PositiveInfinity}, new double[]{0})},
            };
        public static TheoryData<List<ICoordinate>> BadData_Constant_NegativeInfinity =>
           new TheoryData<List<ICoordinate>>
           {
                { ConstantCoordinates(new double[]{double.NegativeInfinity}, new double[]{0})},
           };
        public static TheoryData<List<ICoordinate>> BadData_Constant_RepeatXs =>
           new TheoryData<List<ICoordinate>>
           {
                { ConstantCoordinates(new double[]{1,1}, new double[]{10,11})},
                { ConstantCoordinates(new double[]{1,2,3,3,4}, new double[]{20,10,9,8,7})}

           };
        #endregion
        #region Functions
        internal List<ICoordinatesFunction> GoodFunctions_Constant()
        {
            List<ICoordinatesFunction> funcs = new List<ICoordinatesFunction>();
            CoordinatesFunctionConstants func1 =
                new CoordinatesFunctionConstants(ConstantCoordinates(new double[] { 0, 1, 2, 3, 4 }, new double[] { 0, 1, 2, 3, 4 }));
            funcs.Add(func1);
            return funcs;
        }

        internal CoordinatesFunctionConstants CreateCoordinatesFunctionConstantsBasic()
        {
            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<double> ys = new List<double>() { 10, 11, 12, 13 };
            return (CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(xs, ys);
        }

        internal CoordinatesFunctionConstants CreateCoordinatesFunctionConstantsBasic(InterpolationEnum interpolator)
        {
            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<double> ys = new List<double>() { 10, 11, 12, 13 };
            return (CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(xs, ys, interpolator);
        }

        internal CoordinatesFunctionConstants CreateCoordinatesFunctionConstants(List<double> xs, List<double> ys)
        {
            return (CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(xs, ys);
        }
        internal CoordinatesFunctionConstants CreateCoordinatesFunctionConstants(List<double> xs, List<double> ys, InterpolationEnum interpolator)
        {
            return (CoordinatesFunctionConstants)ICoordinatesFunctionsFactory.Factory(xs, ys, interpolator);
        }

        #endregion
        private static List<ICoordinate> ConstantCoordinates(double[] xs, double[] ys)
        {
            if (xs.Length != ys.Length)
            {
                throw new ArgumentException("xValues were a different length than the yValues.");
            }

            List<ICoordinate> coords = new List<ICoordinate>();
            for (int i = 0; i < xs.Length; i++)
            {
                coords.Add(ICoordinateFactory.Factory(xs[i], ys[i]));
            }

            return new List<ICoordinate>(coords.ToArray());
        }

        #endregion

        #region DistributedData

        #region GoodData
        public static TheoryData<List<ICoordinate>> GoodDataDistributed =>
           new TheoryData<List<ICoordinate>>
           {
                { DistributedCoordinates(new double[]{0}, 
                    new IDistributedOrdinate[] {
                    IDistributedOrdinateFactory.Factory(new Normal(1,0))
                })},
                { DistributedCoordinates(new double[]{0, 1}, new IDistributedOrdinate[]{
                    IDistributedOrdinateFactory.Factory(new Normal(1,0)),
                    IDistributedOrdinateFactory.Factory(new Normal(1,1))})},
                { DistributedCoordinates(new double[]{0, 1}, new IDistributedOrdinate[]{
                    IDistributedOrdinateFactory.Factory(new Triangular(1,2,3)),
                    IDistributedOrdinateFactory.Factory(new Triangular(3,4,5))})},
                { DistributedCoordinates(new double[]{0, 1}, new IDistributedOrdinate[]{
                    IDistributedOrdinateFactory.Factory(new Uniform(1,2)),
                    IDistributedOrdinateFactory.Factory(new Uniform(3,4))})},
                { DistributedCoordinates(new double[]{0, 1,2,3},
                   new IDistributedOrdinate[]
                   {
                       IDistributedOrdinateFactory.Factory(new Normal(1,2)),
                       IDistributedOrdinateFactory.Factory(new Triangular(3,4,5)),
                       IDistributedOrdinateFactory.Factory(new Uniform(5,6)),
                       IDistributedOrdinateFactory.Factory(new Normal(0,1))
                   })
                },


           };

        #endregion
        #region BadData
        public static TheoryData<List<ICoordinate>> BadDataDistributed_RepeatXs =>
            new TheoryData<List<ICoordinate>>
            {
                { DistributedCoordinates(new double[]{1,1},new IDistributedOrdinate[]{ IDistributedOrdinateFactory.Factory( new Normal(0,1)), IDistributedOrdinateFactory.Factory(new Normal(0,1)) }) },
                //{ new List<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), new ScalarDistributed(new Normal()), map)) },
                //{ new List<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map)) },
                //{ new List<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarHistogram(), map), new UnivariateCoordinate(new ScalarHistogram(), IScalarFactory.Factory(0), map)) },
                //{ new List<ICoordinate<IOrdinate, IOrdinate>>(new UnivariateCoordinate(new ScalarDistributed(new Normal()), IScalarFactory.Factory(0), map), new UnivariateCoordinate(IScalarFactory.Factory(0), new ScalarDistributed(new Normal()), map)) }
            };
        #endregion

        #region Functions
        internal CoordinatesFunctionVariableYs CreateDistributedCoordinatesFunctionBasic()
        {
            List<double> xs = new List<double>() { 0, 1, 2, 3 };
            List<IDistributedOrdinate> ys = new List<IDistributedOrdinate>() 
            { 
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)) 
            };
            return (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.None);
        }

        internal CoordinatesFunctionVariableYs CreateDistributedCoordinatesFunction(List<double> xs, List<IDistributedOrdinate> ys)
        {
            return (CoordinatesFunctionVariableYs)ICoordinatesFunctionsFactory.Factory(xs, ys, InterpolationEnum.None);
        }
        #endregion

        internal static List<ICoordinate> DistributedCoordinates(double[] xs, IDistributedOrdinate[] ys)
        {
            if (xs.Length != ys.Length)
            {
                throw new ArgumentException("xValues were a different length than the yValues.");
            }

            List<ICoordinate> coords = new List<ICoordinate>();
            for (int i = 0; i < xs.Length; i++)
            {
                coords.Add(ICoordinateFactory.Factory(xs[i], ys[i]));
            }

            return new List<ICoordinate>(coords.ToArray());
        }

        #endregion

        #region OrdinateYs

        internal List<ICoordinatesFunction> Create_3Constant_StrictMonotonicIncreasing_OrdinateFunctions()
        {
            List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
            List<double> ys2 = new List<double>() { 9, 10, 11, 12 };

            List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
            List<double> ys3 = new List<double>() { 13, 14, 15, 16 };
            //create a constant func
            functions.Add(CreateCoordinatesFunctionConstants(xs1, ys1));
            functions.Add(CreateCoordinatesFunctionConstants(xs2, ys2));
            functions.Add(CreateCoordinatesFunctionConstants(xs3, ys3));

            return functions;
        }

        internal List<ICoordinatesFunction> Create_3Constant_StrictlyIncreasing_OverlappingXs_OrdinateFunctions()
        {
            List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs2 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys2 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs3 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys3 = new List<double>() { 5, 6, 7, 8 };

            //create a constant func
            functions.Add(CreateCoordinatesFunctionConstants(xs1, ys1));
            functions.Add(CreateCoordinatesFunctionConstants(xs2, ys2));
            functions.Add(CreateCoordinatesFunctionConstants(xs3, ys3));

            return functions;
        }

        internal List<ICoordinatesFunction> Create_3Constant_StrictlyIncreasing_NonOverlappingXs_OrdinateFunctions()
        {
            List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs2 = new List<double>() { 4,5,6,7 };
            List<double> ys2 = new List<double>() { 9,10,11,12 };

            List<double> xs3 = new List<double>() { 8,9,10,11 };
            List<double> ys3 = new List<double>() { 13,14,15,16 };

            //create a constant func
            functions.Add(CreateCoordinatesFunctionConstants(xs1, ys1));
            functions.Add(CreateCoordinatesFunctionConstants(xs2, ys2));
            functions.Add(CreateCoordinatesFunctionConstants(xs3, ys3));

            return functions;
        }

        internal List<ICoordinatesFunction> Create_3Constant_WeakMonotonicIncreasing_OrdinateFunctions()
        {
            List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
            List<double> ys2 = new List<double>() { 8, 10, 11, 12 };

            List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
            List<double> ys3 = new List<double>() { 12, 14, 15, 16 };

            //create a constant func
            functions.Add(CreateCoordinatesFunctionConstants(xs1, ys1));
            functions.Add(CreateCoordinatesFunctionConstants(xs2, ys2));
            functions.Add(CreateCoordinatesFunctionConstants(xs3, ys3));

            return functions;
        }

        internal List<ICoordinatesFunction> Create_3Constant_StrictMonotonicDecreasing_OrdinateFunctions()
        {
            List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 20, 19, 18, 17 };

            List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
            List<double> ys2 = new List<double>() { 16, 15, 14, 13 };

            List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
            List<double> ys3 = new List<double>() { 12, 11, 10, 9 };

            //create a constant func
            functions.Add(CreateCoordinatesFunctionConstants(xs1, ys1));
            functions.Add(CreateCoordinatesFunctionConstants(xs2, ys2));
            functions.Add(CreateCoordinatesFunctionConstants(xs3, ys3));

            return functions;
        }
        internal List<ICoordinatesFunction> Create_3Constant_WeakMonotonicDecreasing_OrdinateFunctions()
        {
            List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 20, 19, 18, 17 };

            List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
            List<double> ys2 = new List<double>() { 17, 15, 14, 13 };

            List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
            List<double> ys3 = new List<double>() { 12, 11, 10, 9 };

            //create a constant func
            functions.Add(CreateCoordinatesFunctionConstants(xs1, ys1));
            functions.Add(CreateCoordinatesFunctionConstants(xs2, ys2));
            functions.Add(CreateCoordinatesFunctionConstants(xs3, ys3));

            return functions;
        }

        internal List<ICoordinatesFunction> Create_Constant_Distributed_Constant_NonMonotonic_OrdinateFunctions()
        {
            List<ICoordinatesFunction> functions = new List<ICoordinatesFunction>();

            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
            List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

            List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
            List<IDistributedOrdinate> ys2 = new List<IDistributedOrdinate>() 
            { 
                IDistributedOrdinateFactory.Factory(new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)),
                IDistributedOrdinateFactory.Factory( new Normal(1, 0)) 
            };

            List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
            List<double> ys3 = new List<double>() { 13, 14, 15, 16 };

            //create a constant func
            functions.Add(CreateCoordinatesFunctionConstants(xs1, ys1));
            functions.Add(ICoordinatesFunctionsFactory.Factory(xs2, ys2, InterpolationEnum.None));
            functions.Add(CreateCoordinatesFunctionConstants(xs3, ys3));

            return functions;
        }


        #endregion

        public static bool AreCoordinatesEqual(List<ICoordinate> lista, List<ICoordinate> listb)
        {
            if (lista.Count != listb.Count)
            {
                return false;
            }
            else
            {
                for (int i = 0; i < lista.Count; i++)//(ICoordinate coord in lista)
                {
                    if (lista[i].X != listb[i].X || lista[i].Y.Value() != listb[i].Y.Value())
                    {
                        return false;
                    }
                }
            }
            return true;
        }

       

    }
}
