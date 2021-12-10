import ApiCallerBase from '@/scripts/api/ApiCallerBase'
import OrderSend from '@/types/api/OrderSend'
import OrderItem from '@/types/OrderItem'
import AccountInfo from '@/types/AccountInfo'

export default class OrderSendApiCaller extends ApiCallerBase {
  public async sendOrder (facilityCode: string, accountNo: string, tableNo: string, accountInfo: AccountInfo): Promise<any> {
    const currentDate = this.createDateString(accountInfo.getBusinessDate())
    const currentTime = this.createDateTimeString()
    const cartItems = this.createSlipList(accountInfo.getCartItems())
    // 注文情報の組み立て
    const params: any = {
      FacilityCD: facilityCode,
      BusinessDate: currentDate,
      ReceptionDateTime: currentTime,
      AccountNo: accountNo,
      TableNo: tableNo,
      SlipList: cartItems
    }
    const orderSend = new OrderSend(facilityCode, currentDate, currentTime, accountNo, tableNo, params.SlipList)
    return await super.post('save', params, orderSend, 'オーダー送信')
  }

  // 営業日時の作成'yyyy-mm-dd'形式
  private createDateString (businessDate: Date): string {
    const y = businessDate.getFullYear()
    const m = ('00' + (businessDate.getMonth() + 1)).slice(-2)
    const d = ('00' + businessDate.getDate()).slice(-2)
    return (y + '-' + m + '-' + d)
  }

  // 注文時刻の作成'yyyy-dd-mm hh:MM:ss'形式
  private createDateTimeString (): string {
    const dateTimeString = new Date()
    return dateTimeString.toLocaleString()
  }

  // 伝票情報作成
  private createSlipList (cartItems: OrderItem[]) {
    const slipList: {OrderMenuCD: string, OrderMenuName: string, UnitPrice: number, Quantity: number, OrderRequestName: string}[] = []
    // cartStrage内のorderItemをslipListへ格納する
    cartItems.forEach(item => {
      slipList.push({
        OrderMenuCD: item.menuCode,
        OrderMenuName: item.name,
        UnitPrice: item.price,
        Quantity: item.amount,
        OrderRequestName: item.request
      })
    })
    return slipList
  }
}
