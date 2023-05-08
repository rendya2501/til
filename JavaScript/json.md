# Json

## JavaScriptのObject

端的に言うと、「キーとバリューの集合」。  
配列変数もオブジェクトの一種に当たる。  

>ほとんどのものはオブジェクト  
>JavaScriptにおける通常のオブジェクトは、メンバ（名前）と値を覚えることのできる入れ物で、他の言語でいうところのハッシュテーブルや連想配列みたいなものです。  
>
>オブジェクトリテラルでオブジェクト生成  
>オブジェクトへのメンバの登録はオブジェクトリテラルという形式でもできます。  
>メンバ名の後ろに`:`と値を書き、`,`で区切って、中カッコ`{}`で囲む形式です。  
>[[JavaScript] オブジェクトの基礎](https://qiita.com/yoshi389111/items/245df2d642e49d2acf3a)  

``` js
var json_object = {
  name: ['Bob', 'Smith'],
  age: 32,
  gender: 'male',
  hobby: {
    outdoor: {
      land: ['running','waking']
    },
    indoor: ['movie', 'game', 'card'],
    test: 12
  }
};

json_object.age
// 32
json_object["hobby"]["outdoor"]["land"][1]
json_object.hobby.outdoor.land[1]
// waking
json_object["hobby"]["indoor"]
json_object.hobby.indoor
// ['movie', 'game', 'card']
json_object["hobby"]["test"]
json_object.hobby.test
// 12
```

---

## JSON文字列 → オブジェクトへの変換

JSON.parse()に渡すことでオブジェクトに変換されます。  
あとは通常のオブジェクトの扱いと同じで、ドットで繋げながら取得したい要素にアクセスしています。

``` js
var json_str = '{"id":1, "name":"tanaka", "attribute":{"gender":"male", "phone_number":"xxxxxxxxxxx", "birth":"1991/01/01"}}';
var obj = JSON.parse(json_str)
console.log(obj.name)
console.log(obj.attribute.birth)
// tanaka
// 1991/01/01
```

---

## オブジェクト → JSON文字列への変換

JSON.stringify()に渡してJSON文字列に変換しています。  
typeofで型を見てみると、stringになっているのが分かります。  
普段JavaScript内で使用するときはオブジェクトで良いですが、サーバサイドとデータのやり取りをしたい場合は文字列にして送ると非常に便利です。  

``` js
var obj = {"id":1, "name":"tanaka", "attribute":{"gender":"male", "phone_number":"xxxxxxxxxxx", "birth":"1991/01/01"}}
var json_str = JSON.stringify(obj)
console.log(json_str)
console.log(typeof json_str)
// {"id":1,"name":"tanaka","attribute":{"gender":"male","phone_number":"xxxxxxxxxxx","birth":"1991/01/01"}}
// string
```

---
