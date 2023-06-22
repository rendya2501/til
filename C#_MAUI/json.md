# MAUI json

## MAUIでappsettings.jsonを使う

``` cs : MauiProgram.cs
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddTransient<MainPage>();

        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("projectname.appsettings.json");
        var config = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
        builder.Configuration.AddConfiguration(config);
        return builder.Build();
    }
}

public class Settings
{
    public int KeyOne { get; set; }
    public bool KeyTwo { get; set; }
    public NestedSettings KeyThree { get; set; } = null!;
}

public class NestedSettings
{
    public string Message { get; set; } = null!;
}
```

``` cs
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public partial class MainPage : ContentPage
{
    int count = 0;
    IConfiguration configuration;
    public MainPage(IConfiguration config, ILogger<MainPage> logger)
    {
        InitializeComponent();

        configuration = config;
        logger.LogInformation("Test");
        //configuration = MauiProgram.Services.GetService<IConfiguration>();
    }

    private async void OnCounterClicked(object sender, EventArgs e)
    {
        count++;
        CounterLabel.Text = $"Current count: {count}";

        SemanticScreenReader.Announce(CounterLabel.Text);

        var settings = configuration.GetRequiredSection("Settings").Get<Settings>();
        await DisplayAlert("Config", $"{nameof(settings.KeyOne)}: {settings.KeyOne}" +
            $"{nameof(settings.KeyTwo)}: {settings.KeyTwo}" +
            $"{nameof(settings.KeyThree.Message)}: {settings.KeyThree.Message}", "OK");
    }
}
```

[App Configuration Settings in .NET MAUI (appsettings.json) - James Montemagno](https://montemagno.com/dotnet-maui-appsettings-json-configuration/)  
[GitHub - jamesmontemagno/dotnet-maui-configuration: How to use appsettings.json in .NET MAUI](https://github.com/jamesmontemagno/dotnet-maui-configuration)  
