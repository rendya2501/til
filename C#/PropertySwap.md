# プロパティのSwap奮闘記

FrmaeのPlayer1とPlayer2の入れ替えを実現するために色々やったのでまとめ。  
普通のスワップなら気にする必要もないのだが、プロパティの中身を全て入れ替える場合は単純なSwapではうまく行かなかった。  

``` C# : 最初に作った駄目な奴
    private void CopyRepre(ReservationPlayerView obj)
    {
        if (obj == null)
        {
            return;
        }

        void Swap(ref ReservationPlayerView x, ref ReservationPlayerView y)
        {
            ReservationPlayerView tmp = y;
            y = x;
            x = tmp;
        }

        // temp代表者
        ReservationPlayerView TempRepre = null;
        // 代表者を押した場所にする
        foreach (var frame in SelectedReservation.ReservationFrameList)
        {
            Func<ReservationPlayerView, ReservationPlayerView> Selected = (select) =>
            {
                if (frame.ReservationPlayer1 == select)
                {
                    return frame.ReservationPlayer1;
                }
                if (frame.ReservationPlayer2 == select)
                {
                    return frame.ReservationPlayer2;
                }
                if (frame.ReservationPlayer3 == select)
                {
                    return frame.ReservationPlayer3;
                }
                if (frame.ReservationPlayer4 == select)
                {
                    return frame.ReservationPlayer4;
                }
                return null;
            };


            if (frame.ReservationPlayer1 != obj && (frame.ReservationPlayer1?.ReservationRepreFlag ?? false))
            {
                //TempRepre = frame.ReservationPlayer1;
                //frame.ReservationPlayer1 = obj;
                TempRepre = frame.ReservationPlayer1;
                var TempSelect = Selected(obj);
                frame.ReservationPlayer1 = TempSelect;
                TempSelect = TempRepre;

                frame.ReservationPlayer1.ReservationRepreFlag = false;
            }
            if (frame.ReservationPlayer2 != obj && (frame.ReservationPlayer2?.ReservationRepreFlag ?? false))
            {
                //TempRepre = frame.ReservationPlayer2;
                //frame.ReservationPlayer2 = obj;
                TempRepre = frame.ReservationPlayer2;
                var TempSelect = Selected(obj);
                frame.ReservationPlayer2 = TempSelect;
                TempSelect = TempRepre;

                frame.ReservationPlayer2.ReservationRepreFlag = false;
            }
            if (frame.ReservationPlayer3 != obj && (frame.ReservationPlayer3?.ReservationRepreFlag ?? false))
            {
                if (frame.ReservationPlayer1 == obj)
                {
                    TempRepre = frame.ReservationPlayer3;
                    frame.ReservationPlayer3 = frame.ReservationPlayer1;
                    frame.ReservationPlayer1 = TempRepre;
                }
                if (frame.ReservationPlayer2 == obj)
                {
                    TempRepre = frame.ReservationPlayer3;
                    frame.ReservationPlayer3 = frame.ReservationPlayer2;
                    frame.ReservationPlayer2 = TempRepre;
                }
                if (frame.ReservationPlayer3 == obj)
                {
                    TempRepre = frame.ReservationPlayer3;
                    frame.ReservationPlayer3 = frame.ReservationPlayer3;
                    frame.ReservationPlayer2 = TempRepre;
                }
                if (frame.ReservationPlayer4 == obj)
                {
                    TempRepre = frame.ReservationPlayer3;
                    frame.ReservationPlayer3 = frame.ReservationPlayer4;
                    frame.ReservationPlayer4 = TempRepre;
                }
                frame.ReservationPlayer3.ReservationRepreFlag = false;
            }
            if (frame.ReservationPlayer4 != obj && (frame.ReservationPlayer4?.ReservationRepreFlag ?? false))
            {
                //TempRepre = frame.ReservationPlayer4;
                //frame.ReservationPlayer4 = obj;
                TempRepre = frame.ReservationPlayer4;
                var TempSelect = Selected(obj);
                frame.ReservationPlayer4 = TempSelect;
                TempSelect = TempRepre;

                frame.ReservationPlayer4.ReservationRepreFlag = false;
            }
        }
    }
```

