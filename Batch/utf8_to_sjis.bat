@echo off
setlocal EnableDelayedExpansion

rem カレントディレクトリ内のすべての .sql ファイルを変換するバッチ

for /f "delims=" %%i in ('dir /b *.sql') do (
    rem 元のファイル名
    set file=%%i

    rem 変換後のファイル名
    set newfile=%%~ni_conv%%~xi

    rem 文字コード変換
    cmd /c "chcp 65001 | type !file! | iconv -f utf-8 -t shift-jis > !newfile!"
    
    echo !file!を!newfile!に変換しました。
)

echo 全ての変換が完了しました。
