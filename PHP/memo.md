# php色々

## 配列をカンマ区切りの文字列に変換する方法

implode関数を使用する。  
ちなみにLaravelのEroquent使ってcollectionをカンマ区切りの文字列にするサンプルです。

```php
$some_list = Models\TableModel::
    where()
    ->pluck('FiledName')
    ->toArray();
$str = implode(",", $some_list);
```
