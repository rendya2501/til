# コンポーネント

---

## 確認ダイアログ

ダイアログ定義

``` html
@if (ShowConfirmation)
{
    // オーバーレイ
    <div class="modal-backdrop show"></div>
    // モーダルダイアログ本体
    <div class="modal fade show" style="display:block" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">@Title</h5>
                    <button type="button" @onclick="(() => OnDecide(false))"  class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <p>@Content</p>
                </div>
                <div class="modal-footer">
                    <button type="button" @onclick="(() => OnDecide(false))" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
                    <button type="button" @onclick="(() => OnDecide(true))" class="btn btn-danger">Delete</button>
                </div>
            </div>
        </div>
    </div>
}
```

``` cs
@code {
    [Parameter]
    public string Title { get; set; } = "削除確認";

    [Parameter]
    public string Content { get; set; } = string.Empty;

    protected bool ShowConfirmation { get; set; } = false;

    [Parameter]
    public EventCallback<bool> CallBack { get; set; }

    public void Show()
    {
        ShowConfirmation = true;
        StateHasChanged();
    }

    private Task OnDecide(bool result)
    {
        ShowConfirmation = false;
        return CallBack.InvokeAsync(result);
    }
}
```

使う場合

``` html
<button type="button" class="btn btn-danger" @onclick="(() => deleteConfirmation?.Show())">Delete Game</button>

<DeleteConfirmation @ref=deleteConfirmation Content="削除します。よろしいですか？" CallBack="DeleteExecute"></DeleteConfirmation>
```

``` cs
@code {
    // oter property

    private DeleteConfirmation? deleteConfirmation;
}
```

