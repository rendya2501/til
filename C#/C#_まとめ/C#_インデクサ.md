# インデクサ(indexer)

---

## 概要

インデクサは独自のクラス内部にあるデータに対して、外部から配列と同じ様にアクセスさせるための方法です。

※indexer  
①索引作成者  
②データベースの索引を作るプログラムや人の事。  

[【C#】図で理解するインデクサの基礎](https://resanaplaza.com/%e3%80%90c%e3%80%91%e5%9b%b3%e3%81%a7%e7%90%86%e8%a7%a3%e3%81%99%e3%82%8b%e3%82%a4%e3%83%b3%e3%83%87%e3%82%af%e3%82%b5%e3%81%ae%e5%9f%ba%e7%a4%8e/#i-5)  

---

## インデクサの実装

- 通常のプロパティの変数名の部分を `this [型 変数名]` のように実装する。  
- インデックスとして整数以外の値 (文字列やオブジェクトなど) も使用することができる。  

``` C#
// 通常のプロパティ
public 型 変数名 { get; set; }

// インデクサ
public 型 this[型 変数名] { get; set; }
```

---

## 数値型のインデクサの例

``` C#
public class MyClass
{
    // 要素を保持するList
    public List<string> Values { get; set; } = new List<string>();
    
    // インデクサ
    // MyClassが内部に保持するValuesリストに対して、インデックスを介してアクセスする事ができる。
    public string this[int index] { get { return Values[index]; } set { Values[index] = value; } }
    
    // 要素を追加するメソッド
    public void Add(string str)
    {
        Values.Add(str);
    }
}
```

---

## インデクサがない場合

インデクサーをサポートしていないJavaにおいて、配列リストを表すコレクションの要素へのアクセスは、Listインターフェースのget/setメソッドによって提供される。  

``` Java
var list = new java.util.ArrayList<Integer>(java.util.Collections.nCopies(10, 0));
// index 番目の要素に値を設定。
// void set(int index, E element)
list.set(2, 100);
// index 番目の要素を取得。
// E get(int index)
int val = list.get(2);
```

C#のインデクサでは、配列リストの要素へのアクセスを配列のアクセスと同じように記述することができる。  

``` C#
var list = new System.Collections.Generic.List<int>(new int[10]);
list[2] = 100;
int val = list[2];
```

[Java使いがC#を勉強する　その④　インデクサ](https://shironeko.hateblo.jp/entry/2017/02/09/202843)

---

## 所感

これがなかったらDictionaryは使い物にないことがわかった。  
キーでバリューにアクセスできるのはインデクサのおかげだったんだな。  
割と感動した。  

Listでも、配列のようにアクセスできるのはインデクサーのおかげということがわかった。  
本当に、配列のようにアクセスできる仕組みを提供する機能なんだなーってことがわかりました。  

因みにインデクサを提供するインターフェースはIListらしい。  
→  
[列挙可能から完全なるモノまで – IEnumerableの探索 – C# Advent Calendar 2014](https://www.kekyo.net/2014/12/14/4587)  
>IListインターフェイスには「インデクサ」が定義されています。  
このインデクサを使用可能にするため、敢えてIListインターフェイスを実装しているのではないかと推測しています。  

DictionaryはIDctionaryが提供していたので、厳密にIListで無ければいけないというわけではないみたいだ。  
