//using BankAccountNS;

using PathCheck;

namespace PathCheckTest;

public class UnitTest1
{
    [Theory]
    [InlineData(".\\SettingInfoTest")]
    [InlineData(".\\SettingInfoTest\\")]
    [InlineData(".\\SettingInfoTest\\jsconfig.Hoge.json")]
    public void Path_OK_Test(string inputPath)
    {
        var builder = new PathBuilder(inputPath);
    }

    [Theory]
    [InlineData(".\\SettingInfoTes2t")]
    [InlineData(".\\SettingInfoTest\\jsconfig.Hog2e.json")]
    public void Path_NG_Test(string inputPath)
    {
        Assert.Throws<Exception>(() => { new PathBuilder(inputPath); });
    }
}
