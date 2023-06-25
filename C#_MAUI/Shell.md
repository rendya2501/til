# Shell

## System.ArgumentException: 'Ambiguous routes matched for...'

3階層あるようなページ構成において、3ページ目で`Shell.Current.GoToAsync('..')`したら発生した。  

AppShell.xamlでxaml上とコードビハインドでそれぞれRoutingを定義していると発生する模様。  
XAML側の定義を削除したら解決した。  

>これは、XAML（appshellファイル）でルートを登録し、C#のコードビハインドでルートを登録しようとしたときに発生します。  
>XAMLまたはC#のどちらかを使用して、一度だけルートを登録するようにしてください（両方は使用しないでください）。  
>[forms - Xamarin Shell Raise Ambiguous Routes matched Exception - Stack Overflow](https://stackoverflow.com/questions/58352925/xamarin-shell-raise-ambiguous-routes-matched-exception)  

---

## Shellのタイトルを消す

存在しない画像を割り当てると良い模様

``` xml : MainPage.xaml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="MauiApp.MainPage">

    <!-- タイトル削除 -->
    <Shell.TitleView>
      <Image Source="hoge.png"
             HorizontalOptions="Center"
             VerticalOptions="Center" />
    </Shell.TitleView>

</ContentPage>
```

[[.NET MAUI] タイトルバーからページ名を消す | JI0VWLのFreakな日常](https://ji0vwl.net/index.php/2022/08/15/3379/)

---

## Shell FlyoutItemのハンバーガーメニューを右に配置する方法

`\Platforms\Android\MainActivity.cs`を以下のように設定するとハンバーガーメニューを右に配置することができるが、これでは全てのコンテンツが右に移動してしまう。  

``` cs
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Window.DecorView.LayoutDirection = Android.Views.LayoutDirection.Rtl;
    }
}
```

[how can i move hamburger menu to right side on maui android? - Stack Overflow](https://stackoverflow.com/questions/75267229/how-can-i-move-hamburger-menu-to-right-side-on-maui-android)  

---

## TitleViewのカスタマイズ

[Customize the Title Bar of a MAUI app with these simple steps](https://ewerspej.hashnode.dev/customize-the-title-bar-of-a-maui-app-with-these-simple-steps)  

xamarin時代から真ん中に画像が配置されない模様。  
[xamarin.forms - Xamarin Forms Shell TitleView does not center image - Stack Overflow](https://stackoverflow.com/questions/58613454/xamarin-forms-shell-titleview-does-not-center-image)  

---

## シェルにヘッダーフッターを追加する時に参考になりそうな動画

[.NET MAUI Shell Basic Overview (Flyout, Tabs) - YouTube](https://www.youtube.com/watch?v=E9b1Sun0ecc)  

---

## Flyoutの参考等

[MAUI新生5.1-Shell导航视觉层次结构 - functionMC - 博客园](https://www.cnblogs.com/functionMC/p/17004274.html)  

---

## Flyoutのカスタマイズ

[Custom Flyout Menu in .Net MAUI - YouTube](https://www.youtube.com/watch?v=qs1otgknDHA)  
[GitHub - Abhayprince/CustomFlyoutMAUI: Customizing Flyout (App Drawer) Menu in .Net MAUI](https://github.com/Abhayprince/CustomFlyoutMAUI.git)  

---

## FlyoutItemからログアウトしたい

[このサイト(xamarin.forms - How to capture Click event or command in FlyoutItem - Stack Overflow)](https://stackoverflow.com/questions/75668861/how-to-capture-click-event-or-command-in-flyoutitem)でそのような事例を挙げてくれているが、次の参考サイトのように素直にFlyoutFooterにボタンを配置して実現したほうがいいと思った。  

[Login Flow In .NET MAUI App Shell (App Shell Login Flow) - YouTube](https://www.youtube.com/watch?v=dWnGoZY3XiE)  
[Add Dynamic Shell Item In .NET MAUI (Login Flow Part 2) - YouTube](https://www.youtube.com/watch?v=lSmRAV5IIBs)  
[GitHub - mistrypragnesh40/SimpleLoginUI](https://github.com/mistrypragnesh40/SimpleLoginUI)  

---

## ShellのTitleViewのロゴ等が消失する

偶数回の時にロゴが消失してしまう問題を確認。  
バグの模様。  
MasterPage側にShell.TitleViewを定義したら発生しなくなったので、しばらく様子を見る。  

[Shell TitleView disappearing on tab change · Issue #9687 · dotnet/maui · GitHub](https://github.com/dotnet/maui/issues/9687)  

---

## ShellContentで定義したRouteではTransientの動作をしない

AddTransientは都度新しいインスタンスを作成するDIのモードなのだが、ShellContentでRouteを定義して`GotoAsync($"{//nameof(Hoge)}")`で遷移しても新しいインスタンスが生成されないことを確認した。
そういう問い合わせも上がっており、マイクロソフトの見解的にバグの模様。  

>Q.  
>DIにtransientとして追加されたページをShellContentとして使用する場合、他のページに移動したり戻ったりしても、新しいページやViewModelをインスタンス化することはありません。  
>しかし、Pageをルーティングに追加し、Shell.Current.GoToAsyncメソッドでそのページを行ったり来たりすると、毎回新しいインスタンスが生成されます。  
>これは期待された動作なのでしょうか、それともバグなのでしょうか？ShellContentとして使用されているとき、なぜPageは決して破棄されないのですか？  
>
>A.  
>やあ、@adamradocz 課題がリンクされ、そこからさらにいくつかの課題がリンクされている。  
>この仕組みについてもう少し明確にする必要があることは明らかだと思います。  
>今のところ、多かれ少なかれ同じことに関して多くの異なる問題を抱えないために、この問題を閉じます。ありがとう！  
>
>[Page is not transient when used as ShellContent · Issue #7329 · dotnet/maui · GitHub](https://github.com/dotnet/maui/issues/7329)  

<!--  -->

``` cs
// Get current page
var page = Application.Current.MainPage.Navigation.NavigationStack.LastOrDefault();

// Load new page
await Shell.Current.GoToAsync(nameof(SecondPage), false);

// Remove old page
Application.Current.MainPage.Navigation.RemovePage(page);
```

[c# - Reload a transient page in .NET MAUI from within the page - Stack Overflow](https://stackoverflow.com/questions/73268515/reload-a-transient-page-in-net-maui-from-within-the-page)  
