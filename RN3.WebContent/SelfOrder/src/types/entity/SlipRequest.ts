/**
 * 伝票要望
 */
export class SlipRequest {
  /**
   * オーダー伝票ID
   */
  public OrderSlipID!: string;
  /**
   * オーダー明細No
   * int
   */
  public OrderDetailNo!: number;
  /**
   * 要望コード
   * int
   */
  public RequestCD!: number;
  /**
   * 親要望コード
   * int?
   */
  public ParentRequestCD: number | null = null;
  /**
   * 要望名
   */
  public RequestName!: string;
  /**
   * 要望区分
   * OrderRequestType
   */
  public Type!: number;
  /**
   * 数量
   * decimal?
   */
  public Quantity: number | null = null;
  /**
   * 単価
   * decimal?
   */
  public UnitPrice: number | null = null;
}
