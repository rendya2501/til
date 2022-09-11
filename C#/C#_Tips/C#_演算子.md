# C#_演算子

---

## 名称一覧

``` txt
null合体演算子         | ??
null条件演算子         | ?.
null合体代入演算子     | ??=  → 8.0から使用可能
複合代入演算子         | +=, -=, *=, /=, %=, &=, |=, ^=, <<=, >>=
条件演算子(三項演算子) | ?:
                       | ! 演算子
```

---

## null許容参照型

`a!.○○`の`!.`が探しても全然見つからなかった。  
これは`!.`ではなく、`a!`までがNull許容参照型らしい。  
IDEのnullの警告を抑制する程度の演算子らしい。  

Null許容参照型は8.0以上と警告が出る。  

[null許容参照型](https://ufcpp.net/study/csharp/resource/nullablereferencetype/?p=3#null-forgiving)  
