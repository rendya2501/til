# PowerShell

Windows用の新しいコマンドライン環境・スクリプト言語。  
「CommandPromptの新しい的なやつ」と思えばよい。  

.NetFrameworkが基盤になってる。  
オブジェクト指向をサポートしてて、Win32APIも使えるので結構なんでもできる。  
頑張ればWindowsFormのGUIアプリも組める。  

実質的にWindowsのほぼすべての機能を操作できる強力なCUIだが、何でもできるのはさすがにやばいので実行ポリシーとかいうセキュリティ機能で実行を制御されている。  
なので「ダブルクリックですぐにスクリプトを実行」とかそういうことができない。  

何でもできるすごいやつだけど面倒なやつがPowerShell。  

[PowerShellをもっとメジャーな存在にしたい](https://www.cresco.co.jp/blog/entry/14650/)  

---

## カレントディレクトリの設定方法

### 1. Set-Locationコマンドレット

Set-Locationコマンドレットを使用して、カレントディレクトリを変更できる。  
以下の例では、C:\Users\username\Documentsにカレントディレクトリを変更する。  

``` ps1
Set-Location -Path "C:\Users\username\Documents"
```

### 2. cdコマンド（エイリアス）

cdコマンド（Set-Locationのエイリアス）を使用して、カレントディレクトリを変更できる。  
以下の例では、C:\Users\username\Documentsにカレントディレクトリを変更する。  

``` ps1
cd "C:\Users\username\Documents"
```

---

## スクリプトのカレントディレクトリを設定する

スクリプトのカレントディレクトリを、スクリプトが存在するディレクトリに設定するには、以下のコードをスクリプトの先頭に追加する。  

``` ps1
$scriptPath = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
Set-Location -Path $scriptPath
```

これにより、スクリプトが実行されると、カレントディレクトリがスクリプトが存在するディレクトリに設定される。  

---

## 現在のバージョンを確認する

パワーシェルウィンドウで以下のコマンドを実行する。  

`$PSVersionTable.PSVersion`  

以下のようにメジャーバージョン、マイナーバージョン、ビルド番号、リビジョンなどの詳細が表示される。  

``` txt
Major  Minor  Build  Revision
-----  -----  -----  --------
7      2      0      0
```

---

## パワーシェルのインストール

1. サイトへ飛ぶ [Windows への PowerShell のインストール - PowerShell | Microsoft Learn](https://learn.microsoft.com/ja-jp/powershell/scripting/install/installing-powershell-on-windows?view=powershell-7.3#msi)  

2. **MSI パッケージのインストール**から「.msi」ファイルをダウンロードし、インストールする。  

3. インストールが完了したら、Windows ターミナルやコマンドプロンプトで `pwsh` と入力し、PowerShell 7 を起動する。  

---

[自称IT企業があまりにITを使わずに嫌になって野に下った俺が紹介するWindowsの自動化の方法 - Qiita](https://qiita.com/mima_ita/items/453bb6c313e459c44689)  

[ASCII.jp：PowerShellのコマンドの並びにある典型的なパターン (1/2)](https://ascii.jp/elem/000/004/126/4126349/)  
