# MVVM_DI

WPFでMVVMパターンを実装する場合、どのように行えば良いのかGPT4に聞いてみたのでまとめる。  

---

## ViewModelへのDIは可能か？

Q.  
C#のWPFでMVVMパターンでプログラムを構築するときに、ViewModelで使用するクラスをDIしたいです。  
ViewModelのコンストラクタでDIすることは出来ますか？  

A.  
はい、ViewModel のコンストラクタで DI (依存性注入) を行うことができます。  
ViewModel で使用するクラスやインターフェイスを DI コンテナから取得することで、テストや変更が容易になり、コードの再利用性が向上します。  

例として、以下の手順で ViewModel のコンストラクタで DI を実行できます。  

1. DI フレームワークを選択します。  
   一般的な C# の DI フレームワークには、Microsoft.Extensions.DependencyInjection、Autofac、Unity などがあります。  
   これらのうち、適切なものを選択し、プロジェクトに追加します。  

2. ViewModel で使用するインターフェイスやクラスを定義します。  
   例えば、データアクセスやビジネスロジックを含むサービスを定義できます。  

    ``` cs
    public interface IMyService
    {
        void DoSomething();
    }

    public class MyService : IMyService
    {
        public void DoSomething()
        {
            // 実装
        }
    }
    ```

3. ViewModel のコンストラクタでインターフェイスを受け取ります。  

    ``` cs
    public class MyViewModel : INotifyPropertyChanged
    {
    private readonly IMyService _myService;

    public MyViewModel(IMyService myService)
    {
        _myService = myService;
    }

    // ViewModel の実装
    }
    ```

4. DI コンテナを設定して、サービスと ViewModel を登録します。  
   以下は、Microsoft.Extensions.DependencyInjection を使用した例です。  

    ``` cs
    public class Startup
    {
        public IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            services.AddSingleton<IMyService, MyService>();
            services.AddTransient<MyViewModel>();
            return services.BuildServiceProvider();
        }
    }
    ```

5. アプリケーションの起動時に、DI コンテナから ViewModel を取得します。  

    ``` cs
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var startup = new Startup();
            var serviceProvider = startup.ConfigureServices();
            var mainWindow = new MainWindow
            {
                DataContext = serviceProvider.GetRequiredService<MyViewModel>()
            };
            mainWindow.Show();
        }
    }
    ```

---

## SubViewModelの必要性

Q.  
MVVMパターンではViewに対する処理をViewModelが一括で担当することになると思います。  
そうすると、場合によっては処理を大量に記述する必要があると思います。  

A.  
その通りです。MVVM パターンでは、ViewModel が View に対する処理を一括で担当することになります。  
そのため、場合によっては ViewModel に大量の処理が集まることがあります。  
ただし、適切な設計やリファクタリングによって、この問題は緩和できます。  

以下に、ViewModel の処理を整理するためのいくつかの方法を示します。  

1. サービスとリポジトリの使用  
   データアクセスやビジネスロジックをサービスやリポジトリに切り出し、ViewModel からそれらを呼び出すことで、ViewModel の責務を分散させることができます。  

    ``` cs
    public class MyViewModel : INotifyPropertyChanged
    {
        private readonly IMyService _myService;
        private readonly IMyRepository _myRepository;

        public MyViewModel(IMyService myService, IMyRepository myRepository)
        {
            _myService = myService;
            _myRepository = myRepository;
        }

        // ViewModel の実装
    }
    ```

2. コマンドの切り出し  
   ViewModel で大量のコマンドが集まる場合、コマンドごとに別のクラスを作成することで、コードの可読性と再利用性を向上させることができます。  

    ``` cs
    public class SaveCommand : ICommand
    {
        private readonly IMyService _myService;

        public SaveCommand(IMyService myService)
        {
            _myService = myService;
        }

        public bool CanExecute(object parameter)
        {
            // 実装
        }

        public void Execute(object parameter)
        {
            // 実装
        }

        // ICommand の実装
    }
    ```

3. 子 ViewModel の使用  
   ViewModel が大きくなりすぎる場合、複数の子 ViewModel に機能を分割することで、コードの管理が容易になります。  

    ``` cs
    public class ParentViewModel : INotifyPropertyChanged
    {
        public ChildViewModel1 Child1 { get; }
        public ChildViewModel2 Child2 { get; }

        public ParentViewModel(ChildViewModel1 child1, ChildViewModel2 child2)
        {
            Child1 = child1;
            Child2 = child2;
        }

        // ViewModel の実装
    }
    ```

---

## 複数のViewModel

Q.  
1つのViewに対してViewModelは1つだけのほうが望ましいですか？それとも複数のViewModelを実装する場合もあり得ますか？  

A.  
1つの View に対して ViewModel が1つだけの方が望ましい場合もありますが、複雑な画面構成や機能が多い場合には、複数の ViewModel を利用することがあります。  
そのような場合、子 ViewModel を使用して機能を分割し、コードの管理を容易にします。  

複数の ViewModel を実装する理由:  

1. 機能の分割  
   View が複数の独立した機能を持っている場合、それぞれの機能を個別の ViewModel で処理することで、コードの可読性と再利用性が向上します。  

2. コンポーネントの再利用  
   類似した機能を持つ複数の View がある場合、共通の ViewModel を使用してコンポーネントを再利用し、コードの重複を減らすことができます。  

3. 画面構成の変更に対応  
   画面構成が変更される場合、子 ViewModel を使うことで変更に対応しやすくなります。  
   例えば、タブ切り替え機能がある View であれば、各タブに対応する ViewModel を作成し、それらを親 ViewModel が管理することで、画面構成の変更に対応しやすくなります。  

ただし、1つの View に対して複数の ViewModel を使用する場合でも、適切な設計やコード管理が重要です。  
子 ViewModel の責任範囲を明確にし、適切なインターフェイスやサービスを用意して、コードの可読性と再利用性を保つことが大切です。  
