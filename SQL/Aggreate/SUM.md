# SUM

---

GROUP BY しなくても合計を求めることができる。  

``` txt
ProductID   Price
1           100
2           200
3           300
```

```sql
-- 300
select SUM(Price)
from ProductTable
where ProductID IN (1,2)
```
