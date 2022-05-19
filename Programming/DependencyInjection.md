# DependencyInjection

[DI (Dependency Injection) 依存の注入とは](https://demi-urge.com/dependency-injection/)  
[DI って何でするのかわからない人向けに頑張って説明してみる「本来の意味」](https://qiita.com/okazuki/items/0c17a161a921847cd080)  
[DI (依存性注入) って何のためにするのかわからない人向けに頑張って説明してみる](https://qiita.com/okazuki/items/a0f2fb0a63ca88340ff6)  

## DIの概説

DI (Dependency Injection、依存の注入)とは、設計方法の一つ。  
部品(クラス)同士の直接的な依存をなくし、部品をより抽象的なものに依存させる。  
これによりテストしやすくなったり、部品(クラス)の変更が容易になったりする。  

## 使用目的

DIによって部品同士の直接的な依存をなくすことで、テストのときに、スタブやモックなどを簡単に使えるようにする  
部品(クラス)の変更や交換を容易にする  
など  

## DIの考え方

クラス（実体）に依存させるのはやめて、インタフェース（抽象、ルール）に依存させようということ。  
