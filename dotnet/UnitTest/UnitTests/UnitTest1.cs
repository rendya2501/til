using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnitTests;

public class UnitTest1
{
    // テスト対象メソッド
    public string GetYoubi(DateTime date)
    {
        return date.ToString("dddd", new System.Globalization.CultureInfo("ja-JP"));
    }

    // テストデータ作成クラス
    class TestDataClass : IEnumerable<object[]>
    {
        List<object[]> _testData = new List<object[]>();

        public TestDataClass()
        {
            _testData.Add(new object[] { new DateTime(2017, 4, 27), "木曜日" });
            _testData.Add(new object[] { new DateTime(2017, 4, 28), "金曜日" });
        }

        public IEnumerator<object[]> GetEnumerator() => _testData.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }


    [Theory]
    [ClassData(typeof(TestDataClass))]
    public void Test1(DateTime date, string youbi)
    {
        Assert.Equal(youbi, GetYoubi(date));
    }
}