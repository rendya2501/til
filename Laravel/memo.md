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

・Laravelプロジェクトを作って、collection,Eloquentのテスト環境を作る

・cronとLaravel
app/Console/Commands
app/Console/Kernel.php

Kernel.phpのCommand配列とCommandメソッドに色々登録すると、
各種コマンドクラスのhandleメソッドが実行される。
これが最低限のcronとLaravelの連携。

