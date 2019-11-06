//using Functions;
//using Functions.CoordinatesFunctions;
//using Functions.Ordinates;
//using System;
//using System.Collections.Generic;
//using System.Collections.Immutable;
//using System.Diagnostics.CodeAnalysis;
//using System.Text;
//using Utilities;
//using Utilities.Validation;
//using Xunit;


//namespace FunctionsTests.CoordinatesFunctions
//{
//    [ExcludeFromCodeCoverage]
//    public class CoordinatesFunctionLinkedOrdinatesTests : CoordinateFunctionsTestData
//    {

//        //public static TheoryData<List<ICoordinatesFunction<IOrdinate, IOrdinate>>> Data
//        //{
//        //    get
//        //    {
//        //        var data = new TheoryData<ICoordinatesFunction<IOrdinate, IOrdinate>>();
//        //        data.Add()
//        //    }
//        //}
//        #region TestingConstructor
//        /// <summary>
//        /// Tests the construction of a LinkedCoordinatesFunction with 3 constant monotonic increasing functions
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_GoodData1_Returns_LinkedCoordinatesFunction()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_StrictMonotonicIncreasing_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.True(true);
//        }

//        /// <summary>
//        /// Tests the construction of a LinkedCoordinatesFunction with 3 constant non monotonic functions
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_GoodData2_Returns_LinkedCoordinatesFunction()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_NonMonotonic_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.True(true);
//        }

//        /// <summary>
//        /// Tests the construction of a LinkedCoordinatesFunction with functions: constant, distributed, constant
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_GoodData3_Returns_LinkedCoordinatesFunction()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_Constant_Distributed_Constant_NonMonotonic_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.True(true);
//        }
//        #endregion

//        #region Properties

//        #region Order
//        /// <summary>
//        /// Tests the Order property comes back as strict monotonic increasing
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Order_StrictMonotonicIncreasing_Returns_OrderSetEnum()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_StrictMonotonicIncreasing_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.True(func.Order == OrderedSetEnum.StrictlyIncreasing);
//        }


//        /// <summary>
//        /// Tests the Order property comes back as weak monotonic increasing
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Order_WeakMonotonicIncreasing_Returns_OrderSetEnum()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_WeakMonotonicIncreasing_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.True(func.Order == OrderedSetEnum.WeaklyIncreasing);
//        }


//        /// <summary>
//        /// Tests the Order property comes back as non monotonic
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Order_NonMonotonic_Returns_OrderSetEnum()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_NonMonotonic_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.True(func.Order == OrderedSetEnum.NonMonotonic);
//        }

//        /// <summary>
//        /// Tests the Order property comes back as strict monotonic decreasing
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Order_StrictMonotonicDecreasing_Returns_OrderSetEnum()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_StrictMonotonicDecreasing_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.True(func.Order == OrderedSetEnum.StrictlyDecreasing);
//        }

//        /// <summary>
//        /// Tests the Order property comes back as weak monotonic decreasing
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Order_WeakMonotonicDecreasing_Returns_OrderSetEnum()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_WeakMonotonicDecreasing_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.True(func.Order == OrderedSetEnum.WeaklyDecreasing);
//        }

//        #endregion

//        #region Interpolators

//        /// <summary>
//        /// Tests the interpolators property
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Interpolators_Returns_Interpolators()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_NonMonotonic_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.Equal(interpolators, func.Interpolators);
//        }

//        #endregion

//        #region FunctionsProperty
//        /// <summary>
//        /// Tests the functions property
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Functions_Returns_ListICoordinateFunction()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_NonMonotonic_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.Equal(functions, func.Functions);
//        }
//        #endregion

//        #region CoordinatesProperty

//        /// <summary>
//        /// Tests the coordinates property
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Coordinates_Returns_ListICoordinate()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_WeakMonotonicDecreasing_OrdinateFunctions();

//            List<ICoordinate<double, IOrdinate>> allCoords = new List<ICoordinate<double, IOrdinate>>();
//            foreach (ICoordinatesFunction<double, IOrdinate> function in functions)
//            {
//                allCoords.AddRange(function.Coordinates);
//            }

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.True(allCoords.Count > 0);
//            Assert.True(AreCoordinatesEqual(allCoords, func.Coordinates));
//        }

//        /// <summary>
//        /// Tests the coordinates property
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Coordinates_Returns_ListICoordinate_2()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = new List<ICoordinatesFunction<double, IOrdinate>>();

//            List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
//            List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

//            List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
//            List<double> ys2 = new List<double>() { 9, 10, 11, 12 };

//            List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
//            List<double> ys3 = new List<double>() { 13, 14, 15, 16 };
//            //create a constant func
//            CoordinatesFunctionConstants const1Func = CreateCoordinatesFunctionConstants(xs1, ys1);
//            CoordinatesFunctionConstants const2Func = CreateCoordinatesFunctionConstants(xs2, ys2);
//            CoordinatesFunctionConstants const3Func = CreateCoordinatesFunctionConstants(xs3, ys3);

