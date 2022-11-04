# FactoryMethod

---

## 概要

何らかのクラスのインスタンスが必要となる際に、そのクラスをインスタンス化する部分を集約するために利用されるデザインパターン。  
クラスのインスタンス化を行う部分を集約することで、それに伴う手順を集約化したり、クラスの仕様の変更に対する影響範囲を絞ったりすることができる。  

>factory methodは、インスタンス化のロジックを子クラスに委譲する手段を提供するものです。
[[保存版]人間が読んで理解できるデザインパターン解説#1: 作成系](https://techracho.bpsinc.jp/hachi8833/2020_12_03/46064)  

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

## 実装1

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

---

## 実装2

>ある採用担当の管理職（hiring manager）を題材にして考えてみましょう。  
一般に、面接官（interviewer）があらゆる職種（開発、営業、経理など）向けの面接をひとりですべてこなすのは不可能です。  
欠員の生じた職種によっては、面接を別の人に委任（delegate）しなければならないでしょう。  

``` php : Interviewerの定義
interface IInterviewer {
    public function AskQuestions();
}
```

``` php : Interviewerインターフェースの実装
class Develpoer implements IInterviewer {
    public function AskQuestions(){
        echo 'デザインパターンについて尋ねる';
    }
}

class CommunityExecutive implements IInterviewer {
    public function AskQuestions(){
        echo 'コミュニティ育成について尋ねる';
    }
}
```

``` php : HiringManagerの定義
abstract class HiringManager {
    abstract protected function MakeInterviewer() : IInterviewer;

    public function TakeInterview(){
        $interviewer = $this->MakeInterviewer();
        $interviewer->AskQuestions();
    }
}
```

``` php : HiringManagerの実装
class DevelopmentManager extends HiringManager {
    protected function MakeInterviewer() : IInterviewer {
        return new Developer();
    }
}

class MarketingManger extends HiringManager {
    protected function MakeInterviewer() : IInterviewer {
        return new CommunityExecutive();
    }
}
```

``` php
$devManager = new DevelopmentManager();
$devmanager->TakeInterview(); // デザインパターンについて尋ねる

$marketingManager = new MarketingManager();
$marketingManager->TakeInterview(); // マーケティング育成について尋ねる
```

``` mermaid
classDiagram
direction BT

class IInterviewer{
    <<Interface>>
    AskQuestions()
}

class Interviewer{
    AskQuestions()
}

class HiringManager{
    <<Abstract>>
    MakeInterviewer()
    TakeInterview()
}

class Manager{
    Method()
    OtherMethod()
}

ConcreteCreator --|> Creator : 継承
Creator --> Product : Creates
ConcreteProduct --|> Product : 継承
ConcreteCreator --|> ConcreteProduct : Creates
```
