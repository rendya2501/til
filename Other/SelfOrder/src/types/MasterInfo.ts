import OrderMenuCls from '@/types/api/OrderMenuCls'
import OrderMenu from '@/types/api/OrderMenu'
import OrderRequest from '@/types/api/OrderRequest'

export default class MasterInfo {
  // メニュー分類リスト
  orderMenuClsList: OrderMenuCls[]
  // メニューリスト
  orderMenuList: OrderMenu[]
  // 要望リスト
  orderRequestList: OrderRequest[]

  constructor (orderMenuClsList: OrderMenuCls[], orderMenuList: OrderMenu[], orderRequestList: OrderRequest[]) {
    this.orderMenuClsList = orderMenuClsList
    this.orderMenuList = orderMenuList
    this.orderRequestList = orderRequestList
  }
}
