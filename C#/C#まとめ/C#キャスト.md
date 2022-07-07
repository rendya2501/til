# キャスト

---

## 「is」,「as」「前かっこ」による変換の違い

- 前かっこは変換に失敗するとエラー(System.InvalidCastException)になる。  
- as は変換に失敗するとnullになる。エラーにはならない。  
- is はif文で使うので、失敗したらelseに流れる。  

---

## TryParseとConvertの違い

[C#のint.ParseとConvert.ToInt32の違いをマスター](https://www.wareko.jp/blog/master-difference-between-csharp-int-parse-and-convert-toint32)  

・空文字列 → int.Parse(“”), Convert.ToInt32(“”) は共に例外になる  
・null → int.Parse(null) は例外になる。Convert.ToInt32(null) はゼロになる。  

``` C#
// TryParseを1文で済ませたかったらこう書く。因みにTryParseには文字列しか渡せない。なのでうまくできたが、使えなかった。
FrontIsPrintCopy = bool.TryParse(_BasicInfo.CopyPrintType.GetValueOrDefault(), out bool frontIsPrintCopy) && frontIsPrintCopy;
// _BasicInfo.CopyPrintTypeがnullの場合はGetValueOrDefaultで0になってToBooleanでfalseになる。
FrontIsPrintCopy = System.Convert.ToBoolean(_BasicInfo.CopyPrintType.GetValueOrDefault());
```

---

## is演算子によるnullチェック

クラスAのインスタンスを作ります。  
インスタンス変数にnullを代入します。  
それをオブジェクト型変数bに代入します。  
bをAにキャストした場合どうなるか。  
→  
nullの場合は失敗するが、nullを代入しないと成功する。  
でもなんで？  

``` C#
public class Hello{
    class A {
    }

    public static void Main(){
        A a = new A();
        a = null; // ※
        object b = a;
        
        if (b is A aa) {
            // nullを代入しないとこちら
            System.Console.WriteLine("Success");
        } else {
            // a = nullの場合はこちら
            System.Console.WriteLine("Fail");
        }
    }
}
```

<https://ufcpp.net/study/csharp/datatype/typeswitch/>  
>元々のis演算子の仕様でもあるんですが、nullには型がなくて常にisに失敗します(falseを返す)。  

なるほどね。  
is演算子は実行時の変数の中身を見るが、nullは型がないので、エラーになるわけか。  

しかし、言語仕様で困ったらマイクロソフトではなく未確認飛行Cを見るのが一番だな。  
こっちのほうがわかりやすい。  

``` C#
string x = null;

if (x is string)
{
    // x の変数の型は string なのに、is string は false
    // is 演算子は変数の実行時の中身を見る ＆ null には型がない
    Console.WriteLine("ここは絶対通らない");
}
```

---

## アップキャスト/ダウンキャスト

[C#：アップキャストとダウンキャストについて、サンプルソース付きで解説してみる](https://www.kakistamp.com/entry/2018/04/29/010258)  

**子から親へのキャスト→アップキャスト**  
**親から子へのキャスト→ダウンキャスト**  

・アップキャストは、常に安全に行える  
・ダウンキャストは、エラーが発生する事がある。  
→
・キャストは型を変更するだけで、中身が消える訳ではない。（アクセスできる範囲が変わるだけで、実態は存在している）  
・インスタンス作成時、メモリに領域が割り当てられる。  
・子クラスのインスタンスは、親クラスのインスタンスより多くの領域を必用とする。  
・「親→子」とキャストした場合、子クラスが必要とする領域に、メモリが割り当てられていない状態となる。（そのため、エラーとして弾いている。）  

``` C#
//親クラス
class ParentClass { }
//子クラス
class ChildClass : ParentClass { }

private void Test()
{
    //===========================
    //      アップキャスト
    //===========================
    //子クラスのインスタンスを生成
    ChildClass child01 = new ChildClass();
    //子クラスのインスタンスを、親クラスに代入可能。
    ParentClass parent01 = child01;

    //===========================
    //  ダウンキャスト（エラー）
    //===========================
    ChildClass child02;
    //親クラスのインスタンスを生成
    ParentClass parent02 = new ParentClass();
    //キャストできるかどうか確認（falseになります）
    if (parent02 is ChildClass)
    {
        //キャスト時にエラーが発生する
        child02 = (ChildClass)parent02;
    }
    //asを使えば キャストできない場合 nullが入る
    child02 = parent02 as ChildClass;

    //===========================
    //     ダウンキャスト
    //===========================
    //親クラスのインスタンスを、子クラスで生成
    ParentClass parent03 = new ChildClass();
    //ダウンキャスト可。
    ChildClass child03 = (ChildClass)parent03;
}
```

ダウンキャストがうまくいかない理由はここが詳しく解説している。  
[なぜアップキャストは安全で、ダウンキャストは危険なのか](https://qiita.com/RYO-4947123/items/eaeb48b6fcf97c02710f)  

アップキャストとは？  
基底（親）クラス型の変数に派生（子）クラスのインスタンスを代入する際に行われる型変換です。  

ダウンキャストとは？  
派生（子）クラス型の変数に基底（親）クラスのインスタンスを代入する際に行われる型変換です。  

子クラスを親クラスにアップキャストして問題ないのは、親クラスで定義した領域を確保している状態での代入になるから。  
親クラスを子クラスにダウンキャストして問題なのは、子クラスが実装している分の領域を確保しない状態で代入する事になるから。  
こんなところだよな。  
だから、親クラスを子クラスにダウンキャストしたかったら、親クラスの変数に子クラスのインスタンスを代入して、それを子クラスにキャストするって操作が必要になる。  
そうすれば、子クラスで必要な領域を確保しつつ親クラスで実装した機能のみが使える制限された状態になり、そのまま子クラスに代入しても、  
領域は確保しているから問題ないし、子クラスにキャストする事で制限された機能を解放する事になるから。  

ここも面白い。  
[【Java】アップキャストとダウンキャスト](https://se-tomo.com/2019/09/16/%E3%80%90java%E3%80%91%E3%82%A2%E3%83%83%E3%83%97%E3%82%AD%E3%83%A3%E3%82%B9%E3%83%88%E3%81%A8%E3%83%80%E3%82%A6%E3%83%B3%E3%82%AD%E3%83%A3%E3%82%B9%E3%83%88/)  

アップキャストをすることによって、そのインスタンスでできることが制限されます。
子クラスは
・親クラスの機能
・子クラス独自の機能
の２つを持っていますが、親クラスにアップキャストしたことにより

・親クラスの機能
・（使用不可能）子クラスの機能
では、アップキャストにより親クラスで定義された機能しか利用できなくなる例を見てみましょう。  

``` Java : アップキャスト
package try_catch_01;

class A {}
class B extends A{
    public void method() {
        System.out.println("methodB");
    }
}

public class Main {
    public static void main(String[] args) {
        B b = new B();
        b.method();
        
        // アップキャスト
        A b2a = (A) b;
        b2a.method();//未定義エラー
    }
}
```

つまり、
・アップキャスト後の実態はあくまでサブクラス
・アップキャストにより、アップキャストで定義されたものしか利用できなくなる

``` Java : ダウンキャスト
class A {}
class B extends A{
    public void method() {
        System.out.println("methodB");
    }
}

public class Main {
    public static void main(String[] args) {
        // *****ダメな例******//
        // Aのインスタンス
        A a = new A();
        // Aがもつ機能を無理やり拡張はできない
        B a2b = (B)a;

        // *****OKな例******//
        // BのインスタンスをAの機能に制限した状態
        A a = new B();
        // 制限を解除した。無理やり拡張している訳でない。
        B a2b = (B)a;
    }
}
```

上記のように、

・実態がBクラスであるが、型はAクラス
・つまり、変数aは型Bを型Aに暗黙的にアップキャストした状態
・それは、実態はBクラスだが、Aの範囲に使用制限をかけた状態
・変数aをBクラスに戻し（ダウンキャスト）、アップキャストによる使用制限を解除
