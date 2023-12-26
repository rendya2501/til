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

## 活用状況

1. **多数のクラス間の相互作用：**多くのクラスが互いに密接に通信する必要がある場合、メディエーターを使用すると、これらの通信の複雑さを一箇所で管理できます。  
2. **クラスの再利用性向上：** オブジェクト間の直接的な依存関係を減らすことで、個々のクラスの再利用性が向上します。  
3. **拡張性と保守性の向上：** 新しいクラスや機能を追加する際、既存のクラスを変更する必要が少なくなり、システムの拡張性と保守性が向上します。  
4. **結合度の低減：** クラス間の緩い結合を促進し、システム全体の柔軟性と変更容易性を高めます。  

---

## 活用の具体例

**GUIアプリケーション**：たくさんのユーザーインターフェースコンポーネント（ボタン、テキストボックス、メニューなど）が相互に影響を与える場合、メディエーターを使用することで、これらのコンポーネント間の通信を簡単に管理できます。  
例えば、あるボタンがクリックされると他のテキストボックスが更新されるような場合、メディエーターがこれらの相互作用をコントロールします。  

**チャットアプリケーション**：複数のユーザー間でメッセージを交換する際、メディエーターが各ユーザーからのメッセージを受け取り、適切な宛先に転送します。  
これにより、ユーザーは他のユーザーとの直接的な接続を持たずに済みます。  

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
