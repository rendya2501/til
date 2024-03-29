# フレームワーク

---

## Xamarin

C#でiOSやAndroidに対応したモバイルアプリケーションを開発できるフレームワーク。  
Mono をベースとした Android, iOS などのバインディング。  
2016年2月にマイクロソフトが買収。  

Xamarinでユーザーインターフェイス部分を担っていたXamarin.Formsは、.NET MAUIがその後継となった。  
Xamarinは、.NETおよび.NET MAUIに吸収される形となり、単独のフレームワークとしてはその役割を終えることになる。  
Xamarinの全てのサポート終了は2024年5月1日。  

---

## .NET MAUI (Multi-platform App UI)

シングルコードベースでWindows、macOS、iOS、Androidのユーザーインターフェイスを構築できるフレームワーク。  
2022年5月にバージョン1.0となり正式リリースされた。  

---

## .NET Framework

Windows用のフレームワーク。  
Windows 向けの .NET Core 以前のクラシックなランタイムと API。  
.NET Framework は .NET へと移った。  

.NET Frameworkとは、Microsoft社のソフトウェア開発・実行基盤の一つ。  
同社のWindowsに同梱されており、Windowsアプリケーションの開発・実行環境の一つとして広く普及している。  
.NET対応のアプリケーションソフトは.NET Frameworkが存在する環境なら機種やOSの違いなどに拠らず同じように動作させることができる。  

.NET Frameworkの仕様はCLI（Common Language Infrastructure：共通言語基盤）としてEcma InternationalやISO/IECによって標準化され、公開されている。  
同社以外による.NET Framework互換のCLI実装も存在し、.NET対応ソフトウェアはそれら互換環境でも同じように動作する。  

---

## CLI(Common Language Infrastructure : 共通言語基盤)

米マイクロソフト（Microsoft）社が推進する.NETの実行環境（CLR）および対応プログラムの記述言語（CIL）の標準仕様を定めた規格。  
同社による実装を.NET Frameworkという。  

.Net Frameworkの基幹を構成する実行コードや実行環境等、機種やOSに依存しないプログラムの開発・実行環境を実装するために必要な諸技術の仕様を定めている。  
.NETプログラムの配布形式であるCIL（Common Intermediate Language/共通中間言語/MSIL/IL）の仕様と、開発に用いるプログラミング言語に求められる共通仕様、実行環境（CLR）が実装すべき仕様を定めている。  

CLIは特定のプログラミング言語やOS、コンピュータの機種などに依存しないコンピュータプログラムの構成法を定義しており、高級言語で記述されたプログラム（ソースコード）は、架空のコンピュータの機械語のような形式のCIL（Common Intermediate Language：共通中間言語）によるプログラムに変換（コンパイル）される。  
ソフトウェアはCILで記述された状態で配布され、実行時にそのコンピュータの機械語などへ変換されて実行される。  

Vueの開発の時にCommonLanguageInterfaceと頭文字が同じなので、ついでにまとめておいた。  

---

## CLR (Common Language Runtime : 共通言語ランタイム)

>CLRとは、米マイクロソフト（Microsoft）社がWindowsのプログラム実行環境などとして推進している.NET（ドットネット）で、対応プログラムを実行するための動作環境のこと。  
同社の.NET Frameworkなどが該当する。  

---

## POCO(Plain Old CLR Object)

メソッドを持たず、プロパティだけ持つ、データを格納するだけが目的のクラス。  
共通言語ランタイム（Common Language Runtime、CLR）のみに依存するプレーンなオブジェクト。  

---

## .NET Core  

主にサーバ向けとして登場しその後デスクトップアプリケーションなどにまで範囲を広げたオープンソースのフレームワーク。  

Microsoft社のソフトウェア開発・実行基盤の一つで、.NET Frameworkの基礎的な部分を抽出してWindowsだけでなく他のOS上でも動作するようにしたもの。  
.NETアプリケーションをOSの違いによらず様々な環境で実行できる。  
オープンソースソフトウェアとして公開しており、誰でも自由に入手、利用、改変、再配布などすることができる。  
最初のバージョンは2016年に提供が開始された。  

従来の.NET環境はWindowsでの動作を前提としており、同社が公式に提供するソフトウェア群はすべてWindowsのみの対応だったが、.NET FrameworkはWindowsに加えLinuxやmacOSで動作する。  
同時にWebアプリケーション実行環境のASP.NET（Active Server Pages .NET）や.NETライブラリなどの関連ソフトウェアもオープンソースとして公開されている。  
一方、WPF（Windows Presentation Framework）やWindows Formsなど、Windows関連技術を中心に、.NET Frameworkには含まれるが.NET Coreには含まれない機能もある。  

---

## .NET  

Xamarinの主要な部分と、「.NET Framework」「.NET Core」の3つのフレームワークを統合したフレームワーク。  

---

## ASP.NET

マイクロソフト社が提供している、Web アプリケーション開発フレームワーク。  
または.NET環境のWebサーバ上で、Webページの送信時に動的にプログラムを実行する仕組み。  

ASP（Active Server Pages）の後継で、Active Service Pagesを.Net向けにしたもの。  
動的なウェブサイトやWebアプリケーション、Webサービスの開発や運用に用いられる。  

米マイクロソフト（Microsoft）社のIIS（Internet Information Services）などのWebサーバで利用可能なソフトウェア環境で、Webページ内に記述されたプログラムを閲覧者の送信要求に従って起動し、処理結果をページ内に反映させることができる。  
一つのファイルにHTMLコードなどとプログラムコードを混在させることも、両者を分離して別々のファイルにまとめることもできる。  

