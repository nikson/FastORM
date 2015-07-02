using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FastORM
{
    /// <summary>
    /// MapTable is object class of detail information of a Class-Table mapping.
    /// </summary>

    public class MapTable
    {
        public String TableName { get; set; }
        /// <summary>
        /// Currently only one primary key is supported. ToDo.....
        /// </summary>
        public String PrimaryId { get; set; }
        public Type ClassName { get; set; }
        public Dictionary<String, String> ColumnAndProperties { get; set; }
        public Dictionary<String, Type> ColumnAndType { get; set; }
        public Dictionary<String, Object> PropertiesAndValues { get; set; }

        public MapTable()
        {
            ColumnAndProperties = new Dictionary<string, string>();
            ColumnAndType = new Dictionary<string, Type>();
            PropertiesAndValues = new Dictionary<string, Object>();
        }

        public T GetMapValuedObject<T>()
        {
            if (typeof(T) != ClassName)
                throw new InvalidCastException("Invalid generic type object.");

            T ret = Activator.CreateInstance<T>();

            try
            {
                foreach (String propName in ColumnAndProperties.Values.ToList())
                {
                    PropertyInfo propInfo = ret.GetType().GetProperty(propName);

                    if (propInfo != null)
                    {
                        try
                        {
                            object cval = PropertiesAndValues.ContainsKey(propName) ? PropertiesAndValues[propName]
                                : DBNull.Value;
                            object value = null;

                            if (cval != DBNull.Value)
                                value = Convert.ChangeType(cval, propInfo.PropertyType, null);

                            if (value != null)
                                propInfo.SetValue(ret, value, null);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ret;
        }

        public Boolean SetPropertyValueFromObject(object ret)
        {
            PropertiesAndValues.Clear();
            Boolean retval = true;

            try
            {
                if (ret.GetType() != ClassName)
                    throw new InvalidCastException("Invalid generic type object.");

                foreach (String propName in ColumnAndProperties.Values.ToList())
                {
                    object value = ret.GetType().GetProperty(propName).GetValue(ret, null);

                    this.PropertiesAndValues[propName] = value;
                }
            }
            catch (Exception ex)
            {
                retval = false;
                throw ex;
            }

            return retval;
        }


        #region SQL Query Helper

        private void Validate()
        {
            if (ClassName == null)
                throw new ArgumentException("Class type is null");

            if (String.IsNullOrEmpty(PrimaryId))
                throw new ArgumentNullException("No primary key property found!");
        }
        public String GetSelectQuery(String Id)
        {
            Validate();

            String query = "SELECT * FROM {0} WHERE {1} = {2}";

            return String.Format(query, TableName, PrimaryId, Id);
        }

        public String GetSelectQuery()
        {
            if (ClassName == null)
                throw new ArgumentException("Class type is null");

            return String.Format("SELECT * FROM {0}", TableName);

        }

        public String GetDeleteQuery()
        {
            String query = "Delete FROM {0}";

            return String.Format(query, TableName);
        }

        public String GetDeleteQuery(String Id)
        {
            String query = "Delete FROM {0} WHERE {1} = {2}";

            return String.Format(query, TableName, PrimaryId, Id); ;
        }

        public String GetInsertQuery()
        {
            MapTable map = this;
            String sql = "INSERT INTO {0}({1}) VALUES({2});";

            if (!String.IsNullOrEmpty(map.TableName))
            {
                String cols = String.Empty;
                String val = String.Empty;

                List<String> Column = map.ColumnAndProperties.Keys.ToList();

                for (int i = 0; i < Column.Count; i++)
                {
                    cols += ((cols.Length != 0) ? ", " : "") + Column[i];
                    val += ((val.Length != 0) ? ", " : "") + " :" + Column[i];      // + " ? ";              
                }

                sql = String.Format(sql, map.TableName, cols, val);
            }
            else
            {
                sql = String.Empty;
            }

            return sql;
        }

        public String GetUpdateQuery()
        {
            String sql = "UPDATE {0} SET {1} WHERE ID={2};";

            if (!String.IsNullOrEmpty(TableName))
            {

                String ustr = String.Empty;
                String idStr = String.Empty;

                List<String> Column = ColumnAndProperties.Keys.ToList();

                for (int i = 0; i < Column.Count; i++)
                {
                    if (Column[i].Equals(PrimaryId))
                    {
                        idStr = ":" + Column[i].ToLower();
                        continue;
                    }

                    ustr += ((ustr.Length != 0) ? ", " : "") + Column[i]
                        + " = :" + Column[i];
                }

                sql = String.Format(sql, TableName, ustr, idStr);
            }
            else
            {
                sql = String.Empty;
            }

            return sql;
        }

        public Boolean SetPropertyValueFromReader(DbDataReader reader)
        {
            PropertiesAndValues.Clear();

            // ToDo: may raise exception.....
            foreach (string cols in ColumnAndType.Keys.ToList())
            {
                this.PropertiesAndValues[this.ColumnAndProperties[cols]] = reader[cols];
            }

            return true;
        }

        public IList<T> BindAllParams<T>(Object obj) where T : DbParameter
        {
            return (SetPropertyValueFromObject(obj) == true) ? BindAllParams<T>(true) : new List<T>();
        }

        private IList<T> BindAllParams<T>(bool isNameBinding) where T : DbParameter
        {
            List<T> collection = new List<T>();

            foreach (String cols in ColumnAndProperties.Keys.ToList())
            {
                try
                {
                    Object temp = PropertiesAndValues[ColumnAndProperties[cols]];

                    object value = Convert.ChangeType(temp, ColumnAndType[cols], null);

                    T sqlparam = Activator.CreateInstance<T>();
                    sqlparam.ParameterName = cols;
                    //sqlparam.SourceColumn = cols;
                    sqlparam.Value = value;


                    collection.Add(sqlparam);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return collection;
        }

        #endregion
    }

}
