
# VB殴り書き

---

## 「0.12000」とかいう表示を「0.12」にする

```VB
(0.12000).ToString("G0")
```

---

## 関数を変数に代入

```VB
'FD_HtorilClass
    Dim t = Function(name As String)
        Dim tmp As Decimal = SrcData.AsEnumerable.
            Sum(Function(s)
                    Dim tmpint As Decimal = 0
                    Decimal.TryParse(s.Field(Of String)(name), tmpint)
                    Return tmpint
                End Function)
        If tmp <> 0 Then
            Return tmp.ToString
        Else
            Return String.Empty
        End If
    End Function
```

## Using使って実行した結果を受け取るだけのラムダ式

``` VB
Dim data As DataSet = (
    Function()
        Using cls As New FD_HenpinClass
            Return cls.GetByKey(Me.SlipNumber.Text)
        End Using
    End Function
).Invoke
```

---

``` VB
Dim bagTypeSettingList = Enumerable.
    Repeat(New Object, maxCol).
    Select(Function(s, index) New BagTypeDisplay With {
        .Column = index + 1,
        .TypeClassificationCount = Nothing,
        .TypeClassificationCode = "",
        .TypeClassificationName = "",
        .TypeCount = Nothing,
        .TypeCode = "",
        .TypeName = "",
        .BagTypeCode = Nothing,
        .BagTypeName = "",
        .Standard = "",
        .UnitPrice = "",
        .UnitBox = 0,
        .UnitBag = 0
    }).
    ToList

    Dim bagType = bagTypeDataRowList.
        FirstOrDefault(Function(f) f.Field(Of String)("BagTypeCode") = bagTypeCode)
```

---

## 匿名型の代入

```VB
    Dim linq As New With {
        .ErrApplicantName = String.Empty,
        .ErrPaymentType = "未入金",
        .ErrBillPrice = Decimal.Zero.ToString
    }
    Dim tmpLinq =
        Me.AllBillData.AsEnumerable.
        Where(Function(w) w.Field(Of String)("ReferenceNumber") = argList(Cst整理番号).PadLeft(8, "0"c)).
        Select(Function(s) New With {
                .ErrApplicantName = s.Field(Of String)("ApplicantName"),
                .ErrPaymentType = s.Field(Of String)("ReceiptTypeName"),
                .ErrBillPrice = s.Field(Of Decimal)("BillPrice").ToString
            }).FirstOrDefault
    If tmpLinq IsNot Nothing Then linq = tmpLinq
```

---

## Linq合計いろいろ

```VB
    Dim linq =
        InstallmentPaymentData.AsEnumerable.
        Where(Function(w) w.Field(Of DateTime?)("ReceiptDate") IsNot Nothing).
        Where(Function(w) IsDate(w.Field(Of DateTime?)("ReceiptDate"))).
        GroupBy(Function(g) g.Field(Of String)("SlipNumber")).
        Select(Function(s) New With {
            Key .PaymentPrice = s.Sum(Function(sum) sum.Field(Of Decimal)("PaymentPrice")),
            Key .ArrearsPrice = s.Sum(Function(sum) sum.Field(Of Decimal)("ArrearsPrice"))
        }).FirstOrDefault
```

---

## インデックス、順番ありのディクショナリー

これであれば、インデックス0の位置に空白を差し込むことが可能。  

```VB
Imports System.Collections ' DictionaryEntryを使うのに必要
Imports System.Collections.Specialized


Dim dict As New OrderedDictionary()
```

---

## 目的のデータだけ取得してデータテーブル作成

