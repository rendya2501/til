# Image

## 画像のバインド

文字列で行ける。

``` xml
<Image Source="{Binding CreditLogo}"/>
```

``` cs
[ObservableProperty]
public ImageSource _creditLogo = "credit.png";
```

[Setting image source in view model in .NET MAUI app - Stack Overflow](https://stackoverflow.com/questions/75692776/setting-image-source-in-view-model-in-net-maui-app)  

---

## ImageSourceのQueryProperty

ImageSource型をQueryPropertyで別のページに渡そうとするとエラーが発生してしまう。  
IQueryAttributableを実装してそちらで受け取ると問題ない模様。  

以下のコードではエラーが発生する。  

``` cs
public class FromViewModel 
{
    [RelayCommand]
    public async Task Hoge() 
    {
        await Shell.Current.GoToAsync(
            $"//{nameof(AnotherToPage)}",
            new Dictionary<string, object>
            {
                "TempImageSource" = "hoge.jpg"
            }
        );
    }
}

[QueryProperty(nameof(TempImageSource), nameof(TempImageSource))]
public class AnotherToPageViewModel 
{
    [ObservableProperty] 
    public ImageSource tempImageSource;
}
```

IQueryAttributableを実装して受け取ると問題なく受け取れる

``` cs
public class FromViewModel 
{
    [RelayCommand]
    public async Task Hoge() 
    {
        await Shell.Current.GoToAsync(
            $"//{nameof(AnotherToPage)}",
            new Dictionary<string, object>
            {
                "TempImageSource" = "hoge.jpg"
            }
        );
    }
}

public class AnotherToPageViewModel : IQueryAttributable
{
    [ObservableProperty] 
    public ImageSource tempImageSource;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        TempImageSource = query["TempImageSource"] as ImageSource;
    }
}
```

[shell - Pass ImageSource to another page as parameter - Stack Overflow](https://stackoverflow.com/questions/76023121/pass-imagesource-to-another-page-as-parameter)  