[Blazor で確認ダイアログを削除する](https://www.youtube.com/watch?v=Caw5hmq4dEY)  
[Blazor Tutorial C# - Part 5 - Blazor Component Reference](https://www.youtube.com/watch?v=3Gr83lIaENg)  

[bootstrap](https://getbootstrap.jp/docs/5.0/components/modal/)  

---

## ComboBox

とりあえず、純正のコンポーネントで作ってみたが、Blazorは良質なUIフレームワークがたくさんあるので、特にこだわりがなければそちらを使ったほうがいいかもね。  

``` html
@page "/combo_box"

<h3>ComboBox</h3>

<EditForm Model="@this.dummyModel">

    <InputSelect @bind-Value="_selectId" @oninput="OnItemInput">
        @foreach (var item in this.Values)
        {
            <option value="@item.id">@item.name</option>
        }
    </InputSelect>

    <h2>選択中ID :  @SelectedItem.id</h2>
    <h2>選択中Name :  @SelectedItem.name</h2>
</EditForm>
```

``` cs
@code {
    private DummyModel dummyModel = new();
    public class DummyModel { }

    protected List<(int id, string name)> Values { get; set; }
    protected (int id, string name) SelectedItem { get; set; }
    protected int _selectId;

    protected override async Task OnInitializedAsync()
    {
        this.Values = new List<(int id, string name)>();
        for (var i = 0; i < 10; i++)
        {
            this.Values.Add((id: i, name: $"test{i}"));
        }
        SelectedItem = this.Values.First();
        _selectId = SelectedItem.id;
    }

    private void OnItemInput(ChangeEventArgs e)
    {
        if (e.Value == null) return;

        var selected_id = Int32.Parse(e.Value.ToString());
        var person = Values.FirstOrDefault(x => x.id == selected_id);
        SelectedItem = (person.id, person.name);
    }
}
```

[radzen](https://blazor.radzen.com/)  
[Blazor の InputSelect コンポーネントについて学ぶ](https://www.gunshi.info/entry/2021/11/19/020708)  

- 検索文字列
  - InputSelect  
  - dropdown selection  
  - dropdownlist  

---

## aタグとボタン

`a`タグはButtonとしての役割を与える事もできるらしい。  
下記2つの書き方で同じ動作を実現できる。  

``` html
    <a href='/user/delete/@user.UserId' class="btn btn-outline-danger" role="button">
        Delete
    </a>
```

``` md
    ``` html
        <button class="btn btn-outline-danger " @onclick="(() => ShowDeletePage(user.UserId))">
            Delete
        </button>
    ```

    ``` cs
    @code {
        void ShowDeletePage(int userID)
        {
            NavigationManager.NavigateTo($"/user/delete/{userID}");
        }
    }
    ```
```

■**番外編**  

これでもボタンと同じ役割ができる。  
inputタグでもtypeの指定で色々変化指せる事ができる。  

``` html
<input type="submit" value="Cancel" class="btn btn-warning" />
```

---

## ボタンのonClickでNavigationする方法

`@inject NavigationManager NavigationManager;`  
`@onclick="@(() => NavigationManager.NavigateTo($"/hoge/huga/{@obj.prop}"))"`  

`$`必要。  
`{}`で囲む。
`@`必要。  
全ての要素が必要。  

``` html
    <!-- aタグでの実装 -->
    <a href='/user/edit/@user.UserId' class="btn btn-secondary" role="button"> Edit </a>

    <!-- buttonでの実装 直接 -->
    <button type="button" class="btn btn-secondary"
        @onclick="@(() => NavigationManager.NavigateTo($"/user/edit/{@user.UserId}"))"> Edit </button>

    <!-- buttonでの実装 間接 -->
    <button type="button" class="btn btn-secondary"
         @onclick="(() => ShowEditPage(user.UserId))"> Edit </button>
@code {
    void ShowEditPage(int userID)
    {
        NavigationManager.NavigateTo($"/user/edit/{userID}");
    }
}
```

>\<NavLink class="nav-link" href="@($"editproduct/{product.Id}")">  
[Navigating pages in Blazor](https://stackoverflow.com/questions/68347033/navigating-pages-in-blazor)  

---

## disableへのバインド

disableはboolとなるため、boolのプロパティとのバインドとboolを返却するラムダ式の記述が可能。

``` razor
<button type="button" disabled="@IsDisabled"></button>
<input @bind="IsDisabled" type="checkbox" />
<input disabled="@IsDisabled" type="time" />

@code{
    protected bool IsDisabled { get; set; }
}
```

[html - How to enable/disable inputs in blazor - Stack Overflow](https://stackoverflow.com/questions/55002514/how-to-enable-disable-inputs-in-blazor)  
[How are input components enabled/disabled in Blazor?](https://www.syncfusion.com/faq/blazor/forms-and-validation/how-are-input-components-enabled-disabled-in-blazor)  

---

## コンポーネントにラムダ式を記述する方法

`@`文字は、C# に切り替えるために使用される。  
C# の式の開始と終了を明示的に指定するには、かっこを使用する。  

以上を組み合わせると以下のようになる。  

``` html
<p role="status">Current count: @currentCount</p>

<button class="btn btn-primary" @onclick="IncrementCount">Increment</button>
<button class="btn btn-primary" disabled="@(currentCount <= 0)" @onclick="DecrementCount">Decrement</button>

@code {
    private int currentCount = 0;

    private void IncrementCount() => currentCount++;
    private void DecrementCount() => currentCount--;
}
```

こちらの場合でも動作はするが、警告が表示される。

``` html
<button class="btn btn-primary" disabled=@(currentCount <= 0) @onclick="DecrementCount">Decrement</button>
```

これらは認識されない。

``` html
<button class="btn btn-primary" disabled="(currentCount <= 0)" @onclick="DecrementCount">Decrement</button>
<button class="btn btn-primary" disabled="() => @currentCount <= 0" @onclick="DecrementCount">Decrement</button>
<button class="btn btn-primary" disabled="@(() => @currentCount <= 0)" @onclick="DecrementCount">Decrement</button>
```

[ASP.NET Core Blazor のイベント処理 | Microsoft Learn](https://learn.microsoft.com/ja-jp/aspnet/core/blazor/components/event-handling?view=aspnetcore-7.0#lambda-expressions)  
[【Blazor】EventCallbackで子コンポーネントからイベントを受け取る方法｜Blazorマスターへの道](https://blazor-master.com/blazor-event-callback/)  

---

## PropertyChanged

>Blazorは、ユーザーインタラクションが発生した場合（例：ボタンをクリックした、入力のテキストが変わった）、バインドされたプロパティの変更を自動的にチェックすることができます。  
>この場合、BlazorのJavaScriptコードがC#（=Webassembly）の変更検知をトリガーします。したがって、ユーザーインタラクションの後にUIをリフレッシュしたい場合は、何もする必要はありません。  
>しかし、タイマーなどのユーザーインタラクションが発生していないにもかかわらず、UIを更新したい場合がある。その場合、StateHasChangedを呼び出す必要があります。learn-blazor.comに、より詳細な例を作成しました。  
[c# - How Blazor Framework get notifed when the property value gets changed - Stack Overflow](https://stackoverflow.com/questions/48920530/how-blazor-framework-get-notifed-when-the-property-value-gets-changed)  
