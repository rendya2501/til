using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatePatternSample;

public class Sample1
{
    // ファクトリー的な
    Dictionary<STATE_ENUM, IState> dictionary = new Dictionary<STATE_ENUM, IState>()
    {
        {STATE_ENUM.STATE_1,new ConcreteState1() },
        {STATE_ENUM.STATE_2,new ConcreteState2() }
    };
    public static void Execute()
    {
        var n = STATE_ENUM.STATE_1;

        var context = new Context(dictionary[n]);
        context.Request1();
        context.Request2();
    }
}



enum STATE_ENUM
{
    STATE_1,
    STATE_2
}

public interface IState
{
    void Method1();
    void Method2();
}

public class ConcreteState1 : IState
{
    public void Method1()
    {
        Console.WriteLine("ConcreteState1_Method1");
    }

    public void Method2()
    {
        Console.WriteLine("ConcreteState1_Method2");
    }
}

public class ConcreteState2 : IState
{
    public void Method1()
    {
        Console.WriteLine("ConcreteState2_Method1");
    }

    public void Method2()
    {
        Console.WriteLine("ConcreteState2_Method2");
    }
}

public class Context
{
    private readonly IState state = null;

    public Context(IState state)
    {
        this.state = state;
    }

    public void Request1()
    {
        this.state.Method1();
    }
    public void Request2()
    {
        this.state.Method2();
    }
}