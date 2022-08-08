# 構造体

- 構造体は値型、クラスは参照型  
- 構造体はスタック領域に展開され、クラスはヒープ領域に展開される。  
- int等と同じなので、メソッドを抜けたらメモリから解放される。  
- もちろんクラスより軽量。newの処理も早いし、メモリも食わない。  
- 構造体の初期状態は0初期化状態という。→構造体の既定値(default value)と呼ぶ。クラスの初期状態はnull。  
- 値型なので、newする必要はない。でもnewできる。  
  - int型をnewしているのと同じ意味になる。  
  - newはコンストラクタを呼ぶ。  
  - newしない場合、コンストラクタを呼ばない。  

``` C#
    public struct Circle
    {
        public double r;
        public double CalcCircum(double r) => 3.14 * 2 * r;
        public double CalcArea(double r) => 3.14 * r * r;
    }
    
    // 方法1.new演算子を使う方法
    Circle c1 = new Circle();
    c1.r = 10.0;
    Console.WriteLine("半径{0}の円周は{1}、面積は{2}", c1.r, c1.CalcCircum(c1.r), c1.CalcArea(c1.r));
    
    // 方法2.new演算子を使わない方法
    Circle c2;
    c2.r = 20.0;
    Console.WriteLine("半径{0}の円周は{1}、面積は{2}", c2.r, c2.CalcCircum(c2.r), c2.CalcArea(c2.r));
    
    // 方法3.インスタンス化と同時に初期化
    Circle c3 = new Circle() {r = 30.0};
    Console.WriteLine("半径{0}の円周は{1}、面積は{2}", c3.r, c3.CalcCircum(c3.r), c3.CalcArea(c3.r));

    // 方法4.default演算子を使った方法 C# 2.0～9.0 まで、p1と同じ意味っぽい
    var p4 = default(Circle);
```
