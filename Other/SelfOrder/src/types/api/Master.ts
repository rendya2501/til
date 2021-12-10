import OrderMenuCls from '@/types/api/OrderMenuCls'
import OrderMenu from '@/types/api/OrderMenu'
import OrderRequest from '@/types/api/OrderRequest'

export default class Master {
  // メニュー分類リスト
  OrderMenuClsList: OrderMenuCls[] = []
  // メニューリスト
  OrderMenuList: OrderMenu[] = []
  // 要望リスト
  OrderRequestList: OrderRequest[] = []
}
