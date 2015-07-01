using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastORM
{
    /// <summary>
    /// Set the table name of corresponding Class
    /// </summary>

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {
        private String _tableName = "";

        public TableAttribute(String tableName)
        {
            if (String.IsNullOrEmpty(tableName))
                throw new ArgumentNullException();

            _tableName = tableName;
        }

        public override string ToString()
        {
            return _tableName;
        }
    }

    /// <summary>
    /// Set Column name of corresponding Property
    /// </summary>

    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        private String _columName = "";

        public ColumnAttribute(String columnName)
        {
            if (String.IsNullOrEmpty(columnName))
                throw new ArgumentNullException();

            _columName = columnName;
        }

        public override string ToString()
        {
            return _columName;
        }
    }

    /// <summary>
    /// Set primary id column name 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class IdAttribute : Attribute
    {
        private String _columName = "";

        public IdAttribute(String columnName)
        {
            if (String.IsNullOrEmpty(columnName))
                throw new ArgumentNullException();

            _columName = columnName;
        }

        public override string ToString()
        {
            return _columName;
        }
    }

}
