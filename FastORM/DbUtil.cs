using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Data.SQLite;
using FastORM;

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

        /***********   CREATE ONLY A SINGLE INSTANCE     ********/
        ObjectBuilder fastORM;
        /*************************************/

        public static DbUtil CreateInstance()
        {
            if (_instance == null)
            {
                _instance = new DbUtil();
            }

            return _instance;
        }

        private DbUtil()
        {
            String path = AppDomain.CurrentDomain.BaseDirectory;
            //String path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            DbFilename = "testdb.s3db";
            DbSource = path + "\\" + DbFilename;
            DbIsNew = false;
            DbIsCompress = true;

            fastORM = ObjectBuilder.GetInstance();
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

            using (GetConnection())
            {
                using (_command = new SQLiteCommand(_singleCons))
                {
                    _command.CommandText = String.Format(mt.SelectQuery, mt.TableName, Id);
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

        public List<T> GetAll<T>()
        {
            MapTable mt = fastORM.GetMapTable(typeof(T));
            List<T> data = new List<T>();

            using (GetConnection())
            {
                using (_command = new SQLiteCommand(_singleCons))
                {
                    _command.CommandText = String.Format(mt.SelectAllQuery, mt.TableName);
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

            SQLiteTransaction trans = null;

            try
            {
                using (var con = GetConnection())
                {
                    using (_command = new SQLiteCommand(con))
                    {
                        _command.CommandText = (isUpdate) ? mt.UpdateQuery : mt.InsertQuery;
                        _command.Parameters.AddRange(mt.BindAllParams(obj, true).ToArray());
                        _command.Prepare();

                        trans = con.BeginTransaction();

                        retval = _command.ExecuteNonQuery();
                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();
                retval = 0;
                throw ex;
            }


            return retval;
        }

        public int Delete(Object obj)
        {
            int retval = 0;

            MapTable mt = fastORM.GetMapTable(obj.GetType());

            SQLiteTransaction trans = null;

            try
            {
                int Id = (int)obj.GetType().GetProperty(mt.ColumnAndProperties["id"]).GetValue(obj, null);

                using (var con = GetConnection())
                {
                    using (_command = new SQLiteCommand(con))
                    {
                        _command.CommandText = String.Format(mt.DeleteQuery, mt.TableName, Id);
                        _command.Prepare();

                        trans = con.BeginTransaction();

                        retval = _command.ExecuteNonQuery();
                        trans.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                if (trans != null)
                    trans.Rollback();
                retval = 0;
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

    public class DbVersion
    {
        public static String Version_1_1()
        {
            String query = "create table user2 ( id int primary key, name char(50) );";

            return query;

        }
    }
}
