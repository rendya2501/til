# カレントディレクトリのA1.sqlの文字コードをUTF8からsjisに変換するパワーシェル

#get-content -Encoding UTF8 .\A1.sql | Set-Content .\A1_s.sql  

# カレントディレクトリのすべてのsqlファイルの文字コードをUTF8からsjisに変換するパワーシェル
# Get-ChildItem -Filter *.sql | ForEach-Object {
#     $utf8File = $_.FullName
#     $sjisFile = $_.FullName -replace ".sql$", "_sjis.sql"
#     Get-Content -Encoding UTF8 $utf8File | Set-Content $sjisFile
# }

# カレントディレクトリのすべてのsqlファイルの文字コードをUTF8からsjisに変換しつつ上書きするパワーシェル
Get-ChildItem *.sql | ForEach-Object {
    $sqlFile = $_.FullName
    $content = Get-Content -Path $sqlFile -Encoding UTF8
    Set-Content -Path $sqlFile -Value $content
}

# パワーシェルを実行するために必要なコマンド
# Set-ExecutionPolicy RemoteSigned -Scope Process