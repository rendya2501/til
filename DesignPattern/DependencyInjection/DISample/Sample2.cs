using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DISample;

/// <summary>
/// https://qiita.com/mizunowanko/items/53eed059fc044c5aa5dc
/// </summary>
/// <remarks>
/// 簡単にいえばクラスの内部でnewするのではなく、外部で用意してそれを注入するということです。
/// 
/// メリット
/// ・ソフトウエアの階層をきれいに分離した設計が容易になる
/// ・コードが簡素になり、開発期間が短くなる
/// ・テストが容易になり、「テスト・ファースト」による開発スタイルを取りやすくなる
/// ・特定のフレームワークへの依存性が極小になるため、変化に強いソフトウエアを作りやすくなる（＝フレームワークの進化や、他のフレームワークへの移行に対応しやすくなる）
/// 
/// デメリット
/// ・はじめに工数がかかる場合が多いと思われる クラスをたくさん作るので(大抵の場合)
/// ・プログラムの実行スピードが遅くなる可能性が高い
/// ・クラスやファイルが沢山できる
/// </remarks>
public class Sample2
{
    static void Main(string[] args)
    {
        // こういうnewしているのを一か所にまとめたのがDIコンテナーって認識でいいのかな。
        // あっちでもこっちでもprocessorをnewするなら１か所でnewしてそれをgetする形にしたほうが便利ジャン的な？
        // 一番最初に起動するプログラムの定義をコンテナーに集めるのもありか？
        CreditCardProcessor processor = new PaypalCreditCardProcessor();
        TransactionLog transactionLog = new DatabaseTransactionLog();
        IBillingService billingService = new RealBillingService(processor, transactionLog);
    }
}


public class RealBillingService : IBillingService
{
    private readonly CreditCardProcessor processor;
    private readonly TransactionLog transactionLog;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="processor"></param>
    /// <param name="transactionLog"></param>
    public RealBillingService(
        CreditCardProcessor processor,
        TransactionLog transactionLog)
    {
        this.processor = processor;
        this.transactionLog = transactionLog;
    }

    public Receipt ChargeOrder(PizzaOrder order, CreditCard creditCard)
    {
        //①
        //CreditCardProcessor processor = new PaypalCreditCardProcessor();
        //TransactionLog transactionLog = new DatabaseTransactionLog();

        //②
        //CreditCardProcessor processor = CreditCardProcessorFactory.GetInstance();
        //TransactionLog transactionLog = TransactionLogFactory.GetInstance();

        try
        {
            ChargeResult result = this.processor.Charge(creditCard, order.GetAmount());
            this.transactionLog.LogChargeResult(result);

            return result.WasSuccessful()
                ? Receipt.ForSuccessfulCharge(order.GetAmount())
                : Receipt.ForDeclinedCharge(result.GetDeclineMessage());
        }
        catch (Exception e)
        {
            this.transactionLog.LogConnectException(e);
            return Receipt.ForSystemFailure(e.Message);
        }
    }
}



/// <summary>
/// ファクトリークラス
/// </summary>
/// <remarks>
///  Factoryクラスは、クライアントとサービスの実装とを分離します。
///  単純なFactoryクラスでは、インターフェースを実装したモックをgetterやsetterで操作できます。
/// </remarks>
public class CreditCardProcessorFactory
{
    private static CreditCardProcessor instance;

    public static void SetInstance(CreditCardProcessor processor)
    {
        instance = processor;
    }

    public static CreditCardProcessor GetInstance()
    {
        if (instance == null)
        {
            return new SquareCreditCardProcessor();
        }

        return instance;
    }
}
public class TransactionLogFactory
{
    private static TransactionLog instance;

    public static void SetInstance(TransactionLog processor)
    {
        instance = processor;
    }

    public static TransactionLog GetInstance()
    {
        if (instance == null)
        {
            return new SquareTransactionLog();
        }

        return instance;
    }
}


public class DatabaseTransactionLog : TransactionLog
{
}

public class PaypalCreditCardProcessor : CreditCardProcessor
{
}

public class ChargeResult
{
    public object GetDeclineMessage()
    {
        throw new NotImplementedException();
    }

    public bool WasSuccessful()
    {
        throw new NotImplementedException();
    }
}

public class TransactionLog
{
    internal void LogChargeResult(ChargeResult result)
    {
        throw new NotImplementedException();
    }

    internal void LogConnectException(Exception e)
    {
        throw new NotImplementedException();
    }
}

public class CreditCardProcessor
{
    public ChargeResult Charge(CreditCard creditCard, object p)
    {
        throw new NotImplementedException();
    }
}


public class SquareCreditCardProcessor : CreditCardProcessor
{
}

public class SquareTransactionLog : TransactionLog
{
}



public interface IBillingService
{
    /*クレジットカードに注文代の課金を試みる。
     * 成功しても失敗しても、トランザクションは記録される。
     * @ return トランザクションの領収書（Reciptオブジェクト）を返す。課金が成功した場合、Reciptオブジェクトは正常な値を持つ。失敗した場合、Reciptオブジェクトは課金が失敗した理由を保持する。 
     */
    Receipt ChargeOrder(PizzaOrder order, CreditCard creditCard);
}
public class CreditCard
{
}

public class PizzaOrder
{
    internal object GetAmount()
    {
        throw new NotImplementedException();
    }
}

public class Receipt
{
    public static Receipt ForDeclinedCharge(object p)
    {
        throw new NotImplementedException();
    }

    public static Receipt ForSuccessfulCharge(object v)
    {
        throw new NotImplementedException();
    }

    public static Receipt ForSystemFailure(object p)
    {
        throw new NotImplementedException();
    }
}
