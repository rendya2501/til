# FullOuterJoin

LinqでFULL OUTER JOIN(完全外部結合)を実現するには？  

- LEFT OUTER + RIGHT OUTER  
- 専用のメソッドはないので、拡張メソッドを自作する  

``` cs
var firstNames = new[]
{
    new { ID = 1, Name = "John" },
    new { ID = 2, Name = "Sue" },
};
var lastNames = new[]
{
    new { ID = 1, Name = "Doe" },
    new { ID = 3, Name = "Smith" },
};

// [0]:(1, "John", "Doe")
// [1]:(2, "Sue", null)
var leftOuterJoin = firstNames
    .GroupJoin(
        lastNames,
        firstName => firstName.ID,
        lastName => lastName.ID,
        (firstName, lastName) => (
            ID : firstName.ID,
            FirstName : firstName.Name,
            LastName : lastName.FirstOrDefault()?.Name
        )
    ).ToList();

// [0]:(1, "John", "Doe")
// [1]:(3, null, "Smith")
var rightOuterJoin = lastNames
    .GroupJoin(
        firstNames,
        lastName => lastName.ID,
        firstName => firstName.ID,
        (lastName, firstName) => (
            ID : lastName.ID,
            FisrtName : firstName.FirstOrDefault()?.Name,
            LastName : lastName.Name
        )
    ).ToList();

// [0]: (1, "John", "Doe")
// [1]: (2, "Sue", null)
// [2]: (3, null, "Smith")
var fullOuterJoin = leftOuterJoin.Union(rightOuterJoin);
```

``` cs
var firstNames = new[]
{
    new { ID = 1, Name = "John" },
    new { ID = 2, Name = "Sue" },
};
var lastNames = new[]
{
    new { ID = 1, Name = "Doe" },
    new { ID = 3, Name = "Smith" },
};

//[0]:{ ID = 1, FirstName = "John", LastName = "Doe" }
//[1]:{ ID = 2, FirstName = "Sue", LastName = null }
var leftOuterJoin = firstNames
    .GroupJoin(
        lastNames,
        firstName => firstName.ID,
        lastName => lastName.ID,
        (firstName, lastName) => new
        {
            ID = firstName.ID,
            FirstName = firstName.Name,
            LastName = lastName.FirstOrDefault()?.Name
        }
    ).ToList();

// [0]: { ID = 1, FisrtName = "John", LastName = "Doe" }
// [1]: { ID = 3, FisrtName = null, LastName = "Smith" }
var rightOuterJoin = lastNames
    .GroupJoin(
        firstNames,
        lastName => lastName.ID,
        firstName => firstName.ID,
        (lastName, firstName) => new
        {
            ID = lastName.ID,
            FisrtName = firstName.FirstOrDefault()?.Name,
            LastName = lastName.Name
        }
    ).ToList();

// [0]: { ID = 1, FirstName = "John", LastName = "Doe" }
// [1]: { ID = 2, FirstName = "Sue", LastName = null }
// [2]: { ID = 1, FisrtName = "John", LastName = "Doe" }
// [3]: { ID = 3, FisrtName = null, LastName = "Smith" }
var fullOuterJoin = leftOuterJoin.Union<dynamic>(rightOuterJoin);

// [0]: { ID = 1, FirstName = "John", LastName = "Doe" }
// [1]: { ID = 2, FirstName = "Sue", LastName = null }
// [2]: { ID = 1, FisrtName = "John", LastName = "Doe" }
// [3]: { ID = 3, FisrtName = null, LastName = "Smith" }
fullOuterJoin = leftOuterJoin.Union<dynamic>(rightOuterJoin).Distinct();
```

[c# - LINQ - Full Outer Join - Stack Overflow](https://stackoverflow.com/questions/5489987/linq-full-outer-join)  
