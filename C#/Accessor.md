# Accessor

---

## 最低限のアクセサーの実装

アクセサーは実質、設定ファイルの読み込みとHTTP通信の実行でしかない。  

``` C# : API起点
/// <summary>
/// コース一覧を取得します。
/// </summary>
/// <param name="request"></param>
/// <returns></returns>
public async Task<IEnumerable<CourseParam>> GetCourseList(CourseListGetRequest request)
{
    return await _Accessor.GetResultAsync<IEnumerable<CourseParam>>(PATH + "/course/get/list", request);
}
```

``` C# : Accessor
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RN3.Wpf.Common.Auth;
using RN3.Wpf.Common.Resource;
using RN3.Wpf.Common.Util.Config;
using RN3.Wpf.Common.Util.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static RN3.Wpf.Common.Resource.ConstResource;

namespace RN3.Wpf.Common.Accessor
{
    /// <summary>
    /// HttpAlientを用いたAPI実行クラス
    /// </summary>
    public class HTTPAccessor
    {
        #region メンバ
        /// <summary>
        /// 内包するHttpClient
        /// </summary>
        private static HttpClient _Client = new HttpClient()
        {
            //暫定でタイムアウトを5分にしています。
            Timeout = TimeSpan.FromMinutes(5)
        };
        #endregion

        #region コンストラクタ
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HTTPAccessor()
        {
            var config = ConfigUtil.GetConfigOrDefault().SelectionConnection;
            if (config == null)
            {
                throw new Exception(string.Format(Message.Invalid, "アプリケーション設定"));
            }
        }
        #endregion

        #region 外部公開メソッド
        /// <summary>
        /// 同期通信でAPI実行。認証方式はJWTトークン認証。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        /// <param name="callerFilePath"></param>
        /// <returns></returns>
        public T GetResult<T>(string path, object param = null, [CallerFilePath] string callerFilePath = "")
        {
            //送信情報生成
            var request = CreateRequestMessage(path, SerializeParam(param), GetCallerDirectoryName(callerFilePath));
            request.Headers.Add("Authorization", "Bearer " + AuthenticationInfo.Token);
            //Http送信
            var result = ApiExecute(request);
            if (string.IsNullOrEmpty(result))
            {
                return default;
            }
            return DeserializeResult<T>(result);
        }

        /// <summary>
        /// 非同期通信でAPI実行。認証方式はJWTトークン認証。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        /// <param name="callerFilePath"></param>
        /// <returns></returns>
        /// <remarks>
        /// RequestMessageを生成してSendAsyncで実行するパターン
        /// </remarks>
        public async Task<T> GetResultAsync<T>(string path, object param = null, [CallerFilePath] string callerFilePath = "")
        {
            //送信情報生成
            var request = CreateRequestMessage(path, SerializeParam(param), GetCallerDirectoryName(callerFilePath));
            request.Headers.Add("Authorization", "Bearer " + AuthenticationInfo.Token);
            //非同期API実行
            var result = await ApiExecuteAsync(request);
            if (string.IsNullOrEmpty(result))
            {
                return default;
            }
            return DeserializeResult<T>(result);
        }

        /// <summary>
        /// 同期通信でAPI実行。認証方式はApiKey。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        /// <param name="callerFilePath"></param>
        /// <returns></returns>
        public string GetResultByApiKey(string path, object param = null, [CallerFilePath] string callerFilePath = "")
        {
            var config = ConfigUtil.GetConfigOrDefault().SelectionConnection;
            //送信情報生成
            var request = CreateRequestMessage(path, SerializeParam(param), GetCallerDirectoryName(callerFilePath));
            request.Headers.Add("X-Api-Key", config.X_Api_Key);
            //Http送信
            return ApiExecute(request);
        }

        /// <summary>
        /// 同期通信でAPI実行。認証方式はApiKey。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        /// <param name="callerFilePath"></param>
        /// <returns></returns>
        public T GetResultByApiKey<T>(string path, object param = null, [CallerFilePath] string callerFilePath = "")
        {
            var config = ConfigUtil.GetConfigOrDefault().SelectionConnection;
            //送信情報生成
            var request = CreateRequestMessage(path, SerializeParam(param), GetCallerDirectoryName(callerFilePath));
            request.Headers.Add("X-Api-Key", config.X_Api_Key);
            //Http送信
            var result = ApiExecute(request);
            if (string.IsNullOrEmpty(result))
            {
                return default;
            }
            return DeserializeResult<T>(result);
        }

        /// <summary>
        /// 非同期通信でAPI実行。認証方式はApiKey。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        /// <param name="callerFilePath"></param>
        /// <returns></returns>
        /// <remarks>
        /// RequestMessageを生成してSendAsyncで実行するパターン
        /// </remarks>
        public async Task<string> GetResultByApiKeyAsync(string path, object param = null, [CallerFilePath] string callerFilePath = "")
        {
            var config = ConfigUtil.GetConfigOrDefault().SelectionConnection;
            //送信情報生成
            var request = CreateRequestMessage(path, SerializeParam(param), GetCallerDirectoryName(callerFilePath));
            request.Headers.Add("X-Api-Key", config.X_Api_Key);
            //非同期API実行
            return await ApiExecuteAsync(request);
        }

        /// <summary>
        /// 非同期通信でAPI実行。認証方式はApiKey。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        /// <param name="callerFilePath"></param>
        /// <returns></returns>
        /// <remarks>
        /// RequestMessageを生成してSendAsyncで実行するパターン
        /// </remarks>
        public async Task<T> GetResultByApiKeyAsync<T>(string path, object param = null, [CallerFilePath] string callerFilePath = "")
        {
            var config = ConfigUtil.GetConfigOrDefault().SelectionConnection;
            //送信情報生成
            var request = CreateRequestMessage(path, SerializeParam(param), GetCallerDirectoryName(callerFilePath));
            request.Headers.Add("X-Api-Key", config.X_Api_Key);
            //非同期API実行
            var result = await ApiExecuteAsync(request);
            if (string.IsNullOrEmpty(result))
            {
                return default;
            }
            return DeserializeResult<T>(result);
        }
        #endregion

        #region 内部処理メソッド
        /// <summary>
        /// ObservableCollection用リゾルバ
        /// </summary>
        private class JsonObservableCollectionConverter : DefaultContractResolver
        {
            public override JsonContract ResolveContract(Type type)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return ResolveContract(typeof(ObservableCollection<>).MakeGenericType(type.GetGenericArguments()));
                }
                return base.ResolveContract(type);
            }
        }
        /// <summary>
        /// 戻り値をデシリアライズします。
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private T DeserializeResult<T>(string result)
        {
            // ※Viewで直接一覧にバインドする場合、ListだとメモリリークするためここでObservableCollectionに変換することで対応しています。
            // 　本来はViewModelで行うべきですが、対応箇所が多く漏れの防止のためと、
            // 　入れ子になっているプロパティがIEnumerableだった場合もサポートできることから、ここで処理します。
            return JsonConvert.DeserializeObject<T>(
                result,
                new JsonSerializerSettings()
                {
                    ContractResolver = new JsonObservableCollectionConverter(),
                }
            );
        }
        /// <summary>
        /// 呼び出し元ファイルパスからプロジェクトディレクトリ名を取得します
        /// </summary>
        /// <param name="callerFilePath"></param>
        /// <returns></returns>
        private string GetCallerDirectoryName(string callerFilePath)
        {
            var splitPath = callerFilePath.Split(Path.DirectorySeparatorChar);
            var rootIndex = Array.IndexOf(splitPath, AppInfo.SolutionName);
            if (rootIndex >= 0 && splitPath.Length >= rootIndex)
            {
                // ソリューション名と同じディレクトリ名の次をプロジェクトディレクトリ名と判断します
                return splitPath[rootIndex + 1];
            }
            return null;
        }

        private string GetCallingAssemblyName(string callerDirectoryName)
        {
            //Commonのアセンブリ名を取得
            string commonAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            //実行元のアセンブリ名を取得
            string entryAssemblyName = Assembly.GetEntryAssembly().GetName().Name;

            if (commonAssemblyName != callerDirectoryName)
            {
                //呼び出し元がCommonではない場合、直近のアセンブリを優先
                return callerDirectoryName;
            }
            //呼び出し元もCommonと同じ場合は、一番根元のアセンブリを使用
            return entryAssemblyName;
        }

        /// <summary>
        /// 共通処理、送信情報生成
        /// </summary>
        /// <param name="path"></param>
        /// <param name="param"></param>
        /// <param name="callerDirectoryName"></param>
        /// <returns></returns>
        private HttpRequestMessage CreateRequestMessage(string path, string param, string callerDirectoryName)
        {
            var config = ConfigUtil.GetConfigOrDefault().SelectionConnection;
            var request = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                RequestUri = new UriBuilder(config.Host + config.Domain + path).Uri,
                Content = new StringContent(param ?? string.Empty, Encoding.UTF8, @"application/json")
            };
            request.Headers.Add("StaffCD", AuthenticationInfo.StaffCode);
            if (!string.IsNullOrEmpty(AuthenticationInfo.StaffName))
            {
                request.Headers.Add("StaffName", string.Join(",", Encoding.UTF8.GetBytes(AuthenticationInfo.StaffName)));
            }
            request.Headers.Add("Program", GetCallingAssemblyName(callerDirectoryName)?.Left(200));
            request.Headers.Add("Terminal", Environment.MachineName);

            return request;
        }

        /// <summary>
        /// 同期API実行
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string ApiExecute(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                //SendAsyncでHttp通信を送り応答を待つ。
                response = _Client.SendAsync(request).GetAwaiter().GetResult();
            }
            catch (TaskCanceledException e)
            {
                throw new Exception("接続がタイムアウトしました", e);
            }
            //ボディの内容を取得する。
            var content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            //ステータスコードのチェック
            IsSuccessStatusCode(content, response);
            //JsonのBodyを返す
            return content;
        }

        /// <summary>
        /// 非同期API実行
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<string> ApiExecuteAsync(HttpRequestMessage request)
        {
            HttpResponseMessage response = null;
            try
            {
                //SendAsyncでHttp通信を送り応答を待つ。
                response = await _Client.SendAsync(request);
            }
            catch (TaskCanceledException e)
            {
                throw new Exception("接続がタイムアウトしました", e);
            }
            //ボディの内容を取得する。
            var content = await response.Content.ReadAsStringAsync();
            //ステータスコードのチェック
            IsSuccessStatusCode(content, response);
            //JsonのBodyを返す
            return content;
        }

        /// <summary>
        /// ステータスコードのチェック。
        /// 200以外の場合例外を発生させる。
        /// </summary>
        /// <param name="content"></param>
        /// <param name="response"></param>
        private void IsSuccessStatusCode(string content, HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                CreateHttpReqestException(content, response);
            }
        }

        /// <summary>
        /// 共通HttpRequestException生成処理
        /// </summary>
        /// <param name="content"></param>
        /// <param name="response"></param>
        private void CreateHttpReqestException(string content, HttpResponseMessage response)
        {
            if (string.IsNullOrEmpty(content))
            {
                throw new HttpRequestException(
                    string.Format(
                        Message.HttpError,
                        (int)response.StatusCode, response.ReasonPhrase
                    )
                );
            }
            else
            {
                throw new HttpRequestException(
                    content,
                    new HttpRequestException(
                        string.Format(
                            Message.HttpError,
                            (int)response.StatusCode, response.ReasonPhrase
                        )
                    )
                );
            }
        }

        /// <summary>
        /// HTTPリクエストパラメータをJSON文字列にシリアライズします
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private string SerializeParam(object param)
        {
            if (param is string)
            {
                return param as string;
            }
            else
            {
                return param != null ? JsonConvert.SerializeObject(param) : null;
            }
        }
        #endregion
    }
}
```

