/**
 * 伝票明細
 */
export class SlipDetail {
  /**
   * 会計No
   */
  public AccountNo!: string;
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
   * パターンコード
   * int
   */
  public PatternCD!: number;
  /**
   * メニュー大分類コード
   * int
   */
  public MenuLargeClassCD!: number;
  /**
   * メニュー分類コード
   * int
   */
  public MenuClassCD!: number;
  /**
   * メニューコード
   * int
   */
  public MenuCD!: number;
  /**
   * メニュー名
   */
  public MenuName!: string;
  /**
   * 手書備考
   * byte[]
   */
  public HandwritingNote: number[] = [];
  /**
   * 数量
   * decimal
   */
  public Quantity!: number;
  /**
   * 単価
   * decimal
   */
  public UnitPrice!: number;
}
