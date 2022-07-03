# CREATE TABLE

``` sql
CREATE TABLE [dbo].[Customers]
(
    CustomerID  nchar(5)      NOT NULL,
    CompanyName nvarchar(50)  NOT NULL,
    ContactName nvarchar (50) NULL,
    Phone       nvarchar (24) NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerID])
)
```

---

## 複数の主キーを設定するクエリ

[主キー（primary key）を複数のカラムに、その名は複合主キー（composite primary key ）](https://ts0818.hatenablog.com/entry/2017/02/04/162513)  

``` sql
-- これは死ぬ
CREATE TABLE [IDENTITY_INSERT_TEST_TBL]  
(
    [Id] INT NOT NULL IDENTITY(1,1) PRIMARY KEY,  
    [Code] INT NOT NULL PRIMARY KEY,
    [Name] nvarchar(255)
);

-- 複数の主キーを設定したかったらこのように書かないといけない
-- 意味合い的には2つで1つの主キーとみなす、という感じ
CREATE TABLE [IDENTITY_INSERT_TEST_TBL]  
(
    [Id] INT NOT NULL IDENTITY(1,1),  
    [Code] INT NOT NULL,
    [Name] nvarchar(255),
    PRIMARY KEY([Id],[Code])
);
```
