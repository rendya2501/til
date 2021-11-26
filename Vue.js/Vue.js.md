# 開発についてのあれこれ

## 環境構築に関して

vueへのパスはユーザーの環境変数で充分だった。
SETコマンドで適当な環境変数作っても意味がない。
ユーザーの環境変数のPathに追加する必要がある。
コマンドプロンプトは立ち上げなおさないと、反映されない。

Pathの環境変数はセミコロンで区切る。
最後に C:\Users\[ユーザー名]\AppData\Roaming\npm に追加する。

[管理者権限のないWindowsでvue開発（サンプル付き）](https://qiita.com/nobu-maple/items/9b99cfd22bd811d74765)
WindowsのPathは面倒臭い・・・。

windowsのパスってどうやって設定するの？
→[Windows で環境変数 PATH をいじる方法のまとめ](https://qiita.com/sta/items/6d29da0dc7069ffaae60)  

なんで反映されないの？
→プロンプトやVSCodeの再起動が必要だった。

ユーザーの環境設定を設定するだけで反映されたりするの？
→反映されることが分かった。


`npm install -g @vue/cli`でグローバルインストールした場合、DotNetDev4ではC:\Users\s.ito\AppData\Roaming\npmにインストールされる。

[node_modulesの再インストール方法](https://zenn.dev/mo_ri_regen/articles/node-modules-article)  

[【Node.js】 Windows上で動作させるための設定知識](https://note.affi-sapo-sv.com/nodejs-windows-setting-knowledge.php)  
`インストーラーでNode.jsをインストールした場合、パッケージのグローバルインストール先として C:\Users\ユーザー名\AppData\Roaming\npm が使用されます。`

node_modules インストール
`npm ci`
i → インストール
c → ??。クリーン的な意味だった気がする。



package.jsonファイル
package.jsonファイルにはプロジェクトに関する情報が記述されているファイルです。Vue.jsのみで利用するファイルではなくnpmコマンドでライブラリを管理する際に必須となるファイルでインストールしたライブラリのバージョン情報等も記述されています。
新たにnpmコマンドでライブラリを追加した場合はここに追加されることになります。dependenciesとdevDependenciesに別れていますが、dependenciesに記述されているライブラリは本番環境でも利用するライブラリです。devDependenciesに記述されているライブラリは開発時に利用するライブラリで本番環境では利用されません。


App.vueの拡張子.vueはvue専用のファイルを表している拡張子で、中身は3つのパーツ(template, script, style)で構成されていることが確認できます。このファイルは単一ファイルコンポーネント(SFC : Single File Component)と呼ばれます。Vue.jsを初めて利用する人であれば拡張子はvueはみたことがないかもしれません。Vue.js専用の拡張子で拡張子vueはブラウザでは直接処理することができないのでJavaScriptファイルjsに変換が行われます。


templateタグについて
templateタグにはブラウザ上に表示させたい内容をHTMLで記述することが可能です。HTMLをそのままtemplateタグに記述できることがvue.jsの利点の一つです。



他のファイルからインポートできるように、（1）のMyFirstTSクラスにexportキーワードを付与している点です。


## TypeScript導入の仕方

## 変更の即時反映の仕方

## フォルダ構成
