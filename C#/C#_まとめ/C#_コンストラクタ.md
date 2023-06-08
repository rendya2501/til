# コンストラクタ

---

## 静的コンストラクタ

- 静的コンストラクタは、staticクラスに限らずクラスの静的メンバーを初期化する時に使われる。  
- 静的コンストラクタは明示的に呼び出す事ができないので、引数を渡す事はできない。  
- 静的コンストラクタはシステムによって勝手に呼び出されるため、呼び出されるタイミングを知ることはできない。  
  - プログラムからクラスが最初にアクセスされるタイミングで呼び出される模様。  

クラスの定数にアクセスしても、静的コンストラクタは呼び出されないが、readonlyメンバーにアクセスする前には呼び出されている。  
クラスの定数のアクセスにコードの実行は必要ないが、クラスのstaticなメンバーの初期化にコードの実行が必要な可能性のあるstaticメンバーにアクセスする前には、静的コンストラクタを実行してstaticメンバーを初期化しておく必要があるという事のようだ。  

>静的コンストラクターは、任意の静的データを初期化するため、または 1 回だけ実行する必要がある特定のアクションを実行するために使用されます。  
>最初のインスタンスが作成される前、または静的メンバーが参照される前に、自動的に呼び出されます。  
[静的コンストラクター (C# プログラミング ガイド)](https://docs.microsoft.com/ja-jp/dotnet/csharp/programming-guide/classes-and-structs/static-constructors)  

- ユーザーは、プログラムで静的コンストラクターが実行されるタイミングを制御できません。  
- 静的コンストラクターは自動的に呼び出されます。  
  これにより、最初のインスタンスが作成される前、またはそのクラス (基底クラスではない) で宣言された静的メンバーが参照される前に、クラスが初期化されます。  

``` C#
    class StaticConstructorSample
    {
        public const int CONST_NUMBER = 10;
        public static readonly int READONLY_NUMBER = -99;
        // 通常のコンストラクタ（インスタンスのコンストラクタ）
        public StaticConstructorSample()
        {
            Console.WriteLine("コンストラクタが呼び出されました。");
        }
        // staticコンストラクタ（クラスのコンストラクタ）
        static StaticConstructorSample()
        {
            // static readonlyはほぼ定数に近いが、staticコンストラクタでなら初期化可能
            READONLY_NUMBER = 40;
            Console.WriteLine("静的コンストラクタが呼び出されました。");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Mainメソッドを開始。");
            // 定数にアクセス
            Console.WriteLine($"CONST_NUMBER={StaticConstructorSample.CONST_NUMBER}");
            // 静的定数にアクセス
            // アクセス前に静的コンストラクタが呼び出されて唯一の初期化が実行される
            Console.WriteLine($"READONLY_NUMBER={ StaticConstructorSample.READONLY_NUMBER}");
            // 通常のコンストラクタが実行される
            var StaticClassSample1 = new StaticConstructorSample();

            Console.WriteLine("Mainメソッドを終了。");
        }
    }
    // Mainメソッドを開始。
    // CONST_NUMBER=10
    // 静的コンストラクタが呼び出されました。
    // READONLY_NUMBER=-99
    // コンストラクタが呼び出されました。
    // Mainメソッドを終了。
```

[staticクラス（静的クラス）と静的コンストラクタ](http://ichitcltk.hustle.ne.jp/gudon2/index.php?pageType=file&id=cs003_static_class)  

---

## base

baseキーワードは、C#で派生クラスのコンストラクタから基底クラスのコンストラクタを呼び出すために使用される。  

``` cs
public class Animal
{
    public string Name { get; set; }

    public Animal(string name)
    {
        this.Name = name;
        Console.WriteLine($"Animal constructor called name: {name}");
    }
}

public class Dog : Animal
{
    public Dog(string name) : base(name)
    {
        Console.WriteLine($"Dog constructor called name: {name}");
    }
}

_ = new Dog("Pochi");

// Animal constructor called name: Pochi
// Dog constructor called name: Pochi
```

Animalという基底クラスと、その派生クラスであるDogを定義。  
基底クラスのコンストラクタは名前を引数に取り、それをプロパティに設定しています。  

派生クラスDogのコンストラクタでは、基底クラスのコンストラクタを呼び出すためにbaseキーワードを使用しています。  
これにより、Dogのインスタンスが作成されるときに、まず基底クラスのコンストラクタが呼び出され、その後で派生クラスのコンストラクタが呼び出されます。  
