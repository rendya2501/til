using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyLoginApp.Views;

namespace MyLoginApp.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty]
    public string userName;

    [ObservableProperty]
    public string password;

    [RelayCommand]
    private async void Login()
    {
        if (IsValid(UserName, Password))
        {
            // 認証が成功した場合、SuccessPageに遷移する
            await Shell.Current.GoToAsync($"//{nameof(SuccessPage)}");
        }
        else
        {
            // 認証が失敗した場合、エラーメッセージを表示する
            //await Application.Current.MainPage.DisplayAlert("ログイン失敗", "ユーザー名またはパスワードが正しくありません。", "OK");
            await Shell.Current.DisplayAlert("ログイン失敗", "ユーザー名またはパスワードが正しくありません。", "OK");
        }

        static bool IsValid(string username, string password)
        {
            // このメソッドで、実際のユーザー名とパスワードの検証ロジックを実装してください。
            return username == "admin" && password == "admin";
        }
    }
}