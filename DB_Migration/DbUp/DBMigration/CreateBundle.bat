@echo off
chcp 65001

powershell.exe -ExecutionPolicy Bypass -File ".\src\CreateBundle.ps1"

pause > nul