@echo off
chcp 65001

SET RUNPATH=%~dp0
CD %RUNPATH%

cd .\src
rmdir /s /q bin
rmdir /s /q obj
dotnet build --no-incremental

echo.
echo "並列実行開始"

powershell.exe -ExecutionPolicy Bypass -File ".\CreateBundle.ps1"

echo "処理完了"
echo "press any keys"
pause > nul