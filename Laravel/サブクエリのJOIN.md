# サブクエリのJOIN

サブクエリへクエリをJOINするために、joinSub、leftJoinSub、rightJoinSubメソッドを利用できます。  
各メソッドは３つの引数を取ります。サブクエリ、テーブルのエイリアス、関連するカラムを定義するクロージャです。  
JoinSubメソッドはVer5.8から有効な模様。実務の5.3では存在しないメソッドだった。  

```PHP
$latestPosts = DB::table('posts')
    ->select('user_id', DB::raw('MAX(created_at) as last_post_created_at'))
    ->where('is_published', true)
    ->groupBy('user_id');

$users = DB::table('users')
    ->joinSub($latestPosts, 'latest_posts', function ($join) {
        $join->on('users.id', '=', 'latest_posts.user_id');
    })->get();
```

[参考サイト](https://readouble.com/laravel/5.8/ja/queries.html#joins)  

## joinSubの代用

```php
$sub_query = Models\TestTable::
    join(
        'OtherTable as Temp',
        function ($join) {
            $join->on('Temp.ClsCode', 'TestTable.ClsCode')
                ->where('Temp.Code', 0)
        }
    )
    ->addSelect(~~~);
$query = Models\TestTable2::
    leftJoin(
        \DB::raw("({$this->rawSql($sub_query)}) as Alias"),
        function ($join) {
            $join->on('Alias.ID1', 'Base.ID1')
                ->on('Alias.ID2', 'Base.ID2');
        }
    )
```
