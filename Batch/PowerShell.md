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

## 実行ポリシー

手っ取り早く使うために下記コマンドをPowerShellで実行する。  
`Set-ExecutionPolicy RemoteSigned -Scope CurrentUser`  

現在の実行ポリシーを確認するコマンド  
`Get-ExecutionPolicy`  

結果が`Restricted`(スクリプトの実行を行いません)の場合はパワーシェルスクリプトは実行不可。  

以下解説  
概要でも説明したが、PowerShellは何でもできてしまうのでセキュリティ的な観点から実行が制限されている。  
そのセキュリティ的なものが実行ポリシーというステータスで、パワーシェルのスクリプト実行するためには、そのポリシーを適切に設定する必要がある。  

[Windowsのポチポチ業務を爆速化するPowerShell、スクリプトを実行するための準備](https://tonari-it.com/windows-powershell-script-execution/)  

---

[自称IT企業があまりにITを使わずに嫌になって野に下った俺が紹介するWindowsの自動化の方法 - Qiita](https://qiita.com/mima_ita/items/453bb6c313e459c44689)  
