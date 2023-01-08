# Singleton

---

## 概要

インスタンスが1個しか存在しないことを保証するパターン  

- 指定したクラスのインスタンスが1つしか存在しないことを保証したい  
- インスタンスが1個しか存在しないことをプログラム上で表現したい  

---

## シングルトンがなぜ必要なのか

インスタンスがなぜ一つじゃないといけないのか  
使えるリソースが一つだけの場合、それにアクセスするクラスインスタンスも一つだけの方が都合が良い場合が多いからです。  

例えば、画面に何か出力するとき、JavaではSystem.outを使いますが、それは「標準出力」というたった一つしかないリソースに対してアクセスを行うために、staticフィールドで定義されたインスタンスであり、staticなのでプロセス内には1個しかインスタンスがありません。  
なぜそうしているのかというと、画面に何か書きたいときに、いちいちnewしてクラスインスタンスを作るのは面倒だし、そのたびにnewしていたら無駄にメモリを食うし、それを避けるために一度newしたものを別のクラスでも使い回そうとすると、そのインスタンスの受け渡しをどうするのかという問題が出てくるし……。  
ということで、staticフィールドにインスタンスを作ってそれをみんなで使い回すことにするのです。  

---

## どのような場面でシングルトンを使うのか

シングルトン(パターン)はインスタンスを一つしか作らないことを「保証する」ための仕組みです。  
ただ単にインスタンスを一つにしたいというだけならstaticフィールドにすればできますが、それを知らずに誰かがnewしてしまったらインスタンスが複数になってしまいます。  
あるいは、staticフィールドにインスタンスを作りたいけど、初期化の順番をコントロールしたい、というケースもあるかもしれません。  
マルチスレッドでの動作を考慮しないといけないかもしれません。  
そういうときに一手間かけてシングルトン(パターン)を使います。  

- たくさんのインスタンスからアクセスされると困るとき  
- 毎回、インスタンス生成をするのが手間  
- 共通的な情報保持をして、メモリを節約したい  

---

## Singletonパターンが適さないケース

- 多くの状態(filed、メンバ変数)を持つ : 状態はグローバル変数と同一なので、その数は0に近い方が良い  
- 単体テストの実行順に制約を生む場合 : 単体テストの作成および実行を困難にしてしまう  
- マルチスレッド環境に導入する場合 : Singeltonへのアクセス管理(ロック)にコストが発生してしまう  
- 常に決まった値を渡す(or 出力) : クラスメソッド(static関数)に置き換えた方が良い  

---

## Singletonパターンが適するケース

以下に、Singletonパターンで設計した方が良い可能性のあるクラス(機能)を示します。  

- ロギング → 常に１つのインスタンスからファイルへ出力したい  
- 標準出力 → 画面は1つなので1つのインスタンスから出力  
- 取得したJSONデータをどの画面でも使う時  
- キャッシュ管理  
- スレッドプール管理  
- データベース接続ドライバ  
- ソケット制御ドライバ  

上記のクラス(機能)を踏まえると、リソース管理にSingletonパターンが適していると考えられます。  
リソースに対して複数のインスタンスが管理を行えば、状態管理で不整合が生じる可能性があります。  
この不整合を防止するために、Singletonを用いるのは自然な事だと思います。  

---

## 実装

``` C#
    /// <summary>
    /// シングルトンクラス
    /// 1.Singletonクラスがロードされた時点では、Singletonインスタンスは生成されない
    /// 2.Singleton#getInstance()を最初に呼び出した時に、SingletonHolderクラスがロードされ、Singletonインスタンスが生成される
    /// ギリギリまでSingletonインスタンスを生成しないような挙動になるようです。
    /// </summary>
    class Singleton
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        private Singleton() { }
        /// <summary>
        /// インスタンス
        /// </summary>
        /// <returns></returns>
        public static Singleton Instace { get; } = SingletonHolder.INSTANCE;
        /// <summary>
        /// シングルトン生成クラス
        /// </summary>
        private static class SingletonHolder
        {
            public static readonly Singleton INSTANCE = new Singleton();
        }
    }
```

``` C# : Java版Singleton
public class Singleton
{
    private static Singleton singleton = new Singleton();
    private Singleton() {
        Console.WriteLine("インスタンスを生成しました。");
    }
    public static Singleton GetInstance() {
        return singleton;
    }
}
```

---

## 参考

[デザインパターン「Singleton」](https://qiita.com/shoheiyokoyama/items/c16fd547a77773c0ccc1)  
[[Swift]Singleton(シングルトン)とは？メリット、実装方法、使い方](https://ticklecode.com/swiftsingleton/)  
[シングルトンがなぜ必要なのか](https://teratail.com/questions/36721)  
[【Singeltonパターン】考え方は単純だが、使いどころが大切なデザインパターン【コード例はRubyとJava】](https://debimate.jp/2020/04/26/%E3%80%90singelton%E3%83%91%E3%82%BF%E3%83%BC%E3%83%B3%E3%80%91%E8%80%83%E3%81%88%E6%96%B9%E3%81%AF%E5%8D%98%E7%B4%94%E3%81%A0%E3%81%8C%E3%80%81%E4%BD%BF%E3%81%84%E3%81%A9%E3%81%93%E3%82%8D%E3%81%8C/)  