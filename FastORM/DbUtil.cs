using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using FastORM;
using System.IO;

namespace FastORM
{
    public class DbUtil
    {
        private static DbUtil _instance = null;

        SQLiteConnection _singleCons = null;
        SQLiteCommand _command;
        SQLiteDataReader _reader;

        public String DbFilename { get; set; }
        public String DbSource { get; set; }
        public String DbUsername { get; set; }
        public String DbPassword { get; set; }
        public Boolean DbIsNew { get; set; }
        public Boolean DbIsCompress { get; set; }
        private String connectionStr = @"Data Source={0};Version=3;New={1};Compress={2};";
        // Data Source=testdb.s3db;Version=3;New=False;Compress=True;

        //   CREATE ONLY A SINGLE INSTANCE   
        private ObjectBuilder fastORM;

        public static DbUtil CreateInstance()
        {
            if (_instance == null)
            {
                _instance = new DbUtil();
            }

            return _instance;
        }

        public DbUtil()
        {
            DbIsNew = false;
            DbIsCompress = true;

            fastORM = new ObjectBuilder();

            //SetDbProperty("testdb.s3db");
        }

        // ToDo: Add provider, and connection string
        public DbUtil SetDbProperty(String dbFile)
        {
            String path = AppDomain.CurrentDomain.BaseDirectory;
            DbFilename = dbFile;
            DbSource = Path.Combine(path, DbFilename);
            return this;
        }

        public SQLiteConnection GetConnection()
        {
            _singleCons = new SQLiteConnection(String.Format(connectionStr, DbSource, DbIsNew, DbIsCompress));

            _singleCons.Open();

            return _singleCons;
        }

        public T Get<T>(String Id)
        {
            MapTable mt = fastORM.GetMapTable(typeof(T));

            using (var con = GetConnection())
            {
                using (_command = new SQLiteCommand(con))
                {
                    _command.CommandText = mt.GetSelectQuery(Id);
                    _command.Prepare();

                    using (_reader = _command.ExecuteReader())
                    {
                        if (_reader.Read())
                        {
                            mt.SetPropertyValueFromReader(_reader);
                        }
                    }
                }
            }

            Object ret = mt.GetMapValuedObject<T>();

            return (T)ret;
        }

        public IList<T> GetAll<T>()
        {
            MapTable mt = fastORM.GetMapTable(typeof(T));
            IList<T> data = new List<T>();

            using (var con = GetConnection())
            {
                using (_command = new SQLiteCommand(con))
                {
                    _command.CommandText = mt.GetSelectQuery();
                    _command.Prepare();

                    using (_reader = _command.ExecuteReader())
                    {
                        while (_reader.Read())
                        {
                            mt.SetPropertyValueFromReader(_reader);
                            T ret = mt.GetMapValuedObject<T>();
                            data.Add(ret);
                        }
                    }
                }
            }

            return data;
        }

        public int InsertOrUpdate(Object obj, Boolean isUpdate)
        {
            int retval = 0;

            MapTable mt = fastORM.GetMapTable(obj.GetType());

            try
            {
                using (var con = GetConnection())
                {
                    using (_command = new SQLiteCommand(con))
                    {
                        _command.CommandText = (isUpdate) ? mt.GetUpdateQuery() : mt.GetInsertQuery();
                        _command.Parameters.AddRange(mt.BindAllParams<SQLiteParameter>(obj).ToArray());
                        _command.Prepare();

                        using (var trans = con.BeginTransaction())
                        {
                            retval = _command.ExecuteNonQuery();
                            trans.Commit();
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return retval;
        }

        public int Delete<T>()
        {
            int retval = 0;

            MapTable mt = fastORM.GetMapTable(typeof(T));

            try
            {
                using (var con = GetConnection())
                {
                    using (_command = new SQLiteCommand(con))
                    {
                        _command.CommandText = mt.GetDeleteQuery();
                        _command.Prepare();

                        using (var trans = con.BeginTransaction())
                        {
                            retval = _command.ExecuteNonQuery();
                            trans.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return retval;
        }

        public int Delete(Object obj)
        {
            int retval = 0;

            MapTable mt = fastORM.GetMapTable(obj.GetType());

            try
            {
                object value = obj.GetType().GetProperty(mt.ColumnAndProperties[mt.PrimaryId]).GetValue(obj, null);
                var Id = Convert.ChangeType(value, mt.ColumnAndType[mt.PrimaryId], null);

                using (var con = GetConnection())
                {
                    using (_command = new SQLiteCommand(con))
                    {
                        _command.CommandText = mt.GetDeleteQuery(Id.ToString());
                        _command.Prepare();

                        using (var trans = con.BeginTransaction())
                        {
                            retval = _command.ExecuteNonQuery();
                            trans.Commit();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


            return retval;
        }

        public Object ExecuteQuery(String query, Boolean enableTranscation)
        {
            Object ret = null;

            SQLiteTransaction trans = null;

            try
            {
                using (var con = GetConnection())
                {
                    using (SQLiteCommand command = new SQLiteCommand(con))
                    {
                        command.CommandText = query;
                        command.Prepare();

                        trans = enableTranscation ? con.BeginTransaction() : null;

                        ret = command.ExecuteScalar();

                        if (enableTranscation) trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (enableTranscation) trans.Rollback();
                throw ex;
            }

            return ret;
        }
    }
}
