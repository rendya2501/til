# Mediator

## 概要

メディエーターパターンは、オブジェクト間の直接的な通信を減らし、代わりに「メディエーター」と呼ばれる中央制御オブジェクトを通して通信を行う設計パターンです。  
これにより、各オブジェクトの独立性と再利用性が高まります。  
メディエーターは、オブジェクト間の相互作用を調整し、その複雑さを自身の中で管理することにより、システム全体の結合度を低減します。  
これは特に、多数のクラスやオブジェクトが相互に密接に通信する必要がある大規模なアプリケーションにおいて有効です。  

``` mermaid
flowchart LR
    ObjectA --> Mediator
    ObjectB --> Mediator
    ObjectC --> Mediator
    Mediator --> ObjectD
    Mediator --> ObjectE
    Mediator --> ObjectF
```

---

## 解説サンプル

``` cs
public class LogicA {
    LogicB b;
    public void Do() {
        b.Do();
    }
}

public class LogicB {
    public void Do() {
    }
}
```

最初の例では、LogicA は直接 LogicB と通信しています。  
これは、オブジェクト間で直接的な依存関係を作り出しています。  

``` cs
public class LogicA {
    Mediator m;

    public void Do() {
        m.DoLogic("b");
    }
}

public class Mediator {
    public void DoLogic(string logic) {
        if (logic == "b") {
            new LogicB().Do();
        }
    }
}

public class LogicB {
    public void Do() {
    }
}
```

二番目の例では、LogicA は Mediator を通じて LogicB のメソッドを呼び出します。  
これにより、LogicA と LogicB の間の直接的な通信がなくなり、Mediator がその責任を負うようになります。  
これにより、各クラスの独立性が高まり、コードの再利用性と保守性が向上します。  

[Mediator Design Pattern (C#) - YouTube](https://www.youtube.com/watch?v=VYLD75sU1rw&t=21s)  

---

## 参考

[Mediator Design Pattern (C#) - YouTube](https://www.youtube.com/watch?v=VYLD75sU1rw&t=21s)  
[Mediator を C# で / デザインパターン](https://refactoring.guru/ja/design-patterns/mediator/csharp/example)  
