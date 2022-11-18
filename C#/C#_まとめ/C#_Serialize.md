# C# シリアライズ

## シリアライズとデシリアライズを繰り返すと？

実務において、初期化を繰り返すとメモリーがどんどん増えていくことに気が付いた。  
そこでは、まっさらなデータをDeepCopy(シリアライズとデシリアライズを実行するメソッド)して代入する処理をしていたのだが、もしかしてこの場合、ガベージコレクションされないのかな？と思って調べたら、その通りでガベージコレクションされない模様。  
おとなしくループさせて、必要な項目だけを初期化したらメモリーが増えることはなくなった。  

>オブジェクトをシリアライズすると、ガベージコレクタに回収されず、生存期間が延長されることがある。  
ガベージコレクタはよそから参照されているオブジェクトインスタンスを回収することはできないため、参照の一覧表が存在するかぎり、シリアライズしたオブジェクトはガベージコレクタに回収されない。  
[SER10-J. シリアライズの過程でメモリリークやリソースリークをしない](https://www.jpcert.or.jp/java-rules/ser10-j.html)  

シリアライズとは、メモリ上のデータをバイトに変換すること。  
メモリ上のそのインスタンスが保持するデータ全てがバイト列に置き変わる。  
バイト列なので、ポインタで指し示すデータではなく、実データとなる。  
それを元に戻すのがデシリアライズ。  
デシリアライズすると、バイト列のデータが元に戻るので、全く同じデータを持つインスタンスを作ることができる。  
もちろんこの時、メモリーのアドレスやポインタ等は新しくなっている。  
しかし、これを延々と繰り返すと、同じデータが無限に増やせるので、メモリリークの原因になるらしい。  

[Java のシリアライズ (serializer, 直列化) について](http://funini.com/kei/java/serialize.shtml)

---

## シリアライズしたデータを見てみたい

<https://social.msdn.microsoft.com/Forums/vstudio/ja-JP/4fb1e972-c19b-4bbd-b828-82cb783c13e8/12458125021247212455124631248812434124711252212450125211245212?forum=csharpgeneralja>  
<http://funini.com/kei/java/serialize.shtml>  

シリアル化。  
XMLの他にJsonもある。多分バイト列にするやつもある。  
なるほど。  
参照先の情報をXMLやJsonにまとめるのね。  
そうすれば、通信相手にデータを送ることができる。  

参照情報だけでは、参照先のアドレスしかわからない。ただの数字でしかないからね。  
シリアル化は、そういった参照先の情報を全て持ってきて、XMLなり、Jsonなり、バイトなりに変換する。  
そうすることで、全てのデータをそれらファイルにまとめることができる。  
極端な話、全て文字列にするわけだ。  

そうしてしまえば、まぁただのデータなので、相手に送ることもできるし、参照なんて関係なく文字列として扱うことが出来る。  
もちろん、受け取ったデータから複合する事で新しい参照にそいつらデータをぶち込んで、元のように使うことが出来るわけだ。  
まぁ、参照先は違って当然だけど、別にそれが本質ではないしね。  

だから、JsonSerializerでJson化してDeserializeするとディープコピーができるわけか。  
納得。  

だからシリアル化、一連のデータとするって意味でシリアル化っていうわけか。  
なるほど。なるほど。  

``` C#
    //シリアライズ対象のクラス
    public class SampleClass
    {
        public int Number;
        public string Message;
    }

    class Program
    {
        static void Main(string[] args)
        {
            SampleClass cls = new SampleClass
            {
                Message = "テスト",
                Number = 123
            };

            var serializer = new XmlSerializer(typeof(SampleClass));
            var sw = new StringWriter();
            serializer.Serialize(sw, cls);
            Console.WriteLine(sw.ToString());
            sw.Close();

            var ms = new MemoryStream();
            serializer.Serialize(ms, cls);
            ms.Position = 0;
            var i = (SampleClass)serializer.Deserialize(ms);
            Console.WriteLine(i.ToString());
            ms.Close();
        }
    }
```

``` XML : Console.WriteLineした結果
<?xml version="1.0" encoding="utf-16"?>
<SampleClass xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema">
  <Number>123</Number>
  <Message>テスト</Message>
</SampleClass>
```

---

## json deserialize object to int

サーチボックスで主キーが2つある場合に、主キーを送る仕組みがなかったので、objectに格納して送信するようにした時に、  
キャストエラーになったので色々調べた。  

そもそもobject型に変換されたものはConvert.ToInt32系のメソッドを使って変換しないとエラーになってしまう模様。  
後は、jsonは数値型しかなく、値の劣化の心配がないlong型(int64)に自動的に変換される模様。  

Boxing,Unboxingという仕組みもあり、なかなか奥が深かった。  
実際に、jsonにシリアライズしてデシリアライズしたときにエラーになるので、そういう物と認識したほうがいいかもしれない。  

``` C#
    int i = 3;
    object obj = i;
    // シリアライズして送信してデシリアライズして受け取った体
    var de_json = JsonConvert.DeserializeObject<object>(JsonConvert.SerializeObject(obj));
    // System.InvalidCastException: 'Unable to cast object of type 'System.Int64' to type 'System.Int32'.'
    var de_i = (int)de_json;
```

こっちだとInvalidCastExceptionになるのでJsonに変換する場合とはまた違うのかもしれない。  

``` C#
    double d = 1.23456;
    object o = d;
    int i = (int)o;
    Console.WriteLine(i);
```

[C# で数字を object 型にキャストした値型の扱いについて](https://cms.shise.net/2014/10/csharp-object-cast/)  
[【C#】Boxing / Unboxing ってどこで使われてるのか調べてみた](https://mslgt.hatenablog.com/entry/2017/11/18/132025)  
