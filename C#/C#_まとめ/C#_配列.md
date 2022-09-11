# 配列

---

種類

- 通常の配列  
- 多次元配列  
- ジャグ配列  

``` C# : 基本
型名[] 変数名 = new 型名[] {値1, 値2, .....};

int[] a = new int[] {1, 3, 5, 7, 9};
int[] b = new int[] { 1, 3, 5, 7, 9, }; //最後にカンマがあっても問題ない
int[] a = {1, 3, 5, 7, 9};
var a = new[] {1, 3, 5, 7, 9};

int[] ary1; // JavaでもC#でもOK
int[] ary1 = new int[]{1, 2}; // JavaでもC#でもOK
int[] ary2 = new int[2]{1, 2}; // JavaはNGだけど、C#はOK
int[] ary2 = new int[3]{1, 2}; // これはエラー
```

``` C# : 多次元配列
変数名 = new 型名[長さ1, 長さ2]; // 2次元配列の場合
変数名 = new 型名[長さ1, 長さ2, 長さ3]; // 3次元配列の場合

// 3行2列の行列
double[,] a = new double[,]{{1, 2}, {2, 1}, {0, 1}};
// 2行3列の行列
double[,] b = new double[,]{{1, 2, 0}, {0, 1, 2}};
// 3行3列の行列
double[,] c = new double[3, 3];

int[,] ary = new int[4, 5];
int[,] ary = { { 1, 2 }, { 3, 4 }, { 5, 6 } };
```

``` C# : 配列の配列
// 3行2列の行列
double[][] a = new double[][]{
  new double[]{1, 2},
  new double[]{2, 1},
  new double[]{0, 1}
};

// 2行3列の行列
double[][] b = new double[][]{
  new double[]{1, 2, 0},
  new double[]{0, 1, 2}
};

// 3行3列の行列
double[][] c = new double[3][];
```

``` C# : ジャグ配列
// いわゆる凸凹の配列
int[][] array = new int[2][];
array[0] = new int[3];
array[1] = new int[2];
//C#ではエラーになる。[ ][ ]を使うときは、2次元目をnewを使って後入れしなきゃだって
int[ ][ ]  array ={ {1,2,3},{1,2} };
```

[未確認飛行C 配列](https://ufcpp.net/study/csharp/st_array.html)  
[配列の宣言は種類が多いな～JavaとC#の両方で出来るのどれ？](http://juujisya.jugem.jp/?eid=7)  

---

## 範囲アクセス

`a[i..j]` という書き方で「i番目からj番目の要素を取り出す」というような操作ができるようになりました。  
C# 8.0からの機能なので、.NetFramework(C# 7.3)では使えません。  

``` C#
class Program
{
    static void Main()
    {
        var a = new[] { 1, 2, 3, 4, 5 };
         // 前後1要素ずつ削ったもの
        var middle = a[1..^1];
         // 2, 3, 4 が表示される
        foreach (var x in middle)
            Console.WriteLine(x);
    }
}
```
