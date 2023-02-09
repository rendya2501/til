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
        //Assert.Equal<Exception>()
        Assert.Throws<Exception>(() => { new PathBuilder(inputPath); });
    }


    public void Debit_WithValidAmount_UpdatesBalance()
    {
        //// Arrange
        //double beginningBalance = 11.99;
        //double debitAmount = 4.55;
        //double expected = 7.44;
        //BankAccount account = new BankAccount("Mr. Bryan Walton", beginningBalance);

        //// Act
        //account.Debit(debitAmount);

        //// Assert
        //double actual = account.Balance;
        // Assert.AreEqual(expected, actual, 0.001, "Account not debited correctly");
    }
}
