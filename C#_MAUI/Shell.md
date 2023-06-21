# Shell

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
