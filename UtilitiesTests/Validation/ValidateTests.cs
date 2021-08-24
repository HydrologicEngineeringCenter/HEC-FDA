using System;
using System.Collections.Generic;
using Utilities;
using Xunit;

namespace UtilitiesTests
{
    /// <summary>
    /// Tests the <see cref="Utilities.Validation.Validate"/> class in the <see cref="Utilities"/> project.
    /// </summary>
    public class ValidationExtensionsTests
    {
        #region IsNull()
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsNull(object)"/> function returns <see langword="true"/> for a <see langword="null"/> input <see cref="object"/>.
        /// </summary>
        [Fact]
        public void IsNull_NullInput_Returns_True()
        {
            object obj = null;
            Assert.True(obj.IsNull());
        }
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsNull(object)"/> function returns <see langword="false"/> for a input <see cref="object"/> set to 1 (which is implicitly converted to an <see cref="int"/>).
        /// </summary>
        [Fact]
        public void IsNull_ImplicitIntObjectInput_Returns_False()
        {
            object obj = 1;
            Assert.False(obj.IsNull());
        }
        #endregion
        #region IsFinite()
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsFinite(double)"/> function return <see langword="true"/> for a <see cref="double"/> input set to 1.0.
        /// </summary>
        [Fact]
        public void IsFinite_FiniteInputDouble_Returns_True()
        {
            double x = 1.0;
            Assert.True(x.IsFinite());
        }
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsFinite(double)"/> function returns <see langword="false"/> for a <see cref="double.NaN"/>, <see cref="double.NegativeInfinity"/>, or <see cref="double.PositiveInfinity"/> input value.
        /// </summary>
        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.NegativeInfinity)]
        [InlineData(double.PositiveInfinity)]
        public void IsFinite_NaNOrInfinityInputDouble_Returns_False(double x)
        {
            Assert.False(x.IsFinite());
        }
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsFinite(IRange{double})"/> function returns <see langword="true"/> for a finite range: [0, 1].
        /// </summary>
        [Theory]
        [InlineData(0d, 1d, true, true, true, true)]
        [InlineData(0d, 1d, true, true, false, true)]
        public void IsFinite_FiniteInputRangeDouble_Returns_True(double min, double max, bool inclusiveMin, bool inclusveMax, bool finiteReq, bool notSingleValueReq)
        {
            var testObj = IRangeFactory.Factory(min, max, inclusiveMin, inclusveMax, finiteReq, notSingleValueReq);
            Assert.True(testObj.IsFinite());
        }
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsFinite(IRange{double})"/> function returns <see langword="false"/> for an <see cref="IRange{double}"/> containing <see cref="double.NaN"/>, <see cref="double.NegativeInfinity"/>, or <see cref="double.PositiveInfinity"/> values.
        /// </summary>
        [Theory]
        [InlineData(double.NaN, 1d, true, true, true, true)]
        [InlineData(double.NaN, 1d, true, true, false, true)]
        [InlineData(0d, double.NaN, true, true, true, true)]
        [InlineData(0d, double.NaN, true, true, false, true)]
        [InlineData(double.NaN, double.NaN, true, true, true, true)]
        [InlineData(double.NaN, double.NaN, true, true, false, true)]
        [InlineData(double.NegativeInfinity, 1d, true, true, true, true)]
        [InlineData(double.NegativeInfinity, 1d, true, true, false, true)]
        [InlineData(0d, double.NegativeInfinity, true, true, true, true)]
        [InlineData(0d, double.NegativeInfinity, true, true, false, true)]
        [InlineData(double.NegativeInfinity, double.NegativeInfinity, true, true, true, true)]
        [InlineData(double.NegativeInfinity, double.NegativeInfinity, true, true, false, true)]
        [InlineData(double.PositiveInfinity, 1d, true, true, true, true)]
        [InlineData(double.PositiveInfinity, 1d, true, true, false, true)]
        [InlineData(0d, double.PositiveInfinity, true, true, true, true)]
        [InlineData(0d, double.PositiveInfinity, true, true, false, true)]
        [InlineData(double.PositiveInfinity, double.PositiveInfinity, true, true, true, true)]
        [InlineData(double.PositiveInfinity, double.PositiveInfinity, true, true, false, true)]
        [InlineData(double.NegativeInfinity, double.PositiveInfinity, true, true, true, true)]
        [InlineData(double.NegativeInfinity, double.PositiveInfinity, true, true, false, true)]
        [InlineData(double.PositiveInfinity, double.NegativeInfinity, true, true, true, true)]
        [InlineData(double.PositiveInfinity, double.NegativeInfinity, true, true, false, true)]
        public void IsFinite_NaNorInfinityInputRangeDouble_Returns_False(double min, double max, bool inclusiveMin, bool inclusveMax, bool finiteReq, bool notSingleValueReq)
        {
            var testObj = IRangeFactory.Factory(min, max, inclusiveMin, inclusveMax, finiteReq, notSingleValueReq);
            Assert.False(testObj.IsFinite());
        }
        #endregion
        #region IsNullOrEmpty()
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsNullOrEmpty{T}(System.Collections.Generic.IEnumerable{T})"/> function returns <see langword="true"/> for a <see langword="null"/> input.
        /// </summary>
        [Fact]
        public void IsNullOrEmpty_NullInput_Returns_True()
        {
            IEnumerable<Object> X = null;
            Assert.True(X.IsNullOrEmpty());
        }
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsNullOrEmpty{T}(System.Collections.Generic.IEnumerable{T})"/> function returns <see langword="true"/> for a <see cref="IEnumerable{Object}"/> input with 0 elements.
        /// </summary>
        [Fact]
        public void IsNullOrEmpty_EmptyListOfObject_ReturnsTrue()
        {
            IEnumerable<Object> X = new List<Object>();
            Assert.True(X.IsNullOrEmpty());
        }
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsNullOrEmpty{T}(System.Collections.Generic.IEnumerable{T})"/> function returns <see langword="false"/> for a <see cref="IEnumerable{Object}"/> input with single element: {1.0}.
        /// </summary>
        [Fact]
        public void IsNullOrEmpty_1ElementSet_ReturnsTrue()
        {
            IEnumerable<Object> X = new List<Object>() { 1 };
            Assert.False(X.IsNullOrEmpty());
        }
        #endregion
        #region IsNullItem()
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullItem{T}(IEnumerable{T})"/> function returns <see langword="true"/> for an empty <see cref="IEnumerable{double}"/> input.
        /// </summary>
        [Fact]
        public void IsNullItem_NoItem_Returns_True()
        {
            var testObj = new double[] { };
            Assert.True(testObj.IsNullItem());
        }
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullItem{T}(IEnumerable{T})"/> function returns <see langword="true"/> for a null <see cref="IEnumerable{object}"/> input.
        /// </summary>
        [Fact]
        public void IsNullItem_NullInput_Returns_True()
        {
            IEnumerable<object> testObj = null;
            Assert.True(testObj.IsNullItem());
        }
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullItem{T}(IEnumerable{T})"/> function returns <see langword="true"/> for an <see cref="IEnumerable{object}"/> input with a null element: { 1, null, 3 }.
        /// </summary>
        [Fact]
        public void IsNullItem_NullItem_Returns_True()
        {
            IEnumerable<object> testObj = new object[3] { 1, null, 3 };
            Assert.True(testObj.IsNullItem());
        }
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullItem{T}(IEnumerable{T})"/> function returns <see langword="false"/> for an <see cref="IEnumerable{object}"/> input with no null elements: { 1, 2, 3 }.
        /// </summary>
        [Fact]
        public void IsNullItem_NoNullItem_Returns_False()
        {
            IEnumerable<object> testObj = new object[3] { 1, 2, 3 };
            Assert.False(testObj.IsNullItem());
        }
        #endregion
        #region IsNullorNonFiniteItem()
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullOrNonFiniteItem(IEnumerable{double})"/> function returns <see langword="true"/> for an empty <see cref="IEnumerable{double}"/> input.
        /// </summary>
        [Fact]
        public void IsNullOrNonFiniteItem_NoItem_Returns_True()
        {
            var testObj = new double[] { };
            Assert.True(testObj.IsNullOrNonFiniteItem());
        }
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullOrNonFiniteItem(IEnumerable{double})"/> function returns <see langword="true"/> for a null <see cref="IEnumerable{double}"/> input.
        /// </summary>
        [Fact]
        public void IsNullOrNonFiniteItem_NullInput_Returns_True()
        {
            IEnumerable<double> testObj = null;
            Assert.True(testObj.IsNullOrNonFiniteItem());
        }
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullOrNonFiniteItem(IEnumerable{double})"/> function returns <see langword="true"/> for an <see cref="IEnumerable{double}"/> input with non-finite elements { 1d, double.NaN, 3d}.
        /// </summary>
        [Fact]
        public void IsNullOrNonFiniteItem_NonFiniteItem_Returns_True()
        {
            IEnumerable<double> testObj = new double[] { 1d, double.NaN, 3d };
            Assert.True(testObj.IsNullOrNonFiniteItem());
        }
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullOrNonFiniteItem(IEnumerable{double})"/> function returns <see langword="false"/> for an <see cref="IEnumerable{double}"/> input with all finite elements { 1d, 2d, 3d}.
        /// </summary>
        [Fact]
        public void IsNullOrNonFiniteItem_FiniteItems_Returns_False()
        {
            IEnumerable<double> testObj = new double[] { 1d, 2d, 3d };
            Assert.False(testObj.IsNullOrNonFiniteItem());
        }
        #endregion
        #region IsFiniteSample()
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsFiniteSample(IEnumerable{double}, int)"/> function <see langword="throws"/> an <see cref="ArgumentNullException"/> for a <see langword="null"/> <see cref="IEnumerable{double}"/> sample input. 
        /// </summary>
        [Theory]
        [InlineData(null, 0)]
        [InlineData(null, 1)]
        [InlineData(null, int.MaxValue)]
        public void IsFiniteSample_NullInput_Throws_ArgumentNullException(IEnumerable<double> X, int n)
        {
            Assert.Throws<ArgumentNullException>(() => X.IsFiniteSample(n));
        }
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsFiniteSample(IEnumerable{double}, int)"/> function returns <see langword="true"/> for a  <see cref="IEnumerable{double}"/> input with single element: {1.0} and 1 required element. 
        /// </summary>
        [Fact]
        public void IsFiniteSample_1ElementSet1RequiredElement_Returns_True()
        {
            IEnumerable<double> X = new List<double>() { 1.0 };
            Assert.True(X.IsFiniteSample());
        }
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsFiniteSample(IEnumerable{double}, int)"/> function returns <see langword="false"/> for a  <see cref="IEnumerable{T}"/> input with 1 element: {1.0} and 2 required elements. 
        /// </summary>
        [Fact]
        public void IsFiniteSample_1ElementSet2RequiredElements_Returns_False()
        {
            IEnumerable<double> X = new List<double>() { 1.0 };
            Assert.False(X.IsFiniteSample(2));
        }
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsFiniteSample(IEnumerable{double}, int)"/> function returns <see langword="false"/> for a <see cref="IEnumerable{T}"/>
        /// </summary>
        /// <param name="element"> Provides a single element to the <see cref="ValidationExtensions.IsFiniteSample"/> <see cref="IEnumerable{T}"/> parameter. </param>
        [Theory]
        [InlineData(double.NaN)]
        [InlineData(double.PositiveInfinity)]
        [InlineData(double.NegativeInfinity)]
        public void IsFiniteSample_1NaNFiniteElement1RequiredElement_Returns_False(double element)
        {
            IEnumerable<double> X = new List<double>() {element };
            Assert.False(X.IsFiniteSample());
        }
        #endregion
        #region IsNullOrEmptyCollection()
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullOrEmptyCollection{T}(ICollection{T}, bool)"/> function returns <see langword="true"/> for an empty <see cref="List{double}"/> input.
        /// </summary>
        [Fact]
        public void IsNullOrEmptyCollection_NoItem_Returns_True()
        {
            var testObj = new List<double>();
            Assert.True(ValidationExtensions.IsNullOrEmptyCollection(testObj, false));
        }
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullOrEmptyCollection{T}(ICollection{T}, bool)"/> function returns <see langword="true"/> for a null <see cref="List{object}"/> input.
        /// </summary>
        [Fact]
        public void IsNullOrEmptyCollection_NullInput_Returns_True()
        {
            List<object> testObj = null;
            Assert.True(ValidationExtensions.IsNullOrEmptyCollection(testObj, false));
        }
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullOrEmptyCollection{T}(ICollection{T}, bool)"/> function returns <see langword="true"/> for an <see cref="List{object}"/> input with a null element: { 1, null, 3 }.
        /// </summary>
        [Fact]
        public void IsNullOrEmptyCollection_NullItem_Returns_True()
        {
            List<object> testObj = new List<object>() { 1, null, 3 };
            Assert.True(ValidationExtensions.IsNullOrEmptyCollection(testObj, false));
        }
        /// <summary>
        /// Test that the <see cref="ValidationExtensions.IsNullOrEmptyCollection{T}(ICollection{T}, bool)"/> function returns <see langword="false"/> for an <see cref="IEnumerable{Object}"/> input with no null elements: { 1, 2, 3 }.
        /// </summary>
        [Fact]
        public void IsNullOrEmptyCollection_NoNullItem_Returns_False()
        {
            List<object> testObj = new List<object>() { 1, 2, 3 };
            Assert.False(testObj.IsNullItem());
        }
        #endregion
        #region IsRange()

        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function returns <see langword="true"/> for <see cref="int"/> inputs that are strictly increasing (i.e. min < max).
        /// </summary>
        /// <param name="a"> The minimum parameter for the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function. </param>
        /// <param name="b"> The maximum parameter for the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function. </param>
        [Theory]
        [InlineData(0, 1)]
        [InlineData(int.MinValue, int.MaxValue)]
        public void IsRange_SequentialIntInputs_Return_True(int a, int b)
        {
            Assert.True(ValidationExtensions.IsRange(a, b));
        }
        #endregion
        #region IsOnRange

        #endregion
    }
}
