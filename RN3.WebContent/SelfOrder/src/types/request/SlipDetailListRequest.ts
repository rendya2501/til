/**
 * 伝票明細一覧取得リクエスト
 */
export class SlipDetailListRequest {
  /**
   * 暗号化された会計No一覧
   * 同組のうち、選択された来場者の会計Noをすべて渡してもらう想定。
   */
  public EncryptedAccountNoList: string[] = [];
  
  /**
   * コンストラクタ
   * @param encryptedAccountNoList
   */
  constructor(encryptedAccountNoList: string[]) {
    this.EncryptedAccountNoList = encryptedAccountNoList;
  }
}
