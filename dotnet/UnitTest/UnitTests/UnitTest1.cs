using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnitTests;

public class UnitTest1
{
    // �e�X�g�Ώۃ��\�b�h
    public string GetYoubi(DateTime date)
    {
        return date.ToString("dddd", new System.Globalization.CultureInfo("ja-JP"));
    }

    // �e�X�g�f�[�^�쐬�N���X
    class TestDataClass : IEnumerable<object[]>
    {
        List<object[]> _testData = new List<object[]>();

        public TestDataClass()
        {
            _testData.Add(new object[] { new DateTime(2017, 4, 27), "�ؗj��" });
            _testData.Add(new object[] { new DateTime(2017, 4, 28), "���j��" });
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