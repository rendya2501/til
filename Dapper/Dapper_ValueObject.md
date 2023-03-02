# Dapperで値オブジェクト

Dapperで値オブジェクトをマッピングできるか調査。  
できるけど、ある程度の処理は必要。  

---

## テストデータ

[ここ(Dapper のクエリ - Qiita)](https://qiita.com/masakura/items/3409a766e46580a5ad99)の例をそのまま参考にさせてもらう。  

テーブル : Customer  
|カラム|型|説明|
|---|---|---|
|ID|int|主キー。CustomerIDとして保持する。|
|Name|string|名前|
|Gender|int|0:不明,1:男性,2:女性|

``` sql
CREATE TABLE Customer (
    ID INT PRIMARY KEY,
    Name NVARCHAR,
    Gender INT
)
INSERT INTO Customer 
VALUES (1,'Test',0)
```

---

## コンストラクタ方式

Dapperはコンストラクタ―を利用できるので、コンストラクタ内でValueObjectへマッピングする。  
一番愚直でわかりやすいと思うが、フィールドの数が多い場合、コンストラクタの引数の数がすごいことになる。  

``` cs
using Dapper;
using Microsoft.Data.SqlClient;

public class Dapper_Constructor
{
    public static void Execute()
    {
        string constr = @"Server=<>;Database=<>;User ID=<>;Password=<>;Trust Server Certificate=true";
        using SqlConnection connection = new SqlConnection(constr);
        string query = "SELECT * FROM Customer";
        var result = connection.Query<Customer>(query);
    }
}

// Customerテーブルエンティティ
public class Customer 
{
    public CustomerID ID { get; };
    public string Name { get; }
    public GenderType Gender { get; }

    Customer (string id, string name, int gender)
    {
        ID = new CustomerID(id);
        Name = Name;
        Gender = gender;
    }
}

// CustomerID値オブジェクト
public class CustomerID
{
    public int Value { get; }
    public CustomerID(int value)
    {
        this.Value = value;
    }
}

// 性別Enum
public enum GenderType {
    Unknown = 0,
    Male = 1,
    Female = 2,
}
```

---

## AddTypeHandler方式

Dapperの`SqlMapper.AddTypeHandler()`メソッドなるものを使えば、値オブジェクトへの直接のマッピングが可能。  
しかし、この方式の場合、マッピングさせたい値オブジェクトの分、`TypeHandler`を定義する必要がある。  

``` cs
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

public class Dapper_AddTypeHandler
{
    public static void Execute()
    {
        // アプリケーションの開始時にマッパーを登録する
        SqlMapper.AddTypeHandler(new CustomerIDTypeHandler());
        
        string constr = @"Server=<>;Database=<>;User ID=<>;Password=<>;Trust Server Certificate=true";
        using SqlConnection connection = new SqlConnection(constr);
        string query = "SELECT * FROM Customer";
        var result = connection.Query<Customer>(query);
    }
}

// Customerテーブルエンティティ
public class Customer 
{
    public CustomerID ID { get; };
    public string Name { get; }
    public GenderType Gender { get; }
}

// CustomerID値オブジェクト
public class CustomerID
{
    public int Value { get; }
    public CustomerID(int value)
    {
        this.Value = value;
    }
}

// 性別Enum
public enum GenderType {
    Unknown = 0,
    Male = 1,
    Female = 2,
}

// マッパー
public class CustomerIDTypeHandler : SqlMapper.TypeHandler<CustomerID>
{
    public override void SetValue(IDbDataParameter parameter, CustomerID value)
    {
        parameter.DbType = DbType.Int32;
        parameter.Value = value.Value;
    }

    public override CustomerID Parse(object value)
    {
        return new CustomerID((int)value);
    }
}
```

---

## Builder方式

マッパーを定義する必要はないが、それ以外が長い。

``` cs
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;


public class DapperBuilder
{
    public static void Execute()
    {
        string constr = @"Server=<>;Database=<>;User ID=<>;Password=<>;Trust Server Certificate=true";
        using SqlConnection connection = new SqlConnection(constr);
        string query = "SELECT * FROM Customer";
        var result = connection.Query<Customer>(query);
        var result = connection.Query<TFr_BasicItemBuilder>(query).ToBasicItems();
    }
}

[Table("TFr_BasicItem")]
public class TFr_BasicItemBuilder
{
    public string OfficeCD { get; set; }
    public DateTime BusinessDate { get; set; }
    public int FeeTypeCD { get; set; }
    
    public TFr_BasicItem ToBasicItem()
    {
        return new TFr_BasicItem(OfficeCD, BusinessDate, FeeTypeCD);
    }
}

public sealed class TFr_BasicItem
{
    public string OfficeCD { get; set; }
    public DateTime BusinessDate { get; set; }
    public FeeTypeCD FeeTypeCD { get; set; }

    public TFr_BasicItem(string officeCD, DateTime businessDate, int feeTypeCD)
    {
        OfficeCD = officeCD;
        BusinessDate = businessDate;
        FeeTypeCD = new FeeTypeCD(feeTypeCD);
    }
}


public class FeeTypeCD
{
    public int Value { get; init; }

    public FeeTypeCD(int value)
    {
        this.Value = value;
    }
}

public static class CustomerBuilderExtensions
{
    public static IEnumerable<TFr_BasicItem> ToBasicItems(this IEnumerable<TFr_BasicItemBuilder> builders)
    {
        return builders?.Select(builder => builder.ToBasicItem());
    }
}
```

---

[Dapper のクエリ - Qiita](https://qiita.com/masakura/items/3409a766e46580a5ad99)  