//            //convert to iordinate
//            CoordinatesFunctionOrdinateYs const1OrdFunc = new CoordinatesFunctionOrdinateYs(const1Func);
//            CoordinatesFunctionOrdinateYs const2OrdFunc = new CoordinatesFunctionOrdinateYs(const2Func);
//            CoordinatesFunctionOrdinateYs const3OrdFunc = new CoordinatesFunctionOrdinateYs(const3Func);

//            //add to list of funcs
//            functions.Add(const1OrdFunc);
//            functions.Add(const2OrdFunc);
//            functions.Add(const3OrdFunc);


//            List<ICoordinate<double, IOrdinate>> allCoords = new List<ICoordinate<double, IOrdinate>>();
//            foreach (ICoordinatesFunction<double, IOrdinate> function in functions)
//            {
//                allCoords.AddRange(function.Coordinates);
//            }

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.True( AreCoordinatesEqual(allCoords, func.Coordinates));
//        }

//        #endregion

//        #region IsDistributed
//        ///// <summary>
//        ///// Tests the IsDistributed property. 3 functions with none being distributed.
//        ///// </summary>
//        //[Fact]
//        //public void LinkedCoordinatesFunction_IsDistributed_Returns_False()
//        //{
//        //    List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_NonMonotonic_OrdinateFunctions();

//        //    List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//        //    interpolators.Add(InterpolationEnum.Linear);
//        //    interpolators.Add(InterpolationEnum.Linear);

//        //    CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//        //    Assert.False(func.IsDistributed);
//        //}

//        ///// <summary>
//        ///// Tests the IsDistributed property. 3 functions with one being distributed.
//        ///// </summary>
//        //[Fact]
//        //public void LinkedCoordinatesFunction_IsDistributed_Returns_True()
//        //{
//        //    List<ICoordinatesFunction<double, IOrdinate>> functions = Create_Constant_Distributed_Constant_NonMonotonic_OrdinateFunctions();

//        //    List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//        //    interpolators.Add(InterpolationEnum.Linear);
//        //    interpolators.Add(InterpolationEnum.Linear);

//        //    CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//        //    Assert.True(func.IsDistributed);
//        //}
//        #endregion

//        #region Domain Propert

//        /// <summary>
//        /// Tests the Domain property. 3 functions with a min and max of 0,11.
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Domain_Returns_Tuple_1()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_Constant_Distributed_Constant_NonMonotonic_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.Equal(func.Domain, new Tuple<double, double>(0, 11));
//        }

//        /// <summary>
//        /// Tests the Domain property. 3 functions that all have a min and max of 0,3.
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Domain_Returns_Tuple_2()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_NonMonotonic_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.Equal(func.Domain, new Tuple<double, double>(0, 3));
//        }

//        #endregion

//        #region IsValid Property
//        //todo: add more tests to this once the validator is more fully developed.
//        /// <summary>
//        /// Tests the IsValid property. Should return false if interpolators are null.
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_IsValid_Returns_Bool_NullInterpolators()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_NonMonotonic_OrdinateFunctions();

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, null);
//            Assert.False(func.IsValid);
//        }

//        /// <summary>
//        /// Tests the IsValid property. Should throw an invalid constructor argument exception if the 
//        /// functions are null.
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_IsValid_Returns_Bool_NullFunctions()
//        {
//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            Assert.Throws<InvalidConstructorArgumentsException>(()=> new CoordinatesFunctionLinkedOrdinates(null, interpolators));
//        }


//        /// <summary>
//        /// Tests the IsValid Property. Should return false if the domains of the functions are overlapping.
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_IsValid_Returns_Bool_OverlappingDomains()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_NonMonotonic_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.False(func.IsValid);
//        }

//        /// <summary>
//        /// Tests the IsValid Property. Should return true if functions are not overlapping.
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_IsValid_Returns_Bool_AllGood()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_StrictMonotonicIncreasing_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.True(func.IsValid);
//        }
//        #endregion

//        #region Errors Property

//        //todo: add more tests to this once the validator is more fully developed.
//        /// <summary>
//        /// Tests the Errors property. Should return error if interpolators are null
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Errors_Returns_Bool_NullInterpolators()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_NonMonotonic_OrdinateFunctions();

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, null);
//            int count =0;
//            foreach(IMessage s in func.Errors)
//            {
//                count++;
//            }
//            Assert.True(count>0);
//        }

//        //todo: update this once you get the new messaging code.
//        /// <summary>
//        /// Tests the Error property. Should throw an invalid constructor argument exception if the 
//        /// functions are null.
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Errors_Returns_Bool_NullFunctions()
//        {
//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            Assert.Throws<InvalidConstructorArgumentsException>(() => new CoordinatesFunctionLinkedOrdinates(null, interpolators));
//        }


