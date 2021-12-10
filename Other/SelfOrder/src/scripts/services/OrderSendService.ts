import OrderSendApiCaller from '@/scripts/api/OrderSendApiCaller'
import AccountInfo from '@/types/AccountInfo'

export default class OrderSendService {
  private orderSendApiCaller = new OrderSendApiCaller()
  public sendOrder (facilityCode: string, accountNo: string, tableNo: string, accountInfo: AccountInfo): Promise<number> {
    try {
      const ret = this.orderSendApiCaller.sendOrder(facilityCode, accountNo, tableNo, accountInfo)
      return ret
    } catch (e) {
      return e.status
    }
  }
}
