# Switch式でメソッドを実行できないか色々やった結果

<https://stackoverflow.com/questions/59729459/using-blocks-in-c-sharp-switch-expression>

結果的に出来なかった。  
文法上は怒られないのだが、実際に動かしてみると"Operation is not valid due to the current state of the object."と言われて動かない。  
if elseif でがりがり書くなら、switchの出番なんじゃないかと思ってやってみた。  
でも、switchも絶対にbreakを書かないといけないのが気に食わない。  
そこでswitch式に白羽の矢が立ったのだが、絶対にreturnがないといけない形式なので、  
Actionとして受け取る必要がある。  
若干わかりにくいけど、スマートだから妥協できないかと思ったけど、  
そもそも動かないのであれば、switchの1行表示で妥協するしかなさそうだ。  

Q.MDでコードをそのまま表示するための何かはないのだろうか。  
A.シングルクォート3つで開始部分と終了部分を挟むか、スペース4つのインデントを入れるといいみたい。

    /// <summary>
    /// データプロパティ変更時イベント
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DataPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        // 元のコード。{}がない分まだすっきりしているが、e.PropertyNameを毎回書かないといけないなら,
        // それはswitchを使うべき。
        // 部門コード
        if (e.PropertyName == nameof(Data.DepartmentCD))
            SetDepartmentCD();
        // 商品コード
        else if (e.PropertyName == nameof(Data.ProductCD))
            SetProductCD();
        // 商品分類コード
        else if (e.PropertyName == nameof(Data.ProductClsCD))
            SearchProductCls();

        // 1行にまとめたswitch。普通のよりは断然いいが、breakを毎回書かないといけない。
        // 言語仕様的に仕方がないが・・・。
        switch (e.PropertyName)
        {
            // 部門コード
            case nameof(Data.DepartmentCD): SetDepartmentCD(); break;
            // 商品コード
            case nameof(Data.ProductCD): break;
            // 商品分類コード
            case nameof(Data.ProductClsCD): break;
            default: break;
        }

        // 本命。文法上は怒られないけど実行するとエラーになる。
        // 実行するのではなく,Actionデリゲートとして受け取り、最後に実行する。
        Action result = e.PropertyName switch
        {
            // 部門コード
            nameof(Data.DepartmentCD) => SetDepartmentCD,
            // 商品コード
            nameof(Data.ProductCD) => SetProductCD,
            // 商品分類コード
            nameof(Data.ProductClsCD) => SearchProductCls,
            _ => throw new InvalidOperationException()
        };
        result();
    }
