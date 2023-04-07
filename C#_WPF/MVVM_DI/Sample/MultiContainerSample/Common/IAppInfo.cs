using System.Windows;
using Unity;

namespace Common;

/// <summary>
/// アプリケーション情報を扱います。
/// </summary>
public interface IAppInfo
{
    /// <summary>
    /// クラス登録
    /// </summary>
    /// <param name="containerRegistry">コンテナ</param>
    void RegisterTypes(IUnityContainer containerRegistry);

    /// <summary>
    /// メインウィンドウを作成し、適切なデータコンテキスト（ViewModel）を設定します。
    /// </summary>
    /// <param name="container">依存関係の解決に使用するUnityコンテナ</param>
    /// <returns>作成されたメインウィンドウインスタンス</returns>
    Window CreateMainWindow(IUnityContainer container);
}
