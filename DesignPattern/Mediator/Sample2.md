# サンプル2

このサンプルは、デザインパターンの一つであるメディエーターパターンのC#実装です。  
メディエーターパターンは、複数のオブジェクト間の直接的な通信を避け、代わりに中央の仲介者を通じて通信することで、オブジェクト間の結合を緩和し、保守性と再利用性を向上させるために使われます。  

1. 仲介者インターフェース (IMediator)  
    このインターフェースは、仲介者が実装する必要があるメソッドを定義します。  
    具体的には、同僚を登録するRegisterColleagueメソッドと、メッセージを送信するSendメソッドが含まれます。  

2. 具体的な仲介者 (ConcreteMediator)  
    IMediatorインターフェースを実装したクラスです。  
    各同僚と、その同僚がメッセージを送信すべき他の同僚のリストを保持する_colleagueMappingsという辞書を持っています。  

    - RegisterColleagueメソッド：このメソッドは、同僚を登録し、その同僚が通知すべき他の同僚のリストを設定します。  
    - Sendメソッド：送信者からのメッセージを受け取り、マッピングに基づいて適切な受信者にメッセージを転送します。  

3. 同僚インターフェース (IColleague)  
    このインターフェースは、同僚が実装する必要があるメソッドを定義します。  
    具体的には、仲介者をセットするSetMediator、メッセージを送信するSend、メッセージを受信するReceiveのメソッドが含まれます。  

4. 具体的な同僚 (Colleague1, Colleague2, Colleague3)  
    IColleagueインターフェースを実装した具体的なクラスです。  
    仲介者を通じてメッセージを送受信するロジックを持っています。  

使用例  

- IMediatorとIColleagueのインスタンスを作成します。  
- 各同僚を仲介者に登録し、その同僚が通知すべき他の同僚のリストを指定します。  
- 各同僚がSendメソッドを使ってメッセージを送信すると、仲介者はそのメッセージを適切な受信者に転送します。  

この設計により、各同僚は他の同僚に直接依存せず、仲介者を通じて間接的に通信します。  
これにより、各オブジェクトの独立性が高まり、システム全体の柔軟性と拡張性が向上します。  

## 開発環境

- windows11  
- .Net8  
- トップレベルステートメントなし  

## ソースコード

``` cs
IMediator mediator = new ConcreteMediator();
IColleague colleague1 = new Colleague1();
IColleague colleague2 = new Colleague2();
IColleague colleague3 = new Colleague3();

mediator.RegisterColleague(colleague1, [colleague2]);
mediator.RegisterColleague(colleague2, [colleague3]);
mediator.RegisterColleague(colleague3, [colleague1, colleague2]);

colleague1.Send("Message from Colleague1");
// Colleague2 received message: Message from Colleague1

colleague2.Send("Message from Colleague2");
// Colleague3 received message: Message from Colleague2

colleague3.Send("Message from Colleague3");
// Colleague1 received message: Message from Colleague3
// Colleague2 received message: Message from Colleague3


// 仲介者インターフェース
public interface IMediator
{
    void RegisterColleague(IColleague colleague, IEnumerable<IColleague> colleaguesToNotify);
    void Send(string message, IColleague sender);
}

// 具体的な仲介者
public class ConcreteMediator : IMediator
{
    private Dictionary<IColleague, IEnumerable<IColleague>> _colleagueMappings = [];

    // 同僚を登録し、通知すべき他の同僚のリストを設定
    public void RegisterColleague(IColleague colleague, IEnumerable<IColleague> colleaguesToNotify)
    {
        _colleagueMappings[colleague] = colleaguesToNotify;
        colleague.SetMediator(this);
    }

    public void Send(string message, IColleague sender)
    {
        foreach (var receiver in _colleagueMappings[sender])
        {
            receiver.Receive(message);
        }
    }
}


// 同僚インターフェース
public interface IColleague
{
    void SetMediator(IMediator mediator);
    void Send(string message);
    void Receive(string message);
}

// 具体的な同僚1
public class Colleague1 : IColleague
{
    private IMediator? _mediator;

    public void SetMediator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void Send(string message)
    {
        _mediator?.Send(message, this);
    }

    public void Receive(string message)
    {
        Console.WriteLine("Colleague1 received message: " + message);
    }
}

// 具体的な同僚2
public class Colleague2 : IColleague
{
    private IMediator? _mediator;

    public void SetMediator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void Send(string message)
    {
        _mediator?.Send(message, this);
    }

    public void Receive(string message)
    {
        Console.WriteLine("Colleague2 received message: " + message);
    }
}

// 具体的な同僚3
public class Colleague3 : IColleague
{
    private IMediator? _mediator;

    public void SetMediator(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void Send(string message)
    {
        _mediator?.Send(message, this);
    }

    public void Receive(string message)
    {
        Console.WriteLine("Colleague3 received message: " + message);
    }
}
```

``` mermaid
classDiagram
    class IMediator {
        <<interface>>
        +RegisterColleague(IColleague, IEnumerableIColleague)
        +Send(string, IColleague)
    }

    class ConcreteMediator {
        -DictionaryIColleagueIEnumerableIColleague _colleagueMappings
        +RegisterColleague(IColleague, IEnumerableIColleague)
        +Send(string, IColleague)
    }

    class IColleague {
        <<interface>>
        +SetMediator(IMediator)
        +Send(string)
        +Receive(string)
    }

    class Colleague1 {
        -IMediator _mediator
        +SetMediator(IMediator)
        +Send(string)
        +Receive(string)
    }

    class Colleague2 {
        -IMediator _mediator
        +SetMediator(IMediator)
        +Send(string)
        +Receive(string)
    }

    class Colleague3 {
        -IMediator _mediator
        +SetMediator(IMediator)
        +Send(string)
        +Receive(string)
    }

    IMediator <|.. ConcreteMediator
    IColleague <|.. Colleague1
    IColleague <|.. Colleague2
    IColleague <|.. Colleague3
    ConcreteMediator "1" -- "0..*" IColleague : contains
    IColleague "1" -- "1" IMediator : uses
```
