# MVC(Model View Controler)

MVCとは、設計手法の一つ。  
実装を、Model-View-Controllerの三つの領域に分けて行う。  
この設計を用いることで、画面変更が容易になったりする。  

[やはりお前らのmvcは間違っている](https://www.slideshare.net/MugeSo/mvc-14469802)  
[PHPerのMVCの一体どこが間違っていたのか](https://mugeso.hatenadiary.org/entry/20121224/1356345261)  
[MVCとは？ MVVMとは？ 特徴やメリットはなにか](https://demi-urge.com/mvc-mvvm-features/)  
[MVC、本当にわかってますか？](https://qiita.com/tshinsay/items/5b1724baf32b8b5113c2)  
[Model-View-Controller(MVC) 再考](https://qiita.com/gomi_ningen/items/5b23be2df8c42a199703#msdn%E3%81%AEmvc%E8%A8%98%E4%BA%8B%E3%81%BE%E3%81%A8%E3%82%81)  

---

## 使用目的

ビジネスロジックを、画面表示から独立させるために用いる。  
→画面変更の影響を、ビジネスロジックが受けないようにするため。  

---

## 前提条件

画面表示は頻繁に変更されるが、ビジネスロジックはあまり変更されない。  
したがって、画面変更によって、ビジネスロジックを変更しないですむようにしたい。  

---

## MVCの責務

### Model

Viewから独立した処理を担う。

### View

UI、データの表示、装飾、などを担う。
例１、データ入力で、数値入力させるか、セレクトボックスで選ばせるか。
例２、同じデータでも、数値表示するか、グラフ表示するか。
など、どのように人に見せるかを担う。

### Controller

ViewとModelをつなぐ役割。

---

## 依存関係

### Model

独立

### View

Modelに依存

### Controller

ViewとModelに依存

---

## MVCを利用するメリット・デメリット

メリット
Viewを変更しやすい

デメリット
MVCの構造は冗長。コード量が増え、構造が複雑化する。

---

## なぜMVCは廃れたのか

[mvcモデルがもう古いという理由を教えてください。](http://w3q.jp/t/9347)  

Smalltalkから生まれたのだからそりゃ  
80年代の概念だしな。業界からみたらだいぶ古いほうだね。  

アプリケーションの肥大化に対応しづらい。  
web系のアプリケーション開発の規模が一昔前より増大しているから。  
Controlerが肥大化しやすい。

ModelとControlerの境界が曖昧なので練度の低い人間がMVCを採用するとスパゲッティになりやすい。
Modelに業務ロジックを記述すべきなのに、ルーティングや制御係のControllerに業務ロジックを記載し、Controllerを太らせてしまうことを言います。
