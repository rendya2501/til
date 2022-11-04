# Dispose

---

## Disposeとは？

>C#ではメモリの解放を自動的にしてくれます。  
だから通常は意識しなくても不要になったメモリは解放されていきます。  
しかし、アプリケーションの外側にアクセスした場合は話が変わってきます。  
例えばデータベースにアクセスしたり、ファイルにアクセスした場合です。  
それらのファイルはアプリケーションの外側に存在しているため、ファイルを開いたままとかにすると、ずっとリソースが解放されずに残ってしまいます。  
そこで、外部のリソースを使用したら、使い終わったことを明示的に意思表示する必要があります。  
そのためのメソッドがDisposeになります。  
[C#初心者のための基礎！Disposeとusingの意味と使い方を解説#24](https://anderson02.com/cs/cskiso/cskisoniwari-24/)  

---

## マネージリソースとアンマネージリソース

■**マネージリソース**  

管理対象リソースは、純粋な .NET コードであり、ランタイムによって管理され、その直接制御下にあるリソースです。  

ガベージコレクションがメモリを管理してくれるリソース。  
マネージ(管理)リソースとは IDisposable を実装したオブジェクトのことである  

■**アンマネージリソース**  

ファイルのオープン、ストリーム、ハンドル、固定メモリ、COM オブジェクト、データベース接続など、主に外部との接続関係において必要となる資源を指す模様。  
これらのオープン、クローズはガベージコレクションや.Netの管理対象外であり、プログラマー自身が接続を管理する必要がある。  

ガベージコレクションが管理しないリソース。  
開発者が責任を持ってメモリを管理する必要があるリソース。  

おそらくだが、IDisposableを実装していないが、OpenCloseのメソッドを持ったクラス等はこれに該当すると思われる。  

[What is meant by "managed" vs "unmanaged" resources in .NET?](https://stackoverflow.com/questions/3607213/what-is-meant-by-managed-vs-unmanaged-resources-in-net)  

---

## IDisposable

リソースの解放が必要なインスタンスであることを表現するためのインターフェース。  

IDisposableインターフェースは、概要の通り「使い終わったらリソースを開放する必要がある」ということを表現するためにある。
「使い終わったリソースを開放する」ためのDispose()というメソッドが1つあるだけ。  
逆にいえば、「使い終わったらリソースを開放する必要がある」ようなクラスはIDisposableインターフェースを実装していることになる。  

---

## メモリの解放とオブジェクトの破棄との混同

>オブジェクトを作成し、破棄するというこの一連の流れは、メモリを確保し、解放するという C 言語で多用される流れとよく似ています。  
>そのため、この二つを混同して Dispose メソッドによりメモリが解放されると思ってしまう人がいます。  
>  
>しかし、これは間違いです。  
>先ほど説明したように、メモリの解放はガベージコレクションによって自動的に行われます。  
>それは IDisposable インターフェースを実装するオブジェクトも例外ではありません。  
>  
>Dispose メソッドの役割は、メモリを解放することではなく、使い終わったオブジェクトの後処理をすることです。  
>例えば FileStream の場合は new によってファイルが開かれるので、Dispose はそのファイルを閉じる役割を果たします。  
>これを「リソースの解放」と呼びます。  
>開いたファイルを別のプログラムが書き換えようとすると失敗しますから、それを閉じて別のプログラムから書き換えることができるようにするのがファイルリソースを解放するということになります。  
>[C# のファイナライザ、Dispose() メソッド、IDisposable インターフェースについて](https://qiita.com/Zuishin/items/9efc9c8cbb98300bbc64)

---

## オブジェクトの破棄とは

>オブジェクトの破棄とはオブジェクトによって占められているメモリを解放することではありません。  
>もう使用しないオブジェクトの後処理をして、いつでもメモリを解放できる状態にすることを破棄と呼んでいるだけです。  
>[C# のファイナライザ、Dispose() メソッド、IDisposable インターフェースについて](https://qiita.com/Zuishin/items/9efc9c8cbb98300bbc64)

---

## using  

SqlConnectionの最後に明示的にDisposeを行い、接続を閉じるためにはエラーになったときの事も考えてこのように記述する。

``` C#
SqlConnection connection = new SqlConnection();

try
{
    //処理
}
catch (Exception ex)
{
 
}
finally
{
    connection.Dispose();
}
```

usingを使うとfinally キーワードを使ってDisposeをするのと同等の動作を手軽に記述することができる。  

``` C#
using (SqlConnection connection = new SqlConnection()){

}
```

エラーに対処する場合はusing全体をtry,catchで囲む。  
catchされた時、usingを抜けたのと同義となるので、自動的にDisposeされると思われる。  

``` C#
try{
    using (SqlConnection connection = new SqlConnection()){

    }
}
catch(Exception e)
{

}
```

[usingステートメントにtry~catchを使うときは、usingの外側に書く](https://qiita.com/shibe23/items/30308e05de05976b0edb)  
[StreamReader クラス](https://learn.microsoft.com/ja-jp/dotnet/api/system.io.streamreader?view=netframework-4.8)  

---

## usingのIL

>自動的にDispose()を呼び出してくれ、しかも例外にも対応してくれる便利な構文があります。それがusingです。  
<https://divakk.co.jp/aoyagi/csharp_tips_using.html>

念のため<https://sharplab.io/>で確認してみたら本当に例外処理していた。  
ただ、finallyで必ずDispose()を呼び出すようになっているだけなので、結局try,catchで囲む必要はある模様。  

``` C# : 元のコード
public class C {
    public void Func() {
        using (FileStream fs = new FileStream("test.txt", FileMode.Open)) {
            using (StreamReader sr = new StreamReader(fs)) {
                // 処理する
            }
        }
    }
}
```

``` C# : usingのIL(中間言語)
.class private auto ansi '<Module>'
{
} // end of class <Module>

.class public auto ansi beforefieldinit C
    extends [System.Runtime]System.Object
{
    // Methods
    .method public hidebysig 
        instance void Func () cil managed 
    {
        // Method begins at RVA 0x2050
        // Code size 42 (0x2a)
        .maxstack 2
        .locals init (
            [0] class [System.Runtime]System.IO.FileStream fs,
            [1] class [System.Runtime]System.IO.StreamReader sr
        )

        IL_0000: ldstr "test.txt"
        IL_0005: ldc.i4.3
        IL_0006: newobj instance void [System.Runtime]System.IO.FileStream::.ctor(string, valuetype [System.Runtime]System.IO.FileMode)
        IL_000b: stloc.0
        .try
        {
            IL_000c: ldloc.0
            IL_000d: newobj instance void [System.Runtime]System.IO.StreamReader::.ctor(class [System.Runtime]System.IO.Stream)
            IL_0012: stloc.1
            .try
            {
                IL_0013: leave.s IL_0029
            } // end .try
            finally
            {
                // sequence point: hidden
                IL_0015: ldloc.1
                IL_0016: brfalse.s IL_001e

                IL_0018: ldloc.1
                IL_0019: callvirt instance void [System.Runtime]System.IDisposable::Dispose()

                // sequence point: hidden
                IL_001e: endfinally
            } // end handler
        } // end .try
        finally
        {
            // sequence point: hidden
            IL_001f: ldloc.0
            IL_0020: brfalse.s IL_0028

            IL_0022: ldloc.0
            IL_0023: callvirt instance void [System.Runtime]System.IDisposable::Dispose()

            // sequence point: hidden
            IL_0028: endfinally
        } // end handler

        IL_0029: ret
    } // end of method C::Func

    .method public hidebysig specialname rtspecialname 
        instance void .ctor () cil managed 
    {
        // Method begins at RVA 0x20a4
        // Code size 7 (0x7)
        .maxstack 8

        IL_0000: ldarg.0
        IL_0001: call instance void [System.Runtime]System.Object::.ctor()
        IL_0006: ret
    } // end of method C::.ctor

} // end of class C
```

<https://sharplab.io/#v2:C4LglgNgPgAgDAAhgRgHQEkDyBuAsAKAJgGYkAmBAYQQG8CEGlSYAWJAVgAoBKW+xgfAScAYpACmAZWAAncQEMAtggBmAZwQBeBADtxAdwRiIU2QsWcARMHFrgqYAA9glgDRGJAWQD2AE3GomAAO4jrcvHT4AtGMQpzSckoASgr+Mghq6dp6hgnmKfJpnOrhfFExFQwA9FUIgGeKgGAugJoMgNEM/JUMAL7t0d3lXQSdQA==>

---

## IDisposableの実装

IDisposableを実装するだけではダメ。  
そこそこ気を付けるべき実装がある。  

- デストラクターへの対応  
  - Dispose()が呼ばれるケースを切り分ける  
- ガベージコレクタの対応  
  - フラグによる制御  

``` C#
    /// <summary>
    /// 外部リソースを使う処理を内包したクラス
    /// </summary>
    public class DisposeDemo : IDisposable
    {
        /// <summary>
        /// マネージリソース
        /// 外部リソースを扱うクラス
        /// </summary>
        private readonly MemoryStream stream;

        /// <summary>
        /// privateなDispose()メソッドが何回呼ばれても一回しか処理が走らないようするためのフラグ
        /// true：解放済／false：未解放
        /// </summary>
        private bool disposed = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DisposeDemo()
        {
            stream = new MemoryStream();
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        /// <remarks>
        /// 万が一Disposeを忘れた場合でも、ガベージコレクタによってDisposeを呼び出してもらう事で解放忘れを防ぐ
        /// </remarks>
        ~DisposeDemo()
        {
            Dispose(false);
        }

        /// <summary>
        /// 外部リソースをラップして適当な処理を実装した体の処理
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string Process(string value)
        {
            stream.Write(Encoding.UTF8.GetBytes(value));
            stream.Position = 0;
            var buffer = new byte[4096];
            var length = stream.Read(buffer, 0, (int)stream.Length);
            stream.Flush();
            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// IDisposableによる実装メソッド
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // GC.SuppressFinalize()はメモリが解放されるときにファイナライザを呼び出さないようにするメソッド
            // デストラクタでDiposeしているのはDisposeを忘れたときにガベージコレクトされるときに解放してもらうのが目的である。
            // このメソッドが実行されるということは明示的に解放していることを意味しているので、
            // 明示的に解放している以上、それ以上解放する必要がないため、処理を抑制している。
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose処理の本体
        /// </summary>
        /// <param name="isDisposing">
        /// usingによるDisposeが実行された後、ガベージコレクタによるDisposeの実行を回避するフラグ。
        /// </param>
        /// <remarks>
        /// protected virtualにすることによって、このクラスを継承したクラスが破棄処理を書けるようにしている。
        /// 継承先のクラスから、このDispose()を呼ぶのを忘れてはならない。
        /// </remarks>
        protected virtual void Dispose(bool isDisposing)
        {
            // メモリ解放済みであれば何もしない
            if (disposed)
            {
                return;
            }
            // usingによるDisposeが実行された後、ガベージコレクタによる実行を回避する
            // フラグによる制御がない場合、this.streamはnullになっているのでエラーとなってしまう。
            if (isDisposing)
            {
                // isDisposingブロックの中で行う事はマネージリソースの破棄
                // マネージリソースとはIDisposableを実装しているクラスの事なので、Disposeメソッドを呼び出せばよい。

                if (stream != null)
                {
                    Console.Write("Closing and Disposing");
                    stream.Dispose();
                }
            }

            // アンマネジーリソースの破棄はここで行う
            // アンマネジーリソースとはIDisposableを実装していないがOpenやCloseを持つクラスの事だと思われる。
            // アンマネジーリソースの破棄として、Closeメソッドを呼び出し、インスタンスにnullを代入する処理を行う。  

            disposed = true;
        }
    }
```

[Coding Shorts: IDisposable and IAsyncDisposable in C#](https://www.youtube.com/watch?v=mG4PFlajbzs&t=457s)  
[[C#]イマイチ分かりにくいIDisposableの実装方法をまとめる。](https://clickan.click/idisposable/)  

---

## Disposeパターン

一つわからないのは、Dispose パターンの形。  
マネージリソースを解放してはならないけど、アンマネジーリソースは解放しても良い。  
だけど、`GC.SuppressFinalize(this)`も`Dispose(bool disposing)`もどちらの場合でも大丈夫なように記述しているわけだから、分けるのではなく、そのまま記述して、「アンマネジーリソースはここで解放する処理を書いてね」だけのほうがシンプルな気がするのだが、なぜifで記述をわけるような事をするのだろうか。  
混乱する。  

後はアンマネジーリソースが接続に関する資源というのは分かったけど、それってなんかのclassでインスタンスなのだろうか？  
そういうのじゃないから接続の資源という言い方しかできないのだろうか。  
難しく考えすぎか？
接続の資源を解放するなら単純にcloseメソッドを呼び出せばいいだけの話か？  

IDisposableを実装しているクラスはデストラクタに対応しているはずなので、わざわざラップしたクラスでデストラクタの処理を記述する必要がないということかな。  
転じて、IDisposableを実装していない場合はデストラクタでの処理が記述されていないはずなので、その場合はデストラクト処理を定義しないと、コネクションが浮いたままになってしまうということで、そのような処理が必要という事だろうか。  

うーん、ややこしい。  

``` C#
class Class1 : IDisposable
{
    #region IDisposable Support
    private bool disposedValue = false; // 重複する呼び出しを検出するには

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: マネージド状態を破棄します (マネージド オブジェクト)。
                
                // マネージリソースとはIDisposableを実装しているクラスの事。
                // マネージリソースの破棄とはマネージリソースのDisposeメソッドを呼び出すこと。
            }

            // TODO: アンマネージド リソース (アンマネージド オブジェクト) を解放し、下のファイナライザーをオーバーライドします。
            // TODO: 大きなフィールドを null に設定します。
            
            // アンマネジーリソースが接続のリソースのことを言っているならば、ここで行うべきは接続のクローズとオブジェクトの破棄になるのだろうか。
            // Stream?.Close();
            // Stream = null;

            disposedValue = true;
        }
    }

    // TODO: 上の Dispose(bool disposing) にアンマネージド リソースを解放するコードが含まれる場合にのみ、ファイナライザーをオーバーライドします。
    // ~Class1() {
    //   // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
    //   Dispose(false);
    // }

    // このコードは、破棄可能なパターンを正しく実装できるように追加されました。
    public void Dispose()
    {
        // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
        Dispose(true);
        // TODO: 上のファイナライザーがオーバーライドされる場合は、次の行のコメントを解除してください。
        // GC.SuppressFinalize(this);
    }
    #endregion
}
```

---

## ファイナライザのテスト

IDisposable実装の時の話。  
ガベージコレクションによってファイナライザが実行され、デストラクタからDispose()メソッドを呼び出したときにエラーにならないかをテストしようとしたのだが、どう頑張ってもデストラクタが呼び出されなかった。  

オブジェクトにnullを代入して、そのあとで`GC.Collect()`を実行する事で、ガベージコレクションを発動させれば、デストラクタが実行されると思ったのだが、そう単純な話ではない模様。  

意図的にデストラクタを実行する方法がないか探したのだが、全然文献にヒットしないし、そもそもガベージコレクションの実行は予測できないという文献ばかりだった。  

オブジェクトにnullを代入して、無限ループで`GC.Collect()`も実行してみたが、結果は同じ。  
結局何もわからんかった。  

[C# 明示的にGC.Collect()を実施して直ぐにメモリを開放する](http://kazuki-room.com/execute_gc_collect_explicitly_to_release_the_memory_immediately/)  

---

## SqlConnectionをUsingした場合、Disposeと同時にCloseされるのでFinallyで明示的にCloseする必要はない

VBやってた時に遭遇した事象。  
Disposeを勉強した後だと当たり前に感じるが、こういうロジックがあってこそ事なんだなーと思った。  

<https://docs.microsoft.com/ja-jp/dotnet/api/system.data.sqlclient.sqlconnection.close?redirectedfrom=MSDN&view=netframework-4.7.2#System_Data_SqlClient_SqlConnection_Close>

---

## 所感

詳しく勉強するまでは、ガベージコレクションやメモリに関する解放を明示的に行うための仕組みだと思っていたが、全然違った。  
データベースの接続など、開きっぱなしになってリソースが解放されないことを防ぐための仕組みだった。  
それらがマネージリソースやアンマネジーリソースという概念に繋がって行くことを理解できた。  

Disposeの具体的な処理を記述するのではなく、もともとあるDisposeを呼び出すためにIDisposableを実装する形になるわけか。  

---

## 参考

[Dispose の意味が未だわからないのですが](https://atmarkit.itmedia.co.jp/bbs/phpBB/viewtopic.php?topic=34497&forum=7)  
[C#初心者のための基礎！Disposeとusingの意味と使い方を解説#24](https://anderson02.com/cs/cskiso/cskisoniwari-24/)  
[C# Tips－usingを使え、使えったら使え(^^)－](https://divakk.co.jp/aoyagi/csharp_tips_using.html)  
[[C#]イマイチ分かりにくいIDisposableの実装方法をまとめる。](https://clickan.click/idisposable/)  

正直、このサイトが一番詳しく書かれているので、このサイトだけで十分な気がする。  
[C# のファイナライザ、Dispose() メソッド、IDisposable インターフェースについて](https://qiita.com/Zuishin/items/9efc9c8cbb98300bbc64)  
