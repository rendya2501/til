# Stateパターン

---

## 概要

``` mermaid : クラス図
classDiagram
direction BT

class IState {
    <<interface>>
    +methodA()*
    +methodB()*
}
class Context{
    -state
    +requestA()*
    +requestB()*
}
class ConcreteState1{
    +methodA()*
    +methodB()*
}
class ConcreteState2{
    +methodA()*
    +methodB()*
}

Context o--> IState
ConcreteState1 ..|> IState
ConcreteState2 ..|> IState
```

[slideshare_Gofのデザインパターン stateパターン編](https://www.slideshare.net/ayumuitou52/gof-state)  

---

## 実装

``` C#
    class Program
    {
        static void Main(string[] args)
        {
            var n = STATE_ENUM.STATE_1;
            // ファクトリー的な
            Dictionary<STATE_ENUM, IState> dictionary = new Dictionary<STATE_ENUM, IState>()
            {
                {STATE_ENUM.STATE_1,new ConcreteState1() },
                {STATE_ENUM.STATE_2,new ConcreteState2() }
            };

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
```
