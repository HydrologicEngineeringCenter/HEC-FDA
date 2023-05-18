using HEC.MVVMFramework.Base.Interfaces;
using System;
using System.Collections;
using System.Linq;
using System.Reflection;

namespace HEC.MVVMFramework.Base.Implementations
{
    /// <summary>
    /// Contains static methods for unregistering/registering from reporter hub recusively.  
    /// </summary>
    public static class MessageHubManager
    {
        public static void Register(object obj)
        {
            RegRecursively(obj, MessageHub.Register);
        }
        public static void UnRegister(object obj)
        {
            RegRecursively(obj, MessageHub.Unregister);
        }
        private static void RegRecursively(object obj, Action<IReportMessage> unRegOrReg)
        {
            if (obj == null)
            {
                return;
            }
            if (obj is IReportMessage reporter)
            {
                unRegOrReg(reporter);
            }
            Type objectType = obj.GetType();
            PropertyInfo[] properties = objectType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object propertyValue = property.GetValue(obj);
                if (propertyValue == null)
                {
                    continue;
                }
                if (property.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
                {
                    foreach (object ob in (IEnumerable)propertyValue)
                    {
                        RegRecursively(ob, unRegOrReg);
                    }
                }
                else
                {
                    RegRecursively(property.GetValue(obj), unRegOrReg);
                }
            }
        }
    }
}