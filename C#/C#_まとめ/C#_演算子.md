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

`抑制演算子`ともいうらしい。
以下のように記述するとそういう警告が出る。  

``` cs
int? aaa = 1;
// Null 抑制演算子 ('!') が重複しています
_ = aaa !!?? throw new Exception("aa");
```

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

---

## null!の意味

C#8.0からの問題  

null免除演算子をnullにつけただけ。  
nullなのにnullではない事をアピールしている。  
なんとも奇妙だが、型システム的には実際の値には関心がないからこれでも良いらしい。  

意味的には警告を消すためだけのシグネチャ。  

`! = from Nullable to Non-Nullable`  
`? = from Non-Nullable to Nullable`  

``` cs
// line 1
// null 非許容の プロパティ 'FirstName' には、コンストラクターの終了時に null 以外の値が入っていなければなりません。プロパティ を Null 許容として宣言することをご検討ください。
public string FirstName { get; }

// line 2
// 警告なし
public string LastName { get; } = null!;

// assign null is possible
// 警告なし
public string? MiddleName { get; } = null;
```

>オッケー！？でも、null！ってどういう意味？  
>これはコンパイラに、nullはnullableな値ではないことを伝えるものです。  
>変な感じでしょう？  
>nullリテラルに演算子を適用しているので、変に見えるだけです。  
>しかし、コンセプトは同じです。
>この>場合、NULLリテラルは他の式/型/値/変数と同じです。  
>
>ヌルリテラル型は、デフォルトでヌル可能な唯一の型なのです。  
>しかし、学習したように、どのような型でもnullabilityは!でnon-nullableに上書きすることができます。  
>
>型システムは、変数の実際の値や実行時の値には関心を持ちません。  
>コンパイル時の型だけで、あなたの例では、LastNameに割り当てたい>変数（null！）はnon-nullableで、これは型システムに関する限り有効です。  

[What does null! statement mean?](https://stackoverflow.com/questions/54724304/what-does-null-statement-mean)

---

## 三項演算子で同じインタフェースを実装したクラスがなぜ暗黙的変換といわれるのか

java5以降改善されたらしい。その前までは同様の現象が起こっていた模様。  
C#も今後改善されるのかな。  

---

## 条件演算子のターゲット型推論の強化

azmさんに三項演算子でnull許可のboolを受け取るとき、「片方を変換しないといけないのキモイね」って言われたので、「受け取り側をvarじゃなくてbool?にすれば行けますよ」って言ったけどエラーになった。  
どうやらこれが有効なのはC#9からみたいで、Framework4.8のC#7.3では無理だった。  
家でやるサンプルは基本的に最新版なので、バージョンを意識することがない。  
それを意識するいい体験だったのでまとめた。  

``` C#
// この記述が許されるのはC#9から。
// これはC#9の条件演算子のターゲット型推論の強化に当たるらしい。
bool? aa = true ? false : null;

// C#9以前はこのように書くしかない。
var aa = true ? (bool?)false : null;
```
