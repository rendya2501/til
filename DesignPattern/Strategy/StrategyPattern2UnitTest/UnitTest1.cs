using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace StrategyPattern2UnitTest
{
    /// <summary>
    /// https://takachan.hatenablog.com/entry/2017/11/10/021842
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var h = new Pattern1.Hoge();
            string retStr = h.Func(1, "a");
            Assert.AreEqual("ok", retStr); // 実行結果の確認

            retStr = h.Func(1, "b");
            Assert.AreEqual("ok", retStr); // 実行結果の確認
        }

        [TestMethod]
        public void TestMethod3()
        {
            Pattern3.IHoge retStr = Pattern3.SimpleIHogeFactory.Create(1);
            Assert.AreEqual("ok", retStr.Func("aa")); // 実行結果の確認
            Assert.AreEqual("ng", retStr.Func("aa")); // 実行結果の確認
        }
    }
}
