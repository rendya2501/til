# JSON(JavaScript Object Nation)

[JSONについて調べてみた](https://qiita.com/chihiro/items/dd1912c1406dbfe16b72)  
<https://products.sint.co.jp/topsic/blog/json#:~:text=JSON%E3%81%A8%E3%81%AF%E3%80%8CJavaScript%E3%81%AE,%E3%81%A8%E3%81%A6%E3%82%82%E7%B0%A1%E5%8D%98%E3%81%AB%E3%81%A7%E3%81%8D%E3%81%BE%E3%81%99%E3%80%82>  

---

## JSONとは

データ表現記法の1つ。  
JavaScriptのオブジェクトの書き方を元にしたデータ定義方法・データ交換フォーマット  
様々な言語でサポートされている。  
主に各プログラミング言語間のデータの受け渡しに利用される。  

---

## 形式

Jsonの構成、要素の名前

キー・バリュー
変数名と値


`{}`で囲う。
基本はキーとバリューの組み合わせ。  
配列ならインデックスでアクセスし、オブジェクトならキーを指定してアクセスできる。
オブジェクトの中に配列を入れることもできるし、配列の要素をオブジェクトにすることもできる。

逆にわからないのは、配列の[]と内部の{}の場合だな。
これさえわかれば、申し分ないだろう。

``` json
{
  "key1": [],
  "key2": []
}

{
  "Array1" : [
    {"obj1" : 1},
    {"obj2" : 2}
  ]
}

{
  "name": "taro",
  "age": 23,
  "sex": "man",
  "hobby": {
    "test" : 12,
    "outdoor": {
      "land": ["running", "walking"],
      "sea": ["swimming", "fising"]
    },
    "indoor": ["movie", "game", "card"]
  }
}
```

``` JS
6. print(json_object["hobby"]["outdoor"]["land"][1])
7. print(json_object["hobby"]["indoor"])
8. print(json_object["hobby"]["test"])

1. waking
2. ['movie', 'game', 'card]
3. 12
```

内部{}はオブジェクト。javascriptのあれ。
なのでPerson['name'][0]とかでアクセスできる。

``` JS
var person = {
  name: ['Bob', 'Smith'],
  age: 32,
  gender: 'male',
};
```