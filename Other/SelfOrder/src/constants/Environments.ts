import axios from 'axios'

class Environments {
  private loaded = false

  private debugMode = true
  private webApiKey = '12345678901234567890123456789012'
  private apiUrlBase = 'http://192.168.1.24/RN3.WEB/option/restaurant/self_order'
  private barcodeScanPatchSize = 'medium'
  private barcodeScanDetectedCount = 3
  private sendUrlMessage = ''

  private CameraErrorBC = 'カメラが利用できるデバイスを使用してください。'
  private CameraErrorQR = 'カメラが利用できるデバイスを使用してください。'
  private CameraInitBC = 'カメラの起動が出来ませんでした。'
  private DispConfOrder1 = '確認して注文してください。'
  private DispConfOrder2 = '現在選択している商品はありません'
  private DispMenuDetail1 = '数量を確認して選択して下さい。'
  private DispMenuList1 = 'メニューを選択してください。'
  private DispOrderList1 = '注文履歴を確認してください。'
  private DispQR1 = 'QRコードをスキャンしてください。'
  private DispBarcode1 = 'バーコードが反射して撮影しにく場合は、ホルダーを掲げて、それをスマホで撮影するようにしてください。'
  private DispThanksOrder1 = 'ご注文を受け賜わりました。<br/>調理が終わるまでしばらくお待ちください'

  constructor (load?: boolean) {
    if (load) {
      this.loadSetting()
    }
  }

  public async loadSetting (): Promise<void> {
    if (this.loaded) {
      return
    }
    await axios.get('setting.json').then(response => {
      if (!response.data) {
        return
      }
      if (response.data.DebugMode === false) {
        this.debugMode = false
      } else {
        this.debugMode = true
      }
      if (response.data.WebApiKey) {
        this.webApiKey = response.data.WebApiKey
      }
      if (response.data.ApiUrlBase) {
        this.apiUrlBase = response.data.ApiUrlBase
      }
      if (response.data.BarcodeScanPatchSize) {
        this.barcodeScanPatchSize = response.data.BarcodeScanPatchSize
      }
      if (response.data.BarcodeScanDetectedCount) {
        const count = response.data.BarcodeScanDetectedCount as number
        if (count && count > 0) {
          this.barcodeScanDetectedCount = count
        }
      }
    }).catch(err => { console.log(err) })

    // キャッシュ対策でランダム文字を付与
    this.sendUrlMessage = 'message.json?timestamp=' + new Date().getTime()
    await axios.get(this.sendUrlMessage).then(response2 => {
      if (!response2.data) {
        return
      }
      if (response2.data.CameraErrorBC) {
        this.CameraErrorBC = response2.data.CameraErrorBC
      }
      if (response2.data.CameraErrorQR) {
        this.CameraErrorQR = response2.data.CameraErrorQR
      }
      if (response2.data.CameraInitBC) {
        this.CameraInitBC = response2.data.CameraInitBC
      }
      if (response2.data.DispConfOrder1) {
        this.DispConfOrder1 = response2.data.DispConfOrder1
      }
      if (response2.data.DispConfOrder2) {
        this.DispConfOrder2 = response2.data.DispConfOrder2
      }
      if (response2.data.DispMenuDetail1) {
        this.DispMenuDetail1 = response2.data.DispMenuDetail1
      }
      if (response2.data.DispMenuList1) {
        this.DispMenuList1 = response2.data.DispMenuList1
      }
      if (response2.data.DispOrderList1) {
        this.DispOrderList1 = response2.data.DispOrderList1
      }
      if (response2.data.DispQR1) {
        this.DispQR1 = response2.data.DispQR1
      }
      if (response2.data.DispBarcode1) {
        this.DispBarcode1 = response2.data.DispBarcode1
      }
      if (response2.data.DispThanksOrder1) {
        this.DispThanksOrder1 = response2.data.DispThanksOrder1
      }
    }).catch(err => { console.log(err) })
    this.loaded = true
  }

  public get DEBUG_MODE (): boolean {
    return this.debugMode
  }

  public get API_URL_BASE (): string {
    return this.apiUrlBase
  }

  public get WEBAPI_KEY (): string {
    return this.webApiKey
  }

  public get BARCODE_SCAN_PATCH_SIZE (): string {
    return this.barcodeScanPatchSize
  }

  public get BARCODE_SCAN_DETECTED_COUNT (): number {
    return this.barcodeScanDetectedCount
  }

  public get CAMERA_ERROR_BC (): string {
    return this.CameraErrorBC
  }

  public get CAMERA_ERROR_QR (): string {
    return this.CameraErrorQR
  }

  public get CAMERA_INIT_BC (): string {
    return this.CameraInitBC
  }

  public get DISP_CONFORDER_1 (): string {
    return this.DispConfOrder1
  }

  public get DISP_CONFORDER_2 (): string {
    return this.DispConfOrder2
  }

  public get DISP_MENUDETAIL_1 (): string {
    return this.DispMenuDetail1
  }

  public get DISP_MENULIST_1 (): string {
    return this.DispMenuList1
  }

  public get DISP_ORDERLIST_1 (): string {
    return this.DispOrderList1
  }

  public get DISP_QR_1 (): string {
    return this.DispQR1
  }

  public get DISP_BARCODE_1 (): string {
    return this.DispBarcode1
  }

  public get DISP_THANKSORDER_1 (): string {
    return this.DispThanksOrder1
  }
}

export default Environments
