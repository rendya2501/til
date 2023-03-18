# ソリューション関係

## ソリューションの作成

`dotnet new sln`  
デフォルトではフォルダ名がソリューション名となる。  

例 : [\<SolutionName>]でフォルダを作りつつソリューションを作成する  
`dotnet new -o <SolutionName>`  

---

## ソリューションにプロジェクトを追加する

`dotnet sln add .\projectPath\<project_name>.csproj`  
