# Validation関係

---

## 条件によってバリデーションを切り替える

>条件によってこのアノテーションを無効にしたり有効にしたりする方法はありますでしょうか？(Dだけは必ず有効）  
>FLGというプロパティがあるとして、その値が1の時はRequiredを有効0の時は無効みたいな事が出来ればいいと思っています。
[MVC モデルのバリデーションについて](https://teratail.com/questions/80391)  

`Tag`にBindingさせてもアノテーションによるバリデーションが効くことを発見した。  
これにより、条件によってあるアノテーションを無効にしたり有効にしたりする運用を行えるようになった。  

``` C#
FixedPlayerSetting.ErrorsContainer.SetErrors(
    nameof(FixedPlayerSetting.CustomerCD),
    new List<string>() { "aaaaa" }
);
OnErrorsChanged(nameof(FixedPlayerSetting.CustomerCD));
```

``` cs
public class Hoge {
    /// <summary>
    /// コードがなければ顧客コードの入力必須というRequireIfを実現するためのバリデーションプロパティ
    /// </summary>
    [Range(typeof(bool), "false", "false", ErrorMessage = "顧客コードは必須です。")]
    [Display(Name = "顧客コード")]
    public bool IsCustomerRequire => !PackCD.HasValue && string.IsNullOrEmpty(CustomerCD);
}

Hoge.ErrorsContainer.ClearErrors(nameof(Hoge.IsCustomerRequire));
```

[[WPF] IDataErrorInfoとValidation.ErrorTemplateを利用してエラーを表示する](https://www.pine4.net/Memo/Article/Archives/427)  

---

## アノテーションを使った、リストに1件もない場合のバリデーション

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

[c# - ViewModel validation for a List - Stack Overflow](https://stackoverflow.com/questions/5146732/viewmodel-validation-for-a-list)  

---

## アノテーションによるValidationの抑制

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

[How to suppress validation when nothing is entered](https://stackoverflow.com/questions/1502263/how-to-suppress-validation-when-nothing-is-entered)  

---

## ErrorTemplate

``` xml
    <Validation.ErrorTemplate>
        <ControlTemplate>
            <TextBox Tag="{Binding IsCustomerRequire, Mode=OneWay}" />
        </ControlTemplate>
    </Validation.ErrorTemplate>
```

[[WPF] IDataErrorInfoとValidation.ErrorTemplateを利用してエラーを表示する - Netplanetes](https://www.pine4.net/Memo/Article/Archives/427)  
