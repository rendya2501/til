# Scoreが3番目の生徒を抽出せよ

## データ

``` sql
CREATE TABLE students (
    id INTEGER,
    name VARCHAR(128),
    score INTEGER
)
INSERT INTO students (id, name, score)
VALUES(1,'Alice',85),
    (2,'Bob',90),
    (3,'Charlie',88),
    (4,'Mark',82);
```

---

## 答え

### OFFSET

``` sql
SELECT *
FROM students
WHERE score = (
    SELECT DISTINCT score
    FROM students
    ORDER BY score DESC
    OFFSET 2 ROWS
    FETCH NEXT 1 ROWS only
)
```

### CTE + Window関数

``` sql
WITH CTE AS (
    SELECT *, DENSE_RANK() OVER (ORDER BY score DESC) AS [RANK]
    FROM students
)
SELECT id,name,score
FROM CTE
WHERE [RANK] = 3
```

---

<https://twitter.com/sakamoto_582/status/1647882827378352128>
