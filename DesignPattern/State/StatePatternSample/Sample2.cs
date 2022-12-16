using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatePatternSample;

public class Sample2
{
    private readonly Dictionary<STATE_ENUM, IState> stateDic = new Dictionary<STATE_ENUM, IState>()
    {
        {STATE_ENUM.STATE_PLAY,new PlayState() },
        {STATE_ENUM.STATE_STOP,new StopState() },
        {STATE_ENUM.STATE_PAUSE,new PauseState() }
    };
    private readonly Action<IState> commandDic = new Dictionary<string, Action<IState>>()
    {
        {"q",state =>state.Func_q() },
        {"w",state =>state.Func_w() },
        {"e",state =>state.Func_e() },
        {"r",state =>state.Func_r() }
    };

    public static void Execute()
    {
        // dictionaryなどで予め定義しておけば分岐がなくなる。
        var states = STATE_ENUM.STATE_PLAY;
        var command = "q";

        // STATE_PLAY_q
        commandDic[command]?.Invoke(stateDic[states]);
    }
}



enum STATE_ENUM
{
    STATE_PLAY,
    STATE_STOP,
    STATE_PAUSE
}

public interface IState
{
    void Func_q();
    void Func_w();
    void Func_e();
    void Func_r();
}

public class Context
{
    private readonly IState state = null;

    public Context(IState state)
    {
        this.state = state;
    }

    public void Request_q()
    {
        this.state.Func_q();
    }
    public void Request_w()
    {
        this.state.Func_w();
    }
    public void Request_e()
    {
        this.state.Func_e();
    }
    public void Request_r()
    {
        this.state.Func_r();
    }
}

public class PlayState : IState
{
    public void Func_q()
    {
        Console.WriteLine("STATE_PLAY:q");
    }
    public void Func_w()
    {
        Console.WriteLine("STATE_PLAY:w");
    }
    public void Func_e()
    {
        Console.WriteLine("STATE_PLAY:e");
    }
    public void Func_r()
    {
        Console.WriteLine("STATE_PLAY:r");
    }
}

public class StopState : IState
{
    public void Func_q()
    {
        Console.WriteLine("STATE_STOP:q");
    }
    public void Func_w()
    {
        Console.WriteLine("STATE_STOP:w");
    }
    public void Func_r()
    {
        Console.WriteLine("STATE_STOP:r");
    }
    public void Func_e()
    {
        Console.WriteLine("STATE_STOP:e");
    }
}

public class PauseState : IState
{
    public void Func_q()
    {
        Console.WriteLine("STATE_PAUSE:q");
    }
    public void Func_w()
    {
        Console.WriteLine("STATE_PAUSE:w");
    }
    public void Func_r()
    {
        Console.WriteLine("STATE_PAUSE:r");
    }
    public void Func_e()
    {
        Console.WriteLine("STATE_PAUSE:e");
    }
}