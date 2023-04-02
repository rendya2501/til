# シンプルファクトリーパターン (Simple Factory Pattern)

## 概要

シンプルファクトリーパターンは、オブジェクト生成のロジックをカプセル化するデザインパターンです。  
このパターンでは、ファクトリークラスがオブジェクト生成を担当し、クライアントはファクトリークラスを通じてオブジェクトを取得します。  
シンプルファクトリーパターンは、オフィシャルなGoFデザインパターンではありませんが、実用的で簡単に実装できるため、多くのプロジェクトで使用されています。  

---

## メリット

1. オブジェクト生成のロジックを一箇所に集約できるため、コードの整理や再利用が容易になります。  
2. クライアントが具体的なクラスを知る必要がなくなり、依存関係が軽減されます。  
3. 新しいクラスを追加する際に、ファクトリークラスのみ変更すればよいため、既存のコードへの影響を最小限に抑えられます。  

---

## デメリット

1. オブジェクト生成のロジックがファクトリークラスに集中するため、ファクトリークラスが肥大化しやすくなります。  
   これは、特に多くの異なるオブジェクトを生成する場合に問題となります。  
2. シンプルファクトリーパターンは静的メソッドを使用することが一般的であるため、拡張やテストが難しくなる可能性があります。  

---

## 使用シーン

シンプルファクトリーパターンは、以下のようなシーンで使用されることが適切です。  

1. オブジェクト生成のロジックが複雑であり、コードの整理や再利用が必要な場合。  
2. クライアントが具体的なクラスを知らずに、オブジェクトを生成したい場合。  
3. 新しいクラスを追加する際に、既存のコードへの影響を最小限に抑えたい場合。  

ただし、依存関係注入（DI）のような代替技術が利用可能である場合は、シンプルファクトリーパターンの使用が不要になることがあります。  
DIコンテナは、依存関係の管理とオブジェクト生成の責任を担ってくれるため、シンプルファクトリーパターンを使用する必要がなくなります。  
さらに、DIコンテナを使用することで、コードのテスト性や拡張性が向上します。  

ただし、プロジェクトの規模や要件によっては、DIコンテナを導入することが過剰に感じられる場合もあります。  
そのような場合、シンプルファクトリーパターンは依然として有用なオプションとなります。  
プロジェクトの状況や要件を考慮し、適切なデザインパターンや技術を選択してください。  

---

## オープン/クローズド原則との関連

シンプルファクトリーパターンは、オープン/クローズド原則（OCP）に基づいて設計されています。  
OCPは、クラスが拡張に対して開かれており、修正に対して閉じているべきだという原則です。  
シンプルファクトリーパターンを使用することで、新しいオブジェクトを追加する際に、ファクトリークラスのみ変更すればよく、クライアントコードを修正することなく、システムを拡張できます。  

---

## シンプルファクトリーパターンと他のファクトリーパターンの違い

シンプルファクトリーパターンは、ファクトリーメソッドパターンや抽象ファクトリーパターンと比較して、よりシンプルな構造を持っています。  

ファクトリーメソッドパターンでは、インスタンス生成の責任がサブクラスに委譲されるため、より柔軟な拡張が可能ですが、実装が複雑になります。  

抽象ファクトリーパターンでは、関連するオブジェクトの生成を一括して行うことができますが、シンプルファクトリーパターンよりもさらに複雑な構造を持っています。  

シンプルファクトリーパターンは、これらのパターンと比較して簡単に実装できるため、小規模なプロジェクトや複雑なオブジェクト生成が不要な場合に適しています。  

---

## 注意点

シンプルファクトリーパターンを使用する際には、以下の点に注意してください。  

1. ファクトリークラスが肥大化しやすいため、適切な設計やリファクタリングを行ってください。  
2. シンプルファクトリーパターンが実際に適切な解決策であるかどうかを検討し、他のデザインパターンや技術（DIコンテナなど）との比較を行ってください。  

---

## サンプルコード

この例では、異なるタイプの動物を表すクラスがあり、それらを生成するシンプルファクトリークラスを作成しています。  

``` cs
using System;

// 動物インターフェース
public interface IAnimal
{
    void Call();
}

// Dogクラス
public class Dog : IAnimal
{
    public void Call()
    {
        Console.WriteLine("Dog says: Woof!");
    }
}

// Catクラス
public class Cat : IAnimal
{
    public void Call()
    {
        Console.WriteLine("Cat says: Meow!");
    }
}

// AnimalFactoryクラス
public class AnimalFactory
{
    public enum AnimalType
    {
        Dog,
        Cat
    }

    public static IAnimal CreateAnimal(AnimalType type)
    {
        switch (type)
        {
            case AnimalType.Dog:
                return new Dog();
            case AnimalType.Cat:
                return new Cat();
            default:
                throw new ArgumentException("Invalid animal type");
        }
    }
}

public class Program
{
    public static void Main()
    {
        IAnimal dog = AnimalFactory.CreateAnimal(AnimalFactory.AnimalType.Dog);
        dog.Call();

        IAnimal cat = AnimalFactory.CreateAnimal(AnimalFactory.AnimalType.Cat);
        cat.Call();
    }
}
```

---

## DIコンテナとの併用

DIコンテナを使用する場合、シンプルファクトリーパターンを使用する必要はありません。  
DIコンテナ自体がオブジェクトの生成と管理の責任を担ってくれるため、ファクトリークラスを作成する必要がなくなります。  

``` cs
using System;
using Microsoft.Extensions.DependencyInjection;

// 動物のインターフェース
public interface IAnimal
{
    void Call();
}

// Dogクラス
public class Dog : IAnimal
{
    public void Call()
    {
        Console.WriteLine("Dog says: Woof!");
    }
}

// Catクラス
public class Cat : IAnimal
{
    public void Call()
    {
        Console.WriteLine("Cat says: Meow!");
    }
}

public class Program
{
    public static void Main()
    {
        // サービスコレクションに依存関係を追加
        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        // DIコンテナからサービスプロバイダを構築
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // ServiceProviderを使用して動物のインスタンスを生成
        var dog = serviceProvider.GetRequiredService<Dog>();
        dog.Call();

        var cat = serviceProvider.GetRequiredService<Cat>();
        cat.Call();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IAnimal, Dog>();
        services.AddTransient<IAnimal, Cat>();
    }
}
```
