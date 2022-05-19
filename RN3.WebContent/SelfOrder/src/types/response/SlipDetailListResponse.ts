import { SlipDetail } from '@/types/entity/SlipDetail';
import { SlipRequest } from '@/types/entity/SlipRequest';

/**
 * 伝票明細一覧取得レスポンス
 */
export class SlipDetailListResponse {
  /**
   * 伝票明細一覧
   */
  public SlipDetailList: SlipDetail[] = [];
  /**
   * 伝票要望一覧
   */
  public SlipRequestList: SlipRequest[] = [];
}
