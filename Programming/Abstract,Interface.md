# Abstract,Interface

## とりあえず今の認識  

抽象クラス  

- 複数の概念に存在する共通のバックグラウンド・概念を定義したもの。共通の機能を取り出したモノではない。  

インターフェース  

- オブジェクトに機能を与えるもの  
- オブジェクト間の疎結合を実現するためのもの  
- 「ふるまい」の共通化。コードの再利用ではない。  

---

## AbstractとInterfaceの使い分けではここが一番参考になるかもしれない

[Karakuri.com_抽象クラス(Abstract)とインターフェース(Interface)の違いと実装の使い分けについて](https://www.k-karakuri.com/entry/2018/04/12/%E6%8A%BD%E8%B1%A1%E3%82%AF%E3%83%A9%E3%82%B9%28Abstract%29%E3%81%A8%E3%82%A4%E3%83%B3%E3%82%BF%E3%83%BC%E3%83%95%E3%82%A7%E3%83%BC%E3%82%B9%28Interface%29%E3%81%AE%E9%81%95%E3%81%84%E3%81%A8%E5%AE%9F)  

### 抽象クラスとインターフェースの概念的な違い

- 抽象クラス: 複数の概念から抽象化されたオブジェクト  
- インターフェース: オブジェクトに機能を与えるもの  

抽象クラスはオブジェクトとしてモデル化されたもので、インターフェースはオブジェクトに機能を与えるです。

![a](https://cdn-ak.f.st-hatena.com/images/fotolife/h/hazakurakeita/20160809/20160809014434.png)  

---

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
    public Course StudentCourse { get; set; }
}

public class StudentManager : ICRUD<Student>
{
    public string Create(Student obj)
    {
        throw new NotImplementedException();
    }
    public void Update(Student obj)
    {
        throw new NotImplementedException();
    }
    public Student Read(string StudentId)
    {
        throw new NotImplementedException();
    }
    public void Delete(string StudentId)
    {
        throw new NotImplementedException();
    }
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

#### 理想は1クラスにつき少なくとも1インターフェイス

どのようなクラスにも必ずアクセス用のインターフェイスを用意しておくことが理想と思います。  
面倒と思わずに、インターフェイスを用意しておくだけで、万が一の変更があったときに大いに役立ってくれるでしょう。  

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
- IReadOnlyList :: Listを読み取り専用で提供する  
- IReadOnlyCollection :: IReadOnlyListから更にインデクサによるアクセスを削除  
- IReadOnlyReactiveProperty :: 購読と値の読み取りだけができるReactiveProperty  
```

見ての通り、ただ単にアクセスを制限させるものばかりですね。  
しかし、これが非常に重要な役割を持ちます。  
Listをそのまま渡すのではなく、IReadOnlyListとして渡すだけで、渡した先で勝手に書き換えられる危険性が皆無になりますから、これを使わない手はありません。  

---

## オブジェクト指向でなぜ作るのか？の重要な部分を抽出したページ

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

そして、ArrayList、LinkedListという実装クラスはListという interface の仕様に対して、実装を提供します。

``` java
public class ArrayList<E> implements List<E> {
    public int size(){
        // 具体的な実装
    }
    public boolean removeAll(){
        // 具体的な実装
    }
    public boolean add(E element){
        // 具体的な実装
    }
}

public class LinkedList<E> implements List<E> {
    public int size(){
        // 具体的な実装
    }
    public boolean removeAll(){
        // 具体的な実装
    }
    public boolean add(E element){
        // 具体的な実装
    }
}
```

そして、replaceWithNewElement の引数をListという interface の仕様に従ったクラスと定義します。

``` java
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

## abstractとinterfaceの棲み分け

[インターフェースと抽象クラスの使い分け、活用方法](https://qiita.com/igayamaguchi/items/e1d35db0a14a84bda452)

interfaceは外部向け。abstractは内部向け。  

interfaceはpublicしか記述できない。  
abstractはprotectedが使える。  
なので、外部に公開する場合はinterface。  
内部において、実装を強制する場合はabstractを用いるいいかも。  

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
