# UnitTestメモ

---

## MSTest

[C# 単体テスト チュートリアル - Visual Studio (Windows) | Microsoft Learn](https://learn.microsoft.com/ja-jp/visualstudio/test/walkthrough-creating-and-running-unit-tests-for-managed-code?view=vs-2022)  

---

## InternalClassをテストする方法

プロジェクト内の任意のファイルに以下の2行のコードを記載することで、テストプロジェクトに対してのみinternalクラスを公開することができる模様。  

``` cs
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("<TestProjectNamespace>")]
```

xUnitでも、MSTestやNUnitでも問題ない模様。  

[【C#】internalクラスをテストする方法](https://hirahira.blog/internal-class-test/)  
