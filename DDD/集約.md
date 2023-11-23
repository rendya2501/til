# DDD_集約

## 集約_概要

集約は、永続化の単位となる、クラスの塊のこと。  
Order と OrderDetail は同時に生まれ、同時に保存されるとした場合、 Order や OrderDetail をまとめたのが「集約」となる。  
「集約」の一番親を成すクラスである Order のようなものを「集約ルート」と呼ぶ。  

**Repository は集約の単位で作成する。**  
OrderRepository を作成し、OrderDetailRepository は作成しない。  
OrderDetail の永続化に関する処理は OrderRepository の内部に隠蔽する。  
Repository パターンは、本来この単位で作成することが前提となるデザインパターン。  

[「集約」でデータアクセスの 3 つの課題に立ち向かう ~ 大量の Repository・整合性のないオブジェクトのロード・N + 1 問題 ~](https://qiita.com/os1ma/items/28f5d03d3b92e6a1e1d8)  

---

## 集約をまたがったJoin

Order-OrderDetail,Product-ProductDetail が別の集約である場合、それらを同時に取得して画面に表示する場合はどうすべきか？  

- アプリケーションでJoinする  
- CQRS  

集約をまたがった JOIN について考えていくと、CQRS に到達する。  

集約をまたがった処理については Repository で無理に実施するのではなく、別途 QueryService (+ QueryModel) のようなクラスを作成し、特別なクエリとしてデータベースから取得する。  

---
