@echo off
setlocal

rem 元のファイル名
set file=A1.sql

rem 変換後のファイル名
set newfile=A1_conv.sql

rem 文字コード変換
cmd /c "chcp 65001 | type %file% | iconv -f utf-8 -t shift-jis > %newfile%"

echo 変換が完了しました。
