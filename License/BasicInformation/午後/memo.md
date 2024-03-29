# 午後メモ等

---

[「厳選5題」過去問と解説｜令和元年度 秋期 の過去問やるならこれをやれ](https://www.seplus.jp/dokushuzemi/fe/fenavi/kakomon-gensen/r01_autumn/)

アルゴリズムは避けては通れない。
机上でアルゴリズムをトレースする手順は以下の通り。
1.アルゴリズムを紙に印刷する
2.1行ずつ値の変化を書き込む
3.実際にプログラムを書いて答え合わせ

実際にプログラムを動かしながら処理を追う練習しましょう。

[午後問題の歩き方 ｜ 地道にアルゴリズム問題に取り組む（1）](https://www.seplus.jp/dokushuzemi/fe/fenavi/gogo_arukikata/guide_algorithm_of_questions_at_pm/)  

・アルゴリズム問題 苦手克服への地道なステップ その 1  
基本的なアルゴリズムとデータ構造を覚える  

・アルゴリズム問題 苦手克服への地道なステップ その 2  
擬似言語の読み方は、事前に確実にマスターする  

・アルゴリズム問題の克服への地道なステップ（その 3 ）  
ひたすら過去問題を解いてください。その際に重要なのは、制限時間を設けることです。  
→過去問を解いて、自分流の解法を見出す  

[午後問題の歩き方 ｜ 矢沢久雄の アルゴリズム問題の解き方（2）](https://www.seplus.jp/dokushuzemi/fe/fenavi/gogo_arukikata/guide_algorithm2_of_questions_at_pm/)

・設問に答えることが目的。プログラムの説明とプログラムにざっと目を通したら、すぐに設問を見ることが重要。  

・選択肢が大きなヒントになります。とにかく選択肢をよく見てください。  
→設問と選択肢を見ることで、答えを得るために何が必要なのかがわかります。その必要なことを見つけるために、プログラムの説明とプログラムを見るのです。  

・具体例を想定してプログラムの説明を読む。  

・プログラムは、具体例を想定したプログラムの説明と対応付けて読む  
→プログラムを読むときは、具体例を想定したプログラムの説明と対応付けて読んでください。そうすると、「この穴埋めでは、こういう処理を行えばよいのだ！」と気付きます。  

・繰り返し処理の穴埋めは、繰り返しの最初の処理を想定すればわかる  

プログラムを隅々まで完璧に理解しようなどと思わずに、「問題が解ければいいのだ」と割り切ることが大事です。  
設問が解ければよいのです。選択肢から答えを選べればよいのです。選択肢を見て大いにヒントにする。  

---

## 疑似言語

基本的に、▲が条件文、■が繰り返しを意味するらしい。  
条件式の━はelseを表すことに注意。令和元年秋期でやらかした。  

問題文に「AとBが等しくなるまで繰り返す」と示されていても、プログラムでは「■A ≠ B」(AとBが等しくない限り繰り返す)となる。  

``` C
// 手続き、変数などの名前、型等を宣言する。
// ○


// コメント
/*文*/


// 変数に式の値を代入
// 変数 <- 式


// 関数呼び出し
// ・手続(引数， ･･･)


// 【単体分岐選択処理】
// ▲ 条件式
// |   処理
// ▼

if 条件式 {
    処理;
}


// 【双分岐選択処理】
// ▲ 条件式
// |   処理 1
// +---
// |   処理 2
// ▼

if 条件式 {
    処理1;
} else {
    処理2;
}


//【前判定繰り返し処理】
// ■ 条件式
// |　処理
// ■
while(条件式) {
    処理;
}


// 【後判定繰り返し処理】
// ■
// |  処理
// ■ 条件式

do {
    処理;
} while(条件式);


// 【ループカウンタを使った繰り返し処理】
// ■ 変数 : 初期値,条件式,増分
// |  処理
// ■ 条件式
for (変数 = 初期値 ; 条件式 ; 増分) {
    処理;
}
```
