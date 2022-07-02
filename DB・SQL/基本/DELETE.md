# DELETE

```sql : DELTE 基本
DELETE FROM テーブル名 WHERE (条件);
```

---

## Join

joinの場合、削除するテーブルを指定する必要があるので、DELETEの後ろにエイリアスを指定する(テーブル名ではダメ見たい)。  
DELETE FROM JOIN WHEREの流れは基本的に同じ見たい。  

<https://stackoverflow.com/questions/16481379/how-can-i-delete-using-inner-join-with-sql-server>  

``` sql
DELETE w
FROM WorkRecord2 w
INNER JOIN Employee e
ON EmployeeRun=EmployeeNo
WHERE Company = '1' AND Date = '2013-05-06'
```

---

## 複数テーブルの削除

[【MySQL】共通のIDのデータを複数テーブルからDELETEする](https://hamalabo.net/mysql-multi-delete)  

``` sql
-- 通常の場合
DELETE FROM M_UserData WHERE UserId = 1;
DELETE FROM T_TimeCard WHERE UserId = 1;

-- 複数テーブルの場合
DELETE [User],[Time]
FROM M_UserData As [User]
LEFT JOIN T_TimeCard AS [Time]
ON User.UserId = Time.UserId
WHERE User.UserId = 1
```
