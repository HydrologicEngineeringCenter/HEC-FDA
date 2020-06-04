using System;
using System.Collections.Generic;
using System.Text;
using Functions;

namespace Model
{
    /// <summary>
    /// Provides a method for creation of <see cref="IFdaOrdinate"/>s.
    /// </summary>
    public class IFdaOrdinateFactory
    {
        /// <summary>
        /// Provides a method for creation of invariant <see cref="IFdaOrdinateEnum.Constant"/> <see cref="IFdaOrdinate"/>. 
        /// </summary>
        /// <param name="value"> The ordinate value. </param>
        /// <returns> An <see cref="IFdaOrdinate"/> which wraps the <see cref="Functions.IOrdinate"/> class in the <see cref="Functions"/> assembly. </returns>
        public static IFdaOrdinate Factory(double value)
        {
            return new FdaOrdinate(IOrdinateFactory.Factory(value));
        }
        /// <summary>
        /// Provides a method for the creation of <see cref="IFdaOrdinate"/>s from <see cref="IOrdinate"/>s they wrap in the <see cref="Functions"/> assembly.
        /// </summary>
        /// <param name="ordinate"> The <see cref="IOrdinate"/> to wrap. </param>
        /// <returns> An <see cref="IFdaOrdinate"/>. </returns>
        public static IFdaOrdinate Factory(IOrdinate ordinate)
        {
            return new FdaOrdinate(ordinate);
        }
    }
}
