# NULLを排除した設計

[NULLを排除した設計①](http://onefact.jp/wp/2014/08/26/null%E3%82%92%E6%8E%92%E9%99%A4%E3%81%97%E3%81%9F%E8%A8%AD%E8%A8%88/)  

<https://twitter.com/rude_rockers/status/1522311142689107969>  
>データベースにNULLは極力使わない
SQLは基本的にTRUEかFALSEで検知しているが、そこにNULLがあるとunknownが入りバグの温床に（3値理論）
とは言え完全にNULLのない世界は難しい
数値なら0や-1などで、文字列なら空文字などで対応
日付でも「9999/12/31」なんてやり方があるみたい

[「SQLのアンチパターン」3パターンを解説します！](https://products.sint.co.jp/topsic/blog/sql-anti-pattern)  
