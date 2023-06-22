# Android

## AndroidでHTTP通信を許可する

`AndroidManifest.xml`に`android:usesCleartextTraffic="true"`を追加する。  
お手軽だが、あまりよろしくない模様。  

``` xml
<?xml version="1.0" encoding="utf-8"?>
<manifest ... >
    <application android:usesCleartextTraffic="true"
                    ... >
        ...
    </application>
</manifest>
```

[Android 9でHTTP通信を有効にする方法 | backport](https://backport.net/blog/2018/12/27/how_to_allow_http_on_android_9/)  

---

## Androidで縦方向固定にする方法

`Platforms/Android/MainActivity.cs`の`Activity`アノテーションに`ScreenOrientation = ScreenOrientation.Portrait`を追加するだけで行ける。  

``` cs
[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true, 
    ScreenOrientation = ScreenOrientation.Portrait, //←
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
}
```

[Android + XAMARIN + Force screen to stay in "Portrait" mode (using AndroidManifest) - Stack Overflow](https://stackoverflow.com/questions/36598052/android-xamarin-force-screen-to-stay-in-portrait-mode-using-androidmanife)  
