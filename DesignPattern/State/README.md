# State

---

## 概要

「状態」をクラスとして表現するパターン  

オブジェクトの状態に応じて挙動を変化させる場合に有効。  

- 参考  
  - [slideshare_Gofのデザインパターン stateパターン編](https://www.slideshare.net/ayumuitou52/gof-state)  
  - [デザインパターン ～State～](https://qiita.com/i-tanaka730/items/49ee4e3daa3aeaf6e0b5)  
  - [デザインパターン勉強会 第19回：Stateパターン](https://qiita.com/a1146234/items/bba88081b0f84d70da1f)  
  - [IT専科](http://www.itsenka.com/contents/development/designpattern/state.html)  

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

## 各クラスの役割

>1. State(状態)  
>状態を表すクラスです。  
>状態毎に振舞いが異なるメソッドのインタフェースを定義します。  
>
>2. ConcreteStateA・B(具体的な状態)  
>「State」のインタフェースを実装し、具体的な状態を、「1クラス」 = 「1状態」 で定義します。  
>1つ状態を表すのに複数のオブジェクトは必要ないため、「Singleton」パターンを適用します。  
>
>3. Context(状況判断)  
>現在の状態(「ConcreteStateA」か「ConcreteStateB」)を保持します。  
>利用者へのインタフェースを定義します。  
>状態を変更するメソッドを定義します。(状態の変更は、「ConcreteState」が次ぎの状態として相応しいものを判断し、この状態変更メソッドを呼出すことによって行います。)  
>
>4. Client(利用者)  
>「State」パターンを適用したクラスを用い処理を行います。  
>
>[State パターン](http://www.itsenka.com/contents/development/designpattern/state.html)  

---

## シーケンス図

``` mermaid
sequenceDiagram
    Client->>Context:test
    Context->>StateA: stateMethod1
    StateA->>Context: setState
    Context->>StateB: stateMethos2
    StateB->>Context: contextMethod3
```

``` mermaid
sequenceDiagram
    Alice->>John: Hello John, how are you?
    John-->>Alice: Great!
    Alice-)John: See you later!
```

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

## サンプル1

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

---

## サンプル2

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

        var states = STATE_ENUM.STATE_PLAY;
        var command = "q";
        // dictionaryなどで予め定義しておけば分岐がなくなる。
        // 可読性はひどく悪いが、こんな芸当も可能。
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

---

## サンプル3

Stateを内部クラスとして実装する場合、contextクラスは必要ないかもしれない。  

``` cs
public class Executer
{
    Dictionary<STATE_ENUM, IState> dictionary = new Dictionary<STATE_ENUM, IState>()
    {
        {STATE_ENUM.NormalCurrent,NormalCurrentState.Instance },
        {STATE_ENUM.NormalPast,NormalPastState.Instance }
    };
    private STATE_ENUM _state;

    public void Execute()
    {
        // 拡張条件設定
        this._state = STATE_ENUM.NormalCurrent;

        // データ生成
        CreateArea();

        // return 印刷データ
    }


    #region 地区
    /// <summary>
    /// 地区データを生成します。
    /// </summary>
    private void CreateArea()
    {
        // 地区ごとの来場者一覧を取得
        var area = Enumerable
            .Range(1, 7)
            .GroupJoin(
                dictionary[_state].GetAreaAttendance().GroupBy(g => new { g.AreaClsCD, g.AreaClsName }),
                i => i,
                groupedArea => groupedArea?.Key.AreaClsCD,
                (i, groupedArea) => groupedArea.FirstOrDefault()
            )
            .Select(s => new AreaAttendanceItem()
            {
                AreaClsName = s?.Key?.AreaClsName ?? string.Empty,
                TodayAreaCount = s?.Sum(sum => sum.TodayAreaCount) ?? 0,
                MonthAreaCount = s?.Sum(sum => sum.MonthAreaCount) ?? 0,
                YearAreaCount = s?.Sum(sum => sum.YearAreaCount) ?? 0
            })
            .ToList();

        SetAreaAttendance(area);
    }

    private void SetAreaAttendance(List<AreaAttendanceItem> item)
    {
        // データセット
    }
    #endregion

    #region State
    private class NormalCurrentState : IState
    {
        private static readonly IState _NormalCurrentState = new NormalCurrentState();
        public static IState Instance => _NormalCurrentState;

        public IEnumerable<AreaAttendanceItem> GetAreaAttendance()
        {
            return new List<AreaAttendanceItem>();
        }
    }

    private class NormalPastState : IState
    {
        private static readonly IState _NormalPastState = new NormalPastState();
        public static IState Instance => _NormalPastState;

        public IEnumerable<AreaAttendanceItem> GetAreaAttendance()
        {
            return new List<AreaAttendanceItem>();
        }
    }

    private enum STATE_ENUM
    {
        NormalCurrent,
        NormalPast,
        ClubCurrent,
        CrubPast
    }

    private interface IState
    {
        IEnumerable<AreaAttendanceItem> GetAreaAttendance();
    }
    #endregion

    #region 受け取りクラス
    /// <summary>
    /// 地区来場データ生成において使用する項目
    /// </summary>
    private class AreaAttendanceItem
    {
        /// <summary>
        /// 地区分類コード
        /// </summary>
        public int AreaClsCD { get; set; }
        /// <summary>
        /// 地区分類名
        /// </summary>
        public string? AreaClsName { get; set; }
        /// <summary>
        /// 本日来場者数
        /// </summary>
        public int TodayAreaCount { get; set; }
        /// <summary>
        /// 本月来場者数
        /// </summary>
        public int MonthAreaCount { get; set; }
        /// <summary>
        /// 本年来場者数
        /// </summary>
        public int YearAreaCount { get; set; }
    }
    #endregion
}
```
