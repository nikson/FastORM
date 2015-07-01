using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FastORM.Test
{
    [TestClass]
    public class ObjectBuilderTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            var obuild = ObjectBuilder.GetInstance();

            var d = obuild.GetMapTable(typeof(User));

            Assert.AreEqual(d.TableName, "test_user");            
        }

        [TestMethod]
        public void TestMethod2()
        {
            var obuild = ObjectBuilder.GetInstance();

            var d = obuild.GetMapTable(typeof(User));

            Assert.AreNotEqual(d.TableName, "test_user2");
        }

        [TestMethod]
        public void TestMethod3()
        {
            var obuild = ObjectBuilder.GetInstance();

            var d = obuild.GetMapTable(typeof(User));

            Assert.AreEqual(d.PrimaryId, "id");
        }
    }
}
