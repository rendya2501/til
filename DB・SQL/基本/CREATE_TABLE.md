
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

---

## 自動発番を主キーにした場合のINSERT

IDENTITYフィールドを除けば、どのパターンでもINSERT可能な模様。  

``` sql
-- 列名指定あり → いける
INSERT INTO [IDENTITY_INSERT_TEST_TBL] (Code,[Name]) 
VALUES(1,'A'),(2,'B'),(3,'C');

-- 列名指定なし → いける
INSERT INTO [IDENTITY_INSERT_TEST_TBL] 
VALUES(1,'A'),(2,'B'),(3,'C');

-- 列名指定なし + SELECT → いける
INSERT INTO [IDENTITY_INSERT_TEST_TBL] (Code,[Name]) 
SELECT 1,'A'
INSERT INTO [IDENTITY_INSERT_TEST_TBL] (Code,[Name]) 
SELECT 2,'B'

-- 列名指定なし + SELECT → いける
INSERT INTO [IDENTITY_INSERT_TEST_TBL] 
SELECT 1,'A'
INSERT INTO [IDENTITY_INSERT_TEST_TBL] 
SELECT 2,'B'
```

もちろん、IDENTITYフィールドを含めるとエラーになる。  

``` sql
INSERT INTO [IDENTITY_INSERT_TEST_TBL]
SELECT 1, 2,'B'

-- An explicit value for the identity column in table 'IDENTITY_INSERT_TEST_TBL' can only be specified when a column list is used and IDENTITY_INSERT is ON.
```

---
