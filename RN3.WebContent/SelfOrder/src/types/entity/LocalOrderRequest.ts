/**
 * メニュー詳細で使うオーダー情報を保持するクラス
 */
export class LocalOrderRequest {
  RequestCD: number | null = null;
  RequestName: string = '';
  RequestSuffix: string = '';
  Price: number = 0;
  Qunantity: number = 0;
  isSelected: boolean = false;
  /**
   * コンストラクタ
   * @param init
   */
  constructor(init?: Partial<LocalOrderRequest>) {
    Object.assign(this, init);
  }
}
