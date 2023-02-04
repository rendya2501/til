@echo off
rem 自己完結型でバンドルを発行する

@echo --- win-x64_Start ---
dotnet publish -o Bundle/win -c Release --self-contained true -r win-x64 -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true
@echo --- win-x64_Finished ---

@echo ;
@echo --- linux-x64_Start ---
dotnet publish -o Bundle/linux -c Release --self-contained true -r linux-x64 -p:PublishSingleFile=true
@echo --- linux-x64_Finished ---

@echo ;
@echo --- AllFinished ---
pause > nul