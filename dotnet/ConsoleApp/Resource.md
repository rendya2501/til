# リソース関連

## 埋め込みリソース

exeにファイルを埋め込むこと。  

>Visual Studioで開発をおこなっていると、画像ファイルやテキストファイルなどを、プログラムと一緒に配布する必要が出てきたりします。  
>多くの場合、付属するファイルをある特定のパスに格納することで対応したりします。  
>ただし、画像ファイルをユーザが編集できないようにプログラムを配布する場合もあります。  
>こういった場合、画像ファイルを実行プログラムに埋め込むことで、安易に編集できないように配布することができます。  
>これにより実行ファイルのフォルダに画像ファイルを配置せずとも、ビルドファイル内に埋め込まれたリソースファイルを利用することで画像の表示が可能になります。  
>[Visual Studioの埋め込みリソースについて](https://freestyle.nvo.jp/archives/59)  

---

## フォルダ全体を埋め込みリソースにする方法

特定のフォルダのファイルを全て埋め込みリソースとしたい場合、csprojファイルに以下のタグを設定する。  

``` xml
  <ItemGroup>
    <EmbeddedResource Include="対象フォルダ名\*" />
  </ItemGroup>
```

ワイルドカードにより、拡張子を指定したり、除外したりすることができるので、必要十分な設定が可能。  

実際はこのような形で埋め込むことになる。  

``` xml
<Project Sdk="Microsoft.NET.Sdk">
  <hoge>
    ~~
  </hoge>

  <ItemGroup>
    <EmbeddedResource Include="FugaFolder\*.bmp" />
  </ItemGroup>
</Project>
```

- 参考  
  - [How can I have an entire folder be an embedded resource in a Visual Studio project?](https://stackoverflow.com/questions/8994258/how-can-i-have-an-entire-folder-be-an-embedded-resource-in-a-visual-studio-proje)  

■ **例**

EFCoreのマイグレーションでコンソールアプリで遭遇した事例をそのまま使用する。  
以下のようなフォルダ構成の時にどのように表示されるか調査した。  

フォルダ構成

``` txt
Project
├─Migration
｜  ├─ConsoleAppSample.Migrations.20221129070349_First.cs
｜  ├─ConsoleAppSample.Migrations.20221129070349_First.Designer.cs
｜  ├─ConsoleAppSample.Migrations.20221129070420_Second.cs
｜  ├─ConsoleAppSample.Migrations.20221129070420_Second.Designer.cs
｜  └─ConsoleAppSample.Migrations.AppDbContextModelSnapshot.cs
└─Program.cs
```

出力コード

``` cs
Assembly assembly = Assembly.GetExecutingAssembly();
var stream = assembly.GetManifestResourceNames();
foreach (var item in stream)
    Console.WriteLine(item);
```

■ **```*```の場合の出力**

全て出力される。  

``` xml
  <ItemGroup>
    <EmbeddedResource Include="Migrations\*" />
  </ItemGroup>
```

``` txt
ConsoleAppSample.Migrations.20221129070349_First.cs
ConsoleAppSample.Migrations.20221129070349_First.Designer.cs
ConsoleAppSample.Migrations.20221129070420_Second.cs
ConsoleAppSample.Migrations.20221129070420_Second.Designer.cs
ConsoleAppSample.Migrations.AppDbContextModelSnapshot.cs
```

■ **ワイルドカードを使用**

条件に該当するファイルが出力される。  

``` xml
  <ItemGroup>
    <EmbeddedResource Include="Migrations\*.Designer.cs" />
  </ItemGroup>
```

``` txt
ConsoleAppSample.Migrations.20221129070349_First.Designer.cs
ConsoleAppSample.Migrations.20221129070420_Second.Designer.cs
```

■ **除外条件の設定**

`Remove`プロパティで設定可能な模様。  

``` xml
  <ItemGroup>
    <EmbeddedResource Include="Migrations\*.cs" />
    <EmbeddedResource Remove="Migrations\*.Designer.cs" />
    <EmbeddedResource Remove="Migrations\AppDbContextModelSnapshot.cs" />
  </ItemGroup>
```

``` txt
ConsoleAppSample.Migrations.20221129070349_First.cs
ConsoleAppSample.Migrations.20221129070420_Second.cs
```

`Include`の対として`Exclude`でも除外の指定が可能な模様。  
検証はしていない。  
`Exclude`を使用する場合は下記のように設定すると思われる。  

``` xml
  <ItemGroup>
    <EmbeddedResource
      Include="Migrations\*.cs"
      Exclude="Migrations\*.Designer.cs;Migrations\AppDbContextModelSnapshot.cs" />
  </ItemGroup>
```

- 参考  
  - [How do I exclude files/folders from a .NET Core/Standard project?](https://stackoverflow.com/questions/43173811/how-do-i-exclude-files-folders-from-a-net-core-standard-project)  
  - [方法: ビルドからファイルを除外する - MSBuild | Microsoft Learn](https://learn.microsoft.com/ja-jp/visualstudio/msbuild/how-to-exclude-files-from-the-build?view=vs-2022)  
