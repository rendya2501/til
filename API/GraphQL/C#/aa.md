# C#のGraphQL

バックエンドはPHP,Laravel。
フロントはC#,MAUIの場合にC#でGraphQLを定義する場合どうすれば良いのかまとめ。

## Laravel側のGraphQLの定義

``` graphql
extend type Query {
    # Memberのログイン
    memberLogin(
        email: String! @rules(apply: ["required"])
        password: String! @rules(apply: ["required"])
    ): AccessToken
}

# ログイン用のトークンを発行
type AccessToken {
    accessToken: String
}
```

## PlayGroundでのレスポンスの形

``` txt
{
  "data": {
    "memberLogin": {
      "accessToken": "jwt token strings"
    }
  }
}
```

## C#で型安全な方法でレスポンスを受け取る方法

``` cs
public async Task<string> LoginAsync(string userName, string passWord)
{
    var request = new GraphQLRequest
    {
        Query = @"
          query($email: String!, $password: String!) {
            memberLogin(email: $email, password: $password) {
              accessToken
            }
          }",
        Variables = new
        {
            email = userName,
            password = passWord
        }
    };

    var response = await _client.SendQueryAsync<MemberLoginResponse>(request);
    var res = response.Data.MemberLogin.AccessToken;
    return res;
}

public class MemberLoginResponse
{
    public MemberLogin MemberLogin { get; set; }
}

public class MemberLogin
{
    public string AccessToken { get; set; }
}
```

値を受け取るために2つもクラスを定義しないといけないのは静的型付けのつらいところ。  

## dynamicでも可能

この程度ならdynamicの方が楽。

``` cs
var response = await _client.SendQueryAsync<dynamic>(request);
var res = response.Data.memberLogin.accessToken as string;
```