``` C# : 愚直にやって何とか形にしたやつ
    /// <summary>
    /// 代表者入れ替え
    /// </summary>
    private void SwapRepre(ReservationPlayerView obj)
    {
        if (obj == null)
        {
            return;
        }
        // 今の代表者と押した場所を入れ替える
        foreach (var repre in SelectedReservation.ReservationFrameList)
        {
            // 今の代表者を観測する
            if (repre.ReservationPlayer1 != obj && (repre.ReservationPlayer1?.ReservationRepreFlag ?? false))
            {
                // 今の代表者を観測したら、押された場所を観測する
                foreach (var pushFrom in SelectedReservation.ReservationFrameList)
                {
                    // 押された場所を観測できたら入れ替えを実行する
                    if (pushFrom.ReservationPlayer1 == obj)
                    {
                        (repre.ReservationPlayer1, pushFrom.ReservationPlayer1) = (pushFrom.ReservationPlayer1, repre.ReservationPlayer1);
                    }
                    else if (pushFrom.ReservationPlayer2 == obj)
                    {
                        (repre.ReservationPlayer1, pushFrom.ReservationPlayer2) = (pushFrom.ReservationPlayer2, repre.ReservationPlayer1);
                    }
                    else if (pushFrom.ReservationPlayer3 == obj)
                    {
                        (repre.ReservationPlayer1, pushFrom.ReservationPlayer3) = (pushFrom.ReservationPlayer3, repre.ReservationPlayer1);
                    }
                    else if (pushFrom.ReservationPlayer4 == obj)
                    {
                        (repre.ReservationPlayer1, pushFrom.ReservationPlayer4) = (pushFrom.ReservationPlayer4, repre.ReservationPlayer1);
                    }
                }
                break;
            }
            if (repre.ReservationPlayer2 != obj && (repre.ReservationPlayer2?.ReservationRepreFlag ?? false))
            {
                foreach (var pushFrom in SelectedReservation.ReservationFrameList)
                {
                    if (pushFrom.ReservationPlayer1 == obj)
                    {
                        (repre.ReservationPlayer2, pushFrom.ReservationPlayer1) = (pushFrom.ReservationPlayer1, repre.ReservationPlayer2);
                    }
                    else if (pushFrom.ReservationPlayer2 == obj)
                    {
                        (repre.ReservationPlayer2, pushFrom.ReservationPlayer2) = (pushFrom.ReservationPlayer2, repre.ReservationPlayer2);
                    }
                    else if (pushFrom.ReservationPlayer3 == obj)
                    {
                        (repre.ReservationPlayer2, pushFrom.ReservationPlayer3) = (pushFrom.ReservationPlayer3, repre.ReservationPlayer2);
                    }
                    else if (pushFrom.ReservationPlayer4 == obj)
                    {
                        (repre.ReservationPlayer2, pushFrom.ReservationPlayer4) = (pushFrom.ReservationPlayer4, repre.ReservationPlayer2);
                    }
                }
                break;
            }
            if (repre.ReservationPlayer3 != obj && (repre.ReservationPlayer3?.ReservationRepreFlag ?? false))
            {
                foreach (var pushFrom in SelectedReservation.ReservationFrameList)
                {
                    if (pushFrom.ReservationPlayer1 == obj)
                    {
                        (repre.ReservationPlayer3, pushFrom.ReservationPlayer1) = (pushFrom.ReservationPlayer1, repre.ReservationPlayer3);
                    }
                    else if (pushFrom.ReservationPlayer2 == obj)
                    {
                        (repre.ReservationPlayer3, pushFrom.ReservationPlayer2) = (pushFrom.ReservationPlayer2, repre.ReservationPlayer3);
                    }
                    else if (pushFrom.ReservationPlayer3 == obj)
                    {
                        (repre.ReservationPlayer3, pushFrom.ReservationPlayer3) = (pushFrom.ReservationPlayer3, repre.ReservationPlayer3);
                    }
                    else if (pushFrom.ReservationPlayer4 == obj)
                    {
                        (repre.ReservationPlayer3, pushFrom.ReservationPlayer4) = (pushFrom.ReservationPlayer4, repre.ReservationPlayer3);
                    }
                }
                break;
            }
            if (repre.ReservationPlayer4 != obj && (repre.ReservationPlayer4?.ReservationRepreFlag ?? false))
            {
                foreach (var pushFrom in SelectedReservation.ReservationFrameList)
                {
                    if (pushFrom.ReservationPlayer1 == obj)
                    {
                        (repre.ReservationPlayer4, pushFrom.ReservationPlayer1) = (pushFrom.ReservationPlayer1, repre.ReservationPlayer4);
                    }
                    else if (pushFrom.ReservationPlayer2 == obj)
                    {
                        (repre.ReservationPlayer4, pushFrom.ReservationPlayer2) = (pushFrom.ReservationPlayer2, repre.ReservationPlayer4);
                    }
                    else if (pushFrom.ReservationPlayer3 == obj)
                    {
                        (repre.ReservationPlayer4, pushFrom.ReservationPlayer3) = (pushFrom.ReservationPlayer3, repre.ReservationPlayer4);
                    }
                    else if (pushFrom.ReservationPlayer4 == obj)
                    {
                        (repre.ReservationPlayer4, pushFrom.ReservationPlayer4) = (pushFrom.ReservationPlayer4, repre.ReservationPlayer4);
                    }
                }
                break;
            }
        }
    }
```

