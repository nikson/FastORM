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
using System.Data.SQLite;

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
        Dictionary<Type, Dictionary<String, String>> ColumnMapCollection;
        /// <summary>
        /// Key: Class Type
        /// Value -> key : Column Name, value : Column Type
        /// </summary>
        Dictionary<Type, Dictionary<String, Type>> TypeMapCollection;

        private static ObjectBuilder _instance = null;
        public static ObjectBuilder GetInstance()
        {
            if (_instance == null)
                _instance = new ObjectBuilder();

            return _instance;
        }

        private ObjectBuilder()
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

        public MapTable GetMapTable(Type t)
        {
            if (!TableMapCollection.ContainsKey(t))
                AddedToTableMapCollection(t);

            MapTable ret = new MapTable();

            try
            {
                if (TableMapCollection.ContainsKey(t))
                {
                    ret.ClassName = t;
                    ret.TableName = TableMapCollection[t];
                    ret.ColumnAndProperties = ColumnMapCollection[t];
                    ret.ColumnAndType = TypeMapCollection[t];
                }
            }
            catch { throw; }

            return ret;

        }

        private void AddedToTableMapCollection(Type t)
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
                AddedToColumMapCollection(t);
            }
        }

        private void AddedToColumMapCollection(Type t)
        {
            if (!ColumnMapCollection.ContainsKey(t))
            {
                var props = t.GetProperties().Where(p => p.IsDefined(typeof(ColumnAttribute), false));
                Dictionary<String, String> list = new Dictionary<string, string>();
                Dictionary<String, Type> typelist = new Dictionary<string, Type>();

                foreach (System.Reflection.PropertyInfo info in props)
                {
                    var data = info.GetCustomAttributes(typeof(ColumnAttribute), false);
                    String columName = "";                    
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
    }     
}
