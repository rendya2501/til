# Validation関係

## アノテーションを使った、リストに1件もない場合のバリデーション

<https://stackoverflow.com/questions/5146732/viewmodel-validation-for-a-list>  
画面にエラー状態は表示したくないけど、警告は出したい場合があったのでその備忘録。  

``` C# : 0件は許可しない判定
// 最小値、最大値、エラーメッセージ
[Range(1, int.MaxValue, ErrorMessage = "At least one item needs to be selected")]
// GetOnlyの省略形の書き方
public int ItemCount => Items != null ? Items.Length : 0;
```

``` C# : boolの判定
// 型の指定、最小値、最大値、エラーメッセージ
[Range(typeof(bool), "true", "true", ErrorMessage = "残高は0で保存してください。")]
public bool IsZeroBalanceAmount => Amount == 0;
```

---

## アノテーションによるValidationの抑制

[How to suppress validation when nothing is entered](https://stackoverflow.com/questions/1502263/how-to-suppress-validation-when-nothing-is-entered)  

いつぞや、IsEnableにDataのDepartmentをバインドしてF8を実行した時に、Requireのエラーが出て困ったことがあった。  
その時はどうやって調べたいいかわからなかったので、仕方なくDataではなくコントロールのValueをバインドして解決したが、
今回はバリデートを抑制したいということで、`wpf validation suppress`で調べたらいい感じのが出てきた。  
BindingクラスにValidation関係のプロパティがたくさんあったので、それっぽいやつを指定したら、実現できたのでまとめる。  

今回は`ValidatesOnNotifyDataErrors`をFalseにしたらうまく行った。  
読んで字のごとく、Dataにおけるエラー通知をどうするかって意味だと思われる。  
調べてもこのプロパティはこういうものです！って解説をしているところがまったくない。  
まぁ、解決したからいいか。  

``` XML
<im:GcTextBox
    IsReadOnly="{Binding Data.BankCD, Mode=OneWay, Converter={StaticResource NullOrEmptyToBoolConverter}, ValidatesOnNotifyDataErrors=False}"
    IsTabStop="{Binding Data.BankCD, Mode=OneWay, Converter={StaticResource NotNullOrEmptyToBoolConverter}, ValidatesOnNotifyDataErrors=False}"
/>
```

---

## Validation

[[WPF] IDataErrorInfoとValidation.ErrorTemplateを利用してエラーを表示する](https://www.pine4.net/Memo/Article/Archives/427)  

[MVC モデルのバリデーションについて](https://teratail.com/questions/80391)  
>条件によってこのアノテーションを無効にしたり有効にしたりする方法はありますでしょうか？(Dだけは必ず有効）  
>FLGというプロパティがあるとして、その値が1の時はRequiredを有効0の時は無効みたいな事が出来ればいいと思っています。

TagにBindingさせてもアノテーションによるバリデーションが効くことを発見した。  
これにより、条件によってあるアノテーションを無効にしたり有効にしたりする運用を行えるようになった。  

``` C#
                ErrorsContainer.SetErrors(
                    nameof(FixedPlayerSetting.CustomerCD),
                    new List<string>() { "aaaaa" }
                );
                FixedPlayerSetting.ErrorsContainer.SetErrors(
                    nameof(FixedPlayerSetting.CustomerCD),
                    new List<string>() { "aaaaa" }
                );
                OnErrorsChanged(nameof(FixedPlayerSetting.CustomerCD));
```