``` C# : 愚直に作った後、家で閃いたやつ
// プロパティを特定したうえでの代入はできるんだったら、デリゲートをフルに使って何とかできなかったのか？と今更ながらに思う。
var repreTemp = null;
var sourceTemp = null;
Action repre = null;
Action source = null;

foreach(frame){
    // 現在の代表者の場所を求める
    if (player1) {
        repreTemp = player1;
        repre = (obj) => player1 = obj;
    }
    // 今回押された場所を特定する
    if (player3 == obj) {
        sourceTemp = player3
        source = (repre) => player3 = repre;
    }
}

// 今回押された場所を代表者にする
repre(obj); //or repre(sourceTemp);
// 代表者だった場所に押された情報をいれる。
source(repreTemp);
```

``` C# : 閃いたアイデアを取り入れて実現できたパターン
    /// <summary>
    /// 代表者入れ替え
    /// </summary>
    /// <param name="obj">押された場所のプレーヤー情報。ReservationPlayer[n]</param>
    private void SwapRepre(ReservationPlayerView obj)
    {
        if (obj == null)
        {
            return;
        }
        // 一時的な代表者を格納
        ReservationPlayerView tempRepre = null;
        // 「押された場所→代表者」にするアクション
        Action<ReservationPlayerView> beRepreAction = null;
        // 「代表者→押された場所」にするアクション
        Action<ReservationPlayerView> beSourceAction = null;
        // 枠の1行単位でループ
        foreach (ReservationFrameView frame in SelectedReservation.ReservationFrameList)
        {
            // 今の代表者を特定する
            if (frame.ReservationPlayer1 != obj && (frame.ReservationPlayer1?.ReservationRepreFlag ?? false))
            {
                tempRepre = frame.ReservationPlayer1;
                // 今の代表者は押された場所と入れ替えるのでbeSourceActionを登録する
                beSourceAction = (source) => frame.ReservationPlayer1 = source;
            }
            else if (frame.ReservationPlayer2 != obj && (frame.ReservationPlayer2?.ReservationRepreFlag ?? false))
            {
                tempRepre = frame.ReservationPlayer2;
                beSourceAction = (source) => frame.ReservationPlayer2 = source;
            }
            else if (frame.ReservationPlayer3 != obj && (frame.ReservationPlayer3?.ReservationRepreFlag ?? false))
            {
                tempRepre = frame.ReservationPlayer3;
                beSourceAction = (source) => frame.ReservationPlayer3 = source;
            }
            else if (frame.ReservationPlayer4 != obj && (frame.ReservationPlayer4?.ReservationRepreFlag ?? false))
            {
                tempRepre = frame.ReservationPlayer4;
                beSourceAction = (source) => frame.ReservationPlayer4 = source;
            }
            // 押された場所を特定する
            switch (obj)
            {
                case ReservationPlayerView n when n == frame.ReservationPlayer1:
                    // 押された場所は今の代表者と入れ替えるのでbeRepreActionを登録する
                    beRepreAction = (repre) => frame.ReservationPlayer1 = repre;
                    break;
                case ReservationPlayerView n when n == frame.ReservationPlayer2:
                    beRepreAction = (repre) => frame.ReservationPlayer2 = repre;
                    break;
                case ReservationPlayerView n when n == frame.ReservationPlayer3:
                    beRepreAction = (repre) => frame.ReservationPlayer3 = repre;
                    break;
                case ReservationPlayerView n when n == frame.ReservationPlayer4:
                    beRepreAction = (repre) => frame.ReservationPlayer4 = repre;
                    break;
                default:
                    break;
            }
        }
        // 代表者がnullということは同じ場所をクリックしたことになるので、そのときは処理しない。
        if (tempRepre != null)
        {
            // 今回押された場所を代表者にする。
            beSourceAction(obj);
            // 代表者だった場所に押された場所の情報をいれる。
            beRepreAction(tempRepre);
        }
    }
```

