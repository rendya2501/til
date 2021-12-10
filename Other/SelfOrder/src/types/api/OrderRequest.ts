// 要望
export default class OrderRequest {
  // 要望コード
  OrderRequestCD = 0
  // 要望名
  OrderRequestName = ''

  constructor (orderRequestCD: number, orderRequestName: string) {
    this.OrderRequestCD = orderRequestCD
    this.OrderRequestName = orderRequestName
  }
}
