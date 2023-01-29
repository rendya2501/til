# コマンドラインパーサー

ターミナルで`--help`とか`-h`みたいなのを解析して使いやすいようにあれこれしてくれるライブラリ。  

×  

- [Microsoft/Microsoft.Extensions.CommandLineUtils]  

○  

- [Cysharp/ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework)  
- [dotnet/CliCommandLineParser](https://github.com/dotnet/CliCommandLineParser)  
- [natemcmaster/CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils)  
- [GitHub - commandlineparser/commandline](https://github.com/commandlineparser/commandline)  

---

## Microsoft/Microsoft.Extensions.CommandLineUtils

開発終了  

>以前はASP.NETのコンポーネントの一つとして、Microsoft.Extensions.CommandlineUtilsというものがあった。  
>ただ、コマンドラインオプションというのはどうしても要望が多くなってしまう類のもので、本質ではない部分のメンテコストが
>増大することを危惧したASP.NET Coreチームは、このパッケージをメンテしないことに決定した。  
>[https://qiita.com/skitoy4321/items/6ce755d0374bd9bc8387](https://qiita.com/skitoy4321/items/742d143b069569014769)  

その記事  
[Continue work Microsoft.Extensions.CommandlineUtils · Issue #257 · dotnet/extensions · GitHub](https://github.com/dotnet/extensions/issues/257)  

---

## dotnet/CliCommandLineParser

[GitHub - dotnet/CliCommandLineParser](https://github.com/dotnet/CliCommandLineParser)

Microsoft謹製。  
[Microsoft/Microsoft.Extensions.CommandLineUtils]の実質的な後継？  
他のコマンドラインパーサーの紹介はあるが、こいつの紹介がない。  

---

## natemcmaster/CommandLineUtils

[GitHub - natemcmaster/CommandLineUtils](https://github.com/natemcmaster/CommandLineUtils)

>このライブラリは Microsoft 謹製ではありませんが、開発しているnatemcmaster氏は Microsoft の ASP.NET Core チームの中の人の模様。  
>先ほどのパッケージ[(Microsoft/Microsoft.Extensions.CommandLineUtils)]の正統後継であると考えてよいでしょう。  
>[.NET での CLI 処理ライブラリについて - 鷲ノ巣](https://tech.blog.aerie.jp/entry/2018/06/22/124630)  

---

## Cysharp/ConsoleAppFramework

C#の可能性を切り開いていく専門会社が作ってるOSSライブラリ  

コンソールアプリでCLIプログラムのひな形を紹介している。  
[GitHub - Cysharp/ConsoleAppFramework](https://github.com/Cysharp/ConsoleAppFramework)  

---

## commandlineparser/commandline

[GitHub - commandlineparser/commandline](https://github.com/commandlineparser/commandline)  

wikiが充実しているので、そちらを見ながら構築すれば問題ないだろう。  
[commandlineparser/commandline - wiki](https://github.com/commandlineparser/commandline/wiki/Getting-Started)  

commandlineparser/commandlineを紹介しているページ  
[C#とF#向けコマンドラインパーサーCommandLineParserの紹介 - Qiita](https://qiita.com/skitoy4321/items/742d143b069569014769)  

---

[.NET での CLI 処理ライブラリについて - 鷲ノ巣](https://tech.blog.aerie.jp/entry/2018/06/22/124630)  
