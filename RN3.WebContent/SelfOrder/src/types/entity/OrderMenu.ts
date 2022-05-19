/**
 * メニューマスタ
 */
export class OrderMenu {
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
   * 親メニューコード
   */
  public ParentMenuCD: number = 0;
  /**
   * 要望表示フラグ
   */
  public RequestDisplayFlag: boolean = false
  /**
   * パネル名称
   */
  public MenuName: string = '';
  /**
   * 説明
   */
  public Description: string = '';
  /**
   * 販売単価 decimal?
   */
  public UnitSellingPrice: number = 0;
}
