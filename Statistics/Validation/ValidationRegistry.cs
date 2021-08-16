using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics;
using Utilities;

namespace Statistics.Validation
{
    public static class ValidationRegistry
    {
        private static readonly IDictionary<Type, object> Registry = new Dictionary<Type, object>();

        /// <summary>
        /// Adds a <see cref="IValidator{T}"/> and its associated reference type <typeparamref name="T"/> to the registry.
        /// </summary>
        /// <typeparam name="T"> A concrete reference type implementing the <see cref="IValidate{T}"/> interface. </typeparam>
        /// <param name="obj"> An instance of the reference type to be registered. </param>
        /// <param name="validator"> The <see cref="IValidator{T}"/> to be associated with the <typeparamref name="T"/> type parameter in the registry. </param>
        public static void Register<T>(T obj, IValidator<T> validator) where T : IValidate<T> => Registry.Add(obj.GetType(), validator);
        public static void Register<T>(IValidator<T> validator) where T : IValidate<T> => Registry.Add(typeof(T), validator);
        /// <summary>
        /// Retrieves the <see cref="IValidator{T}"/> associated with <paramref name="entity"/> reference type in the registry.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        private static IValidator<T> Retrieve<T>(T entity) where T : IValidate<T> => Registry[entity.GetType()] as IValidator<T>;
        /// <summary>
        /// Validates the <paramref name="obj"/> instance, by using reflection to dynamically assign the concrete validator to the associated object associated with the <paramref name="obj"/> reference type.
        /// </summary>
        /// <typeparam name="T"> A reference type associated with a concrete validator. </typeparam>
        /// <param name="obj"> The reference type object instance. </param>
        /// <param name="errors"> A list of errors associated with the <paramref name="obj"/> instance. </param>
        /// <returns></returns>
        public static IMessageLevels Validate<T>(this T obj, out IEnumerable<IMessage> errors) where T : IValidate<T>
        {
            IValidator<T> validator = Retrieve(obj);
            return obj.Validate(validator, out errors);
        }
    }
}