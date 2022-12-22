# Abstract,Interface

## とりあえず今の認識  

抽象クラス  

- 複数の概念に存在する共通のバックグラウンド・概念を定義したもの。共通の機能を取り出したモノではない。(結果的に共通になるってだけ)  
- IS-Aの関係  
  犬 IS A 動物＝犬は動物  
  動物クラス（抽象クラス）に犬や人間が持っている共通概念や属性(処理)を書きましょう

インターフェース  

- オブジェクト間の疎結合を実現するためのもの  
- オブジェクトに機能を与えるもの  
- 「ふるまい」の共通化。コードの再利用ではない。  
- CAN-DOの関係  
  レジ CAN bill クレジットカード＝レジはクレジットカード会計が出来る  

---

## AbstractとInterfaceの使い分けではここが一番参考になるかもしれない

[Karakuri.com_抽象クラス(Abstract)とインターフェース(Interface)の違いと実装の使い分けについて](https://www.k-karakuri.com/entry/2018/04/12/%E6%8A%BD%E8%B1%A1%E3%82%AF%E3%83%A9%E3%82%B9%28Abstract%29%E3%81%A8%E3%82%A4%E3%83%B3%E3%82%BF%E3%83%BC%E3%83%95%E3%82%A7%E3%83%BC%E3%82%B9%28Interface%29%E3%81%AE%E9%81%95%E3%81%84%E3%81%A8%E5%AE%9F)  

### 抽象クラスとインターフェースの概念的な違い

- 抽象クラス: 複数の概念から抽象化されたオブジェクト  
- インターフェース: オブジェクトに機能を与えるもの  

抽象クラスはオブジェクトとしてモデル化されたもので、インターフェースはオブジェクトに機能を与えるです。  
決して共通の機能を取り出すわけではありません。  

- 異なる実装の処理を同じインスタンスとして使用したいからインターフェースを使う  
- 処理を共通化したいから抽象クラス使う  
- 複数使いたいからインターフェースを使う  
この考えはオブジェクト指向ではありません。  

### 抽象クラスの具体例

![1](https://cdn-ak.f.st-hatena.com/images/fotolife/h/hazakurakeita/20160809/20160809012622.png)  

``` C#
abstract class Prius{}

class KH_NHW10 : Prius {}
class ZA_NHW20 : Prius {}
class DAA_ZVW50 : Prius {}
```

トヨタ自動車プリウスで見たとき、抽象クラスPriusを継承しているのは各世代のプリウスの型式です。  
今までプリウスには大きく4つの世代と、各マイナーチェンジの兄弟がいます。それらの型式の一部です。  

ここで、抽象クラスと具象クラスには「ほとんど同じ」という関係になっています。  
しかし、プリウスという自動車は厳密には存在しません。  
世代によってデザインは全く異なります。  
その違いを具象クラスが定義します。  
全世代のプリウスの共通機能を集約したものがプリウスというわけではありません。  

### よくある抽象クラスの間違った例

![a](https://cdn-ak.f.st-hatena.com/images/fotolife/h/hazakurakeita/20160809/20160809013114.png)

``` C#
abstract class Car{}

class Tesra_ModelS : Car {}
class MAZDA_Attenza : Car {}
class TOYOTA_Prius : Car {}
```

抽象クラスと具象クラスがほとんど同じということは、Carという抽象クラスからPriusの具象クラスの間には相当の数の抽象クラスが存在することになりますが、多くの教科書にはCarクラスをPriusクラスが継承している絵が描かれています。  
これは間違いです。あまりにも差が大きすぎます。  
Carクラスを継承している他のクラスにテスラの電気自動車や、マツダのクリーンディーゼル車も並んでいます。  
こんな抽象クラスはあり得ません。これでは万能クラスです。  
いずれメンテナンスできなくなり破綻してしまいます。  

### インターフェースの具体例

インターフェースはオブジェクトに機能を与えるものです。  
オブジェクトとして異なるもの同士でも同じ機能を有している場合は同じインターフェースのインスタンスとして扱うことができるというものです。  
抽象クラスとは全然違います。  

![a](https://cdn-ak.f.st-hatena.com/images/fotolife/h/hazakurakeita/20160809/20160809013429.png)  

``` C#
interface IMoterDriven {}

class Prius : IMoterDriven {}
class ModelS : IMoterDriven {}
class Leaf : IMoterDriven {}
```

IMoterDrivenはモーター駆動機能のインターフェースです。  
このインターフェースを実装しているのはトヨタ自動車のPriusクラスやテスラモータースのモデルS、日産自動車リーフがあります。  
先ほどの抽象クラスではこれらのクラスを1つのスーパークラスで汎化することはできませんでしたが、インターフェースなら可能です。  
なぜなら、これらの自動車は全てモーター駆動の機能を有しているためです。  
これらの自動車は、1つのモーター駆動インスタンスとしてモーター駆動に関する動作を表現することができます。  
このように、インターフェースの場合はオブジェクト的に似ている必要はありません。  

### 抽象クラスとインターフェースを使い分けた具体例

抽象クラスはオブジェクトとしてモデル化されたもので、インターフェースはオブジェクトに機能を与えるです。  
あくまで抽象クラスやインターフェースを先に設計し、書くべきで、先に書いた具象クラスから抽象クラスやインターフェースを考えたり生み出したりしてはいけません。  
先に出した抽象クラスやインターフェースを１つの自動車モデルにまとめると下のようになります。  

![a](https://cdn-ak.f.st-hatena.com/images/fotolife/h/hazakurakeita/20160809/20160809014434.png)  

``` C#
abstract class Car{}

interface IEngineDriven {}
interface IMoterDriven {}

abstract class GasPowered : Car,IEngineDriven {}
abstract class Hyblid : Car,IEngineDriven,IMoterDriven {}
abstract class Electrical : Car,IMoterDriven {}

abstract class Attenza : GasPowered {}
abstract class Prius : Hyblid {}
abstract class ModelS : Electrical {}

class KH_NHW10 : Prius {}
class ZA_NHW20 : Prius {}
class DAA_ZVW50 : Prius {}
```

---

## インターフェースって何のメリットがあるのか

[インターフェースって何のメリットがあるんですか？](https://teratail.com/questions/70213)  

わかりやすいような例や例えで教えてくれませんか？

一般的に、インターフェースとは「契約」と言われることが多いです。  
日常生活で身近な契約って何だろて考えたときに、電力会社との契約とかがわかりやすいかなと思います。  
電力会社と契約済みであれば、極端ですが「電気ちょーだい」というと電気が供給されると思います。  
この時、供給された電気はどこでどのように生み出されたのか意識したことはありませんよね？（原子力発電なのか、火力発電なのか、風力発電なのか）  
そして、もしかしたら、将来的に原子力発電は廃止されて、それに変わる新たな発電方式が生み出されるかもしれません。  
その場合でも今まで通り「電気ちょーだい」というと電気が供給されるはずです。  
このように、**利用者は、契約（インターフェース）によって、電源（実装）を意識しなくても、形式的に電気（メソッド）を利用することができます。**  
これがインターフェースの主な存在意義だと思います。  

では具体的に、その考え方がどのように実装に活かされるのかを、簡単な例になりますが見ていきましょう。  
TODO管理のアプリケーションをチーム（複数人）で開発する場面を想像してみてください。  

まず、TODOのデータを保存する場所を考える必要がありますよね。  
最初は、開発スピード等を重視して、この保存先にローカルのファイルを選んだとしましょう。  
次に、少なくともリクエストをコントロールする「クラスA」と、ローカルのファイルに対してTODOデータをCRUDする「クラスB」を実装すると思います。  
※ここでは「クラスA」と「クラスB」との間にインターフェース（契約）を設けないでアプリケーションを作り上げたとします。  

1. リリース後、ユーザーが順調に増え、アプリケーションがまともに動かなくなってきました。  
2. スケーラブルな構成への変更を余儀なくされ、データの保存先をローカルのファイルから、クラウドのRDBMSへ移行することを決めます。  
3. そして、誰かがRDBMSに対してTODOデータをCRUDする「クラスC」を実装しようとします。  

ここからが悲劇の始まりです。  
過去に誰かが実装した「クラスA」は「クラスB」からデータをもらう前提で実装されています。  
今回の変更で「クラスA」は「クラスB」ではなく「クラスC」からデータをもらうことになるので「クラスC」の実装・テストに加え「クラスC」の実装を知った上で「クラスA」の修正・テストも必要になります。  
せっかく責務を分離してクラスを分けていたのに、結局クラスAへの影響も大きそうですね。。。  

もし、最初から「クラスA」と「クラスB（TODOデータをCRUDするクラス）」との間にインターフェース（契約）があれば、今回も「クラスC」がそのインターフェースを実装することによって戻り値の型は保証されるので「クラスA」は「クラスC」の実装を知る必要がありません。  
そして多くの場合「クラスA」にほとんど影響はありません。  
また、この先NoSQLに対してCRUDを行う「クラスD」、新型のDBに対してCRUDを行う「クラスE」・・・が現れても「クラスA」と「TODOデータをCRUDするクラス」との間にインターフェース（契約）があるかぎり「クラスA」はそのクラスの実装を全く意識する必要はありません。  
「クラスA」は「クラスB〜E・・・」がどんな実装をしてどこからデータを取ってこようが、それらのクラスが契約通りに実装してくれれば「クラスA」に変更が加わることは原則ありません。  

以上、簡単な例でしたが前述の通り、**役割ごとにカテゴライズされた各レイヤー間にインターフェース（契約）を設けることによって、各レイヤー間を疎結合にし（依存度を低くし）、変更に強いアプリケーション作ることができる**  
これがインターフェースを利用する最大のメリットと言っても過言ではないと思います。  

補足  
インターフェースのメリットを最大限に活かす仕組みとしてDependency Injection(依存性の注入)というデザインパターンがあります。  
少々複雑な上、質問に対する回答にはならないのでここでは触れませんが、インターフェースを理解された次のステップとして、知っておくと良いと思います。  

---

## CRUDでインターフェースを使うなら

[Implementing a CRUD using an Interface](https://stackoverflow.com/questions/3761615/implementing-a-crud-using-an-interface)  

後はこれにDBへのアクセス制御とキー指定のインターフェースを定義できれば完璧な最小サンプルになるだろう。  

``` C#
public interface ICRUD<T>
{
    string Create(T obj);
    T Read(string key);
    void Update(T obj);
    void Delete(string key);
}

public class Student
{
    public string StudentId { get; set; }
    public string StudentName { get; set; }
}

public class StudentManager : ICRUD<Student>
{
    public string Create(Student obj) => throw new NotImplementedException();
    public void Update(Student obj) => throw new NotImplementedException();
    public Student Read(string StudentId) => throw new NotImplementedException();
    public void Delete(string StudentId) => throw new NotImplementedException();
}

    public void Button_SaveStudent(Event args, object sender)
    {
        Student student = new Student() { StudentId = "1", StudentName = "Cnillincy" };
        new StudentManager().Create(student);
    }
    public void Button_UpdateStudent(Event args, object sender)
    {
        Student student = StudentManager.Read("1");
        student.StudentName = "TestUpdate";
        new StudentManager().Update(student);
    }
    public void Button_DeleteStudent(Event args, object sender)
    {
        new StudentManager().Delete("1");
    }
```

---

## インターフェースの種類

[【C#】インターフェイスの利点が理解できない人は「インターフェイスには３つのタイプがある」ことを理解しよう](https://qiita.com/yutorisan/items/d28386f168f2f3ab166d)  

1. 疎結合を目的としたインターフェイス  
2. クラスの機能を保証することを目的としたインターフェイス  
3. クラスへの安全なアクセスを提供することを目的としたインターフェイス  

### じゃあインターフェイスの本質ってなに

インターフェイスとは「インターフェイス」です。  
「ユーザーインターフェイス」とかの「インターフェイス」と同じ意味です。  

では何のインターフェイスかというと、クラスのインターフェイスです。  
もっと言うと、「クラスにアクセスするための境界面」といえます。  

### 1. 疎結合を目的としたインターフェイス

``` C#
//疎結合を目的として作られたインターフェイス
interface ITextReader
{
    string Read(string path);
} 

class TextReader : ITextReader
{
    //テキストファイルを読み込んだ結果を返す処理
    public string Read(string path) { }
}
```

使い手が、クラスに直接アクセスせずにインターフェイス経由でアクセスさせることで、クラス同士の結合度を下げることが目的です。  

このようにしておくことで、たとえTextReaderクラスに変更が生じたとしても、TextReaderクラスがITextReaderインターフェイスを実装している限り、User側の変更は不要になります。  

#### 利点2.クラスへの結合が弱いので機能追加も容易になる

例えば、実際にテキストファイルを読み込むのではなく、デバッグ用に用意したダミーデータを読み込ませたいとします。  
このとき、ITextReaderインターフェイスがあるおかげで、以下のようにすることができます。  

``` C#
class DebugTextReader : ITextReader
{
    public string Read(string path) { }
}

ITextReader textReader = new TextReader();
ITextReader debugTextReader = new DebugTextReader();
textReader.Read(pathString);
debugTextReader.Read(pathString);
```

ITextReaderインターフェイスを実装したDebugTextReaderクラス新たに登場しています。  
しかし、ユーザー側はあくまでITextReaderインターフェイスにアクセスしています。  
User側はただITextReaderだけを知っていて、その参照先が具体的にどのクラスなのかは知りませんから、これまたUser側の変更は不要になるのです。  

#### このタイプのインターフェイスがないと変更がダイレクトに影響する

以下のように、インターフェイスを用意せずにクラス同士をダイレクトにアクセスさせると、UserクラスがTextReaderクラスの変更の影響をダイレクトに受けるようになります。  

例えば、TextReaderクラスのReadメソッドの名前がLoadに変わったとします。  
これだけで、TextReaderクラスを使っているUser側はRead→Loadへの変更を余儀なくされます。  
一つのクラスなら良いですが、これがたくさんのクラスから依存されていた場合、影響するすべてのコードを変更しなければなりません。  

このようなことにならないよう、インターフェイスを用意しておくことで、TextReaderクラスは必ずITextReaderインターフェイスに準拠した実装にならなければなりません。  
つまり、ITextReaderクラスを実装してさえいれば、ITextReaderインターフェイス経由でアクセスしている他のクラスへの影響はまったくなくなるのです。  

### 2. クラスの機能を保証することを目的としたインターフェイス

このタイプのインターフェイスは、記事の序盤で「よくあるインターフェイスの説明」として挙げた「インターフェイスは、実装するクラスにメソッドの実装を強制するもの」という説明を受けたとしても最も納得しやすいタイプです。  

要は、インターフェイスを実装したクラスは、そのインターフェイスに定義されたメソッドは必ず実装されるのだから、特定の機能があることが保証されますよ、ということですね。  

``` txt : このタイプのインターフェイスとして有名なもの
IEnumerable :: foreachで回すことができる
IEqautable  :: 値の等価性を評価することができる
IDisposable :: 明示的にメモリを開放することができる
IObservable :: クラスからの通知を受け取ることができる 
```

例えば、IEnumerable\<T>インターフェイスを実装したクラスは、IEnumerator\<T>を返すGetEnumerator()メソッドの実装を強制されるので、foreachステートメントで回すことができることが保証されます。  
IEquatable\<T>インターフェイスを実装したクラスは、bool Equals(T other)メソッドの実装を強制されるので、他のオブジェクトとの等価性を比較できることが保証されます。  
このように、クラスに一定の機能があることを保証するために使われるインターフェイスが、このタイプです。  

また、クラスの使い手側も、「あ、このクラスはIEnumerableだからforeachで回せるな」「このクラスはIDisposableだから使い終わったらDisposeしなくちゃいけないんだな」などと、クラスの定義を見ただけでそのクラスの性質を簡単に理解できることも利点の一つですね。  

#### ポリモーフィズムを利用した利点

もちろん、利点はそれだけではなく、ポリモーフィズムを利用した利点もあります。  

例えばIEnumerable\<T>インターフェイスは、List\<T>, Dictionary<TKey, TValue>, T[]など、様々なクラスが実装しているので、IEnumerable\<T>型の変数には、それを実装した様々なクラスを受けることができます。  

例えばメソッドの引数として、IEnumerableで受けておけば、使う側はそこに代入できるインスタンスの選択肢が大幅に増えるわけです。  
逆に、メソッドの引数をListなどの具象クラスにしてしまうと、使う側はListのインスタンスしか代入できなくなってしまいます。  

このように、ポリモーフィズムによるメリットも享受することができます。  

### 3. クラスへの安全なアクセスを提供することを目的としたインターフェイス

``` txt : このタイプのインターフェイスとして有名なもの
- IReadOnlyList             :: Listを読み取り専用で提供する  
- IReadOnlyCollection       :: IReadOnlyListから更にインデクサによるアクセスを削除  
- IReadOnlyReactiveProperty :: 購読と値の読み取りだけができるReactiveProperty  
```

見ての通り、ただ単にアクセスを制限させるものばかりですね。  
しかし、これが非常に重要な役割を持ちます。  
Listをそのまま渡すのではなく、IReadOnlyListとして渡すだけで、渡した先で勝手に書き換えられる危険性が皆無になりますから、これを使わない手はありません。  

---

## オブジェクト指向でなぜ作るのか？の要点

[【Java】abstract と interface の使い分け 〜「オブジェクト指向でなぜ作るのか」から学ぶ〜](https://www.msawady.com/entry/2017/08/28/105800)  

### オブジェクト指向は何を解決したかったのか

機械語→アセンブリ→高級言語(e.g:FORTRAN)→構造化言語(e.g: C言語)とプログラミング言語は進化しましたが、以前として解決されない問題が有りました。  

- グローバル変数の管理  
- モジュールの再利用が困難  

そして問題を加速させたのが、以下の問題でした。  

- システム要件の複雑化（本格的な業務利用）  
- コードベースの巨大化  

それらの問題を解決するために以下のような方針で解決策が取られました。  
こういった方針を具体化したプログラミング手法がオブジェクト指向プログラミングであると本書では述べられています。  

``` txt
方針                                        | 解決したかったこと

変数、サブルーティンの整理・集約            | コードベースの巨大化
重複するサブルーティンの集約                | システム要件の複雑化

共通メインルーティンの作成                  | モジュールの再利用が困難

変数の隠蔽、ローカル変数→インスタンス変数  | グローバル変数の管理
```

### abstract class は何を解決したのか

``` txt
方針                                        解決したかったこと

変数、サブルーティンの整理・集約            コードベースの巨大化
重複するサブルーティンの集約                システム要件の複雑化
```

この問題を解決するためのキーワードは「継承」です。  
まず、「クラス」という仕組みを導入すことで、変数、サブルーティンの整理・集約が行いやすくなりました。  
さらに、「abstract class の継承」を導入することで、「整理されたクラス間で重複するサブルーティンの集約（脱コピペ！）」を直感的に行えるようになり、システム開発の生産性・保守性を大きく上げました。  

実装コストと言う面から見ると、以下のようなメリットが有ります。  

- 重複する変数・サブルーティンを共通化することによる、コードベースの縮小  
- コードを整理することによる、ソースコード管理の効率化（＝保守性の向上）  
- 修正の分散を抑えることによる、保守性の向上  

また、設計の質を向上させる効果も有りました。

- 共通するサブルーティンを集約するための、外部・内部設計のブラッシュアップ  
- 共通のサブルーティンを持つ＝似たような機能/属性である、という抽象化  

これにより、複雑なシステム要件を「現実世界に則した」形で表現し、直感的に設計することが可能になりました。  

before
定番ではありますが、DogクラスとHumanクラスを実装する例を考えます。
abstract クラスを利用しない場合は、重複するコードが発生します。

``` java : before
public class Dog{
    private int legCount;
    
    public Dog(){
       this.legCount = 4; 
    }

    public boolean isMammal(){
        return true;
    }
    
    public boolean isFourLeg(){
        return true;
    }
}

public class Human{
    private int legCount;

    public Human(){
        this.legCount = 2;
    }
    
    public boolean isMammal(){
        return true;
    }
    
    public boolean isFourLeg(){
        return false;
    }
}
```

after
abstract なMammalクラスを実装し、それを継承することで、コードの重複を減らすことが出来ます。  
また、isFourLegメソッドについては「犬はtrue, 人間はfalse」という定義から、「4本足かどうか」という抽象的かつ直感的な再定義を行うことが出来ています。

``` java
public abstract class Mammal {
    private int legCount;

    public Mammal(int legCount) {
        this.legCount = legCount;
    }

    public boolean isMammal() {
        return true;
    }

    public boolean isFourLeg() {
        return this.legCount == 4;
    }
}

public class Dog extends Mammal {
    public Dog() {
        super(4);
    }
}

public class Human extends Mammal {
    public Human() {
        super(2);
    }
}
```

### interface は何を解決したのか

``` txt
方針                        解決したかったこと

共通メインルーティンの作成  モジュールの再利用が困難
```

この問題を解決するためのキーワードは「ポリモーフィズム」です。  
「ポリモーフィズム」を実現することで、「共通化されたメインルーティン」を作成しやすくなり、「再利用可能なモジュール」を作成することが可能になりました。  

before
もし、Listというinterfaceが存在しない世界で、「ArrayList のすべての要素をnew Element()に変更する」メソッドと「LinkedList のすべての要素をnew Element()に変更する」メソッドを実装しようとすると、以下のようになります。

``` java
public class HogeService {
    public void replaceWithNewElement(ArrayList<Element> list) {
        int size = list.size();
        list.removeAll();
        for (int i = 0; i < size; i++) {
            list.add(new Element());
        }
    }
    public void replaceWithNewElement(LinkedList<Element> list) {
        int size = list.size();
        list.removeAll();
        for (int i = 0; i < size; i++) {
            list.add(new Element());
        }
    }
}
```

まったく同じことをやっているのに、ArrayListとLinkedListという2つのクラスを処理するためにreplaceWithNewElementというメインルーティンを2つ用意する必要が有ります。言い方を変えると、replaceWithNewElement(ArrayList list)はreplaceWithNewElement(LinkedList list)として再利用できません。

after
Listというsize(), removeAll(), add()といったメソッドを持つ interface を作成します。

``` java
public interface List<E> {
    public int size();
    public boolean removeAll();
    public boolean add(E element);
}
```

そして、interface は実装を提供せず、仕様を提供します。  
「実現の仕方は約束しないけど、このメソッドはこういうことをする」という仕様の約束だけを行います。  
上記3メソッドについては以下のように仕様が定められます。  

- size(): このリスト内にある要素の数を返します。  
- removeAll(): このリストから、指定されたコレクションに含まれる要素をすべて削除します。  
- add(): 指定された要素をこのリストの最後に追加します。  

``` java
// ArrayList、LinkedListという実装クラスはListという interface の仕様に対して、実装を提供します。
public class ArrayList<E> implements List<E> {
    public int size(){}
    public boolean removeAll(){}
    public boolean add(E element){}
}
public class LinkedList<E> implements List<E> {
    public int size(){}
    public boolean removeAll(){}
    public boolean add(E element){}
}

// そして、replaceWithNewElement の引数をListという interface の仕様に従ったクラスと定義します。
public class HogeService {
    public void replaceWithNewElement(List<Element> list) {
        int size = list.size();
        list.removeAll();
        for (int i = 0; i < size; i++) {
            list.add(new Element());
        }
    }
}
```

こうすることにより、replaceWithNewElementというメインルーティンは、ArrayListでもLinkedListでも利用できるようになりました。  
つまり、モジュールの再利用性が高まったのです。  

1. 仕様(interface)を定義  
2. 仕様に対する実装(implement)を定義  
3. interface を処理するメインルーティンを定義する  

とすることで、「interface を implement するクラスすべてを扱うことに出来る、再利用性が高いメインルーティン」を作成することができます。  

### abstract class と interface の使い分け

abstract class と interface という2つの仕組みは似たような特徴を持ちますが、そもそも解決したい問題が違うのです。  

``` txt
仕組み          目的

abstract class  重複するメソッドの集約
                実装における抽象度の整理

interface       メインルーティンの再利用性の向上
                仕様と実装の分離
```

---

## IS A,CAN DO

[【詳解】抽象クラスとインタフェースを使いこなそう！！](https://qiita.com/yoshinori_hisakawa/items/cc094bef1caa011cb739)  

### 抽象クラスとは？

結論から言うと抽象クラスは継承関係にあり、処理の再利用をしたい時に使うものです。  

抽象クラスと具象クラスの「継承」関係は**IS Aの関係**と言われています。  
**犬 IS A 動物 = 犬は動物** のような親子関係です。  
だから「動物クラス（抽象クラス）に犬や人間が持っている共通動作（処理）を書きましょう！」と言われている。  

子供のみんなが出来ることを抽象的に抽象クラスに書いて、その子にしか出来ない具体的なことは具象クラスに書く  

### インタフェースとは？

結論から言うとインタフェースとはクラス仕様としての型定義をするものです。  

まずインタフェースの言葉の意味は「コンピューターと周辺機器を接続するための規格や仕様、またはユーザーがコンピューターなどを利用するための操作方法や概念のこと。  
ハードウェアでは、機器同士を接続するコネクターの規格を指す。」などが挙げられます。  
もっと簡単に言うと、外から見える何かと何かをくっつける窓口のことです。  

そしてインターフェイスと実装クラスの「実装」の関係は**CAN DOの関係**と言われています。  
**レジ CAN bill クレジットカード = レジはクレジットカード会計が出来る** のような関係です。  
他にもレジは現金会計もできます。  
なのでインタフェースで定義するものはレジが出来ること、会計のみを定義します。  
他の視点から言うと、中は意識せず、外から見てレジが出来ることを定義する感じです。  
そうするとレジを使う店員はいちいちどんな会計か考えずにただ会計をする事が出来る*はずです。  

### なぜ指定できるアクセス修飾子が違うのか？

インタフェースはpublicのみ。  
抽象クラスはpublicとprotectedのみを定義できる。  
publicで定義したメソッドはどこからでも呼び出せるよ！といった意味でprotectedで定義したメソッドはそのパッケージ内だけから呼び出せるよ！といった意味だ。  

と言うことはpublicメソッドは外から呼び出すためのもの  
→外で使う側の人ため  
→インタフェースでは使う人のためにメソッドを定義していると言うことだ。  

ではprotectedメソッドはパッケージ内でしか使えない  
→中で使う人のため  
→抽象クラスは中で使う人のためのものと言うことだ。  

[インターフェースと抽象クラスの使い分け、活用方法](https://qiita.com/igayamaguchi/items/e1d35db0a14a84bda452)  

interfaceは外部向け。abstractは内部向け。  

interfaceはpublicしか記述できない。  
abstractはprotectedが使える。  
なので、外部に公開する場合はinterface。  
内部において、実装を強制する場合はabstractを用いるいいかも。  

### まとめ

抽象クラスは、共通の処理をまとめたりする中で使う人のためのモノ  
インタフェースは、詳細は見せず出来ることを定義し、外から使う人のためのモノ  

---

## ユーチューブで紹介されてたAbstractとInterfaceの使い分けサンプル

[Java C# のインターフェイスと抽象クラス](https://www.youtube.com/watch?v=uA_6W4aWRFg)  

共通の項目があるならそれは抽象クラスにまとめる。  
何ができるかを定義するのはインターフェースで行う。  
それぞれのキャラクターに対して様々でそれぞれであるものは共通の処理を行わせたいならインターフェースで考える。  

ゲームでこそインターフェースの概念は輝くのかもしれない。  
多種多様な要件をインターフェースとして落とし込み、協調していかなければならないから。  

``` C# : ドンキーコングゲームに置ける
    interface IKillable
    {
        public void TakeAHit(int x);
        public void Die();
        public void Respawn();
    }

    interface IMovable
    {
        public void TurnLeft();
        public void TurnRight();
        public void MoveForward();
        public void Climb();
        public void Jump();
    }

    interface IAudible
    {
        public void Speak(string s);
    }

    abstract class GameCharacter
    {
        private string Name;
        private Location Location;
        private FaceDirection FaceDirection;
    }
    enum FaceDirection : byte{left,center,right,}
    struct Location{}

    class Barrel : GameCharacter, IMovable{}
    class Gorilla : GameCharacter{}
    class Mario : GameCharacter, IKillable, IMovable{}
    class Princess : GameCharacter, IAudible{}
```

---

## 教科書みたいな抽象クラスとインターフェースの説明

[抽象クラスとインターフェース](https://java2005.cis.k.hosei.ac.jp/materials/lecture19/abstractclass.html)  

### 抽象クラス

前回の演習で、多角形、長方形、三角形を扱った。これらのクラスとしての階層は次のようになっている。

このうち、長方形と三角形はどのようなものか想像が付くが、多角形はどのようなものであるか想像がつきにくい。多角形は、長方形 (四角形) や三角形が属する抽象的な部類であり、具体例を挙げることができない (具体例を挙げたとしても、三角形や四角形などのほかの部類に属してしまう)。
部類とはクラスであり、具体例とはインスタンスであった。具体例を挙げることができない多角形に対して、次のプログラムでは Polygon のインスタンスを作成してしまっている。

``` java
// 多角形の一例
Polygon p = new Polygon();
```

上記のように、実際には存在しない物体を、new 演算子を使って作成できてしまった。Java ではこのような抽象的な部類を表すためのクラスを表すために、抽象クラスというものが存在する。

``` java
// 多角形を表すクラス
public abstract class Polygon {
    // 空のコンストラクタ
    public Polygon() {
        super();
    }
    // 面積を取得する
    public double area() {
        return 0.0;
    }    
    // 全ての辺の長さの合計
    public double perimeter() {
        return 0.0;
    }
}
```

上記のように、class Polygon の前に abstract の指定をすることによって、このクラスを抽象クラスとして宣言できる。
抽象クラスは、他のクラスの継承関係を階層化するために存在し、インスタンス化することができない。abstract class Polygon を new 演算子によってインスタンス化しようとすると、エラーになる。

### 抽象メソッド

Polygon が抽象クラスとなると、このクラスが持つメソッド area() と perimeter() が意味を成さなくなる。
しかし、これらのメソッドを消してしまうと問題が起こる。

``` java
// 多角形を表すクラス
public abstract class Polygon {
    public Polygon() {
        super();
    }    
    // メソッドを消す
}
Polygon p = new Triangle(3.0, 4.0, 5.0);
// Polygon は area() メソッドを持たないのでエラー
System.out.println(p.area());
```

今度は、Polygon が area() メソッドを持たないため、「多角形 (実際には3.0,4.0,5.0の3辺を持つ三角形) の面積を求める」という操作が行えなくなってしまう。  
(実行時の) ポリモーフィズムを実現するには、親クラスで同じ名前のメソッドを定義していなくてはならない。  
やはり area() と perimeter() は削除することはできないが、「多角形」という抽象的な図形で面積を求めたりするような具体的な方法は存在しない。  
このように振る舞いを具体的に定義できないメソッドを abstract として宣言することにより、このクラスでメソッドの中身を記述しないで済ますことができる。  
このようなメソッドを抽象メソッドと呼ぶ。  

``` java
// 多角形を表すクラス
public abstract class Polygon {
    // 空のコンストラクタ
    public Polygon() {
        super();
    }
    // 面積を取得する
    public abstract double area();    
    // 全ての辺の長さの合計
    public abstract double perimeter();
}
```

抽象メソッドは次のような制約を持つ。  

- 抽象クラス内でしか抽象メソッドは宣言できない  
- 子クラスでオーバーライドして、具体的な振る舞いを定義する必要がある  
- 抽象メソッドにはメソッドの本体を持たせることができない  

抽象メソッドを使うと、抽象クラス内でメソッドの振る舞いを定義せずに、そのクラスを継承した子クラスに具体的なメソッドの振る舞いを定義させることができる。  
つまり、子クラスが持つべき機能を親クラスが指定することができる。  
今回の例では、「多角形に属するクラス (三角形や長方形など) は、その面積と外周の長さを計算する機能を持たなくてはならない」という指定を行った。  

![a](https://java2005.cis.k.hosei.ac.jp/materials/lecture19/abstractclass/abstractmethod.png)  

### その他の抽象クラスの例

その他、抽象クラスの例では「乗り物」が挙げられる。  

``` java
public class Vehicle {
    public void go(String destination) {
        // 目的地へ向かう処理
    }
}

public class Taxi extends Vehicle {
    public void go(String destination) {
        this.accelerate();
        // destination へ向かう処理
        this.brake();
    }
}
public class Helicopter extends Vehicle {
    public void go(String destination) {
        this.takeOff();
        // destination へ向かう処理
        this.land();
    }
}
```

「ここからタクシーを使って東京駅へ」「ここからヘリコプターを使って東京駅へ」と言われれば、どのように実現するかは予想が付く。  
しかし、「ここから乗り物を使って東京駅へ」と言われても乗り物では抽象的なので実現方法が予想しにくい。  
乗り物は「タクシー」や「ヘリコプター」、「新幹線」などといった物の上の階層に存在する抽象的な概念で、「乗り物」という物体は存在しない (繰り返すが、乗り物の具体例を挙げても自動車やヘリコプターといったその下の階層にある物体になってしまう)。  
このような乗り物を表す Vehicle といったクラスは抽象クラスで宣言されるべきであるし、また「目的地へ行く」というメソッド go(String destination) といったメソッドは具体的な振る舞いを規定できないため抽象メソッドとして宣言されるべきである。  

``` java
public abstract class Vehicle {
    // 目的地へ向かう
    public abstract void go(String destination);
}
```

### インターフェース

現実世界に「複合コピー機」という商品が存在する。  
これは、コピー機としての機能を持ちながら、その他にもスキャナ、ファクシミリ、プリンタなどの機能を持つ機械である。  
これを素直に Java で表現しようとすると、次のようになる。  

``` java
public class MultiPurposeCopier extends Copier, Scanner, Fax, Printer {
    // コピー機の機能
    public Paper copy(Paper origin) .. 
    // スキャナの機能
    public Document scan(Paper document) ..
    // ファクシミリの機能
    public Paper receive() ..
    public void send(Paper document, String to) ..
    // プリンタの機能
    public Paper print(Document document) ..
}
```

しかし、Java では「extends Copier, Scanner, Fax, Printer」のような書き方をすることはできない。  

多重継承  
「単一継承」と「多重継承」という用語がある。これはクラスの継承をする際にとれる親クラスの数を表しており、単一継承ならば親クラスの数は1つ、多重継承ならば親クラスの数はいくつでも取れることになっている。  
Java や最近の言語では、多重継承を行えないようなものが多い。  
ここでは詳しく触れないが、多重継承を許すといくつかの問題が起こる可能性があるためである。  
このような場合、Java ではインターフェースと呼ばれるものを使って複数の親を持っているように見せかける。  
インターフェースは抽象メソッドしか持てないクラスのようなもので、仕様 (機能を呼び出す際の決まり事, メソッド名や引数) を表現することができる。  
そして、一つのクラスがいくつもこのインターフェースというものを実装 (クラスの継承に近い) することができる。  
インターフェースの宣言は次のように class の代わりに interface を指定する。  

``` java
public interface Scanner {
    // スキャナの機能
    public abstract Document scan(Paper document);
}
public interface Fax {
    // ファクシミリの機能
    public abstract Paper receive();
    public abstract void send(Paper document, String to);
}
public interface Printer {
    // プリンタの機能
    public abstract Paper print(Document document);
}

public class MultiPurposeCopier extends Copier implements Scanner, Fax, Printer {
    // コピー機の機能
    public Paper copy(Paper origin) .. 

    // スキャナの機能
    public Document scan(Paper document) ..

    // ファクシミリの機能
    public Paper receive() ..
    public void send(Paper document, String to) ..

    // プリンタの機能
    public Paper print(Document document) ..
}
```

このように、「複合コピー機はコピー機を拡張して、スキャナ, ファクシミリ, プリンタを実装する」と読むことができる。  

インターフェースは、上記の複合コピー機ならば「スキャンのボタン」「ファックス送受信のボタン」「プリンタのボタン」というイメージが近い。  
あくまで提供するのは仕様だけであって、中身までは提供してくれない。  
そこで、インターフェースを実装した場合、それぞれの仕様 (抽象メソッド) に対して実際の振る舞い (メソッド本体) を定義してやる必要がある。  
つまり、インターフェースは特定の機能を実装する (与えられた仕様を満たす) という契約であり、implements を指定した場合にはこの契約に従い、必要なメソッドを実装する必要がある。  

インターフェースは次のような制約を持つ。
直接インスタンスを生成することはできない (抽象クラスのときと同様)
public abstract であるようなメソッド以外は定義できない
private, protected, パッケージアクセスのメソッドは定義できない
クラスメソッドは定義できない
本体を持つメソッドは定義できない
抽象メソッド以外は定義できない
public static final であるようなフィールド以外は定義できない
private, protected, パッケージアクセスのフィールドは定義できない
インスタンスフィールドは定義できない
クラスフィールドは、finalを指定しないと定義できない
また、上記の制約から、メソッドに public abstract をわざわざ指定しなくても、自動的に public abstract なメソッドとして定義される。

``` java
public interface Fax {
    // 暗黙に public abstract
    Paper receive();
    void send(Paper document, String to);
}
```

### 骨格と機能の集まり

抽象クラスとインターフェースは似たような概念であるが、利用される目的が大きく違う。  

抽象クラス  
一つしか継承できない  
本体を持つメソッドを定義できる  

インターフェース  
いくつでも実装できる  
本体を持つメソッドを定義できない  

#### 骨格実装  

抽象クラスのアドバンテージは、本体を持つメソッド (abstract でないメソッド) を宣言できるという点である。  
これを利用すると、プログラムの骨格だけを抽象クラスで定義して、その骨格を再利用しながら子クラス内で具体的なプログラムを書くことができるようになる。  
子クラスで記述すべきプログラムは、骨格の部分を除いた子クラス特有の処理だけであるので、似たようなプログラムをいくつか用意する場合には、この骨格実装が有効である。  
例として、2つの double[] 型の値の各要素に対して、一斉に「何らかの操作」を行うようなプログラムを考える。  

``` java
public double[] operation(double[] a1, double[] a2) {
    double[] result = new double[a1.length];
    for (int i = 0; i < a1.length; i++) {
        result[i] = a1[i] (何らかの操作) a2[i];
    }
    return result;
}
```

この、「何らかの操作」という部分だけ変更したい場合、通常は operation(double[], double[]) というメソッド全体を変更する必要がある。  
しかし、変更したい部分は「何らかの操作」というだけなのに、全体を全て書き直すのは手間がかかるし、DRY (Don't Repeat Yourself) の原則にも反している。  
そこで、次のような骨格を形成するクラスを用意する。  

``` java
package j2.lesson06;

// 2つの配列の各要素に対して一斉に「何らかの処理」を行うための骨格
public abstract class DoubleArrayOperator {
    // 2つの配列の各要素に対して一斉に「何らかの処理」を行い、その結果を返す
    public double[] operate(double[] a1, double[] a2) {
        double[] result = new double[a1.length];
        for (int i = 0; i < a1.length; i++) {
            // 何らかの処理をして、結果を result[i] に代入
            result[i] = operate(a1[i], a2[i]);
        }
        return result;
    }
    // 現時点では、「何らかの処理」という抽象的なものにする
    protected abstract double operate(double d1, double d2);
}
```

この骨格によって、「2つの配列の各要素に対して一斉に何らかの処理を行い、その結果を返す」というプログラムのうち、「2つの配列の各要素に対して一斉に…、その結果を返す」という部分だけを記述することができた。  
「何らかの処理」というのは具体的に決まっていないため、抽象メソッドとして宣言する。DoubleArrayOperator は抽象メソッドを持つので抽象クラスとして宣言している。  
この骨格を元に、「2つの配列の各要素に対して一斉に足し算を行い、その結果を返す」というプログラムと、「2つの配列の各要素に対して一斉に掛け算を行い、その結果を返す」という2つのプログラムを作成してみる。  

``` java
package j2.lesson06;

// 2つの配列の各要素に対して一斉に足し算を行う
public class DoubleArrayAdder extends DoubleArrayOperator {

    // 足し算を行う
    protected double operate(double d1, double d2) {
        return d1 + d2;
    }
}
package j2.lesson06;

//2つの配列の各要素に対して一斉に掛け算を行う
public class DoubleArrayMultiplier extends DoubleArrayOperator {

    // 掛け算を行う
    protected double operate(double d1, double d2) {
        return d1 * d2;
    }
}
```

「何らかの処理を行う」という部分が、それぞれ「足し算」と「掛け算」に置き換わっただけなので、親クラスのメソッドのうち「何らかの処理を行う」という抽象メソッドを上書きしているだけである。これだけで、次のように2つの配列に対して一斉に足し算や掛け算を行うことができる。

``` java
DoubleArrayOperator op = new DoubleArrayMultiplier();
double[] multiplicand = {1.0, 2.0, 3.0, 4.0, 5.0};
double[] multiplier = {2.0, 1.0, 2.0, 1.0, 2.0};

// 一斉に掛け算を行う
double[] result = op.operate(multiplicand, multiplier);

for (int i = 0; i < result.length; i++) {
    System.out.println(result[i]);
}
// 2.0
// 2.0
// 6.0
// 4.0
// 10.0
```

Java に標準で組み込まれているクラスには、このように骨格だけ用意されているものが多数ある。  
次回以降に詳しく触れるが、上記の考え方は非常に重要なので覚えておくこと。  

### 継承と集約

インターフェースと抽象クラスにはそれぞれ利点と欠点がある。  
例えば、最近の携帯電話などの多数の機能を持ったクラスを表現する際に大変である。  

``` java
// (最近の)携帯電話は、電話を拡張し、カメラ、電子マネー.. を実装している
public class CellPhone extends Phone implements Camera, DigitalCash {
    ...
}
```

上記のようなクラスを作ると、直接継承している Phone は問題ないとしても、カメラ、電子マネーの機能を全てクラス内に書かなければならない。  Java では多重継承を許していないため、これらの機能を継承によって受け継ぐことはできない。  
このような問題を解決するには、継承関係を表す is-a の他にクラスを階層化する方法が必要である。  
そこで、上記クラスの見方を変えてみると、次のように言い表すこともできる。  

- 携帯電話は電話である -> CellPhone is-a Phone  
- 携帯電話はカメラを(部品として)持つ -> CellPhone has-a Camera  
- 携帯電話は電子マネーを(部品として)持つ -> CellPhone has-a DigitalCash  

これによって、インターフェースを実装する部分を has-a の関係に変換することができた。  
しかし、Camera や DigitalCash はインターフェースであるため、それらを実装するクラスを別に作る。  

``` java
public class CameraModule implements Camera {
    // カメラの機能
    public void takePicture() {}
}
public class DigitalCashModule implements DisitalCash {
    // 電子マネーの機能
    public void charge(int money) {}
    public void pay(int money) {}
}

// これらの機能を部品として CellPhone に集約させることによって、機能を追加する。
public class CellPhone extends Phone implements Camera, DigitalCash {
    // 部品として持たせる
    private Camera camera;
    private DigitalCash digitalCash;

    public CellPhone() {
        this.camera = new CameraModule();
        this.digitalCash = new DigitalCashModule();
    }
    // 携帯電話の各機能が呼ばれたら、部品の各機能を代わりに呼び出す
    public void takePicture() {
        this.camera.takePicture();
    }
    public void charge(int money) {
        this.digitalCash.charge(money);
    }
    public void pay(int money) {
        this.digitalCash.pay(money);
    }
}
```

implements Camera, DigitalCash (カメラと電子マネーを実装する) という部分は、CellPhone そのものではなく、CellPhone に集約された部品が実装してくれている。  
CellPhone はその部品の機能を呼び出しているので、結果として CellPhone もそれらの機能を実装していることになる。  
上記のように書くと、プログラムの再利用が簡単になる。  

``` java
// 電子マネーカードは、カードを拡張し、電子マネーを実装する
public class DigitalCashCard extends Card implements DigitalCash {
    // 部品として電子マネーを持つ
    private DigitalCash digitalCash;
    public DigitalCashCard() {
        this.digitalCash = new DigitalCashModule();
    }
    public void charge(int money) {
        this.digitalCash.charge(money);
    }
    public void pay(int money) {
        this.digitalCash.pay(money);
    }
}
```

このようなプログラムの書き方をまとめると、次のようになる。

1. 部品のインターフェースを宣言する  
2. 部品の機能を実現するクラスを作成する  
3. それぞれの部品のインターフェースを持つクラスを作成する  
   - 部品をインスタンスフィールドとして宣言する  
   - 部品を実現したインスタンスをフィールドに代入する  
   - 各部品に対するメソッドが呼び出されたら、部品内のメソッドを代わりに呼び出す  

実際に、携帯電話を製作している会社も、全て同じ部署でカメラや電子マネーを製作したりすることはほとんどない。  
それらは仕様 (インターフェース) だけ統一しておいて、部品として別の部署や別の会社に製作してもらい、最終的に携帯電話に集約させることになる。  

### オリジナル_DIの話

CellPhoneの内部でカメラモジュールと電子決済モジュールをNewしてしまうと、CellPhoneを使う時、絶対にカメラと電子決済機能がついてきてしまう。  
中には電子決済に対応していないCellPhoneだってあるはず。  
新しい携帯電話を開発したいが、今回はシンプルに設計するためこの機能は搭載しないみたいな。  

そういう場合、外部からカメラモジュール機能だけを注入し、電子決済機能は注入しないという芸当ができる。  
そうすると、CellPhoneはそれらモジュールに依存していない、と言うことができる。  
これが依存性の注入という概念ではなかろうか。  

``` java
public class CameraModule implements Camera {
    // カメラの機能
    public void takePicture() {}
}
public class DigitalCashModule implements DisitalCash {
    // 電子マネーの機能
    public void charge(int money) {}
    public void pay(int money) {}
}

// これらの機能を部品として CellPhone に集約させることによって、機能を追加する。
public class CellPhone extends Phone implements Camera, DigitalCash {
    // 部品として持たせる
    private Camera camera;
    private DigitalCash digitalCash;

    public CellPhone(Camera camera,DigitalCash digitalCash) {
        this.camera = camera;
        this.digitalCash = digitalCash;
    }
    // 携帯電話の各機能が呼ばれたら、部品の各機能を代わりに呼び出す
    public void takePicture() {
        this.camera.takePicture();
    }
    public void charge(int money) {
        this.digitalCash.charge(money);
    }
    public void pay(int money) {
        this.digitalCash.pay(money);
    }
}
```

### まとめ

抽象クラスやインターフェースは、抽象的な概念を表すために用意されている。  
抽象クラスは主に継承の階層を表すために導入された抽象的な部類を宣言する場合や、骨格実装を行う場合に用いられる。  
インターフェースは、いくつかの機能の規格 (名前や呼び出し方) を宣言する場合や、多重継承の代わりに用いられる。  

どちらも利点と欠点を持つ。

抽象クラス
一つしか継承できない
本体を持つメソッドを定義できる

インターフェース
いくつでも実装できる
本体を持つメソッドを定義できない

---

## crud abstract class sample

[How to create an abstract class that will handle basic CRUD with GreenDAO for any Entity model](https://stackoverflow.com/questions/37429441/how-to-create-an-abstract-class-that-will-handle-basic-crud-with-greendao-for-an)  
