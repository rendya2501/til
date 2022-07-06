
# NULLの扱い

---

## NULLのLIKE検索

LIKE検索には引っかからない。  
NULLを検索したかったからISNULLを使うこと。  

---

## NULLのORDERBY

[NULLと戯れる: ORDER BYとNULL](https://qiita.com/SVC34/items/c23341c79325a0a95979)  

どうにも、NULLを最小値とするか最大値とするかは、RDBMS毎に違ったり、設定で変更出来たりするみたい。  
Oracleは最大値扱いだが、SQLServerは最小値扱い見たい。  
まぁ、どちらにせよ、先頭か末尾であることに違いはないということですね。  

---

## NULLをキャスト

(NULL AS CHAR)→NULLのまま

---

## NULLとの演算

全てNULLになる  

`NULL -1 = NULL`  

---

## NULLのSUM

SUMはNULLが1件でも含まれると結果がNULLになる。

---

## NULLのCount

NULLはカウントされない。  

COUNT(*) → 5  
COUNT(Age) → 4  

``` txt
ID | Age
---+----
1  | 11
2  | 22
3  | 33
4  | null
5  | 44
```