```VB
    Dim q As DataTable = typeClassData.AsEnumerable.
        Join(
            typeData.AsEnumerable,
            Function(x) x.Field(Of String)("TypeClassificationCode"),
            Function(y) y.Field(Of String)("TypeClassificationCode"),
            Function(x, y) New With {
                Key .TypeClassCode = x.Field(Of String)("TypeClassificationCode"),
                Key .TypeClassName = x.Field(Of String)("TypeClassificationName"),
                Key .TypeCode = y.Field(Of String)("TypeCode"),
                Key .TypeName = y.Field(Of String)("TypeName")}).
        Join(
            bagTypeData.AsEnumerable,
            Function(x) x.TypeCode,
            Function(y) y.Field(Of String)("TypeCode"),
            Function(x, y) New With {
                Key x,
                Key .BagTypeCode = y.Field(Of String)("BagTypeCode"),
                Key .BagTypeName = y.Field(Of String)("BagTypeName"),
                Key .Standard = y.Field(Of String)("Standard"),
                Key .UnitPrice = y.Field(Of Decimal)("UnitPrice"),
                Key .UnitBox = y.Field(Of Decimal)("UnitBox"),
                Key .UnitBag = y.Field(Of Decimal)("UnitBag")}).
        OrderBy(Function(o) o.x.TypeClassCode).
        ThenBy(Function(o) o.x.TypeCode).
        ThenBy(Function(o) o.BagTypeCode).
        Select(
            Function(s, tmpi)
                tmpi += 1
                Dim dr As DataRow = detailData.NewRow
                With dr
                    .SetField("TypeClassName", s.x.TypeClassName)
                    .SetField("TypeName", s.x.TypeName)
                    .SetField("BagTypeName", s.BagTypeName)
                    .SetField("Standard", s.Standard)
                    .SetField("BagTypeName", s.BagTypeName)
                    .SetField("QuantityBag", Decimal.Zero)
                    .SetField("QuantityBox", Decimal.Zero)
                    .SetField("QuantitySheet", Decimal.Zero)
                    .SetField("UnitPrice", s.UnitPrice)
                    .SetField("UnitBox", s.UnitBox)
                    .SetField("UnitBag", s.UnitBag)


                    If stockDataList.Any Then
                        'TODO:丸め区分によって処理を分ける。
                        Dim stock As StockData =
                                stockDataList.Find(New Predicate(Of StockData)(
                                Function(f) f.BagTypeCode.Equals(s.BagTypeCode)))

                        If stock IsNot Nothing Then
                            .SetField("Stock", Decimal.Parse(stock.StockQuantity.ToString))
                        Else
                            .SetField("Stock", Decimal.Zero)
                        End If
                    Else
                        .SetField("Stock", Decimal.Zero)
                    End If

                    '非表示項目
                    .SetField("SlipNumber", String.Empty)
                    .SetField("WarehousemanCode", Me.WarehousemanName.SelectedValue.ToString.PadLeft(2, "0"c))
                    .SetField("Number", tmpi)
                    .SetField("TypeClassCode", s.x.TypeClassCode)
                    .SetField("TypeCode", s.x.TypeCode)
                    .SetField("BagTypeCode", s.BagTypeCode)
                End With
                Return dr
            End Function).CopyToDataTable
```

---

## データテーブルの中身の更新いろいろ

```VB
'ImportRow
    Dim insertData As DataTable = SaveData.Clone
    For Each row As DataRow In SaveData.AsEnumerable.
        Select(Function(s)
            With s
                .SetField("InsertDateTime", tmpNow)
                .SetField("InsertStaffCode", tmpStaffCode)
                .SetField("InsertProgramName", tmpPrgName)
                .SetField("InsertComputerName", tmpPCName)
                .SetField("UpdateDateTime", tmpNow)
                .SetField("UpdateStaffCode", tmpStaffCode)
                .SetField("UpdateProgramName", tmpPrgName)
                .SetField("UpdateComputerName", tmpPCName)
            End With
            Return s
        End Function)

        insertData.ImportRow(row)
    Next

'ForEach
    For Each row As DataRow In SaveData.Rows
        With row
            .SetField("InsertDateTime", tmpNow)
            .SetField("InsertStaffCode", tmpStaffCode)
            .SetField("InsertProgramName", tmpPrgName)
            .SetField("InsertComputerName", tmpPCName)
            .SetField("UpdateDateTime", tmpNow)
            .SetField("UpdateStaffCode", tmpStaffCode)
            .SetField("UpdateProgramName", tmpPrgName)
            .SetField("UpdateComputerName", tmpPCName)
        End With
    Next
```

