# MVVM_Toolkit

---

## 概要

ユーチューブで発見したこの動画。  
[What is the MVVM pattern, What benefits does MVVM have?](https://www.youtube.com/watch?v=AXpTeiWtbC8)  
BindableBaseではなく、Attributeでバインディングできるようになっているのを発見したのでまとめた。  

`mvvm toolkit`で検索をかけても日本語のページが全然ヒットしないので、2022年8月9日時点では新しい技術な模様。  

MicroSoft曰く、今までの(BindableBaseを用いた)MVVMは`MVVM Basic`という方式らしく、つまり愚直で標準的な実装だった模様。  
標準では(それでもそれなりにラップされているが)コード量が多く、プロパティにひと手間加えなければいけなかったのが、パッケージを導入することで緩和されるっぽい。  

[MVVM Basic からの移行](https://docs.microsoft.com/ja-jp/dotnet/communitytoolkit/mvvm/migratingfrommvvmbasic)  

---

## インストール

nugetからパッケージをインストールするだけでよい。  

- `CommunityToolkit.Mvvm ver8.0.0以上`  

Microsoftからも出てるけど、Versionの警告が出るし、CommunityToolkitが推奨されてるっぽいのでそっちをインストールすべし  
[Microsoft.Toolkit.Mvvm](https://docs.microsoft.com/en-us/windows/communitytoolkit/mvvm/introduction)  

[.NET standard2.x時代のMVVMライブラリ](https://qiita.com/hqf00342/items/40a753edd8e37286f996)  

---

## 実装

最新の技術のためなのか、.NetFrameWorkのWPFではアトリビュート方式では動かなかった。  
.Net6のWPFでは問題なく動いた。  
.NetFrameWorkでも.Net6と同じ書き方はできるのだが、認識される情報にずれがあるので厳しいのかもしれない。  

しかし、ObservableObjectが持つBindableBase的な機能は従来通りの書き方ができるので、.NetFrameWorkのWPFでMVVM開発するなら、このパッケージをインストールするだけでよさそう。  
どちらにしても、Toolkitをインストールすることによるデメリットは皆無っぽい。  
