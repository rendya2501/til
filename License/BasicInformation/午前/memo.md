# 午前メモ

## 人月

<https://e-words.jp/w/%E4%BA%BA%E6%9C%88.html>  

作業量(工数)を表す単位の1つ。  
1人が1か月働いた作業量を1とした物。  
人数×月数  

1人で1ヶ月かかる仕事の量が「1人月」で、10人で6ヶ月かかれば60人月（10×6）、100人で半月かかれば50人月（100×0.5）となる。  

## 営業日

3営業日以内、ってのはね
会社が稼働してる日を1営業日として数えるの
金曜日に投げといたら月曜には出来てるって事じゃないのよシンジくん
死になさい

---

## 符号拡張とは(sign extension)

符号拡張とは、符号付きのデータをビット長の大きいデータに変換する際に、値を変えないようにビットを補ってデータを拡張することである。
符号拡張のためには、符号ビットと同じ値で同じ大きさになるように埋める。

例: 4ビット「-5」を5ビットに符号拡張する場合  

4bit -5 =  1011  
5bit -5 = 11011  

これで不都合がないのか検証してみる。  

0101 : 5  
1010 : 反転  
1011 : -5  

11011 : -5  
00100 : 反転  
00101 : 5  
11010 : 反転  
11011 : -5  

計算上は問題ないことがわかる。  

---

## 算術シフト

符号付きの2進数のビットパターンを右、あるいは、左へずらすことである。  
算術シフトでは、符号ビットを除いたビットパターンをずらし、符号ビットはずらさない。あふれたビットは切り捨てて、空いた部分に「0」を挿入する。  
→  
例:符号付き2進数の「11110111」（10進数で-9）を左へ1ビットシフト  
「11101110」（10進数で-18）になる。  

算術シフトでビットパターンを右へずらす場合、あふれたビットは切り捨てるが、空いた部分には符号ビットと同じ値を挿入する。  
→  
例:「10110100」（10進数で-76）を右へ1ビットシフト  
「11011010」（10進数で-38）  
例:「00110100」（10進数で52）を右へ1ビットシフト  
「00011010」（10進数で26）になる。  
