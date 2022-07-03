# REPLACE

[SQLServerのREPLACE 文字列を置換する](https://sql-oracle.com/sqlserver/?p=195)  

``` sql
--文字列'SATOU'をREPLACEで連結する
--[結果] 'KATOU'
SELECT REPLACE('SATOU','S','K') ;
```

``` sql
--置換対象がない
--[結果] 'SATOU'
SELECT REPLACE('SATOU','Z','Y') ;
```

ここではREPLACEを使って「SATOU」の「Z」を「Y」に置換しようとしました。  
しかし、「SATOU」には「Z」が含まれないので「Y」には置換されませんでした。  
置換する対象がない場合はそのままの文字列が返ってきます。  
