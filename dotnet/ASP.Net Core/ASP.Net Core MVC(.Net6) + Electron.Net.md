# ASP.Net Core MVC (.Net6) + EntityFrameworkCore + Electron.Net

---

## やり方

[このサイト(Asp.Net Core 6.0 MVC CRUD Operations with EF Core)](https://www.youtube.com/watch?v=VYmsoCWjvM4)で紹介されていASP.NET Core MVCのサンプルがgithubで公開されているので、それをクローンして、nugetからElectrionNET.APIをインストールして、パッケージマネージャーコンソールから[このサイト(【Electron】続・HTML+JS+C#=クライアントアプリケーション!? Electron.Netで開発・デプロイしてみる)](https://qiita.com/nqdior/items/3280de6737f925b89726)で紹介されているコマンドを叩いたらexe化出来た。  

以外とあっけなかった。  

---

## 所感

意外とすんなりできた。  
ASP.Net Core MVCでもElectronできることが分かった。  

Visual Studioならnugetから`ElectronNET.API`をインストールするだけでデバッグメニューにElectronが出てくるので、そのままデスクトップアプリとして開発ができる。  
ただ、毎回のデバッグは少しもたつく。  
NodeJSとか色々な機能をごちゃ混ぜにした仕組みだから仕方ないといえば仕方ないか。  

ホットリロードは効かない。  
Electron自体、コンパイルして出力する仕組みだから仕方ないかも。  
でも、最終的にexe化できればいいだけなので、普段はASP.Netアプリとしてデバッグして、出力をElectronでexeに出来れば問題ないのかも？  

exe化できることも確認した。  
ただ、インストーラーからexeをインストールする形になるので、ダブルクリックでお手軽実行みたいな感じではない。  
と、思ったが隣にあるフォルダにexeがあったので、普通にいけた。  

今回はASP.Net Core MVCをElectronを使ってexe化したわけだが、Blazorなる技術もあるらしく、これとElectronを合わせている記事も散見される。  
Blazorはrazorを使ったフレームワークということなので、こっちが最新技術っぽいので、MVCのrazorテンプレートをElectronするのはちょっと古いのかも？  
今更感があるってやつかな。  
でもって、この流れはマイクロソフトも気がついているのか、.Net7ではWebアプリケーションをデスクトップアプリケーションにもモバイルアプリケーションにも展開できる技術が来るっぽい？  
それがMAUIなのか新しいBlazorなのか定かではないけど、そうなればもうわざわざElectronを通してデスクトップアプリケーション化する必要もなくなるんだよなぁと思った。  
技術の進化が早すぎる。  

---

[TRANSFORMING YOUR ASP.NET CORE MVC APP TO NATIVE WITH ELECTRON](https://blogs.msmvps.com/bsonnino/2022/01/01/transforming-your-asp-net-core-mvc-app-to-native-with-electron/)  
[Asp.Net Core 6.0 MVC CRUD Operations with EF Core](https://www.youtube.com/watch?v=VYmsoCWjvM4)  
[【Electron】続・HTML+JS+C#=クライアントアプリケーション!? Electron.Netで開発・デプロイしてみる](https://qiita.com/nqdior/items/3280de6737f925b89726)  

[Blazor+Electron.NETでクロスプラットフォームGUIを作った時のメモ](https://narazaka.hatenablog.jp/entry/2019/12/24/Blazor%2BElectron_NET%E3%81%A7%E3%82%AF%E3%83%AD%E3%82%B9%E3%83%97%E3%83%A9%E3%83%83%E3%83%88%E3%83%95%E3%82%A9%E3%83%BC%E3%83%A0GUI%E3%82%92%E4%BD%9C%E3%81%A3%E3%81%9F%E6%99%82%E3%81%AE%E3%83%A1)  
