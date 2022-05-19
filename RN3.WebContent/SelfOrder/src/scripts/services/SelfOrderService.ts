import { ApiCallerBase } from '@/scripts/services/ApiCallerBase';
import { CallStaffRequest } from '@/types/request/CallStaffRequest';
import { SaveSlipRequest } from '@/types/request/SaveSlipRequest';
import { FramePlayerListResponse } from '@/types/response/FramePlayerListResponse';
import { FramePlayerListRequest } from '@/types/request/FramePlayerListRequest';
import { MenuImageRequest } from '@/types/request/MenuImageRequest';
import { MenuRequest } from '@/types/request/MenuRequest';
import { MenuResponse } from '@/types/response/MenuResponse';
import { SlipDetailListRequest } from '@/types/request/SlipDetailListRequest';
import { SlipDetailListResponse } from '@/types/response/SlipDetailListResponse';

/**
 * セルフオーダーAPIサービス
 */
export class SelfOrderService extends ApiCallerBase {
  /**
   * スタッフを呼び出します。
   */
  public async callStaff(
    request: CallStaffRequest,
    authenticationKey: string
  ): Promise<any> {
    return await super.post(
      'call_staff',
      request,
      authenticationKey,
      'スタッフ呼び出し'
    );
  }

  /**
   * 伝票を保存します。
   */
  public async save(
    request: SaveSlipRequest,
    authenticationKey: string
  ): Promise<any> {
    return await super.post('save', request, authenticationKey, '伝票保存');
  }

  /**
   * 同組来場者一覧を取得します。
   * @param request 同組来場者一覧取得リクエスト
   * @returns 同組来場者一覧
   */
  public async getFramePlayerList(
    request: FramePlayerListRequest,
    authenticationKey: string
  ): Promise<FramePlayerListResponse> {
    return await super.post(
      'frame_player/get/list',
      request,
      authenticationKey,
      '同組来場者一覧取得'
    );
  }

  /**
   * メニュー画像を取得します。
   * @param request
   * @returns
   */
  public async getMenuImage(
    request: MenuImageRequest,
    authenticationKey: string
  ): Promise<Array<number>> {
    return await super.post(
      'get/menu_image',
      request,
      authenticationKey,
      'メニュー画像取得'
    );
  }

  /**
   * メニューを取得します
   * @param request
   * @returns
   */
  public async getMenu(
    request: MenuRequest,
    authenticationKey: string
  ): Promise<MenuResponse> {
    return await super.post(
      'get/menu',
      request,
      authenticationKey,
      'メニュー取得'
    );
  }

  /**
   * 伝票明細一覧取得
   * @param request 伝票明細一覧取得条件
   * @returns 伝票明細一覧
   */
  public async GetSlipDetailList(
    request: SlipDetailListRequest,
    authenticationKey: string
  ): Promise<SlipDetailListResponse> {
    return await super.post(
      'slip_detail/get/list',
      request,
      authenticationKey,
      '伝票明細一覧取得'
    );
  }
}
