# FactoryMethod

---

## 概要

何らかのクラスのインスタンスが必要となる際に、そのクラスをインスタンス化する部分を集約するために利用されるデザインパターン。  
クラスのインスタンス化を行う部分を集約することで、それに伴う手順を集約化したり、クラスの仕様の変更に対する影響範囲を絞ったりすることができる。  

factoryMethodにわたす引数によって、生成オブジェクトを切り替える、という使い方をするデザインパターンの模様。  

``` C#
Parent FactoryMethod(int type)
{
    switch (type)
    {
        case 1:
            return new Child_01();
        case 2:
            return new Child_02();
        case 3:
            return new Child_03();
        default:
            throw new Exception();
    }
}
```

[C#でデザインパターン~Factoryパターン編~](https://tech-blog.cloud-config.jp/2019-11-06-csharp-gof-factorymethodpattern/)  
[なぜあんなに難しい？Factory Methodパターン](https://blog.ecbeing.tech/entry/2021/01/20/114000)  
[デザインパターン「Factory Method」](https://qiita.com/shoheiyokoyama/items/d752834a6a2e208b90ca)  

---

## クラス図

``` mermaid
classDiagram
direction BT

class Creator{
    Create()
    FactoryMethod()
}

class Product{
    Method()
    OtherMethod()
}

class ConcreteCreator{
    FactoryMethod()
}

class ConcreteProduct{
    Method()
    OtherMethod()
}

ConcreteCreator --|> Creator : 継承
Creator --> Product : Creates
ConcreteProduct --|> Product : 継承
ConcreteCreator --|> ConcreteProduct : Creates
```

---

## 実装

FactoryMethodは諸説ありすぎて、ちょっとまとめきれていない。  
クラス図と実装が違うけどクラス図の通りに実装したらコード量が多すぎてやってられない。  

クラスをnewしてインスタンスを返す場所がFactoryクラスって認識でいいのだろうか。  
てか、それくらいシンプルじゃないとやってられない。  

``` C#
    class Program
    {
        static void Main(string[] args)
        {
            var productInstanceA = ProductFactory.CreateProductA("product");
            var productInstanceB = ProductFactory.CreateProductB("product");
            Console.WriteLine(productInstanceA.Description); // product_A
            Console.WriteLine(productInstanceB.Description); // product_B
        }
    }

    public interface IProduct
    {
        string Description { get; set; }
    }

    public class Product_A : IProduct
    {
        public string Description { get; set; }
        public Product_A(string description)
        {
            Description = description + "_A";
        }
    }
    public class Product_B : IProduct
    {
        public string Description { get; set; }
        public Product_B(string description)
        {
            Description = description + "_B";
        }
    }
    static class ProductFactory
    {
        public static IProduct CreateProductA(string description) => new Product_A(description);
        public static IProduct CreateProductB(string description) => new Product_B(description);
    }
```
