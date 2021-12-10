import Order from '@/types/api/Order'

export default class Player {
  BusinessDate: Date
  AccountNo: string
  Name: string
  Kana: string
  SlipList: Order[] = []

  constructor (businessDate: Date, accountNo: string, name: string, kana: string) {
    this.BusinessDate = businessDate
    this.AccountNo = accountNo
    this.Name = name
    this.Kana = kana
  }
}
