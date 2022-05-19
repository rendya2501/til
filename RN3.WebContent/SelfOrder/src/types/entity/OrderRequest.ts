/**
 * オーダー要望
 */
export class OrderRequest {
  /**
   * パターンコード
   */
  public PatternCD: number = 0;
  /**
   * メニュー大分類コード
   */
  public MenuLargeClassCD: number = 0;
  /**
   * メニュー分類コード
   */
  public MenuClassCD: number = 0;
  /**
   * メニューコード
   */
  public MenuCD: number = 0;
  /**
   * 要望コード
   */
  public RequestCD: number = 0;
  /**
   * 親要望コード int?
   */
  public ParentRequestCD: number = 0;
  /**
   * 要望名
   */
  public RequestName: string = '';
  /**
   * 要望接尾辞
   */
  public RequestSuffix: string = '';
  /**
   * 要望区分 OrderRequestType
   */
  public Type: number = 0;
  /**
   * 金額 decimal?
   */
  public Price: number = 0;
}
