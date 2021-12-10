import axios from 'axios'
import Environments from '@/constants/Environments'
import ApiParameter from '@/types/api/ApiParameter'
import ToastFactory from '@/scripts/ToastFactory'
import ResponseError from '@/types/api/ResponseError'

export default class ApiCallerBase {
  private toast = new ToastFactory()
  private environments = new Environments()

  public async get<T> (uri: string, defaultValue: T, title = '', params: ApiParameter[] = []): Promise<T> {
    await this.environments.loadSetting()
    let ret: T = defaultValue
    const param = this.convertParameter(params)
    const uriStr = `${this.environments.API_URL_BASE}/${uri}${param}`
    console.log(title + '取得 [ 開始 ]')
    await axios.get(uriStr)
      .then(response => { ret = response.data })
      .catch(error => {
        throw new ResponseError(error)
      })
      .catch(error => this.handleError(error, title && title.length > 0 ? error.data : ''))
    console.log(title + '取得 [ 終了 ]')
    return ret
  }

  public async post<TResult> (uri: string, postData: any, defaultValue: TResult, title = ''): Promise<TResult> {
    await this.environments.loadSetting()
    let ret: any = {}
    const uriStr = `${this.environments.API_URL_BASE}/${uri}`
    console.log(title + ' [ 開始 ]')
    await axios.post(uriStr, postData, {
      headers: this.getPostHeader()
    })
      .then(response => { ret = response.data })
      .catch(error => {
        throw new ResponseError(error)
      })
      .catch(error => this.handleError(error, title && title.length > 0 ? error.data : ''))
    console.log(title + ' [ 終了 ]')
    return ret
  }

  private getPostHeader (): any {
    return {
      'Content-Type': 'application/json',
      'x-api-key': this.environments.WEBAPI_KEY
    }
  }

  protected handleError (error: any, message: string) {
    if (message && message.length > 0) {
      this.toast.error(message)
    } else {
      this.toast.error(message)
      throw error
    }
  }

  private convertParameter (params: ApiParameter[]): string {
    if (params.length === 0) {
      return ''
    }
    let paramString = '?'
    params.forEach(param => {
      if (paramString !== '?') {
        paramString += '&'
      }
      paramString += param.key + '=' + encodeURIComponent(param.value)
    })
    return paramString
  }

  protected convertBoolean (value: boolean): string {
    return value ? 'true' : 'false'
  }
}
