import { ApiConstant } from '@/constants/constant';

/**
 * 同組来場者一覧取得リクエスト
 */
export class FramePlayerListRequest {
  /**
   * 連携種別区分コード
   * 12:RoundNaviWeb
   */
  private CoopClsTypeCD: number = ApiConstant.CoopClsTypeCD;
  /**
   * 暗号化されたWeb会員番号
   */
  public EncryptedWebMemberCD: string;

  /**
   * コンストラクタ
   * @param encryptedWebMemberCD 暗号化されたWeb会員番号
   */
  constructor(encryptedWebMemberCD: string) {
    this.EncryptedWebMemberCD = encryptedWebMemberCD;
  }
}
