// 伝票情報
export default class Slip {
  orderMenuCD: string = ''
  orderMenuName: string = ''
  unitPrice: number = 0
  quantity: number = 0
  orderRequestName: string = ''

  constructor (orderMenuCD: string, orderMenuName: string, unitPrice: number, quantity: number, orderRequestName: string) {
    this.orderMenuCD = orderMenuCD
    this.orderMenuName = orderMenuName
    this.unitPrice = unitPrice
    this.quantity = quantity
    this.orderRequestName = orderRequestName
  }
}