プログラムの実行は同社の.NET Frameworkなどの.NET実行環境（CLR）によって行われる。  
.NETに共通する様々なプログラミング言語（Visual Basic. NET、C#、JScript.NETなど）で開発でき、共通のクラスライブラリなどを通じて豊富な機能を呼び出すことができる。  

---

## ASP.NET Core MVC

MVCフレームワークを実現する、Webフレームワークの.net coreバージョン。  

[ASP.NET Core MVCを新人に説明してみよう](https://qiita.com/saxxos/items/f8bdf3a0a9d6b8e3cfef)  

---

## Mono

Linux や macOS などで動く .NET Framework / .NET ランタイム  
Mono は既に .NET の一部となっている模様。  

---

## Unity

Mono をベースとしたゲームエンジン

---

## EntityFramework

「.NET Framework」における標準の「O/Rマッパー」。  

「O/Rマッパー」というのは「インピーダンスミスマッチ」と呼ばれる、オブジェクト指向言語とリレーショナルデータベースとの間の構造の「差異」を吸収するための「O/Rマッピング」の技術やそのためのライブラリを指します。  

[Entity Frameworkとは？概要や仕組みについて](https://rainbow-engine.com/whatis-entity-framework/)  

---

## ClickOnce

ClickOnceとは、米マイクロソフト（Microsoft）社の.NET Framework環境に対応したソフトウェアの配布方式の一つ。  
.NET Framework 2.0に搭載されたソフトウェアの配布・更新技術。  
Webブラウザを通じてネットワークから簡単な操作でソフトウェアを取得して導入することができる。  

実行時に配布サーバに最新版があるか自動的に確認する機能が組み込まれており、開発者は利用者に配布済みのソフトウェアを簡単に更新、修正することができる。  

ソフトウェアの配置場所は、通常のアプリケーションと同じようにローカル環境に導入する方法と、常にネットワーク上の所定の場所からプログラムを読み込んで起動するオンラインモードを選択することができる。  

ソフトウェア導入時には必ずしも管理者権限は必要なく、管理者と利用者が分かれている法人ユーザーなどで利便性が高い。  
不審なソフトウェアを実行できないよう、デジタル署名による発行元の確認、配布元の位置に応じた権限の設定（CAS：Code Access Security）などのセキュリティ機能にも対応する。  

>一旦インストールしたアプリケーションをその後に起動するたびに自動的にWebページ側に問い合わせて、 新しいバージョンが存在する場合は更新インストールが行なわれるという仕組みなのです。  
>[「配布の問題」を解決する「ClickOnce」](https://www.asahi-net.or.jp/~ef2o-inue/vbnet/sub13_05_010.html)  

<!--  -->
>ClickOnceはWindows FormsやWindows Presentation Foundation（WPF）のアプリケーションを配置するためのマイクロソフトの技術。  
>.NET Framework 2.0以降から利用可能。JavaプラットフォームにおけるJava Web Startに相当する。  
>
>ClickOnceの主な目標はクライアントアプリケーションの導入を容易にし、信頼性を向上させることである。  
>ClickOnceでは、Webページのリンクをクリックするだけでアプリケーションの実行が行われる。  
>さらに、従来のアプリケーション配置モデルにあった  
>
>・アプリケーションのアップデートの困難さ  
>・コンピュータに与える影響の大きさ  
>・インストール時に管理者の権限が必要であること  
>
>といった問題を解決することができる。  
>ClickOnceで配置されたアプリケーションはコンピュータ単位ではなくユーザ単位でインストールされる。  
>従ってインストールするのに管理者の権限は必要にならない。  
>ClickOnceアプリケーションはそれぞれ独立しており、互いに影響を及ぼすことはできない。  
>[wiki]  

---

## MSIX

Windows 10で導入されたWindowsアプリケーションのパッケージ形式とその配布方式。  
事実上のClickOnceの後継テクノロジー。  

・.NET Coreで作成したアプリケーションはClickOnceによる配布に対応していない  
・MSIXを作成するにはUWPの開発環境が必要  
・MSIXはWindows 10の環境でないと使用できない、という最大の課題がある  

[WPFアプリのmsixによるweb配布、自動更新方法](https://qiita.com/smi/items/17cdc56c08b8a5dab58e)  
[.NET Coreに対応したInputManで次世代のアプリ配布方法を試す（MSIX編）](https://devlog.grapecity.co.jp/inputman-dotnetcore-create-msix-package/)  
[MSIX でサービスをインストールしてみた](https://zenn.dev/okazuki/articles/fb36fad5d6d0a6906b68)  

---

## 参考リンク

[Xamarinのサポートは2024年5月1日で終了。マイクロソフトが.NET MAUI関連のロードマップを示す](https://www.publickey1.jp/blog/22/xamarin202451net_maui.html)  
[Xamarin のサポート終了と .NET 統合の話](https://zenn.dev/mayuki/articles/64f86fbc1d3da0)  
[[Microsoft] Windowsデスクトップ向け業務アプリ開発は.NETと.NET Frameworkどっち？ - 2020年3月版](https://qiita.com/sengoku/items/a93d0be65483479f1253#fn3)  

[【ゆっくり解説】C# で.NETはどれを使えばいいの？たくさんのバージョンがある.NETを大雑把に解説](https://www.youtube.com/watch?v=lCiI4LwZQJk)
