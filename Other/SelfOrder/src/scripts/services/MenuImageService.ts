import MenuImage from '@/types/api/MenuImage'
import MenuImageApiCaller from '@/scripts/api/MenuImageApiCaller'

export default class MenuImageService {
  private menuImageApiCaller = new MenuImageApiCaller()

  private debugMenuImage: MenuImage = new MenuImage('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')

  public async getMenuImage (facilityNo: string, menuNo: string): Promise<string> {
    // const menuImage = this.debugMenuImage
    // return menuImage.OrderMenuImage
    const image = await this.menuImageApiCaller.getMenuImage(facilityNo, menuNo)
    return image.OrderMenuImage
  }
}
