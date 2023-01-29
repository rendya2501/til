:: y押したら押した時刻を出力するサンプル
@echo off

REM "今日の日付"
SET NOWDATE=%date:~-10,4%%date:~-5,2%%date:~-2,2%
SET OUT_LOGFILE=Logfile_%NOWDATE%.txt

cd /d %~dp0 >> %OUT_LOGFILE%

set /P USR_INPUT_STR="データ移行を行いますがよろしいですか？(Y/N) " 
if /i %USR_INPUT_STR% neq y goto cancel

echo *---------------------------------------* >> %OUT_LOGFILE%
set time_tmp=%time: =0%
set now=%date:/=%%time_tmp:~0,2%%time_tmp:~3,2%%time_tmp:~6,2%
echo 開始 : %now% >> %OUT_LOGFILE%
echo *---------------------------------------* >> %OUT_LOGFILE%

:cancel
echo.
echo.
echo 処理を中止しました。
::goto :end

::pause