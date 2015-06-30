using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace FastORM
{
    public class SqliteDbUtil
    {       
        public bool SetPasswrod(String dbfile, String currentpass, String newpassword)
        {
            bool retval = false;
            
            //String connStr = String.Format("Data Source={0};Version=3;New=False;Compress=True;Password={1};", dbfile, password);
            String connStr = String.Format("Data Source={0};Version=3;New=False;Compress=True;",
                dbfile);

            if (!String.IsNullOrEmpty(currentpass))
                connStr = String.Format("Data Source={0};Version=3;New=False;Compress=True;Password={1};", 
                    dbfile, currentpass);
            
            try
            {
                using (var cnn = new SQLiteConnection(connStr))
                {
                    cnn.Open();
                    cnn.ChangePassword(newpassword);
                    //cnn.ChangePassword("");

                    String sql = "SELECT count(*) FROM sqlite_master WHERE type='table'";
                    using (SQLiteCommand myCommand = new SQLiteCommand(sql, cnn))
                    {
                        var a = myCommand.ExecuteScalar();                    
                        Console.WriteLine("Total : " + a.ToString() + " tables found!");
                    }

                    retval = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                // throw e;
            }

            return retval;
        }
    }
}
