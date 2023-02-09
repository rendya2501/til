# UnitTestメモ

## xUnit

[How To Create xUnit Test In Visual Studio 2022](https://www.c-sharpcorner.com/article/how-to-create-xunit-test-in-visual-studio-2022/)  
[単体テスト (xUnit) - アルパカのメモ](https://vicugna-pacos.github.io/dotnetcore/unittest/)  
[C#： VSCodeでxUnitを使用し煩雑なテストを自動化して開発コスト削減](https://artisan.jp.net/blog/c_sharp-xunit-vscode/)  

---

## MSTest

[C# 単体テスト チュートリアル - Visual Studio (Windows) | Microsoft Learn](https://learn.microsoft.com/ja-jp/visualstudio/test/walkthrough-creating-and-running-unit-tests-for-managed-code?view=vs-2022)  

---

## InternalClassをテストする方法

``` cs
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("<TestProjectNamespace>")]
```

[【C#】internalクラスをテストする方法](https://hirahira.blog/internal-class-test/)  
