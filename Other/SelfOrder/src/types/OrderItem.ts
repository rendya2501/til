export default class OrderItem {
  id: number
  menuCode: string
  name: string
  price: number
  amount: number
  request: string

  constructor (id: number, menuCode: string, name: string, price: number, amount: number, request: string) {
    this.id = id
    this.menuCode = menuCode
    this.name = name
    this.price = price
    this.amount = amount
    this.request = request
  }

  public get subtotal (): number {
    return this.price * this.amount
  }
}
