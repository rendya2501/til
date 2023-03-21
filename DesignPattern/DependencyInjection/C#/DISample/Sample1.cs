using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISample;

public class Sample1
{
    public static void Execute()
    {
        Console.WriteLine("Hello World!");

        //インターフェースを実装していさえすればOK
        var bModoki = new B();
        var cModoki = new C();
        var dModoki = new D();

        //Aのコンスタクタ経由でB、C、Dのインスタンスを渡す
        var a = new A(bModoki, cModoki, dModoki);
        a.Do();
    }
}

interface IB
{
    void Do();
}
interface IC
{
    void Do();
}
interface ID
{
    void Do();
}

class A
{
    private readonly IB _b;
    private readonly IC _c;
    private readonly ID _d;

    public A(IB b, IC c, ID d)
    {
        this._b = b;
        this._c = c;
        this._d = d;
    }

    public void Do()
    {
        this._b.Do();
        this._c.Do();
        this._d.Do();
    }
}

class B : IB
{
    public void Do()
    {
        Console.WriteLine("B");
    }
}
class C : IC
{
    public void Do()
    {
        Console.WriteLine("C");
    }
}
class D : ID
{
    public void Do()
    {
        Console.WriteLine("D");
    }
}