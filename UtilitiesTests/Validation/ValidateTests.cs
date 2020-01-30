using System;
using Xunit;

using Utilities;
using System.Collections.Generic;

namespace UtilitiesTests
{
    /// <summary>
    /// Tests the <see cref="Utilities.Validation.Validate"/> class in the <see cref="Utilities"/> project.
    /// </summary>
    public class ValidatedTests
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
        /// Tests that the <see cref="ValidationExtensions.IsFinite(double)"/> function returne <see langword="true"/> for a <see cref="double"/> input set to 1.0.
        /// </summary>
        [Fact]
        public void IsFinite_GoodInput_Returns_True()
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
        public void IsFinite_NaNOrInfinityInput_Returns_False(double x)
        {
            Assert.False(x.IsFinite());
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
        #region IsRange()
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function returns <see langword="false"/> for <see cref="int"/> inputs that are not strictly increasing (i.e. min = max, min > max).
        /// </summary>
        /// <param name="a"> The minimum parameter for the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function. </param>
        /// <param name="b"> The maximum parameter for the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function. </param>
        [Theory]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(int.MaxValue, int.MinValue)]
        public void IsRange_NonSequentialIntegerInputs_Return_False(int a, int b)
        {
            Assert.False(ValidationExtensions.IsRange<int>(a, b));
        }
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function returns <see langword="false"/> for <see cref="double"/> inputs that are not strictly increasing (i.e. min = max, min > max).
        /// </summary>
        /// <param name="a"> The minimum parameter for the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function. </param>
        /// <param name="b"> The maximum parameter for the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function. </param>
        [Theory]
        [InlineData(1.0, 0.0)]
        [InlineData(1.0, 1.0)]
        [InlineData(double.PositiveInfinity, double.NegativeInfinity)]
        public void IsRange_NonSequentialDoubleInputs_Return_False(double a, double b)
        {
            Assert.False(ValidationExtensions.IsRange<double>(a, b));
        }
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
        /// <summary>
        /// Tests that the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function returns <see langword="true"/> for <see cref="double"/> inputs that are strictly increasing (i.e. min < max).
        /// </summary>
        /// <param name="a"> The minimum parameter for the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function. </param>
        /// <param name="b"> The maximum parameter for the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function. </param>
        [Theory]
        [InlineData(0, 1)]
        [InlineData(double.NegativeInfinity, double.PositiveInfinity)]
        public void IsRange_SequentialDoubleInputs_Return_True(double a, double b)
        {
            Assert.True(ValidationExtensions.IsRange(a, b));
        }
        /// <summary>
        /// Illustrates the unusual behavior (<see cref="double.NaN"/> &lt <see cref="double.NegativeInfinity"/>) values in the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function due to oddities in the way that C# handles the <see cref="IComparable"/> value of <see cref="double.NaN"/>.
        /// </summary>
        /// <param name="a"> The minimum parameter for the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function. </param>
        /// <param name="b"> The maximum parameter for the <see cref="ValidationExtensions.IsRange{T}(T, T)"/> function. </param>
        [Fact]
        public void IsRange_NaNInputs_Return_True()
        {
            Assert.True(ValidationExtensions.IsRange(double.NaN, double.NegativeInfinity));
        }
        #endregion
        #region IsOnRange

        #endregion
    }
}
