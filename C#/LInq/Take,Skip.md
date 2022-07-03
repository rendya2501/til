
# Take,Skip

[Take, Skip](https://symfoware.blog.fc2.com/blog-entry-1927.html)  

- Take:指定した数だけ要素をより出します。  
- Skip:指定した数だけ読み飛ばします。  

リストがあって、入力した人より下の人を対象にしたい場合があった。  
Linqでなんかないか探したらありました。  

最初はTakeしか見つけられなくて、一番上を飛ばすために、Reserse().Take().Reverse()なんてくそ面倒くさいことをする始末だった。  
そこで見つけたSkip。まさに神だったね。  

``` C#
// 単純なTakeの例。
// 先頭から2つの要素を取得する。
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Take(2)) {
    Console.WriteLine(item);
}
// →1,2


// 単純なSkipの例。
// 先頭から2つの要素を読み飛ばす。Skipした以降の全要素が取得できます。
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Skip(2)) {
    Console.WriteLine(item);
}
// →3,4,5


// 実際には値を読み飛ばすSkipと合わせて使用することになると思います。
// 1つ読み飛ばし、先頭から2つの要素を取得。
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Skip(1).Take(2)) {
    Console.WriteLine(item);
}
// →2,3


// TakeしたあとにSkipすることも可能。
// 先頭から2つの要素を取得し、1つ読み飛ばす。
int[] src = new int[]{1, 2, 3, 4, 5};
foreach(int item in src.Take(2).Skip(1)) {
    Console.WriteLine(item);
}
// →2
```
