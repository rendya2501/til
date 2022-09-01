# Stateパターン

---

## 概要

「状態」をクラスとして表現するパターン  

オブジェクトの状態に応じて挙動を変化させる場合に有効。  

- 参考  
  - [slideshare_Gofのデザインパターン stateパターン編](https://www.slideshare.net/ayumuitou52/gof-state)  
  - [デザインパターン ～State～](https://qiita.com/i-tanaka730/items/49ee4e3daa3aeaf6e0b5)  
  - [デザインパターン勉強会　第19回：Stateパターン](https://qiita.com/a1146234/items/bba88081b0f84d70da1f)  

---

## メリット

- コマンドや状態の追加で既存に影響がない。  
- 処理の修正は各Stateクラス内で完結する。  
- コードの見通しがよくなる。  
- 分岐を減らせる可能性がある。  

---

## デメリット

- 状態に応じてクラスが増える。  

---

## 分岐された関数を実行する時のアンチパターン

[分岐アンチパターン](https://qiita.com/pakkun/items/9bef9132f168ba0befd7)  
このサイトの分岐処理のベストプラクティスとしてStateパターンが照会されていたのがきっかけでStateをまとめることになった。  

以下のように、状態によって実行する関数を変化させたい場合にStateパターンが有効である。  

``` php
if ($article->type === 'news') {
    return $this->getNewsInfo();
} else if ($article->type === 'entertainment') {
    return $this->getEntertainmentInfo();
} else if ($article->type === 'recipe') {
    return $this->getRecipeInfo();
}
return null;
```

get○○Infoはインターフェースとして括ることができる。  
状態とオブジェクトの関係は連想配列やDictionaryでまとめておけば、キーに応じたインスタンスが生成される。  
後はインターフェースのメソッドを呼び出せばインスタンスを気にする必要がなくなるという訳。  

``` php
$type_classes = [
    'news' => NewsType::class,
    'entertainment' => EntertainmentType::class,
    'recipe' => RecipeType::class
];

if (!array_key_exists($article->type, $type_classes))
{
    return null;
}

$type_instance = new $type_classes[$article->type];
return $type_instance->getInfo();
```

- Stateパターンを使えば、仕様を一箇所にまとめることができる。  
- 分岐で「状態」や「タイプ」を参照し始めたら、Stateパターンを適応できる可能性が高い。  

---

## クラス図

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

``` C#
class Program
    {
        static void Main(string[] args)
        {
            Dictionary<STATE_ENUM, IState> stateDic = new Dictionary<STATE_ENUM, IState>()
            {
                {STATE_ENUM.STATE_PLAY,new PlayState() },
                {STATE_ENUM.STATE_STOP,new StopState() },
                {STATE_ENUM.STATE_PAUSE,new PauseState() }
            };
            var commandDic = new Dictionary<string, Action<IState>>()
            {
                {"q",state =>state.Func_q() },
                {"w",state =>state.Func_w() },
                {"e",state =>state.Func_e() },
                {"r",state =>state.Func_r() }
            };

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
```
