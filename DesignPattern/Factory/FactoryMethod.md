# ファクトリメソッドパターン (Factory Method Patter)

---

## サンプルプログラム

このサンプルプログラムでは、AnimalFactoryという抽象ファクトリークラスがあります。  
これはCreateAnimal()メソッドを持っており、具体的なファクトリークラス（DogFactoryおよびCatFactory）がこのメソッドをオーバーライドして、IAnimalインターフェースを実装する具体的なクラス（DogおよびCat）を生成します。  

PetOwnerクラスは、AnimalFactory型の依存関係を要求し、ファクトリーメソッドパターンを使ってペット（IAnimal）を生成します。  
この設計により、PetOwnerクラスは具体的な動物クラス（DogやCat）に依存せず、代わりに抽象ファクトリークラス（AnimalFactory）に依存します。  

``` cs
using System;

public interface IAnimal
{
    void Speak();
}

public class Dog : IAnimal
{
    public void Speak()
    {
        Console.WriteLine("Woof!");
    }
}

public class Cat : IAnimal
{
    public void Speak()
    {
        Console.WriteLine("Meow!");
    }
}

public abstract class AnimalFactory
{
    public abstract IAnimal CreateAnimal();
}

public class DogFactory : AnimalFactory
{
    public override IAnimal CreateAnimal()
    {
        return new Dog();
    }
}

public class CatFactory : AnimalFactory
{
    public override IAnimal CreateAnimal()
    {
        return new Cat();
    }
}

public class PetOwner
{
    private readonly IAnimal _pet;

    public PetOwner(AnimalFactory animalFactory)
    {
        _pet = animalFactory.CreateAnimal();
    }

    public void LetPetSpeak()
    {
        _pet.Speak();
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        // DogFactoryを使用してPetOwnerを作成
        var dogOwner = new PetOwner(new DogFactory());
        dogOwner.LetPetSpeak(); // 出力: Woof!

        // CatFactoryを使用してPetOwnerを作成
        var catOwner = new PetOwner(new CatFactory());
        catOwner.LetPetSpeak(); // 出力: Meow!
    }
}
```