---

## テーブル定義

```VB
    Dim result As New DataTable
    With result
        ' 登録番号
        .Columns.Add("ApplicantCode", GetType(String))
        ' 郵便番号
        .Columns.Add("ZipCode", GetType(String))
        ' 住所1
        .Columns.Add("Address1", GetType(String))
        ' 住所2
        .Columns.Add("Address2", GetType(String))
    End With
```

---

## 配列の初期化

```VB
Dim Name() As String = System.Linq.Enumerable.Repeat(初期値, 要素数).ToArray
```

---

## デバッグの時のみ有効なメソッドの定義

```VB
    <Conditional("DEBUG")>
    Private Sub Test()

    End Sub

    #If DEBUG Then
        'DotNetDev環境では平成29年から新元号01年とされ、正常に動かないのでデバッグ時のみ無理やり新元号1年と変換させる。
        Select Case tmpYear
            Case "29"
                tmpYear = "01"
            Case "30"
                tmpYear = "02"
        End Select
    #End If
```

---

## ストップウォッチ

```VB
    Dim sw As New Stopwatch
    sw.Start()

    sw.Stop()
    Debug.Write("データを検索しました。(" & sw.ElapsedMilliseconds & "ms)")
```

---

## ビルダーパターン VB.NetVer

```VB
Class BuilderSampleStudentGradeClass
    '総合評価
    Private total As String
    '算数
    Private arithmetic As String
    '科学
    Private chemistry As String
    '国語
    Private language As String
    '英語
    Private english As String
    '社会
    Private social As String
    '歴史
    Private history As String
    ' コンストラクタ
    Private Sub New(ByVal builder As Builder)
        Me.total = builder.total
        Me.arithmetic = builder.arithmetic
        Me.chemistry = builder.chemistry
        Me.language = builder.language
        Me.english = builder.english
        Me.social = builder.social
        Me.history = builder.history
    End Sub
    Public Class Builder
        '総合評価
        Protected Friend total As String
        '算数
        Protected Friend arithmetic As String
        '科学
        Protected Friend chemistry As String
        '国語
        Protected Friend language As String
        '英語
        Protected Friend english As String
        '社会
        Protected Friend social As String
        '歴史
        Protected Friend history As String
        Protected Friend Shared ReadOnly Property Instance As Builder = New Builder
        Protected Friend Function setArithmetic(ByVal val As String) As Builder
            MyClass.arithmetic = val
            Return Me
        End Function
        Protected Friend Function setChemistry(ByVal val As String) As Builder
            MyClass.chemistry = val
            Return Me
        End Function
        Protected Friend Function setlanguage(ByVal val As String) As Builder
            MyClass.language = val
            Return Me
        End Function
        Protected Friend Function setenglish(ByVal val As String) As Builder
            MyClass.english = val
            Return Me
        End Function
        Protected Friend Function setsocial(ByVal val As String) As Builder
            MyClass.social = val
            Return Me
        End Function
        Protected Friend Function sethistory(ByVal val As String) As Builder
            MyClass.history = val
            Return Me
        End Function
        Protected Friend Function build() As BuilderSampleStudentGradeClass
            Return New BuilderSampleStudentGradeClass(Me)
        End Function
    End Class
End Class

Class Test111
    Dim test As BuilderSampleStudentGradeClass =
        BuilderSampleStudentGradeClass.Builder.Instance
            .setArithmetic("A")
            .setChemistry("E")
            .setlanguage("D")
            .setenglish("A")
            .setsocial("B")
            .sethistory("B")
            .build()
End Class
```

---

## 各コントロールのF4ドロップダウンを無効

