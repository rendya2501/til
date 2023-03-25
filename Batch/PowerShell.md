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

## PowerShellで並列処理を実行する方法

### 1. Start-Job cmdlet

バックグラウンドで非同期にジョブを実行する方法となる。  
ジョブが完了するのを待ち、その結果を受信するためにWait-JobとReceive-Jobコマンドレットを使用する。  

``` ps1
# Create script blocks for tasks
$task1 = { ... }
$task2 = { ... }

# Start the tasks as background jobs
$job1 = Start-Job -ScriptBlock $task1
$job2 = Start-Job -ScriptBlock $task2

# Wait for all jobs to complete
Wait-Job -Job $job1, $job2

# Display the output from the jobs
Receive-Job -Job $job1
Receive-Job -Job $job2

# Clean up
Remove-Job -Job $job1, $job2
```

win用とlinux用でpublishするならこんな感じになる。  

``` ps1
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# Change the working directory to the script's directory
$scriptPath = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
Set-Location -Path $scriptPath

# Create script blocks for win-x64 and linux-x64 tasks
$win_x64_task = {
    Set-Location -Path $using:scriptPath
    Write-Output "--- win-x64_Start ---"
    dotnet publish -o ../Bundle/win_bundle -c Release --self-contained true -r win-x64 -p:IncludeNativeLibrariesForSelfExtract=true
    Write-Output "--- win-x64_Finished ---"
}

$linux_x64_task = {
    Set-Location -Path $using:scriptPath
    Write-Output "--- linux-x64_Start ---"
    dotnet publish -o ../Bundle/linux_bundle -c Release --self-contained true -r linux-x64
    Write-Output "--- linux-x64_Finished ---"
}

# Start the tasks as background jobs
$winJob = Start-Job -ScriptBlock $win_x64_task
$linuxJob = Start-Job -ScriptBlock $linux_x64_task

# Wait for all jobs to complete
Wait-Job -Job $winJob, $linuxJob

# Display the output from the jobs
Receive-Job -Job $winJob
Receive-Job -Job $linuxJob

# Clean up
Remove-Job -Job $winJob, $linuxJob

Write-Output "--- AllFinished ---"
```

### 2. ForEach-Object cmdlet (with -Parallel switch)

PowerShell 7以降では、ForEach-Object cmdletに-Parallelスイッチが導入された。  
これを使って並列処理を行うことができる。  

``` ps1
$tasks = @(
    @{
        Name = 'Task1'
        ...
    },
    @{
        Name = 'Task2'
        ...
    }
)

$tasks | ForEach-Object -Parallel {
    # Task script here
} -ThrottleLimit 2
```

win用とlinux用でpublishするならこんな感じになるが動かない。  

``` ps1
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

# Change the working directory to the script's directory
$scriptPath = Split-Path -Parent -Path $MyInvocation.MyCommand.Definition
Set-Location -Path $scriptPath

$tasks = @(
    @{
        Name = 'win-x64'
        PublishArgs = '-o ../Bundle/win_bundle -c Release --self-contained true -r win-x64 -p:IncludeNativeLibrariesForSelfExtract=true'
    },
    @{
        Name = 'linux-x64'
        PublishArgs = '-o ../Bundle/linux_bundle -c Release --self-contained true -r linux-x64'
    }
)

$tasks | ForEach-Object -Parallel {
    param($task)
    Write-Output "--- $($task.Name)_Start ---"
    dotnet publish $($task.PublishArgs)
    Write-Output "--- $($task.Name)_Finished ---"
} -ThrottleLimit 2 -ArgumentList {$_}

Write-Output "--- AllFinished ---"
```

### 3. Invoke-Parallel cmdlet (from third-party module)

RamblingCookieMonsterのPowerShellコミュニティによって開発されたサードパーティのモジュールで、Invoke-Parallelコマンドレットを提供している。  
この方法では、PowerShell Galleryからモジュールをインストールして使用する必要がある。  

インストールの段階から面倒くさいしお勧めしない。  

``` ps1
Install-Module -Name PoshRSJob

$tasks = @(
    @{
        Name = 'Task1'
        ...
    },
    @{
        Name = 'Task2'
        ...
    }
)

$tasks | Invoke-Parallel -ScriptBlock {
    # Task script here
}
```

1. 管理者権限でPowerShellを実行してモジュールをインストールする。  
2. カレントユーザーのスコープにモジュールをインストールする。  

以下の手順でPoshRSJobモジュールをインストールできる。

オプション1: 管理者権限でPowerShellを実行  

1. 右クリックでPowerShellを起動し、「管理者として実行」を選択する。  
2. 次のコマンドを実行してPoshRSJobモジュールをインストールする。  

``` bat
Install-Module -Name PoshRSJob
```

オプション2: カレントユーザーのスコープにモジュールをインストール  

1. 通常のPowerShellセッションで次のコマンドを実行し、カレントユーザーのスコープにPoshRSJobモジュールをインストールする。  

``` txt
Install-Module -Name PoshRSJob -Scope CurrentUser
```

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
