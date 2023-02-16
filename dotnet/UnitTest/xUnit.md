# xUnit

.NET Frameworkや.Net Coreで利用できる単体テストツール。  
.NET用のユニットテストフレームワーク。  

他にもMSTestとかNUnitとかあるらしいが、一番人気らしい。  

正式名称は`xUnit.net`  

---

基本的にxUnitプロジェクト単体でもデバッグができる。  
なので簡単なクラスの動作確認や言語仕様の確認だけならテストプロジェクトだけで十分かも。  

---

## xUnitプロジェクトの作成

基本的なプロジェクト作成コマンド  
`dotnet new xunit`  

dotnetコマンドがあるので、それを叩けばよい。  
-nや-oの要領は他のプロジェクトと同じ。  

VisualStudioの場合でも、xUnit用のプロジェクトテンプレートがある。  
プロジェクト作成画面からテストで検索すれば出てくるので苦労することはないだろう。  

---

## テストの実行

基本となるコマンド  
`dotnet test`  

これを実行するとすべてのテストが実行される。  
実行したいテストを選択したりすることもできるので、さらに詳しく見たいなら公式を参考にされたし。  
[選択した単体テストの実行 - .NET | Microsoft Learn](https://learn.microsoft.com/ja-jp/dotnet/core/testing/selective-unit-tests?pivots=xunit)  

VisualStudioの場合は、起動プロジェクトを上部のコンボボックスから選択できるので、それで。  

---

## VSCodeでxUnitを使う

ここら辺を参考にしながらやれば問題ない。  
[VisualStudioCode + xUnit + テスト用の環境変数読み込み](https://zenn.dev/gatabutsu/articles/18a9696e0854be)  

基本的には、テストプロジェクトを作成したら、ソリューションに追加すればよろしい。  

---

## テストの属性

xUnitには、テストを定義するために使用できる2つの主要な属性、`Fact`と`Theory`がある。  
単一のテストデータの場合は`Fact`を、複数のテストデータの場合は`Theory`を使えばよい。  

### Fact

単体のテストメソッドを表し、特定の入力に対して単一の結果を返す場合に使用する属性。  
テストに対する前提条件が常に同じ場合に適している。  

- テストコードが固定されており、データを動的に生成する必要がない場合
- 期待される結果が単一である場合  
- 期待される結果がテストコードにハードコードする場合  

``` cs
[Fact]
public void TestAdd()
{
    Assert.Equal(4, 2 + 2);
}
```

### Theory

複数のテストケースをまとめて定義する場合に使用する属性。  
テストに対する前提条件が異なる場合に適している。  
異なるテストデータを複数回使用してテストを実行することができるため、コードをより効率的に共有できる。  

- データを動的に生成する必要がある場合  
- 複数の期待される結果をテストする場合  
- 期待される結果がテストコードにハードコードされない場合  

``` cs
[Theory]
[InlineData(2, 2, 4)]
[InlineData(3, 4, 7)]
[InlineData(10, 20, 30)]
public void TestAdd(int a, int b, int expected)
{
    Assert.Equal(expected, a + b);
}
```

---

## Assertクラス

|アサート名|概要|
|---|---|
|Equal            |一致を確認                                             |
|NotEqual         |不一致を確認                                           |
|Same             |インスタンスの一致を確認(同じ参照であること)           |
|NotSame          |インスタンスの不一致を確認(異なる参照であること)       |
|Contains         |含まれることを確認                                     |
|DoesNotContain   |含まれないことを確認                                   |
|InRange          |範囲の間であることを確認                               |
|NotInRange       |範囲の間に含まれないことを確認                         |
|IsAssignableFrom |代入できるかを確認                                     |
|Empty            |初期値であることを確認                                 |
|NotEmpty         |初期値でないことを確認                                 |
|True             |True であることを確認                                  |
|False            |False であることを確認                                 |
|IsType           |指定の型(タイプ)であることを確認                       |
|IsNotType        |指定の型(タイプ)ではないことを確認                     |
|Null             |Null であることを確認                                  |
|NotNull          |Null でないことを確認                                  |
|Throws           |指定した例外が発生することを確認                       |
|ThrowsAny        |指定した例外かそのサブクラスの例外が発生することを確認 |

---

``` cs
using System;
using Xunit;

public class FactTests
{
    /// <summary>
    /// Equal 一致を確認
    /// </summary>
    [Fact]
    public void TestEqual()
    {
        Assert.Equal(4, 2 + 2);
    }

    /// <summary>
    /// NotEqual 不一致を確認
    /// </summary>
    [Fact]
    public void TestNotEqual()
    {
        Assert.NotEqual(5, 2 + 2);
    }

    /// <summary>
    /// Same インスタンスの一致を確認(同じ参照であること)
    /// </summary>
    [Fact]
    public void TestSame()
    {
        var obj1 = new Object();
        var obj2 = obj1;
        Assert.Same(obj1, obj2);
    }

    /// <summary>
    /// NotSame インスタンスの不一致を確認(異なる参照であること)
    /// </summary>
    [Fact]
    public void TestNotSame()
    {
        var obj1 = new Object();
        var obj2 = new Object();
        Assert.NotSame(obj1, obj2);
    }

    /// <summary>
    /// Contains 含まれることを確認
    /// </summary>
    [Fact]
    public void TestContains()
    {
        var list = new[] { 1, 2, 3 };
        Assert.Contains(2, list);
    }

    /// <summary>
    /// DoesNotContain 含まれないことを確認
    /// </summary>
    [Fact]
    public void TestDoesNotContain()
    {
        var list = new[] { 1, 2, 3 };
        Assert.DoesNotContain(4, list);
    }

    /// <summary>
    /// InRange 範囲の間であることを確認
    /// </summary>
    [Fact]
    public void TestInRange()
    {
        var value = 5;
        Assert.InRange(value, 1, 10);
    }

    /// <summary>
    /// NotInRange 範囲の間に含まれないことを確認
    /// </summary>
    [Fact]
    public void TestNotInRange()
    {
        var value = 15;
        Assert.NotInRange(value, 1, 10);
    }

    /// <summary>
    /// IsAssignableFrom 代入できるかを確認
    /// </summary>
    [Fact]
    public void TestIsAssignableFrom()
    {
        Assert.IsAssignableFrom<Object>(new Object());
    }

    /// <summary>
    /// Empty 初期値であることを確認
    /// </summary>
    [Fact]
    public void TestEmpty()
    {
        var list = new int[0];
        Assert.Empty(list);
    }

    /// <summary>
    /// NotEmpty 初期値でないことを確認
    /// </summary>
    [Fact]
    public void TestNotEmpty()
    {
        var list = new[] { 1, 2, 3 };
        Assert.NotEmpty(list);
    }

    /// <summary>
    /// True True であることを確認
    /// </summary>
    [Fact]
    public void TestTrue()
    {
        var result = true;
        Assert.True(result);
    }

    /// <summary>
    /// False False であることを確認
    /// </summary>
    [Fact]
    public void TestFalse()
    {
        var result = false;
        Assert.False(result);
    }

    /// <summary>
    /// IsType 指定の型(タイプ)であることを確認
    /// </summary>
    [Fact]
    public void TestIsType()
    {
        var obj = new Object();
        Assert.IsType<Object>(obj);
    }

    /// <summary>
    /// IsNotType 指定の型(タイプ)ではないことを確認
    /// </summary>
    [Fact]
    public void TestIsNotType()
    {
        var obj = new Object();
        Assert.IsNotType<int>(obj);
    }

    /// <summary>
    /// Null Nullであることを確認
    /// </summary>
    [Fact]
    public void TestNull()
    {
        Object obj = null;
        Assert.Null(obj);
    }

    /// <summary>
    /// NotNull Nullでないことを確認
    /// </summary>
    [Fact]
    public void TestNotNull()
    {
        var obj = new Object();
        Assert.NotNull(obj);
    }

    /// <summary>
    /// Throws 指定した例外が発生することを確認
    /// </summary>
    [Fact]
    public void TestThrows()
    {
        Action act = () => { throw new ArgumentException(); };
        Assert.Throws<ArgumentException>(act);
    }

    /// <summary>
    /// ThrowsAny 指定した例外かそのサブクラスの例外が発生することを確認
    /// </summary>
    [Fact]
    public void TestThrowsAny()
    {
        Action act = () => { throw new ArgumentException(); };
        Assert.ThrowsAny<Exception>(act);
    }
}

public class TheoryTests
{
    [Theory]
    [InlineData(2, 2, 4)]
    [InlineData(3, 4, 7)]
    [InlineData(10, 20, 30)]
    public void TestAdd(int a, int b, int expected)
    {
        Assert.Equal(expected, a + b);
    }

    [Theory]
    [InlineData("hello world", 'o', true)]
    [InlineData("hello world", 'z', false)]
    [InlineData("", 'x', false)]
    public void TestContains(string str, char c, bool expected)
    {
        Assert.Equal(expected, str.Contains(c));
    }

    [Theory]
    [InlineData("hello world", "world", true)]
    [InlineData("hello world", "goodbye", false)]
    [InlineData("", "", true)]
    public void TestContainsString(string str, string sub, bool expected)
    {
        Assert.Equal(expected, str.Contains(sub));
    }
}
```

---

## 参考

ここが一番充実しているので、見ながらやれば問題ないだろう。  
[xUnit.net でユニットテストを始める - Qiita](https://qiita.com/takutoy/items/84fa6498f0726418825d)  

VSCodeでテストする場合に参考になると思われる。  
[C#： VSCodeでxUnitを使用し煩雑なテストを自動化して開発コスト削減](https://artisan.jp.net/blog/c_sharp-xunit-vscode/)  

[単体テスト (xUnit) - アルパカのメモ](https://vicugna-pacos.github.io/dotnetcore/unittest/)  
[VisualStudioCode + xUnit + テスト用の環境変数読み込み](https://zenn.dev/gatabutsu/articles/18a9696e0854be)  
[How To Create xUnit Test In Visual Studio 2022](https://www.c-sharpcorner.com/article/how-to-create-xunit-test-in-visual-studio-2022/)  
