# linux で指定フォルダのjsonファイルを読み出すことはできるのか？

出来た。  
linuxに拡張子の概念はないはずだが、ちゃんと抜き出せることを確認した。  

---

## やりたいこと

こんな感じのappsettings.jsonを作る。  

``` json
{
  "ConnectionStrings": {
    "Dat": "Server=<ServerName>;Database=appsettings;User ID=<id>;Password=<password>;Trust Server Certificate=true",
    "Sys": "Server=<ServerName>;Database=appsettings.sys;User ID=<id>;Password=<password>;Trust Server Certificate=true"
  }
}
```

他にも`appsettings.dev.json`とか`appsettings.Hoge.json`とか作ってフォルダにまとめる。  
テストのため関係ないファイルも作っておく。  

以下のような配置にして、`FolderRead.exe`を実行した時に、上記jsonの接続情報を取得したい。  

``` txt
├─FolderRead
│ ├─TestFolder
│ │ ├─appsettings.Dev.json
│ │ ├─appsettings.Hoge.json
│ │ ├─appsettings.json
│ │ ├─jj.json.txt
│ │ ├─test
│ │ └─test.txt
│ └─FolderRead.exe
```

検証先として `AlmaLinux` 上で実行した。  

---

## 実装

1. コンソールアプリプロジェクトを作成する。  

2. 2つのパッケージをnugetからインストールする。  
   - nuget  
     - ConsoleAppFramework  
     - Microsoft.Extensions.Configuration.Json  

3. program.csを以下のようにする。  

    ``` cs
    using Microsoft.Extensions.Configuration;

    var app = ConsoleApp.Create(args);

    app.AddCommand(
        "Hoge",
        ([Option("f", "Message to display.")] string folderPath) =>
        {
            // 引数の対象フォルダのパスから、フォルダ内のjsonファイルの絶対パスを取得する。
            var filePath =  Directory.GetFiles(folderPath, "*.json");
            // ファイルの数分ループ
            foreach (string path in filePath)
            {
                // 絶対パスが表示される
                Console.WriteLine(path);
                // ファイル名だけが表示される
                Console.WriteLine(Path.GetFileName(path));

                // ConfigurationBuilderを使ってjsonを読みだす。
                // パスとファイル名を指定してbuildする。
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(folderPath)
                    .AddJsonFile(Path.GetFileName(path))
                    .Build();
                // GetConnectionStringのDatとSysの情報を出力する
                Console.WriteLine($"Dat: {configuration.GetConnectionString("Dat")}");
                Console.WriteLine($"Sys: {configuration.GetConnectionString("Sys")}");
                Console.WriteLine();
            }
        }
    );
    app.Run();
    ```

4. 次のコマンドでexeを発行する。  
   `dotnet publish -o Bundle/linux -c Release --self-contained true -r linux-x64 -p:PublishSingleFile=true`  

---

## 実行

ターミナルを開き、コマンドを実行  
`./FolderRead Hoge --folder /home/www/folderread/TestFolder/`  

パスの指定は `--folder /home/www/folderread/TestFolder/` でも `--folder "/home/www/folderread/TestFolder/"` でも問題なかった。

結果ログ

``` log
/home/www/folderread$ ls
FolderRead
TestFolder

/home/www/folderread$ chmod 755 FolderRead
/home/www/folderread$ ./FolderRead --help
Usage:  <Command>

Commands:
  help       Display help.
  Hoge       
  version    Display version.

/home/www/folderread$ ./FolderRead Hoge --help
Usage: Hoge [options...]

Options:
  -f, --folder <String>    Message to display. (Required)


/home/www/folderread$ ./FolderRead Hoge --folder /home/www/folderread/TestFolder/
/home/www/folderread/TestFolder/appsettings.Dev.json
appsettings.Dev.json
Dat: Server=<server>;Database=appsettings.dev;User ID=<id>;Password=<password>;Trust Server Certificate=true
Sys: Server=<server>;Database=appsettings.dev.sys;User ID=<id>;Password=<password>;Trust Server Certificate=true

/home/www/folderread/TestFolder/appsettings.Hoge.json
appsettings.Hoge.json
Dat: Server=<server>;Database=appsettings.hoge;User ID=<id>;Password=<password>;Trust Server Certificate=true
Sys: Server=<server>;Database=appsettings.hoge.sys;User ID=<id>;Password=<password>;Trust Server Certificate=true

/home/www/folderread/TestFolder/appsettings.json
appsettings.json
Dat: Server=<server>;Database=appsettings;User ID=<id>;Password=<password>;Trust Server Certificate=true
Sys: Server=<server>;Database=appsettings.sys;User ID=<id>;Password=<password>;Trust Server Certificate=true

/home/www/folderread$ ./FolderRead Hoge --folder "/home/www/folderread/TestFolder/"
/home/www/folderread/TestFolder/appsettings.Dev.json
appsettings.Dev.json
Dat: Server=<server>;Database=appsettings.dev;User ID=<id>;Password=<password>;Trust Server Certificate=true
Sys: Server=<server>;Database=appsettings.dev.sys;User ID=<id>;Password=<password>;Trust Server Certificate=true

/home/www/folderread/TestFolder/appsettings.Hoge.json
appsettings.Hoge.json
Dat: Server=<server>;Database=appsettings.hoge;User ID=<id>;Password=<password>;Trust Server Certificate=true
Sys: Server=<server>;Database=appsettings.hoge.sys;User ID=<id>;Password=<password>;Trust Server Certificate=true

/home/www/folderread/TestFolder/appsettings.json
appsettings.json
Dat: Server=<server>;Database=appsettings;User ID=<id>;Password=<password>;Trust Server Certificate=true
Sys: Server=<server>;Database=appsettings.sys;User ID=<id>;Password=<password>;Trust Server Certificate=true
```

相対パスは駄目な模様。  
`./FolderRead Hoge --folder ./TestFolder/`  

``` txt
appsettings.Dev.json
Fail in application running on <>c.<<Main>$>b__0_0
System.ArgumentException: The path must be absolute. (Parameter 'root')
   at Microsoft.Extensions.FileProviders.PhysicalFileProvider..ctor(String root, ExclusionFilters filters)
   at Microsoft.Extensions.FileProviders.PhysicalFileProvider..ctor(String root)
   at Microsoft.Extensions.Configuration.FileConfigurationExtensions.SetBasePath(IConfigurationBuilder builder, String basePath)
   at Program.<>c.<<Main>$>b__0_0(String folder)
   at System.RuntimeMethodHandle.InvokeMethod(Object target, Void** arguments, Signature sig, Boolean isConstructor)
   at System.Reflection.MethodInvoker.Invoke(Object obj, IntPtr* args, BindingFlags invokeAttr)
```