``` C# : 場合によっては無駄なループが発生することに気が付いたので、平行して検索すればよくね？って思って作ったTask並列実行パターン
    /// <summary>
    /// 代表者入れ替え
    /// </summary>
    /// <param name="obj">押された場所のプレーヤー情報。ReservationPlayer[n]</param>
    private void SwapRepre(ReservationPlayerView obj)
    {
        if (obj == null)
        {
            return;
        }
        // 一時的な代表者を格納
        ReservationPlayerView tempRepre = null;
        // 「押された場所→代表者」にするアクション
        Action<ReservationPlayerView> beRepreAction = null;
        // 「代表者→押された場所」にするアクション
        Action<ReservationPlayerView> beSourceAction = null;

        // 今の代表者を特定するためのタスク
        Task findRepre = Task.Run(() =>
        {
            foreach (ReservationFrameView frame in SelectedReservation.ReservationFrameList)
            {
                // 今の代表者を特定する
                if (frame.ReservationPlayer1 != obj && (frame.ReservationPlayer1?.ReservationRepreFlag ?? false))
                {
                    tempRepre = frame.ReservationPlayer1;
                    // 今の代表者は押された場所と入れ替えるのでbeSourceActionを登録する
                    beSourceAction = (source) => frame.ReservationPlayer1 = source;
                    break;
                }
                else if (frame.ReservationPlayer2 != obj && (frame.ReservationPlayer2?.ReservationRepreFlag ?? false))
                {
                    tempRepre = frame.ReservationPlayer2;
                    beSourceAction = (source) => frame.ReservationPlayer2 = source;
                    break;
                }
                else if (frame.ReservationPlayer3 != obj && (frame.ReservationPlayer3?.ReservationRepreFlag ?? false))
                {
                    tempRepre = frame.ReservationPlayer3;
                    beSourceAction = (source) => frame.ReservationPlayer3 = source;
                    break;
                }
                else if (frame.ReservationPlayer4 != obj && (frame.ReservationPlayer4?.ReservationRepreFlag ?? false))
                {
                    tempRepre = frame.ReservationPlayer4;
                    beSourceAction = (source) => frame.ReservationPlayer4 = source;
                    break;
                }
            }
        });
        // 押された場所を特定するためのタスク
        Task findSource = Task.Run(() =>
        {
            foreach (ReservationFrameView frame in SelectedReservation.ReservationFrameList)
            {
                // 押された場所を特定する
                switch (obj)
                {
                    case ReservationPlayerView n when n == frame.ReservationPlayer1:
                        // 押された場所は今の代表者と入れ替えるのでbeRepreActionを登録する
                        beRepreAction = (repre) => frame.ReservationPlayer1 = repre;
                        break;
                    case ReservationPlayerView n when n == frame.ReservationPlayer2:
                        beRepreAction = (repre) => frame.ReservationPlayer2 = repre;
                        break;
                    case ReservationPlayerView n when n == frame.ReservationPlayer3:
                        beRepreAction = (repre) => frame.ReservationPlayer3 = repre;
                        break;
                    case ReservationPlayerView n when n == frame.ReservationPlayer4:
                        beRepreAction = (repre) => frame.ReservationPlayer4 = repre;
                        break;
                    default:
                        break;
                }
            }
        });

        // 代表者と押した場所を検索する処理を並行実行
        Task.WaitAll(findRepre, findSource);
        // 代表者がnullということは同じ場所をクリックしたことになるので、そのときは処理しない。
        if (tempRepre != null)
        {
            // 今回押された場所を代表者にする。
            beSourceAction(obj);
            // 代表者だった場所に押された場所の情報をいれる。
            beRepreAction(tempRepre);
        }
    }
```

