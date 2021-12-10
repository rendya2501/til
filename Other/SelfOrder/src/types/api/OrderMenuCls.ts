// メニュー分類
export default class OrderMenuCls {
  // メニュー分類コード
  OrderMenuClsCD = 0
  // メニュー分類名
  OrderMenuClsName = ''

  constructor (orderMenuClsCD: number, orderMenuClsName: string) {
    this.OrderMenuClsCD = orderMenuClsCD
    this.OrderMenuClsName = orderMenuClsName
  }
}
