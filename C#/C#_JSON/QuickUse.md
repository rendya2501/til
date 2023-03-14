# サクッとカレントディレクトリのappsetting.jsonを読み取る

``` json
{
  "MySetting": "Hello, world!"
}
```

``` cs
using System;
using System.IO;
using Microsoft.Extensions.Configuration;


var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();

var mySetting = configuration["MySetting"];
Console.WriteLine($"MySetting value is {mySetting}");
// MySetting value is Hello, world!
```
