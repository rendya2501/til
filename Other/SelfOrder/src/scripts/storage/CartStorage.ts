import OrderItem from '@/types/OrderItem'

export default class CartStorage {
  private KEY_PREFIX = 'e-table-order-cart-'

  private facilityNo: string
  private accountNo: string

  private get key (): string {
    return this.KEY_PREFIX + this.facilityNo + '-' + this.accountNo
  }

  constructor (facilityNo: string, accountNo: string) {
    this.facilityNo = facilityNo
    this.accountNo = accountNo
  }

  public get cartItems (): OrderItem[] {
    const ret: OrderItem[] = []
    if (this.key in localStorage) {
      const objs = JSON.parse(localStorage.getItem(this.key) as string) as OrderItem[]
      objs.forEach(obj => ret.push(new OrderItem(obj.id, obj.menuCode, obj.name, obj.price, obj.amount, obj.request)))
    } else {
      localStorage.setItem(this.key, JSON.stringify([]))
    }
    return ret
  }

  public update (target: OrderItem): void {
    const cartItems = this.cartItems
    const index = cartItems.findIndex(item => item.id === target.id)
    cartItems[index] = target
    localStorage.setItem(this.key, JSON.stringify(cartItems))
  }

  public append (target: OrderItem): void {
    const cartItems = this.cartItems
    if (cartItems.length === 0) {
      target.id = 1
    } else {
      target.id = Math.max.apply(null, cartItems.map(item => item.id)) + 1
    }
    cartItems.push(target)
    localStorage.setItem(this.key, JSON.stringify(cartItems))
  }

  public delete (target: OrderItem): void {
    const cartItems = this.cartItems
    const postDeleteItems = cartItems.filter(item => item.id !== target.id)
    localStorage.setItem(this.key, JSON.stringify(postDeleteItems))
  }

  public clear (): void {
    localStorage.setItem(this.key, JSON.stringify([]))
  }
}
