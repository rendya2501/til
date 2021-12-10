import ApiCallerBase from '@/scripts/api/ApiCallerBase'
import MenuImage from '@/types/api/MenuImage'

export default class MenuImageApiCaller extends ApiCallerBase {
  public async getMenuImage (facilityNo: string, menuNo: string): Promise<MenuImage> {
    const params: any = {
      OrderMenuCD: menuNo
    }
    const menuImage = new MenuImage('')
    return await super.post('menu_image/get', params, menuImage, 'メニュー画像取得')
  }
}
