// 注文済み情報
export default class Order {
  OrderMenuName: string = ''
  UnitPrice: number = 0
  Quantity: number = 0
  Price: number = 0
  OrderRequestName: string = ''

  constructor (orderMenuName: string, unitPrice: number, quantity: number, price: number, orderRequestName: string) {
    this.OrderMenuName = orderMenuName
    this.UnitPrice = unitPrice
    this.Quantity = quantity
    this.Price = price
    this.OrderRequestName = orderRequestName
  }
}
