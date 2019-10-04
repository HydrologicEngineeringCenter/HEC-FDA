using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataBase_Reader;
using System.Collections;

namespace FdaModel.Utilities
{
    public static class DataWriter
    {
        public static void WriteData<T>(T data)
        {
            WriteTables(new SqLiteReader(Project.Instance.FilePath + Project.Instance.Name + ".sqlite"), typeof(T).Name, data);
        }

        private static void WriteTables<T>(SqLiteReader database, string tableName, T data)
        { 
            List<Type> tableColumnTypes = new List<Type>();
            Dictionary<string, List<object>> tableData = new Dictionary<string, List<object>>(); 
            foreach (PropertyInfo property in typeof(T).GetType().GetProperties())
            {
                if (IsNewTable(data) == true)
                {
                    tableColumnTypes.Add(typeof(string));
                    tableData.Add(property.Name, new List<object> { property.Name + "WhatWouldWoodyDo(WWWD)" });
                    WriteTables(database, tableName + "&" + property.Name, property); // does this have the data?
                }
                else
                {
                    if (property.PropertyType.GetInterface("IEnumerable") == null)
                    {
                        if (tableData.ContainsKey(property.Name))
                        {
                            tableColumnTypes.Add(property.GetType());
                            tableData.Add(property.Name, new List<object> { property.GetValue(property, null) });
                        }
                        else
                        {
                            tableData[property.Name].Add(property.GetValue(property.GetValue(property, null)));
                        }                   
                    }
                    else
                    {
                        tableColumnTypes.Add(typeof(string));
                        tableData.Add(property.Name, new List<object> { property.Name + "WhatWouldWoodyDo(WWWD)" });
                        // This is the type. Does it go into a single table or multiple tables...
                        
                        

                        int i = 0;
                        Type enumerableType = property.GetType().GetGenericArguments()[0];
                        Dictionary<string, List<object>> listTableData = new Dictionary<string, List<object>>();
                        foreach (T item in (IEnumerable)property.GetValue(property.Name, null))
                        {
                            if (IsNewTable(enumerableType) == true)
                            {
                                tableColumnTypes.Add(typeof(string));
                                tableData.Add(item.GetType().Name, new List<object> { item.GetType().Name + "WhatWouldWoodyDo(WWWD)" });
                            }
                            WriteTables(database, tableName + "&" + item.GetType().Name, item);
                        }
                    }
                }
            }
            WriteTableData(tableName, tableColumnTypes, tableData);
        }

        public static void WriteTableData(string tableName, List<Type> columnTypes, Dictionary<string, List<object>> tableData)
        {
            SqLiteReader database = new SqLiteReader(Project.Instance.FilePath + Project.Instance.Name + ".sqlite");
            if (database.TableNames.Contains(tableName) == true)
            {
                throw new Exception("Whoa. Clobber data and rewrite, right?");
            }
            else
            {
                database.CreateTable(tableName, tableData.Keys.ToArray(), columnTypes.ToArray());
                //write out rows of data.
            }
        }

        public static bool IsNewTable<T>(T data)
        {
            //check if is something in consequences assist or FDA namespace.
            return false;
        }

    }   
}
