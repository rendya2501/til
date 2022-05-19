import { Player } from '@/types/entity/Player';

/**
 * 同組来場者一覧取得レスポンス
 */
export class FramePlayerListResponse {
  /**
   * 暗号化された代表者の会計No
   */
  public EncryptedAccountNo: string = '';
  /**
   * 複数選択モードか
   */
  public IsMultiSelect: boolean = false;
  /**
   * 営業日
   */
  public BusinessDate: Date = new Date();
  /**
   *  来場者一覧
   */
  public PlayerList: Player[] = [];
}
