# DependencyInjection

[DI (Dependency Injection) 依存の注入とは](https://demi-urge.com/dependency-injection/)  
[DI って何でするのかわからない人向けに頑張って説明してみる「本来の意味」](https://qiita.com/okazuki/items/0c17a161a921847cd080)  
[DI (依存性注入) って何のためにするのかわからない人向けに頑張って説明してみる](https://qiita.com/okazuki/items/a0f2fb0a63ca88340ff6)  

https://qiita.com/mizunowanko/items/53eed059fc044c5aa5dc

簡単にいえばクラスの内部でnewするのではなく、外部で用意してそれを注入するということです。  

メリット  
・ソフトウエアの階層をきれいに分離した設計が容易になる  
・コードが簡素になり、開発期間が短くなる  
・テストが容易になり、「テスト・ファースト」による開発スタイルを取りやすくなる  
・特定のフレームワークへの依存性が極小になるため、変化に強いソフトウエアを作りやすくなる（＝フレームワークの進化や、他のフレームワークへの移行に対応しやすくなる）  

デメリット  
・クラスをたくさん作るので大抵の場合はじめに工数がかかる場合が多いと思われる  
・プログラムの実行スピードが遅くなる可能性が高い  
・クラスやファイルが沢山できる  

## DIの概説

DI (Dependency Injection、依存の注入)とは、設計方法の一つ。  
部品(クラス)同士の直接的な依存をなくし、部品をより抽象的なものに依存させる。  
これによりテストしやすくなったり、部品(クラス)の変更が容易になったりする。  

## 使用目的

DIによって部品同士の直接的な依存をなくすことで、テストのときに、スタブやモックなどを簡単に使えるようにする  
部品(クラス)の変更や交換を容易にする  
など  

## DIの考え方

クラス（実体）に依存させるのはやめて、インタフェース（抽象、ルール）に依存させようということ。  

---

## DIコンテナの実装例

[C#でDIコンテナを使用してみる](https://remix-yh.net/1332/)  
[Unity Container: Constructor Injection](https://www.tutorialsteacher.com/ioc/constructor-injection-using-unity-container)  

``` C#
using MVPSample.Models;
using MVPSample.Presenters;
using MVPSample.Views;
using System;
using System.Windows;
using Unity;

namespace MVPSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            // DIで実装した場合
            IUnityContainer container = new UnityContainer();
            container.RegisterType<IRectangleView, MainWindow>();
            container.RegisterType<IRectangleModel, RectangleModel>();
            container.Resolve<RectanglePresenter>();
            container.Resolve<IRectangleView>().Show();
            container.Resolve<App>().Run();
            //①
            //container.RegisterInstance(new RectanglePresenter(view, container.Resolve<IRectangleModel>()));
            //②
            //container.RegisterType<RectanglePresenter>(
            //    new InjectionConstructor(
            //        new ResolvedParameter<IRectangleView>(),
            //        new ResolvedParameter<IRectangleModel>()
            //    )
            //);


            // DIなしで実装した場合
            //IRectangleModel model = new RectangleModel();
            //IRectangleView view = new MainWindow();
            //_ = new RectanglePresenter(view, model);
            //view.Show();
            //new App().Run();
        }
    }
}
```

実務でもUnityのDIコンテナ使ってた。  

``` C# : 実務コード
using Unity;

namespace RN3.Wpf.Common.App
{
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
    }
}

namespace RN3.Wpf.Master.Product
{
    /// <summary>
    /// アプリケーション情報を扱います。
    /// </summary>
    public class AppInfo : IAppInfo
    {
        /// <summary>
        /// クラス登録
        /// </summary>
        /// <param name="containerRegistry">コンテナ</param>
        public void RegisterTypes(IUnityContainer containerRegistry)
        {
            containerRegistry.RegisterType<EditServiceAdapter>();
            containerRegistry.RegisterType<ListServiceAdapter>();
        }
    }
}
```
