import MasterInfo from '@/types/MasterInfo'
import MasterApiCaller from '@/scripts/api/MasterApiCaller'
import OrderMenuCls from '@/types/api/OrderMenuCls'
import OrderMenu from '@/types/api/OrderMenu'
import OrderRequest from '@/types/api/OrderRequest'

export default class MasterService {
  private masterApiCaller = new MasterApiCaller()

  // テスト用データ
  private debugAccount: MasterInfo = new MasterInfo(
    [
      new OrderMenuCls(1, '食事'),
      new OrderMenuCls(2, 'おつまみ'),
      new OrderMenuCls(3, 'ソフトドリンク'),
      new OrderMenuCls(4, 'アルコール')
    ],
    [
      new OrderMenu(1, 1, 'カレーライス', 1000),
      new OrderMenu(1, 2, 'ステーキ', 1500),
      new OrderMenu(2, 3, '枝豆', 350),
      new OrderMenu(2, 4, 'オニオンライス', 400),
      new OrderMenu(3, 1, 'オレンジジュース', 250),
      new OrderMenu(3, 2, 'コーラ', 250),
      new OrderMenu(3, 3, 'ウーロン茶', 250),
      new OrderMenu(4, 1, '生ビール（中）', 450)
    ],
    [
      new OrderRequest(1, '食前'),
      new OrderRequest(2, '食後')
    ]
  )

  public async getMasterInfo (facilityNo: string): Promise<MasterInfo> {
    // マスター情報を取得
    const master = await this.masterApiCaller.getMaster(facilityNo)

    const masterInfo = new MasterInfo([], [], [])

    master.OrderMenuClsList.forEach(orderMenuCls => {
      masterInfo.orderMenuClsList.push(new OrderMenuCls(orderMenuCls.OrderMenuClsCD, orderMenuCls.OrderMenuClsName))
    })
    master.OrderMenuList.forEach(orderMenu => {
      masterInfo.orderMenuList.push(new OrderMenu(orderMenu.OrderMenuClsCD, orderMenu.OrderMenuCD, orderMenu.OrderMenuName, orderMenu.UnitPrice))
    })
    master.OrderRequestList.forEach(orderRequest => {
      masterInfo.orderRequestList.push(new OrderRequest(orderRequest.OrderRequestCD, orderRequest.OrderRequestName))
    })

    return masterInfo
  }
}
