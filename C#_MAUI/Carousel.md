# Carousel

XAML

``` xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage  
    xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
    xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
    xmlns:toolkit="http://schemas.microsoft.com/dotnet/2022/maui/toolkit"
    xmlns:vm="clr-namespace:CarouselViewModels"
    x:Class="CarouselViews.IntroPage">
    
    <Grid Margin="10" RowDefinitions="*,Auto">
        
        <CarouselView 
            Grid.Row="0" 
            Position="{Binding Position}"
            IndicatorView="indicatorView" 
            Loop="False"
            HorizontalOptions="FillAndExpand" 
            VerticalOptions="FillAndExpand" 
            ItemsSource="{Binding IntroScreens}">
            <CarouselView.ItemTemplate>
                <DataTemplate x:DataType="vm:IntroScreenModel">
                    <Grid RowDefinitions="*,*">
                        <Image 
                            Grid.Row="0"
                            Source="{Binding IntroImage}" 
                            HeightRequest="100" 
                            WidthRequest="100"
                            VerticalOptions="End"
                            HorizontalOptions="Center"
                            Aspect="AspectFit" />

                        <VerticalStackLayout 
                            Grid.Row="1"
                            HorizontalOptions="Center" 
                            VerticalOptions="Start">
                            <Label 
                                Text="{Binding IntroTitle}" 
                                FontSize="Medium" 
                                FontAttributes="Bold" 
                                TextColor="White"
                                HorizontalTextAlignment="Center"/>
                            <Label 
                                Margin="0,10,0,0"
                                Text="{Binding IntroDescription}" 
                                FontSize="Small" 
                                TextColor="White" 
                                HorizontalTextAlignment="Center"/>
                        </VerticalStackLayout>
                    </Grid>
                </DataTemplate>
            </CarouselView.ItemTemplate>
        </CarouselView>
        
        <Grid ColumnDefinitions="*,*,*" Grid.Row="1" >
            <Button 
                Grid.Column="0"
                Text="スキップ"
                TextColor="White"
                HorizontalOptions="Start"
                BackgroundColor="Transparent" 
                Command="{Binding SkipOrStartCommand}"
                IsVisible="{Binding IsEnd, Converter={toolkit:InvertedBoolConverter}}"/>

            <IndicatorView 
                Grid.Column="1"
                x:Name="indicatorView" 
                VerticalOptions="Center"
                HorizontalOptions="Center"
                IndicatorSize="10"
                IndicatorColor="LightGray"  
                SelectedIndicatorColor="{x:StaticResource Primary}" />

            <Button 
                Grid.Column="2"
                Text="次へ"
                TextColor="White"
                HorizontalOptions="End"
                BackgroundColor="Transparent" 
                Command="{Binding NextCommand}">
                <Button.Triggers>
                    <DataTrigger  
                        TargetType="Button"
                        Binding="{Binding IsEnd}"
                        Value="True">
                        <Setter Property="Text" Value="はじめる" />
                    </DataTrigger>
                </Button.Triggers>
            </Button>
        </Grid>        
    </Grid>    
</controls:BaseContentPage>
```

ViewModel

``` cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CarouselViews;
using System.Collections.ObjectModel;

namespace CarouselViewModels;

/// <summary>
/// 導入ページViewModel
/// </summary>
public partial class IntroPageViewModel : BaseViewModel
{
    /// <summary>
    /// 現在の選択地点
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsEnd))]
    public int _position = 0;

    /// <summary>
    /// カルーセル
    /// </summary>
    public ObservableCollection<IntroScreenModel> IntroScreens { get; set; } = new();

    /// <summary>
    /// 最後かどうか
    /// </summary>
    public bool IsEnd => (IntroScreens.Count - 1) == Position;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public IntroPageViewModel()
    {
        IntroScreens.Add(new IntroScreenModel
        {
            IntroTitle = "オンラインチェックイン",
            IntroDescription = "スマホをかざしてスムーズにチェックイン",
            IntroImage = "hoge.png"
        });
        IntroScreens.Add(new IntroScreenModel
        {
            IntroTitle = "スマホ決済",
            IntroDescription = $"お支払いもスマホをかざすだけ。{Environment.NewLine}*事前にクレジットカード登録が必要です。",
            IntroImage = "fuga.png"
        });
        IntroScreens.Add(new IntroScreenModel
        {
            IntroTitle = "プレイ動画",
            IntroDescription = "ラウンド中の動画を確認し、ダウンロード",
            IntroImage = "piyo.png"
        });
    }


    /// <summary>
    /// スキップかスタート
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    public async Task SkipOrStart()
    {
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }

    /// <summary>
    /// 次へコマンド
    /// </summary>
    /// <returns></returns>
    [RelayCommand]
    public async Task Next()
    {
        if (Position >= IntroScreens.Count - 1)
        {
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
        else
        {
            Position += 1;
        }
    }
}

/// <summary>
/// 導入ページの表示項目
/// </summary>
public class IntroScreenModel
{
    public string IntroTitle { get; set; }
    public string IntroImage { get; set; }
    public string IntroDescription { get; set; }
}
```

[Onboarding Screen UI In .NET MAUI / Xamarin (Using Carousel View) - YouTube](https://www.youtube.com/watch?v=R6Uah-USuxU)  
[Mastering MAUI CarouselView & MAUI IndicatorView: DotNet MAUI Tutorial - YouTube](https://www.youtube.com/watch?v=kw9-_GLruUg)  
