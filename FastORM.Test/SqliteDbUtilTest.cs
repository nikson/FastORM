using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastORM.Test
{
    [TestClass]
    public class SqliteDbUtilTTest
    {
        [TestMethod]
        public void PasswordSetTest()
        {
            var su = new SqliteDbUtil();
            var ret = su.SetPasswrod("testdb.s3db", null, "testdb");
            //var ret = su.SetPasswrod("testdb.s3db", "testdb", null);

            Assert.IsTrue(ret);            
        }
    }
}
