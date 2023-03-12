# CommunityToolkit

---

## 概要

ユーチューブで発見したこの動画。  
[What is the MVVM pattern, What benefits does MVVM have?](https://www.youtube.com/watch?v=AXpTeiWtbC8)  
BindableBaseではなく、Attributeでバインディングできるようになっているのを発見したのでまとめた。  

`mvvm toolkit`で検索をかけても日本語のページが全然ヒットしないので、2022年8月9日時点では新しい技術な模様。  

MicroSoft曰く、今までの(BindableBaseを用いた)MVVMは`MVVM Basic`という方式らしく、愚直で標準的な実装だった模様。  
標準では(それでもそれなりにラップされているが)コード量が多く、プロパティにひと手間加えなければいけなかったのが、パッケージを導入することで緩和されるっぽい。  

[MVVM Basic からの移行](https://docs.microsoft.com/ja-jp/dotnet/communitytoolkit/mvvm/migratingfrommvvmbasic)  

---

## インストール

nugetからパッケージをインストールする。  
`CommunityToolkit.Mvvm`で検索すると`CommunityToolkit.Mvvm`なるものがヒットするのでそちらをインストールする。  

dotnetコマンドによるインストールコマンドは以下の通り  
`dotnet add package CommunityToolkit.Mvvm --version 8.1.0`  

`CommunityToolkit.Mvvm`で検索すると`Microsoft.Toolkit.Mvvm`なるものもヒットするが、`CommunityToolkit`の方をインストールすればよい。  
こちらもMicrosoftが開発しているものらしいが、非推奨の警告が出ている。  
尚且つ公式のドキュメントで解説しているのは`CommunityToolkit`の方であり、Microsoft自身も`CommunityToolkit`推しっぽいのでそっちをインストールしておけばとりあえず良い。  

Microsoft.Toolkit.Mvvmについての解説記事  
[.NET standard2.x時代のMVVMライブラリ](https://qiita.com/hqf00342/items/40a753edd8e37286f996)  

---

## .NET Frameworkでの実装

最新の技術のためなのか、.NET FrameworkのWPFではアトリビュート方式では動かなかった。  
.Net6のWPFでは問題なく動いた。  

.NET Frameworkでも.Net6と同じ書き方はできるのだが、認識される情報にずれがあるので厳しいのかもしれない。  

しかし、ObservableObjectが持つBindableBase的な機能は従来通りの書き方ができるので、.NET FrameworkのWPFでMVVM開発するなら、このパッケージをインストールするだけでよさそう。  
どちらにしても、Toolkitをインストールすることによるデメリットはあまりなさそう。  

---

## INotifyPropertyChangedアノテーションの警告

`INotifyPropertyChanged`アノテーションをクラスに付与すると警告が表示される。  
何も継承するものがない場合は`INotifyPropertyChanged`アノテーションよりも`ObservableObject`を継承したほうがパフォーマンスが良くなる模様。  
それは[こちら(MVVM Toolkit 警告 MVVMTK0032)](https://learn.microsoft.com/ja-jp/dotnet/communitytoolkit/mvvm/generators/errors/mvvmtk0032)で解説している。  

適当に何か継承するだけで警告は消えるので、何もなかったらこっちがいいよってだけの話みたい。  

``` cs
// この形にすると警告が表示される。
[INotifyPropertyChanged]
public partial class WarningViewModel
{
}

// 適当に何か継承させるだけで警告は消える
[INotifyPropertyChanged]
public partial class SampleViewModel : Hoge
{
}

public class Hoge {}
```

[MVVM Toolkit 警告 MVVMTK0032 - .NET Community Toolkit | Microsoft Learn](https://learn.microsoft.com/ja-jp/dotnet/communitytoolkit/mvvm/generators/errors/mvvmtk0032)  

---

今のところ、ここが一番解説が充実している。  
これを見ながら組めば寄り合えずのものはできるのではなかろうか。  
[CommunityToolkit.Mvvm V8 入門 - Qiita](https://qiita.com/kk-river/items/d974b02f6c4010433a9e)  

ただでさえ記事が少ないので、これも添えておく。  
[.NET用 MVVM Toolkit v8でMVVMコードを短く - Qiita](https://qiita.com/hqf00342/items/d12bb669d1ac6fed6ab6)  

公式の解説。  
分かるようでわからない。  
[Microsoft.Toolkit.Mvvm](https://docs.microsoft.com/en-us/windows/communitytoolkit/mvvm/introduction)  
