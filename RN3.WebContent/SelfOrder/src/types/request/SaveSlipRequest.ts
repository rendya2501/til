import { SaveSlipHeader } from '@/types/entity/SaveSlipHeader';

/**
 * 伝票登録リクエスト
 */
export class SaveSlipRequest {
  /**
   * 伝票ヘッダー一覧
   */
  public SlipHeaderList: SaveSlipHeader[] = [];
}
