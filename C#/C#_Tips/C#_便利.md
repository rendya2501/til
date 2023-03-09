# 便利コードまとめ

## イミディエイト クリップボード コピー

`Clipboard.SetText(コピーしたい文字列)`  

[クリップボードに文字列をコピーする、クリップボードから文字列を取得する](https://dobon.net/vb/dotnet/string/clipboard.html)  
Clipboard.SetTextメソッドを使えば、簡単に文字列をクリップボードにコピーできます。  

VB6で長いSQLを取得する時にお世話になったでそ。  
最新の環境でも普通に使えたのでメモ。  
これでStringBuilderをToStringしたときに、長ったらしく横に展開されるあれも楽に取ることができる。  
内容的にVisualStudio側に置いてもいいのだが、とりあえずC#のほうに置いておく。  

---

## イミディエイトウィンドウでオブジェクトの中身を出力する方法

`?object.ToString()` で行ける。  

sqlのパラメータを取得したい時、カーソルを当てたときのメニューでは長すぎると「...」で切れてしまうので、全部取得したくなった。  
真っ先に浮かんだのはjsonにシリアライズすることだったが、どうSystem.~~~からやろうとしてもエラーになる。  
Usingはビルド時にそのクラスの一番上で定義されていなければ無理な模様。  
結果的にToStringだけで出力できることを発見して、事実それで解決できた。  

一応、シリアライズを頑張ったやつも乗せておく。  

[.NET 内で JSON のシリアル化と逆シリアル化 (マーシャリングとマーシャリングの解除) を行う方法](https://docs.microsoft.com/ja-jp/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-6-0)  
→  
`System.Text.Json.JsonSerializer.Serialize(param)`  

[C# Object引数の中身をtextに羅列したい](https://oshiete.goo.ne.jp/qa/5988834.html)  
→  
>nullでないことを条件として，ToStringメソッドを呼び出せば，何らかの文字列が得られるはずです。  
>他にも，GetTypeメソッドを呼べば元の型が (ほぼ) 得られますし，リフレクションを使えば元の型を知らなくてもフィールド等の値を得られますから，頑張ればそちらでも情報を得られると思います。  

---

## ASP.Netでデバッグメッセージを出力する

`System.Diagnostics.Debug.WriteLine(“hogehoge”);`  

[[C#] ASP.NET（C＃）でデバッグメッセージをConsole.WriteLineみたいに出力する。](https://hokatsu.sakura.ne.jp/c-sharp/output-in-aspnet-like-console-writeline/)  

---

## C#でURLを既定のブラウザで開く

[C#でURLを既定のブラウザで開く](https://qiita.com/tsukasa_labz/items/80a94d202f5e88f1ddc0)  

なんかうまいこといかない場合もある模様。  
次必要になったらまとめる。  

``` C#
Process.Start(
    new ProcessStartInfo()
    {
        FileName = "https://www.google.co.jp",
        UseShellExecute = true,
    }
);
```

---

## IL(中間言語の確認)

<https://sharplab.io/>

[C#で手軽にILや内部を確認するなら「SharpLab」！](https://qiita.com/RyotaMurohoshi/items/a6a252915f11f7559efe)  
