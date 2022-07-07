# MVC(Model View Controler)

MVCとは、設計手法の一つ。  
実装を、Model-View-Controllerの三つの領域に分けて行う。  
この設計を用いることで、画面変更が容易になったりする。  

[CRUD with MVP pattern, C#, WinForms and SQL Server](https://www.youtube.com/watch?v=WSBy_Ypgk38)  
この動画を見る限り、MVPはWeb,Mobile向けのアーキテクチャーな模様。  
でも、MVPとMVVMが全てで使えるなら、そっち使ったほうがいいよね。  

[やはりお前らのmvcは間違っている](https://www.slideshare.net/MugeSo/mvc-14469802)  
[PHPerのMVCの一体どこが間違っていたのか](https://mugeso.hatenadiary.org/entry/20121224/1356345261)  
[MVC、本当にわかってますか？](https://qiita.com/tshinsay/items/5b1724baf32b8b5113c2)  
[Model-View-Controller(MVC) 再考](https://qiita.com/gomi_ningen/items/5b23be2df8c42a199703#msdn%E3%81%AEmvc%E8%A8%98%E4%BA%8B%E3%81%BE%E3%81%A8%E3%82%81)  

[MVCモデルについて](https://zenn.dev/dd_sho/articles/36abe63831d909)

---

## なぜMVCは廃れたのか

[mvcモデルがもう古いという理由を教えてください。](http://w3q.jp/t/9347)  

>Smalltalkから生まれたのだからそりゃ  
>80年代の概念だしな。業界からみたらだいぶ古いほうだね。  
>
>アプリケーションの肥大化に対応しづらい。  
>web系のアプリケーション開発の規模が一昔前より増大しているからControlerが肥大化しやすい。  
>
>ModelとControlerの境界が曖昧なので練度の低い人間がMVCを採用するとスパゲッティになりやすい。  
>Modelに業務ロジックを記述すべきなのに、ルーティングや制御係のControllerに業務ロジックを記載し、Controllerを太らせてしまうことを言います。  

MVCは廃れたという話だったが、そうなるのも必然かもしれない。  
感想としては矢印が多すぎるので、実装している途中で責務が曖昧になってコントローラーが肥大化しそう。  
でもって、矢印に一貫性がない。  
MVCは調べていくと、あっちではこういう風に紹介されているが、こっちでは全く別のことを言っている、という感じが多々ある。  
ここまで情報が錯綜しているということは、それだけ複雑だということ。  
複雑なのはすぐに排除されるので、そりゃ廃れるわなって感じ。  

少なくとも、今更採用するモデルではないような？  
ViewとModelの分離の歴史の一つとして認識する程度でいいかもしれない。  

---

## MVCモデルとは

役割ごとにModel, View, Controllerに分割してコーディングを行うモデル  

- Model --> システムの中でビジネスロジックを担当する  
- View --> 表示や入出力といった処理をする  
- Controller --> ユーザーの入力に基づき，ModelとViewを制御する  

![a](https://res.cloudinary.com/zenn/image/fetch/s--ihPNJ9nz--/c_limit%2Cf_auto%2Cfl_progressive%2Cq_auto%2Cw_1200/https%3A//qiita-image-store.s3.amazonaws.com/0/139470/200f787f-17ef-cbf9-3c7c-896acf7e3fba.png)  

---

## 使用目的

ビジネスロジックを、画面表示から独立させるために用いる。  
→  
画面変更の影響を、ビジネスロジックが受けないようにするため。  
画面表示は頻繁に変更されるが、ビジネスロジックはあまり変更されない。  
したがって、画面変更によって、ビジネスロジックを変更しないですむようにしたい。  

---

## MVCモデルの処理の流れ

Viewが処理を受け取るのではなく、Controllerが処理を受け取る。  
必要とあらば、受け付けた内容からModelを呼び出し、結果を受け取る。
受け取った結果をコントローラーはViewへ通知する。  
Viewは受け取った内容を画面に反映させ、ユーザーに通知する。  

ユーザーからの入力があった場合の処理の流れ。  

![a](https://res.cloudinary.com/zenn/image/fetch/s--dnSuJ5_O--/c_limit%2Cf_auto%2Cfl_progressive%2Cq_auto%2Cw_1200/https%3A//qiita-image-store.s3.amazonaws.com/0/139470/c65f3051-84ee-7e8a-8255-fd68272e6740.png)  

Model --> 実際にデータの処理を行う  
View --> Modelの状態を表示する  
Controller --> ModelとViewに処理をお願いする  

MVCモデルを参考にする前と後で特に違いが出る点

- ControllerはModelとViewの操作だけを記述すれば良いため肥大化を避けることができる．  
- Viewはもらった値を表示するのではなく，Modelを参照してModel内のデータの状態を表示する．  

## メリット

1. 機能毎に分割されているため，分業して作業を進めやすくなる(各人の得意な所に集中できる)
2. それぞれが独立しているので変更・修正があった場合にその影響を受けにくい

## デメリット

MVCの構造は冗長。コード量が増え、構造が複雑化する。  

---

## MVCの責務

### Model

Viewから独立した処理を担う。  
独立している。  

### View

UI、データの表示、装飾、などを担う。
例１、データ入力で、数値入力させるか、セレクトボックスで選ばせるか。
例２、同じデータでも、数値表示するか、グラフ表示するか。
など、どのように人に見せるかを担う。

Modelに依存している。  

### Controller

ViewとModelをつなぐ役割。
ViewとModelに依存  
