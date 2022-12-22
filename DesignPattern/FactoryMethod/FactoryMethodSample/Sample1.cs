using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FactoryMethodSample;

public class Sample1
{
    static void Main(string[] args)
    {
        Factrory factrory = new IDCardFactory();
        Product card1 = factrory.Create("test1");
        Product card2 = factrory.Create("test2");
        Product card3 = factrory.Create("test3");

        card1.Use();
        card2.Use();
        card3.Use();

        Console.ReadKey();
    }
}

public abstract class Product
{
    public abstract void Use();
}

internal abstract class Factrory
{
    public Product Create(string owner)
    {
        Product p = CreateProduct(owner);
        RegisterProduct(p);
        return p;
    }
    protected abstract Product CreateProduct(string owner);
    protected abstract void RegisterProduct(Product product);
}

public class IDCard : Product
{
    private String owner;
    public IDCard(String owner)
    {
        Console.WriteLine(owner + "のカードを作ります。");
        this.owner = owner;
    }
    public override void Use() => Console.WriteLine(owner + "のカードを使います。");
    public String GetOwner => owner;
}

internal class IDCardFactory : Factrory
{
    private List<string> owners = new List<string>();
    protected override Product CreateProduct(String owner) => new IDCard(owner);
    protected override void RegisterProduct(Product product) => owners.Add(((IDCard)product).GetOwner);
    //public List<string> GetOwners() => owners;
}

