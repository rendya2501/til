# MVVM(Model View ViewModel)

[世界で一番短いサンプルで覚えるMVVM入門](https://resanaplaza.com/%E4%B8%96%E7%95%8C%E3%81%A7%E4%B8%80%E7%95%AA%E7%9F%AD%E3%81%84%E3%82%B5%E3%83%B3%E3%83%97%E3%83%AB%E3%81%A7%E8%A6%9A%E3%81%88%E3%82%8Bmvvm%E5%85%A5%E9%96%80/)  
[[C#/WPF/MVVM]今さらMVVMについて調べた](https://qiita.com/mitsu_at3/items/49d21ba5fb38f15ea922)  
[WPF4.5入門 その60「データバインディングを前提としたプログラミングモデル」](https://blog.okazuki.jp/entry/2014/12/23/180413)  
[プログラミングな日々](https://days-of-programming.blogspot.com/2017/11/mvvm.html)  

- Model／View／ViewModelの3つの責務にGUIアプリケーションを分割するデザインパターン、またはプログラム構造  
- 画面と処理を分離する作り方  

[カズキ]  
MVVMパターンは、View（XAML + コードビハインド）とViewModelと呼ばれるModelをViewに適したインターフェースに変換するレイヤと、アプリケーションを記述するModelのレイヤからなります。ViewとViewModel間は、基本的にデータバインディングによって連携を行います。ViewModelはModelの変更を監視したり、必要に応じてModelのメソッドの呼び出しを行います。この関係を図で表すと以下のようになります。

[世界で一番短いサンプルで覚えるMVVM入門]  
画面と処理を分離すると、プログラム開発の分業がやり易くなります。
また、画面と処理が分離することで、仕様変更における修正箇所が必要最小限に抑えられるというメリットもあります。

処理とUIを切り離すのが目的。
WindownFormはコードビハインドにビジネスロジックもコントロールのイベントも全部載せていたので、UI制御とビジネスロジックが1つのコードの中に含まれてしまっていた。  

---

## 目的

- デザイナーとプログラマーの分担開発  
→ 生産性の向上  
- 画面(プレゼンテーション・ロジック)と処理(ビジネス・ロジック)を明確に分離する  
→コードの見通しの良さ、テストが容易になる  

### テスト容易性向上(テスタビリティ)

画面とコードがくっついてるとユニットテストがし辛い。  
そこで画面(View)とコード(ViewModel/Model)に分離してやるとやりやすくなる。  

例  

- 文字が小さい、背景色が変などの表示 → View（XAML）をチェック  
- あるデータがDBと表示で数が違う・ずれているなど、ViewとModelが絡むもの → ViewModelをチェック  
- 保存データの形式が変、DBとの通信など、Viewに無関係 → Modelをチェック  

### 生産性向上(プロダクティヴィティ)

画面(View)とコード(ViewModel/Model)が分離してると、デザイナは画面(View)、プログラマはコード(ViewModel/Model)と明確に分業できる。  

### 移植性向上(ポータビリティ)

画面(View)とコード(ViewModel/Model)が分離してると、画面(View)を用意するだけで他のプラットフォームに対応できる・・・かもしれない。  
と言うのも、これについて触れられている情報を殆ど見かけなくて、あくまで勝手なイメージ。  
ただ理論上は可能なはず・・・。  

---

## 各クラスの役割

ViewはViewModelに依存し、ViewModelはModelに依存します。  
逆方向の依存はありません。  

### View

画面。それ以上でもそれ以下でもない。  
ViewModelが提供するプロパティに沿って画面を定義する。  

- 画面デザイン  
- 対応するViewModelのインスタンス（ViewはViewModelに依存、DataContext）  
- コントロールがViewModelのどのプロパティと連動するか（Binding）  

### ViewModel

画面とロジックを結ぶ橋渡し役。  
画面からどんな情報が欲しいか定義して、ロジックの求める形で情報を渡して、ロジックから結果を貰って、結果を画面に渡してあげる。  
大体どんな画面か知っていて、画面寄りの処理（入力チェックとか）はこの人がやるべき？（不明瞭）  

- 対応するModelのインスタンス（ViewModelはModelに依存）  
- 画面の入力データを受け取る  
- 入力データがModelに渡せるかどうかの入力チェック  
- エラーをViewに通知（INotifyDataErrorInfo）  
- ボタンなどのイベント処理（ICommand）  
- 入力データをModelの求める形に変換して渡す  
- Modelのプロパティ変更通知を受け取る  
- プロパティ変更をViewに通知（INotifyPropertyChanged）  

### Model

ロジック。やるべき処理をやる人。  
ViewModelからパラメータを貰って処理を行う。  

- ViewModelからパラメータを受け取る  
- パラメータのエラーチェック  
- エラーをViewModelに通知（INotifyDataErrorInfo）  
- 実際にやりたい処理  
- 結果をプロパティに格納  
- プロパティ変更をViewに通知（INotifyPropertyChanged  

### ViewModelとControllerの違い

ViewModelは、ViewとViewModelの二つで一組である。  
ViewModelはViewと対応した「データ」と「処理(関数)」を持つが、Controllerは必ずしもViewと対応した「データ」や「処理」は持たない。  
これがMVVMとMVCの違いである。  

なお、ModelとViewの橋渡しを行う点は、ViewModelもControllerと同じである。  

### aaaaa

[DIYプログラミングでMVVMを推奨しない理由](https://resanaplaza.com/2020/08/13/%E3%80%90wpf%E3%80%91diy%E3%83%97%E3%83%AD%E3%82%B0%E3%83%A9%E3%83%9F%E3%83%B3%E3%82%B0%E3%81%A7mvvm%E3%82%92%E6%8E%A8%E5%A5%A8%E3%81%97%E3%81%AA%E3%81%84%E7%90%86%E7%94%B1/)  


[MVVMの「ViewModelはViewを知ってはいけない」の意図を知りたい。](https://teratail.com/questions/292367)  


MVVMで言われている、「ViewModelはViewを知ってはいけない(依存してはいけない)」というのは、
Viewが持つインスタンスをViewModelが使用することを問題としており、
System.Windows.MediaのDrawingGroupやSystem.Windows.ShapesのRectangleなどをViewModel内で使用し、Viewに公開することは問題ではない理解で合っていますか？
(System.Windows.ShapesのRectangleを使うことは無いかもしれませんが…)

以下のような記事をいくつか読みました。
https://note.com/sigsky/n/n0617c2a2cb60

ベストアンサー

依存とは
たとえばユーザープログラマーがライブラリのクラスを使う時、このユーザーの書いたコードはそのライブラリに依存します。

ライブラリがバージョンアップした場合、ユーザーはそれに対応するためにコードを書き直す必要にせまられることがあります。これを「ユーザーのコードはそのライブラリに依存している」と言います。

しかし逆に、ユーザがいくら自分のコードをバージョンアップしても、ライブラリの作者はライブラリに手を入れる必要はありません。これを「そのライブラリはユーザーの書いたコードに依存していない」と言います。

相互依存
このように依存が片方から片方の場合、依存している方のバージョンアップに依存されている方は付き合う必要がありません。

しかし、お互いに依存した場合はどうなるでしょうか？

特に結合が密であればあるほど問題は深刻になりますが、片方がバージョンアップすればもう片方はそれに従ってバージョンアップしなければいけません。

するとそのバージョンアップに従ってもう片方が更にバージョンアップし、それに従ってもう一方的が更に更にバージョンアップし……と、さながらトランプの塔のように、どこか一カ所崩れたら全て崩れる様相になります。

これが相互依存です。

これを避けるため、プログラマーはコードをモジュール化し、相互依存を無くし、結合を疎にするのです。

View と ViewModel
さて View と ViewModel の関係ですが、V は VM のメソッド(あるいはコマンド)を呼び、プロパティを変更します。ユーザーコードがライブラリを呼ぶのと同じ関係ですね。つまり、V は VM に依存します。

ここで VM が V に依存するとどうなるでしょうか。

相互依存になりますね。

相互に依存しても最初のうちは問題になりません。しかし、後から手を入れる際の労力が半端なく大きいものになる可能性があります。

---

綺麗なMVVMの実装。  
[Foreign keys in local databases with SQLite-net and .NET MAUI - DEV Community](https://dev.to/icebeam7/foreign-keys-in-local-databases-with-sqlite-net-and-net-maui-22a1)  
[GitHub - icebeam7/DemoFK](https://github.com/icebeam7/DemoFK)  

MVVMに置けるService(通信部分)を実装する時に参考になる。  
[C# Simple Interfaces: Service. Software is an art form in so many… | by Justin Coulston | Dev Genius](https://blog.devgenius.io/c-simple-interfaces-service-d9d1921912e4)  

ViewModelはModelの情報をViewが扱えるように加工をするだけ  
[MVVMとはなんぞやを公理から求めてみる - 滅入るんるん](https://blog.meilcli.net/2018/10/21/165056)  
