# Java基本まとめ

## final

・クラスに付ける場合→継承の禁止  
・メソッドにつける場合→オーバーライドの禁止  
・変数に付ける場合→再代入の禁止(定数)  

---

## throws

メソッドの宣言に追加するキーワード。  
指定した例外を発生させることを宣言する。  
throwsがついたメソッドを呼び出す場合は例外の対策をしないといけない。  
それだけの宣言みたいだ。  
実際あまり使われないみたいだし、インターフェースと組み合わせた場合、  
抽象が実装に引っ張られるから失敗作だとどこかで効いたことがある。  

---

## implements

javaのインターフェース実装命令。  
C#もこれだったか？  
違う。C#は「:」で済むんだった。であれば継承は?  
継承も「:」か。  
概念は知っているが、あまり使わないからパッと出てこないな。  

extendsがjavaに置ける継承だ。  

---

## import static 命令

クラスのメンバーをクラス名を指定せずに呼び出せるようになる。  

``` Java
// Mathクラスの全てのメンバ指定
import static java.lang.Math.*;
public class Sample{
    public static void main(String[] args) {
        // 普通Math.absって呼び出すけどそうする必要がない。
        System.println(abs(-10));
    }
}
```

---

## Enum

[【初心者向け】JavaのEnum型について解説！基本的なメソッドも紹介！](https://www.tech-teacher.jp/blog/java-enum/)  
基本情報でEnumのメソッドを知らないために1問間違えたのでまとめ。  

JavaにおいてはEnumはClassの一種っぽい。  
Objectクラスを継承している。  
とりあえず、JavaのEnumにはOrdinalメソッドなるものが実装されているらしい。  
他にも4つくらいあるが、これはJavaの仕様でそう実装されているみたい。  
この際なので全部まとめる。  

・ordinalメソッド  
Enum型の宣言された順番を取得するメソッド。  
→  
基本情報で出たのはこれ。  
電卓の数字の定義くらいなら順番に取得すれば、その通りになるって寸法か。  

・valueOfメソッド  
指定した文字列と列挙型を比較するときに使います。  

``` Java
public class Test {
    protected enum Name {Tanaka, Sato, Kimura};
    public static void main(String[] args) {
        if(Name.valueOf("Tanaka") == Name.Tanaka) {
            System.out.println("同じ");
        }
    }
}
```

・valuesメソッド  
列挙型のデータすべてを取得する場合に使います。  

---

## HashMap

C#でいうListっぽいことしてるのがこれ。  
Listではaddメソッドで要素を追加するが、JavaのHashMapはput(置く)がそれに当たるらしい。  
直観的にそうだろうとは思ったが、確証がなかったので後学のためにまとめておく。  

---

## toStringが実行されるタイミングは？

``` txt
図1「メソッドmainの出力結果」の通り、"(2 + 5)"と表示するコードです。
Addition クラスの toString メソッドが正解なので add.toString() としたいですが、選択肢にはありません。
Javaの言語仕様上、System.out.println() の引数にオブジェクトを渡すと、内部ではそのオブジェクトの toString() メソッドが呼び出されます。
このため、add オブジェクトのみでも同様の動作となり、整形式が出力されます。よって、解答は「ア」です。
```

Javaの言語仕様上、System.out.println() の引数にオブジェクトを渡すと、内部ではそのオブジェクトの toString() メソッドが呼び出されます。  
なるほどね。まぁ、選択肢をヒントにして考えてみても、そういう動作してくれないと答えにならないからね。  
実行されるタイミングというよりも、何が実行するのかってのが適切かな。  

---

## ==とEqualsの比較の違いは？

オブジェクトの同一性、同値性に関する設問です。  

**同一性**  
同じオブジェクトを参照していることです。「==」で比較します。  

**同値性**  
オブジェクトの値が等しいことです。比較する値は各クラスによって独自に実装します。  
全てのクラスの親であるObjectクラスが実装しているequalsメソッドでのチェックは同一性チェック。  

問題文に「二つのインスタンスを表す値が等しいので true になるべきだが、今のクラス Constant の実装では false になる」という記述があります。  
「値」なので同値性チェックとなる。  
同値性チェックで true にしたい場合、Javaでは equals() メソッドをオーバーライドします。  
equals() メソッドは、java.lang.Object クラスに実装されており、全てのクラスは Object クラスのサブクラスです。  
よって、オーバーライドしない場合、Object クラスの equals() メソッドが呼ばれます。  
そして、Objectクラスのequalsメソッドでのチェックは同一性チェックです。  

new Constant(9) == new Constant(9)
同一性チェックのため問題文に合いません。

new Constant(9).equals(new Constant(9))
正しい。

new Constant(9).evaluate() == new Constant(9).evaluate()
同一性チェックのため問題文に合いません。

new Constant(9).evaluate().equals(new Constant(9).evaluate())
コンパイルエラーです。evaluate() メソッドの戻り値はプリミティブ型(int型)です。

``` Java : Constantクラスにequalsメソッドを実装する場合の例
@Override
public boolean equals(Object obj) {
    if (obj instanceof Constant) {
        return this.evaluate() == ((Constant)obj).evaluate();
    } else {
        return false;
    }
}
```

---

## foreach

[拡張for文(for-each文)を使って要素を順に取得する](https://www.javadrive.jp/start/for/index8.html)  

Javaに置けるforeachは「拡張for文」というものを使って実現するらしい。  
どうやらJava7までは拡張for文がforeach代わりになっていて、Java8からLinqみたいな感じのforEach命令が追加されたらしい。  
C#みたいに書く、foreach文はJavaには存在しない模様。  
JavaScriptのforeachもこんな感じだったような。  
まぁ、どちらにせよ、何やりたいかわかるので別にいいや。  

``` Java : 拡張For文・Foreach
for (データ型 変数名: コレクション){
    // 繰り返しの中で実行される処理
    ...
}

String name[] = {"Suzuki", "Katou", "Yamada"};
for (String str: name){
  System.out.println(str);
}

// foreachで書くならこうなる
name.forEach(s -> System.out.println(s));
```
