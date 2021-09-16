# Laravel関係の色々

## LaravelのDB接続情報

.envファイル

---

## ファサード

<https://qiita.com/minato-naka/items/095f2a1beec1d09f423e>  

staticメソッドのようにメソッドを実行できる仕組み。  
実際にはstaticじゃなくて、ちゃんとnewして実行している。  
詳しくは解説を読むべし。  
読んで思ったのは、うまいこと出来てるんだなって事。  

---

## LaravelのCollection

[【5.5対応】Laravel の Collection を使い倒してみたくなった 〜 サンプルコード 115 連発 1/3](https://qiita.com/nunulk/items/9e0c4a371e94d763aed6)  

Collection : 配列のラッパークラスらしい。  

Laravel 標準の ORM である Eloquent で複数レコードを取得する際に、この Collection のインスタンス (正確には Collection を継承したクラスのインスタンス) で返ってきたりしますが、もちろん、アプリケーション内で自由に使うことができます。  
→  
だからそのままLinq的記述が出来たわけね。  

どうもCollectionはLaravelに含まれている機能というだけで、Collection自体がパッケージとして提供されているわけではなさそうだ。  
なのでCollectionを使うならLaravelを使わないとダメみたい。  
というわけで、次は適当にLaravelプロジェクトを作成して、Collectionを使ってみるだけのサンプルをやってみる事かな。  

---

・Laravelプロジェクトを作って、collection,Eloquentのテスト環境を作る

・cronとLaravel
app/Console/Commands
app/Console/Kernel.php

Kernel.phpのCommand配列とCommandメソッドに色々登録すると、
各種コマンドクラスのhandleメソッドが実行される。
これが最低限のcronとLaravelの連携。

