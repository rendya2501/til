# Livet

[Livetで始めるWPF(ざっくり)入門](https://qiita.com/Kokudori/items/ab5fcac4b31d7290e630)  
[PrismとLivetで画面を閉じるMVVM](https://redwarrior.hateblo.jp/entry/2020/08/31/090000)  
[WPF Livet使ってWPFアプリを作り始めた](https://hakase0274.hatenablog.com/entry/2021/01/31/200000)  
[LivetにおけるWPFでのダイアログ表示のいろは](https://days-of-programming.blogspot.com/2018/01/livetwpf.html)  
[C# WPFアプリ(Livet)の画面遷移を理解する](https://setonaikai1982.com/livet_screen-trans/)  

[WPF Prism を使ってみた](https://houwa-js.co.jp/2020/09/20200915/)  
[Livet](https://github.com/runceel/Livet)  

---

## Livetとは

Livet(リベット) は WPF のための MVVM(Model/View/ViewModel) パターン用インフラストラクチャです。  
.NET Framework 4.5.2 以上及び .NET Core 3.1, .NET 6 で動作し zlib/libpng ライセンスで提供しています。  
zlib/libpng ライセンスでは、ライブラリとしての利用にとどめるのであれば再配布時にも著作権表示などの義務はありません。  

---

## Prismとの違いは？

## Prism Template Pack

[Prism Template Packを使わないでXamarin.FormsソリューションにPrismを適用する](https://qiita.com/ats-y/items/f76098612786b3e9cf4a)  

Visual Studio 2019 でPrism Template Pack がインストールできなくなった。  
サポートが終了した模様。  
Visual Studio 2022出ないとインストールできない。  
マニュアルにある通りは出来なくなった。  

Prism Template Pack で作ったプロジェクトを見ながら、手動で必要なコンポーネントをインポートして同じような形にして起動までできたので、Prism Template Packがなくても開発はできそうなことを確認した。  

Prism Template Packで作成したデフォルトの状態がこれ。  
なので、安直に手で必要なパッケージをインストールして、再現することが可能。  
あれこれ書き換えるけどね。  
