using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using FastORM;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastORM.Test
{
    [TestClass]
    public class DbUtilTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            DbUtil dbm = DbUtil.CreateInstance();

            String query = "select count(*) from test_user";

            object result = dbm.ExecuteQuery(query, false);

            Console.WriteLine("result : " + result.ToString());
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void TestMethod2()
        {
            DbUtil dbm = DbUtil.CreateInstance();
            

            User u = new User();
            u.id = 2;
            u.name = "NKP";
            u.password = "paul";
            u.username = "paul";

            int a = dbm.InsertOrUpdate(u, false);
            Assert.AreEqual(1, a);
        }

        [TestMethod]
        public void TestMethod3()
        {
            DbUtil dbm = DbUtil.CreateInstance();

            int id = 2;
            var u = dbm.Get<User>(id.ToString());            
            Assert.AreEqual("NKP", u.name);
            u.name = "Nikson2";
            int a = dbm.InsertOrUpdate(u, true);
            Assert.AreEqual(1, a);            
        }

        [TestMethod]
        public void TestMethod4()
        {
            DbUtil dbm = DbUtil.CreateInstance();

            List<User> data = dbm.GetAll<User>().ToList();

            Assert.AreEqual(0, data.Count);
        }

        [TestMethod]
        public void TestMethod5()
        {
            DbUtil dbm = DbUtil.CreateInstance();


            var u = new User();
            u.id = 2;

            int count = dbm.Delete(u);

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void TestMethod6()
        {
            DbUtil dbm = DbUtil.CreateInstance();

            int count = dbm.Delete<User>();

            Assert.AreEqual(0, count);
        }
    }
}
