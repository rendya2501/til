# Equalsメソッドと等値演算子の違い

---

## 発端

Javaの時にもまとめたかもしれないが、改めて==とEqualsの違いをまとめる。  
事の発端はY君がEqualsで文字列の比較をしていて、左辺の文字列がnullで.Equalsメソッドで比較しようとしてエラーになるのを発見したためだ。  
文字列の比較にEqualsを使う必要性は何だったのかわからなかったので、そもそもどういう違いがあるのか、==ではダメだったのかを調べた。  

>C#ではNULLがあり得る単純な文字列比較の場合、“==”を使用した方が良いです。  
[バグのもと!?”==”と”Equals”の使い分け](https://fledglingengineer.xyz/equals/)  

→
早速結論が出た。  

[==演算子とEqualsメソッドの違いとは？［C#］](https://atmarkit.itmedia.co.jp/ait/articles/1802/28/news028.html)  
[2つの値が等しいか調べる、等値演算子(==)とEqualsメソッドの違い](https://dobon.net/vb/dotnet/beginner/equality.html)  
[【C#】文字列を比較する（== 演算子、Equalメソッド、Compareメソッド）](https://nyanblog2222.com/programming/c-sharp/193/)  

---

## == 値の等価

値の等価とは、比較する2つのオブジェクトの中身が同じであるということです。  

``` C#
string a = new string("Good morning!");
string b = new string("Good morning!");

if(a == b) // True
{
    Console.WriteLine("True!");
}
```

こちらは”a”と”b”の中身の文字列を比較しており、中身の文字列が一致しているためTrueとなります。  
![a](https://fledglingengineer.xyz/wp-content/uploads/2020/09/image-24.png)  

---

## Equals 参照の等価

一方で、参照の等価とは、比較する両者が同じインスタンスを参照しているということです。  

``` C#
string a = new string("Good morning!");
string b = new string("Good morning!");

if (a.Equals(b)) // True
{
    Console.WriteLine("True!");
}
```

![q](https://fledglingengineer.xyz/wp-content/uploads/2020/09/image-22.png)

---

## オブジェクトの比較

先程の値の参照に”(object)”を付けた場合どうなるか?  
その場合、オブジェクトの比較となり、“a”と”b”はそれぞれ異なるオブジェクトのため、Falseとなる。  

``` C#
string a = new string("Good morning!");
string b = new string("Good morning!");

if ((object)a == (object)b) // False
{
    Console.WriteLine("True!");
}
```

![s](https://fledglingengineer.xyz/wp-content/uploads/2020/09/image-25.png)

---

## NULLの判定は”==”を使うべき

``` C#
string name1 = "Mike";
string name2 = "Mike";

if(name1 == name2) // True
{
    Console.WriteLine("True!");
}
```

上記の場合は、特にインスタンスを生成しているわけではないので、“==”を使用しても、”Equals”を使用しても値の等価となり、一見問題ないように考えられます。  

しかし、以下の場合はどうなるでしょうか?

``` C#
// “name1″にnullが入るかもしれないため、nullチェックを設けたとする。  
string name1 = null;

// “==”を使用したif文は正常に動作し、Trueを返す。  
 if(name1 == null) // True
{
    Console.WriteLine("True!");
}

// 一方で、”Equals”を使用したif文では例外が発生する。  
// こちらはコンパイル時に、エラーとはならないため、思わぬバグを生んでしまう可能性があるため単純な文字列の比較は==が無難。  
if (name1.Equals(null)) // System.NullReferenceException: 'Object reference not set to an instance of an object.'
{
    Console.WriteLine("True!");
}
```

---

## 等値演算子とEqualsメソッドの違い

C#では、値型の比較に==演算子を使うと「値の等価」を調べることになります。  
参照型の比較に==演算子を使うと、通常は「参照の等価」を調べます。  
しかし、String型のように、クラスで等値演算子がオーバーロードされているならば、参照型でも==演算子で「値の等価」を調べます。  

Equalsメソッドは、値型の比較に使うと、「値の等価」を調べます。  
参照型の比較に使うと、通常は「参照の等価」を調べます。  
しかし、String型のように、クラスのEqualsメソッドがオーバーライドされていれば、参照型でも「値の等価」を調べます。  

「参照の等価」を調べるためには、Object.ReferenceEqualsメソッドを使用することもできます。  
さらにC#では、Object型にキャストしてから==演算子で比較することでも、確実に参照の等価を調べることができます。  

---

## Stringクラス

等値演算子とEqualsメソッドで値の等価を調べることができるクラス（等値演算子がオーバーロードされ、かつ、Equalsメソッドがオーバーライドされているクラス）は多くありません。  

その代表は、Stringクラスです。  
その他にもVersionクラスなどもそのようですが、とりあえずStringクラスはこのように特別なクラスであることを覚えておいてください。  
このようなクラスでは、参照型にもかかわらず、等値演算子やEqualsメソッドで「値の等価」を調べることができます。  

---

## 結局、どちらを使うべきか

値型の等価は==演算子で調べるのが良いでしょう。  
参照型で値の等価を調べるには、Equalsメソッドを使うのが確実でしょう。  
参照型で明確に参照の等価を調べたいならば、Object.ReferenceEqualsメソッド（またはObject型にキャストしてから==演算子）を使います。

``` C#
//値型の等価を調べる
int i1 = 1;
int i2 = i1 * i1;
Console.WriteLine(i1 == i2); //true
Console.WriteLine(i1.Equals(i2)); //true
Console.WriteLine(object.Equals(i1, i2)); //true

//参照型の等価を調べる
//o1とo2は別のインスタンス
object o1 = new object();
object o2 = new object();
Console.WriteLine(o1 == o2); //false
Console.WriteLine(o1.Equals(o2)); //false
Console.WriteLine(object.Equals(o1, o2)); //false

//o1とo2は同じインスタンス
o2 = o1;
Console.WriteLine(o1 == o2); //true
Console.WriteLine(o1.Equals(o2)); //true
Console.WriteLine(object.Equals(o1, o2)); //true

//String型の等価を調べる
//s1とs2は同じ値だが、別のインスタンス
string s1 = new string('a', 10);
string s2 = new string('a', 10);
Console.WriteLine(s1 == s2); //true
Console.WriteLine(s1.Equals(s2)); //true
Console.WriteLine(object.ReferenceEquals(s1, s2)); //false
Console.WriteLine(object.Equals(s1, s2)); //true
```

``` C# : なんか個人でいろいろ頑張った後
    string A = "AA";
    string B = "BB";

    Console.WriteLine(A.Equals(B));
    Console.WriteLine( A == B);
    Console.WriteLine(Equals(A, B));

    B = "AA";

    Console.WriteLine(A.Equals(B));
    Console.WriteLine(A == B);
    Console.WriteLine(Equals(A, B));

    A = null;
    B = null;

    Console.WriteLine(A == B);
    Console.WriteLine(A?.Equals(B) ?? false);
    Console.WriteLine(Equals(A, B));
```
