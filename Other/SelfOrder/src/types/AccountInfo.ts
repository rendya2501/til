import OrderItem from '@/types/OrderItem'
import TotalInfo from '@/types/TotalInfo'

export default class AccountInfo {
  holderNo: string
  businessDate: Date
  accountNo: string
  name: string
  cartItems: OrderItem[]
  orderedItems: OrderItem[]

  constructor (holderNo: string, businessDate: Date, accountNo: string, name: string, cartItems: OrderItem[], orderedItems: OrderItem[]) {
    this.holderNo = holderNo
    this.businessDate = businessDate
    this.accountNo = accountNo
    this.name = name
    this.cartItems = cartItems
    this.orderedItems = orderedItems
  }

  public getCartTotal (): TotalInfo {
    const total = new TotalInfo()
    this.cartItems.forEach(item => {
      total.price += item.subtotal
      total.amount += item.amount
    })
    return total
  }

  public getOrderedTotal (): TotalInfo {
    const total = new TotalInfo()
    this.orderedItems.forEach(item => {
      total.price += item.subtotal
      total.amount += item.amount
    })
    return total
  }

  public getBusinessDate (): Date {
    return this.businessDate
  }

  public getAccountNo (): string {
    return this.accountNo
  }

  public getCartItems (): OrderItem[] {
    return this.cartItems
  }

  public deleteCartItem (targetIndex: number): void {
    this.cartItems.splice(targetIndex, 1)
  }
}
