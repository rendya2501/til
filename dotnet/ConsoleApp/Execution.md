# 実行関係

## コンソールアプリのコマンドライン引数

文字列のまとまりはダブルコーテーション。  
スプリットはスペース。  

``` cs
Console.WriteLine("Hello, World!");
foreach (var arg in args)
    Console.WriteLine(arg);
```

``` txt
TestConsole "tetst  aaa" "aaa wwww"

Hello, World!
tetst  aaa
aaa wwww
```

``` txt
TestConsole 'tetst  aaa' "aaa wwww"

Hello, World!
'tetst
aaa'
aaa wwww
```

---

## コンソールアプリから別のexeを引数ありで実行する

``` cs
using System;
using System.Diagnostics;

// Processクラスのオブジェクトを作成
Process process = new Process();
// コマンドプロンプト
process.StartInfo.FileName = "cmd.exe";
// コマンドプロンプトに渡す引数
process.StartInfo.Arguments = "/c dir";
// ウィンドウを表示しない
process.StartInfo.CreateNoWindow = true;
process.StartInfo.UseShellExecute = false;
// 標準出力および標準エラー出力を取得可能にする
process.StartInfo.RedirectStandardOutput = true;
process.StartInfo.RedirectStandardError = true;

// プロセス起動
process.Start();
// 標準出力を取得
string standardOutput = process.StandardOutput.ReadToEnd();
// 標準出力を表示
Console.WriteLine(standardOutput);
```

[C#でのexeファイルの扱い方とは？exeの起動・exeの実行結果を取り込む・exeの終了を待ち合わせる・exeのパスを取得する方法](https://www.fenet.jp/dotnet/column/language/9700/)

---

## git.bashでexeを実行する方法

`./<exe_name>`  

例:カレントディレクトリのefbundle.exeを実行する  
`./efbundle`  

- 気を付けること  
  - exe名の直接指定では実行できない  
  - `.\`ではなく`./`  
  - `./`の後にスペースは入れない  

[Windows git-bash.exeでバッチファイルを実行](https://teratail.com/questions/100039)  
