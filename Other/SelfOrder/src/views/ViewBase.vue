<script lang="ts">
import { Vue } from 'vue-property-decorator'
import ToastFactory from '@/scripts/ToastFactory'
import AccountService from '@/scripts/services/AccountService'
import AccountInfo from '@/types/AccountInfo'
import TableService from '@/scripts/services/TableService'
import FacilityTableInfo from '@/types/FacilityTableInfo'

export default class ViewBase extends Vue {
  protected toast = new ToastFactory()
  protected tableService = new TableService()
  protected facilityTableInfo: FacilityTableInfo | null = null
  protected accountService = new AccountService()
  protected accountInfo: AccountInfo | null = null
  protected category: string | null = null
  protected qrscan: string | null = null

  async mounted (): Promise<void> {
    this.setCategoryFromQuery()
    await this.setQrScanFromQuery()
    await this.setFacilityTableFromQuery()
    await this.setAccountFromQuery()

    window.addEventListener('resize', this.onWindowResize)
    window.addEventListener('touchstart', this.preventMultipleTouches, { passive: false })
    window.addEventListener('touchmove', this.preventMultipleTouches, { passive: false })
    this.onWindowResize()
  }

  public onWindowResize (): void {
    const vh = window.innerHeight
    const appDiv = document.getElementById('app')
    if (appDiv) {
      appDiv.style.height = vh + 'px'
    }
    const mainDivs = document.getElementsByClassName('main')
    if (mainDivs && mainDivs.length > 0 && mainDivs[0] instanceof HTMLElement) {
      (mainDivs[0] as HTMLElement).style.height = (vh - 50) + 'px'
    }
  }

  public preventMultipleTouches (e: TouchEvent): void {
    if (e.touches.length >= 2 || e.targetTouches.length >= 2) {
      e.preventDefault()
    }
  }

  protected getQueryString (param: string | (string | null)[]): string | null {
    return param ? String(param) : null
  }

  protected async onAccountInfoChanged (): Promise<void> {
    // オーバーライドしてください
  }

  protected async onFacilityTableInfoChanged (): Promise<void> {
    // オーバーライドしてください
  }

  protected async setFacilityTableFromQuery (): Promise<void> {
    return this.setFacilityTable(this.getQueryString(this.$route.query.facilityNo), this.getQueryString(this.$route.query.tableNo))
  }

  protected async setFacilityTable (facilityNo: string | null, tableNo: string | null): Promise<void> {
    if (facilityNo && tableNo) {
      const facilityTableInfo = await this.tableService.getFacilityTable(String(facilityNo), String(tableNo))
      this.facilityTableInfo = facilityTableInfo
    } else {
      this.facilityTableInfo = null
    }
    this.onFacilityTableInfoChanged()
  }

  protected async setAccountFromQuery (): Promise<void> {
    return this.setAccount(this.getQueryString(this.$route.query.holderNo))
  }

  protected async setAccount (holderNo: string | null): Promise<void> {
    if (this.facilityTableInfo && this.facilityTableInfo.facilityNo && holderNo) {
      this.accountInfo = await this.accountService.getAccountInfo(this.facilityTableInfo.facilityNo, String(holderNo))
    } else {
      this.accountInfo = null
    }
    this.onAccountInfoChanged()
  }

  protected async setCategoryFromQuery (): Promise<void> {
    const selectedCategory = this.getQueryString(this.$route.query.category)
    if (selectedCategory) {
      this.category = selectedCategory
    }
  }

  protected async setQrScanFromQuery (): Promise<void> {
    const qrscan = this.getQueryString(this.$route.query.qrscan)
    if (qrscan) {
      this.qrscan = qrscan
    }
  }

  protected getElementById (id: string): HTMLElement {
    return document.getElementById(id) as HTMLElement
  }

  protected focusById (id: string): void {
    this.getElementById(id).focus()
  }

  // ボタンを一時的に無効化する(画面遷移をともなうボタンの押下時に使用)
  protected changeDisabledTemporaryBottonStatus (): void {
    // ボタンを無効化する
    this.changeDisabledBottonStatus(true)

    // 一定時間後にボタンを有効化する（画面遷移が中断された際に、再び操作できるようにするため）
    // -> 画面処理で無効化したボタンが再度有効になるためコメントアウト
    // setTimeout(() => {
    //  this.changeDisabledBottonStatus(false)
    // }, 5000)
  }

  // ボタンを無効/有効化する
  protected changeDisabledBottonStatus (disabled : boolean): void {
    const bottonItems = document.getElementsByClassName('btn') as HTMLCollection

    for (let i = 0; i < bottonItems.length; i++) {
      const btnItem = bottonItems[i] as HTMLButtonElement
      if (btnItem) {
        btnItem.disabled = disabled
      }
    }
  }

  protected get qrParameter (): { [key: string]: string } {
    const params: { [key: string]: string } = {}
    if (this.qrscan) {
      params.qrscan = this.qrscan
    }
    return params
  }

  protected get tableParameter (): { [key: string]: string } {
    const params = this.qrParameter
    if (this.facilityTableInfo) {
      if (this.facilityTableInfo.facilityNo) {
        params.facilityNo = this.facilityTableInfo.facilityNo
      }
      if (this.facilityTableInfo.tableNo) {
        params.tableNo = this.facilityTableInfo.tableNo
      }
    }
    return params
  }

  protected get fullParameter (): { [key: string]: string } {
    const params = this.tableParameter
    if (this.accountInfo && this.accountInfo.holderNo) {
      params.holderNo = this.accountInfo.holderNo
    }
    return params
  }

  protected addCategoryParameter (): { [key: string]: string } {
    const params = this.fullParameter
    params.category = this.category ? this.category : '1'
    return params
  }

  protected detailParameter (category: string, idx: string): { [key: string]: string } {
    const params = this.fullParameter
    params.category = this.category ? this.category : '1'
    params.idx = idx
    return params
  }

  protected moveToRegistTableNumber (): void {
    this.$router.push({ name: 'RegistTableNumber' })
  }

  protected moveToScanAccountNumber (): void {
    // SPAとQuaggaの相性が悪いので、バーコード読み込み画面については通常の画面遷移を行う
    let href = 'ScanAccountNumber?'
    let first = true

    for (const [key, value] of Object.entries(this.fullParameter)) {
      if (!first) {
        href += '&'
      }
      href += key + '=' + value
      first = false
    }

    location.href = href
  }

  protected backToScanAccountNumber (): void {
    // QRコードスキャン画面以外から戻るときは、ホルダー番号の情報を渡さないようにする
    let href = 'ScanAccountNumber?'
    let first = true

    for (const [key, value] of Object.entries(this.tableParameter)) {
      if (!first) {
        href += '&'
      }
      href += key + '=' + value
      first = false
    }
    location.href = href
  }

  protected moveToMenuList (): void {
    this.$router.push({ path: 'MenuList', query: this.addCategoryParameter() })
  }

  protected moveToMenuDetail (category: string, idx: string): void {
    this.$router.push({ path: 'MenuDetail', query: this.detailParameter(category, idx) })
  }

  protected moveToConfirmOrder (): void {
    this.$router.push({ path: 'ConfirmOrder', query: this.addCategoryParameter() })
  }

  protected moveToThanksForOrders (): void {
    this.$router.push({ path: 'ThanksForOrders', query: this.fullParameter })
  }

  protected moveToOrderedList (): void {
    this.$router.push({ path: 'OrderedList', query: this.addCategoryParameter() })
  }
}
</script>
