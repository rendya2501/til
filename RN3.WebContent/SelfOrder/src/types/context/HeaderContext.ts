/**
 * ヘッダーコンポーネントコンテキスト
 */
export class HeaderContext {
  /**
   * 会計No
   */
  public AccountNo: string = '';
  /**
   * プレーヤー名
   */
  public PlayerName: string = '';
  /**
   * 合計金額
   */
  public TotalPrice: number = 0;
  /**
   * 合計数量
   */
  public TotalQuantity: number = 0;
  
  /**
   * コンストラクタ
   */
  constructor(init?: Partial<HeaderContext>) {
    Object.assign(this, init);
  }
}
