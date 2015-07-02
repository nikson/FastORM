/* 
 * 
 * It is a free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License
 *
 * You should have received a copy of the GNU General Public License
 * along with Application.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FastORM
{    
    /// <summary>
    /// FastORM is a rapid Object-Relational-Mapper Class. 
    /// </summary>

    public class ObjectBuilder
    {
        /// <summary>
        /// Key : Class Type 
        /// Value : Table Name
        /// </summary>
        private Dictionary<Type, String> TableMapCollection;
        /// <summary>
        /// Key: Class Type
        /// Value -> key : Column Name, value : Property Name
        /// </summary>
        private Dictionary<Type, Dictionary<String, String>> ColumnMapCollection;
        /// <summary>
        /// Key: Class Type
        /// Value -> key : Column Name, value : Column Type
        /// </summary>
        private Dictionary<Type, Dictionary<String, Type>> TypeMapCollection;

        private static ObjectBuilder _instance = null;
        public static ObjectBuilder GetInstance()
        {
            return _instance = _instance ?? new ObjectBuilder();
        }

        public ObjectBuilder()
        {
            MapCollectionBootstrap();
        }

        private void MapCollectionBootstrap()
        {
            TableMapCollection = new Dictionary<Type, string>();
            TableMapCollection.Clear();

            ColumnMapCollection = new Dictionary<Type, Dictionary<String, String>>();
            ColumnMapCollection.Clear();

            TypeMapCollection = new Dictionary<Type, Dictionary<string, Type>>();
            TypeMapCollection.Clear();
        }

        /// <summary>
        /// A object for Class-Table mapped column, properties and type 
        /// </summary>
        /// <param name="t"> type of object </param>
        /// <returns></returns>
        public MapTable GetMapTable(Type t)
        {
            MapTable ret = null;

            if (!TableMapCollection.ContainsKey(t))
                AddToTableMapCollection(t);

            try
            {
                if (TableMapCollection.ContainsKey(t))
                {
                    ret = new MapTable();
                    ret.ClassName = t;
                    ret.TableName = TableMapCollection[t];
                    ret.ColumnAndProperties = ColumnMapCollection[t];
                    ret.ColumnAndType = TypeMapCollection[t];
                    ret.PrimaryId = GetPrimaryColumn(t);
                }
            }
            catch { throw; }

            return ret;

        }

        /// <summary>
        /// Check the provided object  has any table mapping attributes, if exist then process 
        /// all mapped properties 
        /// </summary>
        /// <param name="t"></param>
        private void AddToTableMapCollection(Type t)
        {
            var list = (TableAttribute[])t.GetCustomAttributes(typeof(TableAttribute), false);

            String mapTableName = "";
            if (list.Length > 0)
            {
                mapTableName = list.GetValue(0).ToString();
            }

            if (!String.IsNullOrEmpty(mapTableName))
            {
                TableMapCollection.Add(t, mapTableName);
                AddToColumMapCollection(t);
            }
        }

        /// <summary>
        /// Find all mapped properties and add in internal dictionary 
        /// </summary>
        /// <param name="t"></param>
        private void AddToColumMapCollection(Type t)
        {
            if (!ColumnMapCollection.ContainsKey(t))
            {
                var props = t.GetProperties().Where(p => p.IsDefined(typeof(ColumnAttribute), false)
                    || p.IsDefined(typeof(IdAttribute), false) );

                Dictionary<String, String> list = new Dictionary<string, string>();
                Dictionary<String, Type> typelist = new Dictionary<string, Type>();

                foreach (PropertyInfo info in props)
                {
                    String columName = "";    

                    // if Id attribute define then read Id attributes otherwise ColumnAttribute
                    var data = info.IsDefined(typeof(IdAttribute), false)
                        ? info.GetCustomAttributes(typeof(IdAttribute), false)
                        : info.GetCustomAttributes(typeof(ColumnAttribute), false);
                                    
                    if (data.Length > 0)
                        columName = data.GetValue(0).ToString();

                    if (!String.IsNullOrEmpty(columName))
                    {
                        list.Add(columName, info.Name);
                        typelist.Add(columName, info.PropertyType);
                    }
                }

                ColumnMapCollection.Add(t, list);
                TypeMapCollection.Add(t, typelist);
            }
        }
    
        /// <summary>
        /// Find the PrimaryKey column 
        /// </summary>
        /// <param name="t"> type of object</param>
        /// <returns></returns>
        private String GetPrimaryColumn(Type t)
        {
            String columName = String.Empty;

            var props = t.GetProperties().Where(p => p.IsDefined(typeof(IdAttribute), false));

            foreach (PropertyInfo info in props)
            {
                var data = info.GetCustomAttributes(typeof(IdAttribute), false);

                if (data.Length > 0)
                {
                    columName = data.GetValue(0).ToString();
                    // Single column primary key is supported only 
                    break;
                }

            }

            return columName;
        }
    
    }     
}
