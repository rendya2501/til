# プロジェクトファイルメモ

大体GPTに聞いた内容をまとめている。

---

## 相対参照

以下のようなフォルダ構成の時に、VisualStudioで開いた時にScriptsフォルダの内容を相対参照したい。

``` txt
/DBMigration
    /Scripts
        /Dat
            /yyyymm
            /yyyymm
            /Exclude
        /Sys
            /yyyymm
            /yyyymm
            /Exclude
    /Src
        DBMigration.csproj
        DBMigration.sln
```

`<Link>`タグを使えばよい。  

``` xml
<Project Sdk="Microsoft.NET.Sdk">

    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFramework>net6.0</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
        <AssemblyName>Bundle</AssemblyName>
        <DebugType>embedded</DebugType>
        <PublishSingleFile>true</PublishSingleFile>
        <SelfContained>true</SelfContained>
    </PropertyGroup>

    <ItemGroup>
        <EmbeddedResource Include="..\Scripts\Dat\*\*.sql">
            <Link>Scripts\Dat\%(RecursiveDir)%(Filename)%(Extension)</Link>
        </EmbeddedResource>
        <EmbeddedResource Include="..\Scripts\Sys\*\*.sql">
            <Link>Scripts\Sys\%(RecursiveDir)%(Filename)%(Extension)</Link>
        </EmbeddedResource>
        <EmbeddedResource Remove="..\Scripts\Dat\Exclude\*" />
        <EmbeddedResource Remove="..\Scripts\Sys\Exclude\*" />
    </ItemGroup>
</Project>
```

---

## Linkタグの意味

``` xml
<EmbeddedResource Include="..\Scripts\Dat\*\*.sql">
    <Link>Scripts\Dat\%(RecursiveDir)%(Filename)%(Extension)</Link>
</EmbeddedResource>
```

1. `<EmbeddedResource>`タグは、リソースをプロジェクトに埋め込むための指定です。  
   これにより、ビルド時にリソースがアセンブリ（DLLまたはEXEファイル）に組み込まれます。  

2. `Include="..\Scripts\Dat\*\*.sql"`は、埋め込むリソースファイルを指定しています。  
   この場合、Scripts\Datフォルダとそのサブフォルダ内のすべての.sqlファイルが対象となります。  
   *はワイルドカードで、任意のフォルダまたはファイル名にマッチします。  

3. `<Link>`タグは、埋め込まれたリソースの論理パスを指定します。  
   この場合、`Scripts\Dat\%(RecursiveDir)%(Filename)%(Extension)`が論理パスです。  
   `%(RecursiveDir)`、`%(Filename)`、および`%(Extension)`は、MSBuildのメタデータ項目で、それぞれリソースファイルの相対ディレクトリ、ファイル名（拡張子なし）、および拡張子を表します。  
   このパスは、プログラムでリソースを参照する際に使用されます。

このコードにより、Scripts\Datフォルダおよびそのサブフォルダ内にあるすべての.sqlファイルがプロジェクトに埋め込まれ、ビルド時にアセンブリに組み込まれます。  
これによって、アプリケーションはこれらの.sqlファイルにアクセスでき、データベース操作などに使用できます。  

---

## `%(RecursiveDir)%(Filename)%(Extension)`

これはこのように書かなければならないことが決まっている模様。  

`%(RecursiveDir)`、`%(Filename)`、および`%(Extension)`は、MSBuildのプリデファインされたメタデータ項目であり、特定の意味を持っています。  
これらのメタデータ項目は、プロジェクトファイル内で特定の方法で使用することが想定されています。  

1. `%(RecursiveDir)`: ワイルドカードを使用してファイルを検索した場合、この項目は検索対象のフォルダに対するファイルの相対ディレクトリを表します。  
2. `%(Filename)`: ファイル名を拡張子なしで表します。  
3. `%(Extension)`: ファイルの拡張子を表します。  

これらのメタデータ項目を使用することで、MSBuildプロジェクトファイル内で動的にファイル名やディレクトリ構造を操作できます。  
そのため、これらの項目は決まった方法で使用されることが推奨されており、コード例のように記述することが一般的です。  

---

## ビルドアクション

ビルドアクションは、プロジェクト内のファイルがビルドプロセス中にどのように扱われるかを指定するものです。以下は、一般的なビルドアクションの例です。

1. Compile：ソースコードファイル（C#の場合は.csファイル）がコンパイルされ、アセンブリ（DLLまたはEXEファイル）に組み込まれます。通常、ソースコードファイルに適用されます。

2. Content：アプリケーションのリソースやデータファイルに使用されます。これらのファイルは、通常、ビルド時に出力ディレクトリにコピーされ、実行中のアプリケーションがアクセスできるようになります。

3. EmbeddedResource：リソースファイルがアセンブリに埋め込まれます。このタイプのリソースは、実行中のアプリケーションからリフレクションを使用してアクセスできます。通常、画像やローカリゼーションデータなどのリソースファイルに適用されます。

4. None：特定のビルドアクションが必要ないファイルに使用されます。ただし、これらのファイルに対して追加のプロパティ（例えば、出力ディレクトリへのコピー）を設定することができます。

5. Page：WPF（Windows Presentation Foundation）アプリケーションのXAMLファイルに適用されるアクションです。これにより、コンパイル時にXAMLファイルがバイナリに変換され、アセンブリに組み込まれます。

6. ApplicationDefinition：WPFアプリケーションのメインXAMLファイル（通常はApp.xaml）に適用されるアクションです。これにより、アプリケーションのエントリポイントとなるコードが生成されます。

これらは一般的なビルドアクションの例であり、プロジェクトの種類や使用しているフレームワークによっては、追加のカスタムビルドアクションがある場合があります。  
どのビルドアクションを使用するかは、ファイルの種類とアプリケーションでのそのファイルの使用方法によって異なります。  

---

## ContentとNoneで明確に差が出るパターン

ContentとNoneのビルドアクションの違いは主に意味的なものであり、実際のビルドプロセスに大きな違いをもたらすわけではありません。  
しかし、いくつかのシナリオでは、違いが明確になる場合があります。  

1. パッケージングやデプロイメント:  
Contentビルドアクションが適用されたファイルは、通常、アプリケーションのリソースやデータファイルとして扱われます。  
これらのファイルは、アプリケーションをパッケージ化やデプロイメントする際に、特別な取り扱いがされることがあります。  
例えば、ClickOnceデプロイメントでは、Contentファイルは自動的にアプリケーションのデプロイメントマニフェストに含まれます。  

2. フレームワークやツールの違い:  
特定のフレームワークやツールは、Contentビルドアクションが適用されたファイルを特別に扱う場合があります。  
例えば、ASP.NET Coreプロジェクトでは、Contentビルドアクションが適用されたファイルがウェブサイトのコンテンツファイルとして扱われ、自動的に静的ファイルサーバによって配信されます。  
一方、Noneビルドアクションが適用されたファイルは、そのような自動的な取り扱いがされません。  

3. ソリューションエクスプローラでの表示:  
Visual Studioのソリューションエクスプローラでは、ContentビルドアクションとNoneビルドアクションのファイルが異なるアイコンで表示されることがあります。  
これにより、ファイルがどのような目的で使用されているかを視覚的に識別しやすくなります。  
