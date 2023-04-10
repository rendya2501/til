/*
Aメソッド,Bメソッドを実行した後、CメソッドかDメソッドだけを実行できるようにしたい。
インテリセンスレベルで制限するためのサンプル
*/

// A→C
IHogeBuilder hogeBuilder1 = new ConcreteBuilder();
Product product1 = hogeBuilder1.AMethod().CMethod();
// Product with steps: A Method, C Method
Console.WriteLine(product1);

// A→D
IHogeBuilder hogeBuilder2 = new ConcreteBuilder();
Product product2 = hogeBuilder2.AMethod().DMethod();
// Product with steps: A Method, D Method
Console.WriteLine(product2);

// B→C
IHogeBuilder hogeBuilder3 = new ConcreteBuilder();
Product product3 = hogeBuilder3.BMethod().CMethod();
// Product with steps: B Method, C Method
Console.WriteLine(product3);

// B→D
IHogeBuilder hogeBuilder4 = new ConcreteBuilder();
// Product with steps: B Method, D Method
Product product4 = hogeBuilder4.BMethod().DMethod();
Console.WriteLine(product4);


// Hoge Builder Interface
public interface IHogeBuilder
{
    IFugaBuilder AMethod();
    IFugaBuilder BMethod();
}

// Fuga Builder Interface
public interface IFugaBuilder
{
    Product CMethod();
    Product DMethod();
}

// Concrete Builder
public class ConcreteBuilder : IHogeBuilder, IFugaBuilder
{
    private Product _product;

    public ConcreteBuilder()
    {
        _product = new Product();
    }

    public IFugaBuilder AMethod()
    {
        _product.Steps.Add("A Method");
        return this;
    }

    public IFugaBuilder BMethod()
    {
        _product.Steps.Add("B Method");
        return this;
    }

    public Product CMethod()
    {
        _product.Steps.Add("C Method");
        return _product;
    }

    public Product DMethod()
    {
        _product.Steps.Add("D Method");
        return _product;
    }
}

// Product
public class Product
{
    public List<string> Steps { get; set; }

    public Product()
    {
        Steps = new List<string>();
    }

    public override string ToString()
    {
        return $"Product with steps: {string.Join(", ", Steps)}";
    }
}