``` C# : ConfigUtil
using Microsoft.Extensions.Configuration;
using RN3.Wpf.Common.Data.AppSetting;
using RN3.Wpf.Common.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace RN3.Wpf.Common.Util.Config
{
    /// <summary>
    /// コンフィグユーティリティ
    /// </summary>
    public static class ConfigUtil
    {
        private static Configurations _ConfigInstance = null;

        /// <summary>
        /// 接続中のサーバ名
        /// </summary>
        public static string Connection_ServerName
        {
            get => Settings.Default.Connection_ServerName;
        }

        /// <summary>
        /// コンフィグを取得します。
        /// </summary>
        public static Configurations GetConfigOrDefault()
        {
            if(_ConfigInstance != null)
            {
                return _ConfigInstance;
            }
            //アプリケーション用フォルダパス取得
            var configurationDirectory = GetOrCreateAppPath();
            var baseAppsettingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
            var appsettingsFilePath = Path.Combine(configurationDirectory, "appsettings.json");
            if (File.Exists(baseAppsettingsFilePath) && !File.Exists(appsettingsFilePath))
            {
                // デフォルトとなるappsetting.jsonが存在し、まだ設定ファイルが存在しなければ複写する
                File.Copy(baseAppsettingsFilePath, appsettingsFilePath);
            }
            var configuration = new ConfigurationBuilder()
                .SetBasePath(configurationDirectory)
                .AddJsonFile("appsettings.json", false, true).Build();
            var configurations = new Configurations
            {
                Connections = configuration.GetSection("Connections")?.Get<IEnumerable<Connection>>() ?? new List<Connection>(),
                ConnectionServerName = Settings.Default.Connection_ServerName
            };
            if (configurations.Connections.Count() <= 0)
            {
                throw new System.Exception(string.Format(Resource.Message.Invalid, "アプリケーション設定"));
            }
            if (string.IsNullOrEmpty(Settings.Default.Connection_ServerName))
            {
                configurations.ConnectionServerName = configurations.Connections.First().ServerName;
                Settings.Default.Connection_ServerName = configurations.ConnectionServerName;
                Settings.Default.Save();
            }

            _ConfigInstance = configurations;
            return configurations;
        }

        /// <summary>
        /// コンフィグを保存します。
        /// </summary>
        /// <param name="configurations"></param>
        public static void SaveConfig(Configurations configurations)
        {
            //選択した接続先を保存
            Settings.Default.Connection_ServerName = configurations.ConnectionServerName;
            Settings.Default.Save();
        }

        /// <summary>
        /// アプリケーション用フォルダパスを取得します。存在しなければ作成します。
        /// </summary>
        /// <returns></returns>
        private static string GetOrCreateAppPath()
        {
            //ユーザのAppData\Localフォルダのパスを取得。
            string localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData, Environment.SpecialFolderOption.Create);
            //プロセス名を取得
            string processName = Process.GetCurrentProcess().ProcessName;
            //アプリケーション用フォルダパス作成
            string appPath = localAppDataPath + "\\" + processName;
            //アプリケーション用フォルダパスのフォルダが存在しなければ、フォルダを作成
            if (!Directory.Exists(appPath))
            {
                Directory.CreateDirectory(appPath);
            }

            return appPath;
        }
    }
}
```

``` C# : Configurations
using System.Collections.Generic;
using System.Linq;

namespace RN3.Wpf.Common.Data.AppSetting
{
    /// <summary>
    /// アプリケーション設定
    /// </summary>
    public class Configurations
    {
        /// <summary>
        /// 接続中のサーバ名
        /// </summary>
        public string ConnectionServerName { get; set; }
        /// <summary>
        /// 接続先一覧
        /// </summary>
        public IEnumerable<Connection> Connections { get; internal set; }
        /// <summary>
        /// Connectionsの選択中の要素
        /// </summary>
        public Connection SelectionConnection
        {
            get
            {
                return Connections.FirstOrDefault(f => f.ServerName.Equals(ConnectionServerName)) ??
                    Connections.First();
            }
        }
    }
}
```

---
