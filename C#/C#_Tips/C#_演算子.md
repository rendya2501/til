# C#_演算子

---

## 早見表

``` txt
null合体演算子         | ??
null条件演算子         | ?.
null合体代入演算子     | ??=
null免除演算子         | 変数名の後ろに「!」をつける

複合代入演算子         | +=, -=, *=, /=, %=, &=, |=, ^=, <<=, >>=
条件演算子(三項演算子) | <evaluation> ? <true> : <false>
```

---

## null合体代入演算子

左側が null の場合、右側の値を代入する。  
8.0から使用可能  

例:  
a が null である場合に、 b の値を a に代入する。  

``` cs
int? a = null;
int b = 5;
a ??= b;
// a = 5
```

---

## null許容参照型

変数名の後ろの `!` 記号は`null免除演算子(null-forgiving operator)`と呼ばれる。  
8.0から使用可能  

null免除演算子をnull許容参照型の変数名の後ろに記述すると、その変数は null でないとみなされる。  

``` cs
string hoge = "";
string? newstr = "huga";

// ここでは、'newstr' は null である可能性があります。
// Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
hoge = newstr;

// 演算子を適応すると上記のような警告は発生しない。
hoge = newstr!;
```

null免除演算子を記述してもワーニングの発生を抑えられるだけで、nullのオブジェクトにアクセスしたことによる例外は回避できない。  

``` cs
Hoge? item = null;
_ = item!.name; //←errorが発生する

class Hoge {
    public string? name {get;set;}
}
```

[変数名の後ろに"!" がある - null 免除演算子の利用 (C#プログラミング)](https://www.ipentec.com/document/csharp-null-forgiving-operator)  

以下、null免除演算子をnull許容参照型と勘違いしてた時のまとめ  
見事に勘違いしてたので残しておく。  

>`a!.○○`の`!.`が探しても全然見つからなかった。  
>これは`!.`ではなく、`a!`までがNull許容参照型らしい。  
>IDEのnullの警告を抑制する程度の演算子らしい。  
>
>Null許容参照型は8.0以上と警告が出る。  

[null許容参照型](https://ufcpp.net/study/csharp/resource/nullablereferencetype/?p=3#null-forgiving)  
