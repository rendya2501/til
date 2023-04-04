@echo off
chcp 65001

SET RUNPATH=%~dp0
CD %RUNPATH%

cd .\src
rmdir /s /q bin
rmdir /s /q obj
dotnet build --no-incremental

cd .\bin\Debug\net6.0\win-x64
call cmd /k "Bundle.exe"