```VB
' ECO FD_Syukka00_01

    Private Sub F4DropDown_KeyDown(sender As Object, e As KeyEventArgs) Handles SlipNumber.KeyDown,
            DepositInputList.KeyDown
        Try
            If e.KeyCode = Keys.F4 Then e.Handled = True
        Catch ex As Exception
            Call ErrorLogClass.WriteErrorLog(ex, True)
        End Try
    End Sub
```

---

## MultiRowの抑制

```VB
'FS_Nyukin00

    ''' <summary>
    ''' MultiRowのF4 キーによるドロップダウンウィンドウの表示の抑制
    ''' https://dev.grapecity.co.jp/support/kb/detail.asp?id=26038
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Sub GcMultiRow1_EditingControlShowing(
        ByVal sender As System.object,
        ByVal e As EditingControlShowingEventArgs
    ) Handles DepositInputList.EditingControlShowing
        If TypeOf e.Control Is GcNumber OrElse
            TypeOf e.Control Is GcDateTime OrElse
            TypeOf e.Control Is GcTextBox OrElse
            TypeOf e.Control Is GcComboBox Then

            RemoveHandler e.Control.KeyDown, AddressOf Me.F4DropDown_KeyDown
            AddHandler e.Control.KeyDown, AddressOf Me.F4DropDown_KeyDown
        End If
    End Sub

    Private Sub F4DropDown_KeyDown(sender As Object, e As KeyEventArgs) Handles SlipNumber.KeyDown,
            DepositInputList.KeyDown,
            ProvisionReceiptDate.KeyDown
        Try
            If e.KeyCode = Keys.F4 Then e.Handled = True
        Catch ex As Exception
            Call ErrorLogClass.WriteErrorLog(ex, True)
        End Try
    End Sub
```

---

## イベントの有効無効の設定

MultiRowのイベントにより、指定したコントロールへカーソルが移動しないため。  

```VB
'FD_NyukaD00

    'DataSourceのセット時にCellEnterイベントが発生し上のステップで指定している
    'ArrivalDate.Select()が動作しなくなってしまうため、
    '不要なイベントの発生を避ける
    RemoveHandler Me.ArrivalList.CellEnter, AddressOf ArrivalList_CellEnter
    Try
        Me.ArrivalList.DataSource = detailData
    Catch ex As Exception
        Throw
    Finally
        AddHandler Me.ArrivalList.CellEnter, AddressOf ArrivalList_CellEnter
    End Try
```

---

## 呼び出し元の名称orメソッドを取得する

``` VB
    Dim StFrame As New StackFrame(1)
    Debug.Print(StFrame.GetMethod.DeclaringType.Name.ToString)
    Debug.Print(StFrame.GetMethod.Name.ToString)
```

---

## エラーに関して

- 業務エラーは例外としてスローし、上位で適切な処置を行う必要があります。
- 業務エラーを例外で表すことにより、可読性・保守性の向上に繋がります。
- 業務エラーを例外で表しても、適切な設計を行えばパフォーマンスに問題はありません。
- 業務エラーを表す例外に対して適切な処理を行っている限り、型以外の例外情報は不要です。
- 業務エラーを表す例外がハンドルされなかった場合、例外情報がとても重宝します。
- 業務エラーを表す例外はException クラスから派生させます。

業務エラーはExceptionクラスを派生して業務エラー用のクラスを作ってそれを検知するというのが正しい例外処理のようだ。  
それならば本来の例外と業務エラーをごっちゃにしてExceptionだけで取得する必要がなくなる。  
業務エラーなら派生クラスで再定義、本来のエラーはexceptionで取得。  
業務エラーをフォームに組み込むならば、親クラスで定義してもらわないと無理だな。  

---

## WhenAllとWaitAll

Task.WhenAllはAsync Await中で使うものだ。  
Asyncの中でAwait Task.WhenAllとすることで、別スレッドの処理の完了をすべて待つという意味合いになる。  

Asyncを使わない、通常のタスクとして実行するならTask.WaitAllだ。  
これならAsyncでない場合、すべての処理が終わるまで待ってくれる。  
