# パス関係まとめ

---

## カレントディレクトリ

[C#でカレントディレクトリを取得する](https://qiita.com/ot-nemoto/items/1282e10d3a02db4f490a)  

``` C#
// 現在の作業ディレクトリの完全修飾パスを取得
Console.WriteLine(System.Environment.CurrentDirectory);
// アプリケーションの現在の作業ディレクトリを取得
Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
// 現在実行中のコードを格納しているアセンブリの、マニフェストを格納している読み込み済みファイルの完全パスまたは UNC 位置の親ディレクトリを取得
Console.WriteLine(System.IO.Directory.GetParent(Assembly.GetExecutingAssembly().Location));
// アセンブリを探すためにアセンブリ リゾルバーが使用したベース ディレクトリを取得
Console.WriteLine(System.AppDomain.CurrentDomain.BaseDirectory);
// 現在のプロセスに対するコマンド ライン引数を格納している文字列配列の先頭（ファイルパス）のディレクトリ情報
Console.WriteLine(System.IO.Path.GetDirectoryName(System.IO.Path.GetFullPath(Environment.GetCommandLineArgs()[0])));
// 現在アクティブなプロセスのメイン モジュールの完全なパスのディレクトリ情報
Console.WriteLine(System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName));
```

実務でConfigを取得する時に使っていたのは `AppDomain.CurrentDomain.BaseDirectory` だった。

要件によって切り替える必要がある。  

- 実行ファイルと同じディレクトリにファイルを出力させるプログラムを作成。  
- また、このプログラムは、他のアプリケーションから呼ばれることを想定している。  

この場合 `System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName))`  

---

## 相対パスの指定の仕方

[DOBON.NET](https://dobon.net/vb/dotnet/file/getabsolutepath.html#getfullpath)  

カレントパスの取得がなければ、基準となるのはexeがある場所。  

``` C#
// C:\Users\Desktop\CSharpSample1\CSharpSample1\SandBox1\bin\Debug\file.txt
Console.WriteLine(System.IO.Path.GetFullPath(@"..\file.txt"));
// C:\Users\Desktop\CSharpSample1\CSharpSample1\SandBox1\bin\file.txt
Console.WriteLine(System.IO.Path.GetFullPath(@"..\..\file.txt"));
```

---

## AppData\Localフォルダのパスを取得

``` C# : ユーザのAppData\Localフォルダのパスを取得
// C:\Users\ユーザー名\AppData\Roaming
string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
```
