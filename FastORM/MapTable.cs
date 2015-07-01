using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
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

        #region SQL Query Property
        public String SelectQuery
        {
            get
            {
                if (ClassName == null)
                    throw new ArgumentException("Class type is null");

                String query = "SELECT * FROM {0} WHERE ID = {1}";

                return query;
            }
        }

        public String SelectAllQuery
        {
            get
            {
                if (ClassName == null)
                    throw new ArgumentException("Class type is null");

                String query = "SELECT * FROM {0}";

                return query;
            }
        }

        public String InsertQuery
        {
            get
            {
                if (ClassName == null)
                    throw new ArgumentNullException("Class Type is null");

                return GetInsertQuery(ClassName);
            }
        }

        public String UpdateQuery
        {
            get
            {
                if (ClassName == null)
                    throw new ArgumentException("Class type is null");

                return GetUpdateQuery(ClassName);
            }
        }

        public String DeleteQuery
        {
            get
            {
                if (ClassName == null)
                    throw new ArgumentException("Class type is null");

                String query = "Delete FROM {0} WHERE ID = {1}";

                return query;
            }
        }

        #endregion 

        public MapTable()
        {
            ColumnAndProperties = new Dictionary<string, string>();
            ColumnAndType = new Dictionary<string, Type>();
            PropertiesAndValues = new Dictionary<string, Object>();
        }

        private String GetInsertQuery(Type type)
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

        private String GetUpdateQuery(Type type)
        {
            MapTable map = this;
            String sql = "UPDATE {0} SET {1} WHERE ID={2};";

            if (!String.IsNullOrEmpty(map.TableName))
            {

                String ustr = String.Empty;
                String idStr = String.Empty;

                List<String> Column = map.ColumnAndProperties.Keys.ToList();

                for (int i = 0; i < Column.Count; i++)
                {
                    if (Column[i].ToLower().Equals("id"))
                    {
                        idStr = ":" + Column[i].ToLower();
                        continue;
                    }

                    ustr += ((ustr.Length != 0) ? ", " : "") + Column[i]
                        + " = :" + Column[i];
                }

                sql = String.Format(sql, map.TableName, ustr, idStr);
            }
            else
            {
                sql = String.Empty;
            }

            return sql;
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
                    System.Reflection.PropertyInfo propInfo = ret.GetType().GetProperty(propName);

                    if (propInfo != null)
                    {
                        try
                        {
                            object cval = PropertiesAndValues[propName];
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

        public Boolean SetPropertyValueFromReader(SQLiteDataReader reader)
        {
            PropertiesAndValues.Clear();

            try
            {
                foreach (string cols in ColumnAndType.Keys.ToList())
                {
                    this.PropertiesAndValues[this.ColumnAndProperties[cols]] = reader[cols];
                }
            }
            catch
            {
                throw;
            }

            return true;
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

        public List<SQLiteParameter> BindAllParams(Object obj, bool isNameBinding)
        {
            try
            {
                bool ret = SetPropertyValueFromObject(obj);

            }
            catch { throw; }

            return BindAllParams(true);
        }

        private List<SQLiteParameter> BindAllParams(bool isNameBinding)
        {
            List<SQLiteParameter> collection = new List<SQLiteParameter>();

            foreach (String cols in ColumnAndProperties.Keys.ToList())
            {
                try
                {
                    Object temp = PropertiesAndValues[ColumnAndProperties[cols]];

                    object value = Convert.ChangeType(temp, ColumnAndType[cols], null);

                    SQLiteParameter sqlparam = new SQLiteParameter(cols, value);

                    collection.Add(sqlparam);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return collection;
        }
    }

}
