import AccountInfo from '@/types/AccountInfo'
import PlayerApiCaller from '@/scripts/api/PlayerApiCaller'
import OrderItem from '@/types/OrderItem'
import CartStorage from '@/scripts/storage/CartStorage'

export default class AccountService {
  private playerApiCaller = new PlayerApiCaller()

  private debugAccount: AccountInfo = new AccountInfo('99999001256', new Date(), '200', '鈴木　太郎', [],
    [
      new OrderItem(0, '', 'かつ丼', 800, 2, 'ご飯少なめ'),
      new OrderItem(0, '', '生ビール', 750, 3, '')
    ]
  )

  public async getAccountInfo (facilityNo: string, holderNo: string): Promise<AccountInfo> {
    // プレーヤー情報取得を取得
    // const accountInfo = this.debugAccount
    const player = await this.playerApiCaller.getPlayer(facilityNo, holderNo)

    const businessDate = player.BusinessDate ? new Date(player.BusinessDate) : new Date()

    const accountInfo = new AccountInfo(holderNo, businessDate, player.AccountNo, player.Name, [], [])
    player.SlipList.forEach(order => {
      accountInfo.orderedItems.push(new OrderItem(0, '', order.OrderMenuName, order.UnitPrice, order.Quantity, order.OrderRequestName))
    })

    // カート情報を取得
    const cartStorage = new CartStorage(facilityNo, holderNo)
    accountInfo.cartItems = cartStorage.cartItems

    return accountInfo
  }

  // カート情報を全て削除
  public clearCartItems (facilityNo: string, accountInfo: AccountInfo): AccountInfo {
    const cartStorage = new CartStorage(facilityNo, accountInfo.holderNo)
    cartStorage.clear()
    accountInfo.cartItems = cartStorage.cartItems
    return accountInfo
  }

  // カート情報を追加
  public appendCartItem (facilityNo: string, accountInfo: AccountInfo, menuCode: string, name: string, price: number, amount: number, request: string): AccountInfo {
    const cartStorage = new CartStorage(facilityNo, accountInfo.holderNo)
    const orderItem = new OrderItem(0, menuCode, name, price, amount, request)
    cartStorage.append(orderItem)
    accountInfo.cartItems = cartStorage.cartItems
    return accountInfo
  }

  // カート情報を削除
  public deleteCartItemById (facilityNo: string, accountInfo: AccountInfo, id: number): AccountInfo {
    const targetItem = accountInfo.cartItems.find(item => item.id === id)
    if (!targetItem) {
      return accountInfo
    }
    return this.deleteCartItem(facilityNo, accountInfo, targetItem)
  }

  // カート情報を更新
  public deleteCartItem (facilityNo: string, accountInfo: AccountInfo, orderItem: OrderItem): AccountInfo {
    const cartStorage = new CartStorage(facilityNo, accountInfo.holderNo)
    cartStorage.delete(orderItem)
    accountInfo.cartItems = cartStorage.cartItems
    return accountInfo
  }

  // カート情報を更新
  public updateCartItemById (facilityNo: string, accountInfo: AccountInfo, id: number, amount: number): AccountInfo {
    const targetItem = accountInfo.cartItems.find(item => item.id === id)
    if (!targetItem) {
      return accountInfo
    }
    targetItem.amount = amount
    return this.updateCartItem(facilityNo, accountInfo, targetItem)
  }

  // カート情報を更新
  public updateCartItem (facilityNo: string, accountInfo: AccountInfo, orderItem: OrderItem): AccountInfo {
    const cartStorage = new CartStorage(facilityNo, accountInfo.holderNo)
    cartStorage.update(orderItem)
    accountInfo.cartItems = cartStorage.cartItems
    return accountInfo
  }
}
