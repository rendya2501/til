# C#におけるHTTPクライアント

## 種類と解説

C#で利用できる主なHTTPクライアントクラスは以下の3つ。

1. **WebClient**
    - .NET Framework 2.0から利用可能
    - 基本的なHTTPリクエストとレスポンスを行う機能を提供
    - 同期および非同期メソッドが利用可能
    - 現在は推奨されておらず、.NET Coreおよび.NET 5以降では利用できない

2. **HttpWebRequest**
    - .NET Framework 1.1から利用可能
    - より低レベルで柔軟性が高く、HTTPリクエストのカスタマイズが可能
    - 現在は推奨されておらず、.NET Coreおよび.NET 5以降では限定的な互換性が提供されている

3. **HttpClient**
    - .NET Framework 4.5から利用可能
    - 現在推奨されている方法で、簡潔なAPIと高いパフォーマンスを提供
    - 非同期プログラミングをサポート
    - .NET Coreおよび.NET 5以降でも利用可能で、将来のプラットフォームでもサポートされることが予想される

---

## サードパーティ製HTTPクライアントライブラリ

特定の目的や機能が必要な場合、以下のようなサードパーティ製のHTTPクライアントライブラリを検討することもできます。

- RestSharp
- Flurl.Http

ただし、新しいプロジェクトでHTTP通信を行う場合は、推奨される**HttpClient**を使用することが望ましいです。

---

## HttpClientの基本的な実装

``` cs
using System.Text;

await GetRequest();
await PostRequest();
await PutRequest();
await DeleteRequest();


static async Task GetRequest()
{
    using HttpClient httpClient = new HttpClient();
    string getUrl = "https://api.example.com/data";

    try
    {
        HttpResponseMessage response = await httpClient.GetAsync(getUrl);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"GET Response: {responseBody}");
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"GET Error: {e.Message}");
    }
}

static async Task PostRequest()
{
    using HttpClient httpClient = new HttpClient();
    string postUrl = "https://api.example.com/data";

    try
    {
        HttpContent content = new StringContent("{\"key\":\"value\"}", Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PostAsync(postUrl, content);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"POST Response: {responseBody}");
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"POST Error: {e.Message}");
    }
}

static async Task PutRequest()
{
    using HttpClient httpClient = new HttpClient();
    string putUrl = "https://api.example.com/data/1";

    try
    {
        HttpContent content = new StringContent("{\"key\":\"updatedValue\"}", Encoding.UTF8, "application/json");
        HttpResponseMessage response = await httpClient.PutAsync(putUrl, content);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"PUT Response: {responseBody}");
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"PUT Error: {e.Message}");
    }
}

static async Task DeleteRequest()
{
    using HttpClient httpClient = new HttpClient();
    string deleteUrl = "https://api.example.com/data/1";

    try
    {
        HttpResponseMessage response = await httpClient.DeleteAsync(deleteUrl);
        response.EnsureSuccessStatusCode();
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"DELETE Response: {responseBody}");
    }
    catch (HttpRequestException e)
    {
        Console.WriteLine($"DELETE Error: {e.Message}");
    }
}
```

---

[C# で HTTP POSTWeb リクエストを作成する | Delft スタック](https://www.delftstack.com/ja/howto/csharp/csharp-post-request/)  