``` C# : Taskで変数やデリゲートまで取れれば一時変数も必要なくね？と思って作った最終パターン
    /// <summary>
    /// 代表者入れ替え
    /// </summary>
    /// <param name="obj">押された場所のプレーヤー情報。ReservationPlayer[n]</param>
    private void SwapRepre(ReservationPlayerView obj)
    {
        if (obj == null)
        {
            return;
        }

        // 今の代表者を特定し、代表者の一時変数と押された場所にするためのActionを返却するタスク
        Task<(ReservationPlayerView tempRepre, Action<ReservationPlayerView> beSourceAction)> findRepre =
            Task<(ReservationPlayerView, Action<ReservationPlayerView>)>.Factory.StartNew(() =>
            {
                foreach (ReservationFrameView frame in SelectedReservation.ReservationFrameList)
                {
                    // 今の代表者を特定する
                    if (frame.ReservationPlayer1 != obj && (frame.ReservationPlayer1?.ReservationRepreFlag ?? false))
                    {
                        // 一時変数と今の代表者は押された場所と入れ替えるのでbeSourceActionを登録する
                        return (frame.ReservationPlayer1, (source) => frame.ReservationPlayer1 = source);
                    }
                    else if (frame.ReservationPlayer2 != obj && (frame.ReservationPlayer2?.ReservationRepreFlag ?? false))
                    {
                        return (frame.ReservationPlayer2, (source) => frame.ReservationPlayer2 = source);
                    }
                    else if (frame.ReservationPlayer3 != obj && (frame.ReservationPlayer3?.ReservationRepreFlag ?? false))
                    {
                        return (frame.ReservationPlayer3, (source) => frame.ReservationPlayer3 = source);
                    }
                    else if (frame.ReservationPlayer4 != obj && (frame.ReservationPlayer4?.ReservationRepreFlag ?? false))
                    {
                        return (frame.ReservationPlayer4, (source) => frame.ReservationPlayer4 = source);
                    }
                }
                return (null, null);
            });
        // 押された場所を特定し、押されたプレーヤーの一時変数と代表者にするためのActionを返却するタスク
        Task<(ReservationPlayerView tempSource, Action<ReservationPlayerView> beRepreAction)> findSource =
            Task<(ReservationPlayerView, Action<ReservationPlayerView>)>.Factory.StartNew(() =>
            {
                foreach (ReservationFrameView frame in SelectedReservation.ReservationFrameList)
                {
                    // 押された場所を特定する
                    switch (obj)
                    {
                        // switchのパターンマッチング
                        case ReservationPlayerView n when n == frame.ReservationPlayer1:
                            // 一時変数と押された場所は今の代表者と入れ替えるのでbeRepreActionを登録する
                            return (frame.ReservationPlayer1, (repre) => frame.ReservationPlayer1 = repre);
                        case ReservationPlayerView n when n == frame.ReservationPlayer2:
                            return (frame.ReservationPlayer2, (repre) => frame.ReservationPlayer2 = repre);
                        case ReservationPlayerView n when n == frame.ReservationPlayer3:
                            return (frame.ReservationPlayer3, (repre) => frame.ReservationPlayer3 = repre);
                        case ReservationPlayerView n when n == frame.ReservationPlayer4:
                            return (frame.ReservationPlayer4, (repre) => frame.ReservationPlayer4 = repre);
                        default:
                            break;
                    }
                }
                return (null, null);
            });

        // 代表者と押した場所を検索する処理を並行実行
        Task.WaitAll(findRepre, findSource);
        
        // 代表者がnullということは同じ場所をクリックしたことになるので処理しない。
        if (findRepre.Result.tempRepre != null)
        {
            // 今回押された場所を代表者にする。引数は押された場所の情報。
            findRepre.Result.beSourceAction(findSource.Result.tempSource);
            // 代表者だった場所に押された場所の情報をいれる。引数は代表者の情報。
            findSource.Result.beRepreAction(findRepre.Result.tempRepre);
        }
    }

    // // 代表者と押した場所を検索する処理を並行実行
    // Task.WaitAll(findRepre, findSource);
    // // これでも結果を取得できる。
    // var aa = Task.WhenAll(findRepre, findSource);
    // var a = aa.Result[0];
    // var b = aa.Result[1];
```
