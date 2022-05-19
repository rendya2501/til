/**
 * プレーヤー選択ダイアログコンテキスト
 */
export class SelectPlayerDialogContext {
  /**
   * 暗号化された会計No
   */
  public encryptedAccountNo!: string;
  /**
   * 会計No
   */
  public accountNo!: string;
  /**
   * プレーヤー名
   */
  public playerName!: string;
  /**
   * 選択されているかフラグ
   */
  public isSelected: boolean = false;
  
  /**
   * コンストラクタ
   * @param init
   */
  constructor(init?: Partial<SelectPlayerDialogContext>) {
    Object.assign(this, init);
  }
}
