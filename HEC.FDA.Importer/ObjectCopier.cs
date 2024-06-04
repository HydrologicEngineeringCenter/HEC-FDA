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
            if (!typeof(T).IsSerializable)
            {
                throw new ArgumentException("The type must be serializable.", "source");
            }

            // Don't serialize a null object, simply return the default for that object
            if (Object.ReferenceEquals(source, null))
            {
                return default(T);
            }

            //This method of making a deep copy will be obsolete in Net 9.0 and this will break. 
            IFormatter formatter = new BinaryFormatter();
            System.IO.Stream streamMem = new MemoryStream();
            using (streamMem)
            {
                formatter.Serialize(streamMem, source);
                streamMem.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(streamMem);
            }

            //Tried to use JSON instead, but it broke occupancy types for some reason. Will investigate later if we need to move forward in DOTNET version. 
            //var json = JsonSerializer.Serialize(source); // Serialize the object into JSON
            //var obj = JsonSerializer.Deserialize<T>(json);
            //return obj;
        }
    }
}
