# Laravel

---

## Illuminate

### Illuminateとは何か？

Laravelの大事なコンポーネントが置いてある場所。  
`Illuminate\Http\Requestクラス`の、hasメソッドのように、各クラスとメソッドが定義されている。  
これがあるおかげで、デフォルトで使えるありがたいメソッドがたくさんある。  

### Illuminateはどこにあるのか？

`vender > laravel > framework > src > Illuminate`  

vendorディレクトリはlaravelでプロジェクトを作成した時に自動生成される。  
vendorディレクトリはcomposerと依存関係にある。  

[【Laravel】Illuminateとは何か？ファイルはどこにあるのかを実例で解説](https://prograshi.com/framework/laravel/what-is-illuminate/)  

---

## LaravelのDB接続情報

.envファイル

---

## ファサード

<https://qiita.com/minato-naka/items/095f2a1beec1d09f423e>  

staticメソッドのようにメソッドを実行できる仕組み。  
実際にはstaticじゃなくて、ちゃんとnewして実行している。  
詳しくは解説を読むべし。  
読んで思ったのは、うまいこと出来てるんだなって事。  
