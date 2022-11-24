# TransactionScope

## TransactionScope概要

>TransactionScope とは、コードブロック内の処理をトランザクション処理にしてくれるものです。  
>NET Framework 2.0 から利用することができます。  
>TransactionScope クラスを使用するとコードブロック内の処理で Complete() が呼ばれるとコミットし、Complete() が呼ばれることなくブロックを抜けると自動でロールバックしてくれます。  
>[[C#] TransactionScope の使い方](https://webbibouroku.com/Blog/Article/cs-transaction-scope)  

---

## TransactionScopeを使うときの注意事項

- TransactionScopeのインスタンスを作成した後にSqlConnectionをOpenしないとトランザクションは有効にならない  
- 実行した際に「MSDTC」を要求された場合は分散トランザクションに昇格してしまっているため、分散トランザクションにするつもりがない場合は構造の見直しが必要  

[TransactionScopeを使うときの注意事項](https://morumoru.hateblo.jp/entry/2015/01/15/222433)  

---

## TransactionScopeは複数接続時にMSDTCが利用される

SQLServer2016の教科書で紹介されていた内容をそのまま引用する。  

>TransactionScope クラスは、SqlConnection オブジェクトによってOpenメソッドが複数実行された場合 (複数の接続が確立された場合) には、MSDTC (Distributed Transaction Coordinator) サービスが利用されて、自動的に分散トランザクションとして実行されます。  
分散トランザクション (Distributed Transaction) は、複数サーバーにまたがったトランザクションのことで、この調整 (Coordinate) を行うための機能がMSDTCサービスです。  
このため、 MSDTCサービスが停止している場合には、例外が発生します。  

---

## TransactionError

- `The current TransactionScope is already complete.`  
- `The operation is not valid for the state of the transaction.`  

などの問題が発生した。  

`The operation is not valid for the state of the transaction.`はTransactionのcompleteの書き忘れがあるかタイムアウトで発生するらしいが、これは`The current TransactionScope is already complete.`が発生している時に適切に処理が実行されないために発生する問題なので、Completeの問題が完了すれば自ずと発生しなくなる。  
因みにOnScopeの中で更にOnScopeした時に意図的に発生させることができるので、やはりトランザクションの問題なのだ。  

ログの書き込みに置けるトランザクションを適切に管理できていないことが原因らしいが、そもそもデータベースを跨いだ場合、それを管理する機能や自動的に分散トランザクション扱いにするなど、結構色々あったのでまとめていきたい。  

[SQL Server 2017 の on Linux における分散トランザクションのサポート状況について](https://blog.engineer-memo.com/2017/12/28/sql-server-2017-%E3%81%AE-on-linux-%E3%81%AB%E3%81%8A%E3%81%91%E3%82%8B%E5%88%86%E6%95%A3%E3%83%88%E3%83%A9%E3%83%B3%E3%82%B6%E3%82%AF%E3%82%B7%E3%83%A7%E3%83%B3%E3%81%AE%E3%82%B5%E3%83%9D%E3%83%BC/)  
[「トランザクションの状態に対して操作が有効ではありません」が発生します](https://teratail.com/questions/89449)
