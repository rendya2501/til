import Slip from '@/types/api/Slip'

export default class OrderSend {
  facilityDC: string
  businessDate: string
  receptionDateTime: string
  accountNo: string
  tableNo: string
  orderList: Slip[] = []

  constructor (facilityCD: string, businessDate: string, receptionDateTime: string, accountNo: string, tableNo: string, slip: Slip[]) {
    this.facilityDC = facilityCD
    this.businessDate = businessDate
    this.receptionDateTime = receptionDateTime
    this.accountNo = accountNo
    this.tableNo = tableNo
    this.orderList = slip
  }
}
