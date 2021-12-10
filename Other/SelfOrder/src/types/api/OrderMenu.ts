// メニュー
export default class OrderMenu {
  // メニュー分類コード
  OrderMenuClsCD = 0
  // メニューコード
  OrderMenuCD = 0
  // 商品名
  OrderMenuName = ''
  // 単価
  UnitPrice = 0

  constructor (orderMenuClsCD: number, orderMenuCD: number, orderMenuName: string, unitPrice: number) {
    this.OrderMenuClsCD = orderMenuClsCD
    this.OrderMenuCD = orderMenuCD
    this.OrderMenuName = orderMenuName
    this.UnitPrice = unitPrice
  }
}
