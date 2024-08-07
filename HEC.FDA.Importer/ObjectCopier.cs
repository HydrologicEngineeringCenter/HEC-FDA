using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

/// <summary>
/// Reference Article http://www.codeproject.com/KB/tips/SerializedObjectCloner.aspx
/// Provides a method for performing a deep copy of an object.
/// Binary Serialization is used to perform the copy.
/// </summary>

namespace Importer
{
    public static class ObjectCopier
    {
        /// <summary>
        /// Perform a deep Copy of the object.
        /// </summary>
        /// <typeparam name="T">The type of object being copied.</typeparam>
        /// <param name="source">The object instance to copy.</param>
        /// <returns>The copied object.</returns>
        public static T Clone<T>(T source)
        {
            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

        //This includes all attributed fields and props, public or not. This is what we should use if we need to get at private fields
        //This is the most flexible, but requires decorating every class and member we wish to serialize, so going to work with JSON till we cant.
        https://learn.microsoft.com/en-us/dotnet/api/system.runtime.serialization.datamemberattribute?view=net-8.0
            //using MemoryStream stream = new();
            //DataContractSerializer dataContractSerializer = new(typeof(T));
            //dataContractSerializer.WriteObject(stream, source);
            //stream.Position = 0; //sets us back to the beginning, else we'll get an unexpected end of file exception. 
            //var target = (T)dataContractSerializer.ReadObject(stream);
            //return target;

            JsonSerializerOptions options = new()
            {
                IncludeFields = true
            };
            var json = JsonSerializer.Serialize(source, options);
            var obj = JsonSerializer.Deserialize<T>(json, options);
            return obj;
        }
    }
}
