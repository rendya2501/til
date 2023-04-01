@echo off
chcp 65001

cd .\src
rmdir /s /q bin
rmdir /s /q obj
dotnet build --no-incremental

powershell.exe -ExecutionPolicy Bypass -File ".\CreateBundle.ps1"

echo "press any keys"
pause > nul