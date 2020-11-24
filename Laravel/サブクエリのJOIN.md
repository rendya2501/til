# サブクエリのJOIN

サブクエリへクエリをJOINするために、joinSub、leftJoinSub、rightJoinSubメソッドを利用できます。  
各メソッドは３つの引数を取ります。サブクエリ、テーブルのエイリアス、関連するカラムを定義するクロージャです。  
JoinSubメソッドはVer5.8から有効な模様。teelaの5.3では存在しないメソッドだった。  

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

```PHP
$sub_query = Models\TestTable::
    join(
        'TmGeneral as LevelName',
        function ($join) {
            $join->on('LevelName.Code', 'TmCalendarDemandLevel.DemandLevelCode')
                ->where('LevelName.GolfCode', 0)
        }
    )
    ->addSelect(~~~);
$calendar = Models\TmCalendar::
    leftJoin(
        \DB::raw("({$this->rawSql($sub_query)}) as Alias"),
        function ($join) {
            $join->on('Alias.ID1', 'Base.ID1')
                ->on('Alias.ID2', 'Base.ID2');
        }
    )
```

```PHP
// 需要レベル一覧のサブクエリ
$demand_level_list = Models\TmCalendarDemandLevel::
    join(
        'TmGeneral as LevelName',
        function ($join) {
            $join->on('LevelName.Code', 'TmCalendarDemandLevel.DemandLevelCode')
                ->where('LevelName.GolfCode', 0)
                ->where('LevelName.Section', 'demand_level_code_name')
                ->where('LevelName.ValidFlag', \Config::get('const.valid_flag_type.VALID'));
        }
    )
    ->where('TmCalendarDemandLevel.CourseGroupCode', $courseGroupCode)
    ->where('TmCalendarDemandLevel.TimeBandCode', $timeBandCode)
    ->addSelect(
        'TmCalendarDemandLevel.GolfCode',
        'TmCalendarDemandLevel.BusinessDate',
        'TmCalendarDemandLevel.DemandLevelCode',
        'LevelName.Name as DemandLevelName',
        'TmCalendarDemandLevel.RateCode'
    );
$calendar = Models\TmCalendar::
    leftJoin(
        \DB::raw("({$this->rawSql($demand_level_list)}) as DemandLevelList"),
        function ($join) {
            $join->on('DemandLevelList.GolfCode', 'TmCalendar.GolfCode')
                ->on('DemandLevelList.BusinessDate', 'TmCalendar.BusinessDate');
        }
    )
    ->where('TmCalendar.GolfCode', $golfCode)
    ->whereBetween('TmCalendar.BusinessDate', [$startDate, $endDate])
    ->select('TmCalendar.BusinessDate')
    ->selectRaw('null as BusinessDay')
    ->selectRaw('null as HolidayFlag')
    ->addSelect(
        'TmCalendar.ChargeClassCode',
        'DemandLevelList.DemandLevelCode',
        'DemandLevelList.DemandLevelName',
        'DemandLevelList.RateCode'
    )
    ->selectRaw('null as PortalSiteList')
    ->get();
```
