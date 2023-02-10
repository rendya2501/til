# xUnit

正式名称は`xUnit.net`  

.NET Frameworkや.Net Coreで利用できる単体テストツール。  
.NET用のユニットテストフレームワーク。  

他にもMSTestとかNUnitとかあるらしいが、一番人気らしい。  

---

## テストの属性

次のコードは、xUnitで用意されている全てのテストを実行する方法を示しています。

``` cs
using System;
using Xunit;

namespace XUnitTestProject
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Your test logic here
        }

        [Theory]
        [InlineData(1, 2, 3)]
        [InlineData(3, 4, 7)]
        public void Test2(int a, int b, int expected)
        {
            // Your test logic here
            Assert.Equal(expected, a + b);
        }

        [Fact]
        public void Test3()
        {
            // Your test logic here
            Assert.Throws<DivideByZeroException>(() => 1 / 0);
        }

        [Fact]
        public async void Test4()
        {
            // Your async test logic here
        }
    }
}
```

この例では、4つのテストが定義されています：

Test1：Fact属性を持つ、単一のテストメソッドです。
Test2：Theory属性を持つ、複数のテストデータに対するテストメソッドです。
Test3：例外を投げるテストメソッドです。
Test4：非同期テストメソッドです。
このように、xUnitには様々な種類のテストを実行するためのアトリビュートが用意されています。詳細については、xUnitのドキュメンテーションを参照してください。

---

## Assertクラス

|アサート名|概要|
|---|---|
|Equal            |一致を確認|
|NotEqual         |不一致を確認|
|Same             |インスタンスの一致を確認|
|NotSame          |インスタンスの不一致を確認|
|Contains         |含まれることを確認|
|DoesNotContain   |含まれないことを確認|
|InRange          |範囲の間であることを確認|
|NotInRange       |範囲の間に含まれないことを確認|
|IsAssignableFrom |代入できるかを確認|
|Empty            |初期値であることを確認|
|NotEmpty         |初期値でないことを確認|
|True             |True であることを確認|
|False            |False であることを確認|
|IsType           |型一致を確認|
|IsNotType        |型不一致を確認|
|Null             |Null であることを確認|
|NotNull          |Null でないことを確認|
|Throws           |指定した例外が発生することを確認|
|ThrowsAny        |指定した例外かそのサブクラスの例外が発生することを確認|

True：真偽値式が真であることをテストします。
False：真偽値式が偽であることをテストします。

Null：オブジェクトがnullであることをテストします。
NotNull：オブジェクトがnullでないことをテストします。

Same：2つのオブジェクトが同じ参照であることをテストします。
NotSame：2つのオブジェクトが異なる参照であることをテストします。

Throws：指定されたアクションが特定の例外をスローすることをテストします。

IsType：オブジェクトが指定のタイプであることをテストします。
IsAssignableFrom：オブジェクトが特定のタイプか、そのサブタイプであることをテストします。

---

[How To Create xUnit Test In Visual Studio 2022](https://www.c-sharpcorner.com/article/how-to-create-xunit-test-in-visual-studio-2022/)  
[単体テスト (xUnit) - アルパカのメモ](https://vicugna-pacos.github.io/dotnetcore/unittest/)  
[C#： VSCodeでxUnitを使用し煩雑なテストを自動化して開発コスト削減](https://artisan.jp.net/blog/c_sharp-xunit-vscode/)  
[xUnit.net でユニットテストを始める - Qiita](https://qiita.com/takutoy/items/84fa6498f0726418825d)  
