/**
 * メニュー画像取得リクエスト
 */
export class MenuImageRequest {
  /**
   * パターンコード
   */
  public PatternCD!: number;
  /**
   * メニュー大分類コード
   */
  public MenuLargeClassCD!: number;
  /**
   * メニュー分類コード
   */
  public MenuClassCD!: number;
  /**
   * メニューコード
   */
  public MenuCD!: number;
  
  /**
   * コンストラクタ
   * @param init
   */
  constructor(init?: Partial<MenuImageRequest>) {
    Object.assign(this, init);
  }
}
