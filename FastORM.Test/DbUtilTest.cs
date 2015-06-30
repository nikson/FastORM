using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using FastORM;

namespace FastORM.Test
{
    public class DbUtilTest
    {
        DbUtil dbm = DbUtil.CreateInstance();

        public void test()
        {
            try
            {
                String query = "select count(*) from test_user";

                object result = dbm.ExecuteQuery(query, false);

                Console.WriteLine("result : " + result.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
            }

        }

        public void test1()
        {
            try
            {
                User u = new User();
                u.id = 2;
                u.name = "NKP";
                u.password = "paul";
                u.username = "paul";

                int a = dbm.InsertOrUpdate(u, false);
                //int a = dbm.Delete(u);
                List<User> data = dbm.GetAll<User>();

                Console.WriteLine("result : " + a.ToString());

                //int id = 2;
                //User u = dbm.Get<User>(id);                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetBaseException().Message);
            }

        }
    }
}