//        /// <summary>
//        /// Tests the Error Property. Should return error string if the domains of the functions are overlapping.
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Errors_Returns_Bool_OverlappingDomains()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_NonMonotonic_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            int count = 0;
//            foreach (IMessage s in func.Errors)
//            {
//                count++;
//            }
//            Assert.True(count > 0);
//        }

//        /// <summary>
//        /// Tests the Error Property. Should return no error strings if functions are not overlapping.
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Error_Returns_Bool_AllGood()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_StrictMonotonicIncreasing_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            int count = 0;
//            foreach (IMessage s in func.Errors)
//            {
//                count++;
//            }
//            Assert.True(count == 0);
//        }
//        #endregion

//        #endregion

//        #region Functions
//        //todo: write these unit tests
//        #region F(x)
//        /// <summary>
//        /// Tests that the F(x) function returns the correct value with x being within the domain 
//        /// of a non distributed function.
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Fx_Returns_Double()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_Constant_Distributed_Constant_NonMonotonic_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            //todo: John this is running into an issue because your F(x) function is trying to see if this x
//            //value is exactly on a coordinate, but in this case the 1 is getting turned into 1.000000000000015
//            Assert.True(func.F(1) == new Constant(6));
//        }

//        /// <summary>
//        /// Tests that the F(x) function returns the correct value with x being within the domain 
//        /// of a non distributed function.        
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_Fx_Returns_Double_2()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_Constant_Distributed_Constant_NonMonotonic_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            //todo: John this is running into an issue because your F(x) function is trying to see if this x
//            //value is exactly on a coordinate, but in this case the 1 is getting turned into 1.000000000000015
//            Assert.True(func.F(0) == new Constant(5));
//        }
//        #endregion

//        #region InverseF(x)
//        /// <summary>
//        /// Tests that the InverseF(x) function returns the correct value with y being within the range 
//        /// of a non distributed function.        
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_InverseFx_Returns_Double()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_StrictMonotonicIncreasing_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            //todo: John this is running into an issue because the inverseF doesn't evaluate the first coordinate. It starts at i+1 for some reason.
//            double xVal = func.InverseF(new Constant(5));
//            Assert.True(xVal == 0);
//        }

//        /// <summary>
//        /// Tests that the InverseF(x) function returns an InvalidOperationException if the function is not strictly increasing      
//        /// </summary>
//        [Fact]
//        public void LinkedCoordinatesFunction_InverseFx_Returns_Double_2()
//        {
//            List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_WeakMonotonicDecreasing_OrdinateFunctions();

//            List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//            interpolators.Add(InterpolationEnum.Linear);
//            interpolators.Add(InterpolationEnum.Linear);

//            CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//            Assert.Throws<InvalidOperationException>(()=> func.InverseF(new Constant(5)));
//        }

//        #endregion

//        #region Sample
//        ///// <summary>
//        ///// Tests that the InverseF(x) function returns an InvalidOperationException if the function is not strictly increasing      
//        ///// </summary>
//        //[Fact]
//        //public void LinkedCoordinatesFunction_Sample_Returns_ICoordinatesFunction()
//        //{
//        //    List<double> xs1 = new List<double>() { 0, 1, 2, 3 };
//        //    List<double> ys1 = new List<double>() { 5, 6, 7, 8 };

//        //    List<double> xs2 = new List<double>() { 4, 5, 6, 7 };
//        //    List<double> ys2 = new List<double>() { 9, 10, 11, 12 };

//        //    List<double> xs3 = new List<double>() { 8, 9, 10, 11 };
//        //    List<double> ys3 = new List<double>() { 13, 14, 15, 16 };
//        //    //create a constant func
//        //    CoordinatesFunctionConstants const1Func = CreateCoordinatesFunctionConstants(xs1, ys1);
//        //    CoordinatesFunctionConstants const2Func = CreateCoordinatesFunctionConstants(xs2, ys2);
//        //    CoordinatesFunctionConstants const3Func = CreateCoordinatesFunctionConstants(xs3, ys3);

//        //    List<ICoordinate<double, double>> originalCoords = new List<ICoordinate<double, double>>(const1Func.Coordinates); // new List<List<ICoordinate<double,double>>>(const1Func.Coordinates);
//        //    originalCoords.AddRange(const2Func.Coordinates);
//        //    originalCoords.AddRange(const3Func.Coordinates);

//        //    List<ICoordinatesFunction<double, IOrdinate>> functions = Create_3Constant_StrictMonotonicIncreasing_OrdinateFunctions();

//        //    List<InterpolationEnum> interpolators = new List<InterpolationEnum>();
//        //    interpolators.Add(InterpolationEnum.Linear);
//        //    interpolators.Add(InterpolationEnum.Linear);

//        //    CoordinatesFunctionLinkedOrdinates func = new CoordinatesFunctionLinkedOrdinates(functions, interpolators);
//        //    ICoordinatesFunction<double, double> sampleFunction = func.Sample(.5);
//        //    Assert.True(AreCoordinatesEqual( sampleFunction.Coordinates, originalCoords));
//        //}

        
//        #endregion

//        #endregion

//    }
//}
