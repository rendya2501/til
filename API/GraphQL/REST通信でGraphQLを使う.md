# REST通信でGraphQLを使う

GraphQLの検証は、各ライブラリのplaygroundでしかできないと思っていたけど、Talend API Testerでも出来たのでまとめ。  

---

graphqlの定義  

``` graphql
extend type Query {
    # Adminのログイン
    adminLogin(
        user_name: String! @rules(apply: ["required"])
        password: String! @rules(apply: ["required"])
    ): AccessToken
}

# ログイン用のトークンを発行
type AccessToken {
    accessToken: String
}
```

playgroundでのquery  

``` graphql : query
query($user_name: String!, $password: String!) {
    adminLogin(user_name: $user_name, password: $password) {
        accessToken
    }
}
```

playgroundでのvariables  

``` json : variables
{
    "user_name": "sample_username",
    "password": "sample_password"
}
```

playgroundでのresponse  

``` json : response
{
    "data": {
        "adminLogin": {
            "accessToken": "very long jwt token"
        }
    }
}
```

---

API Testerでの定義  

- method : `POST`  
- url `http://xxx.xxx.xxx.xxx:yyyy/graphql`  
- header  
  - Content-Type : `application/json`  

body

``` json : body
{
    "query":
    "{ adminLogin(user_name: \"sample_username\", password: \"sample_password\") { accessToken } }"
}
```

response

``` json : response
{
    "data": {
        "adminLogin": {
            "accessToken": "very long jwt token"
        }
    }
}
```
