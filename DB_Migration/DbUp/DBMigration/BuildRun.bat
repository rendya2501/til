@echo off
chcp 65001

cd .\src
dotnet build --no-incremental

cd .\bin\Debug\net6.0\win-x64
call cmd /k "Bundle.exe"
