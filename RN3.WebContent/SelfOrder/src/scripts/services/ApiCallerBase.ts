import axios from 'axios';
import { Environments } from '@/constants/Environments';
import { ApiParameter } from '@/types/entity/ApiParameter';
import { ResponseError } from '@/types/entity/ResponseError';
import { ApiConstant } from '@/constants/constant';

/**
 *
 */
export class ApiCallerBase {
  private environments = new Environments();

  /**
   * Getメソッド
   * @param uri
   * @param defaultValue
   * @param title
   * @param params
   * @returns
   */
  protected async get<T>(
    uri: string,
    defaultValue: T,
    title = '',
    params: ApiParameter[] = []
  ): Promise<T> {
    let res: T = defaultValue;
    await this.environments.loadSetting();
    const param = this.convertParameter(params);
    const uriStr = `${this.environments.API_URL_BASE}/${uri}${param}`;
    console.log(title + '取得 [ 開始 ]');
    await axios
      .get(uriStr)
      .then(response => {
        res = response.data;
      })
      .catch(error => {
        throw new ResponseError(error);
      });
    console.log(title + '取得 [ 終了 ]');
    return res;
  }

  /**
   * Postメソッド
   * @param uri
   * @param postData
   * @param authenticationKey
   * @param title
   * @returns
   */
  protected async post<TResult>(
    uri: string,
    postData: any,
    authenticationKey: string,
    title = ''
  ): Promise<TResult> {
    await this.environments.loadSetting();
    let res: any = {};
    const uriStr = `${this.environments.API_URL_BASE}/${uri}`;
    const header = {
      headers: this.getPostHeader(authenticationKey)
    };
    console.log(title + ' [ 開始 ]');
    await axios
      .post(uriStr, postData, header)
      .then(response => {
        res = response.data;
      })
      .catch(error => {
        throw new ResponseError(error);
      });
    console.log(title + ' [ 終了 ]');
    return res;
  }

  /**
   * Postヘッダーを取得します。
   * @returns
   */
  private getPostHeader(authenticationKey: string): any {
    return {
      'Content-Type': 'application/json',
      'coop-cls-type-name': ApiConstant.CoopClsTypeName,
      'encrypted-coop-customer-code': authenticationKey
    };
  }

  /**
   * パラメーターを変換します。
   * @param params
   * @returns
   */
  private convertParameter(params: ApiParameter[]): string {
    if (params.length === 0) {
      return '';
    }
    let paramString = '?';
    params.forEach(param => {
      if (paramString !== '?') {
        paramString += '&';
      }
      paramString += param.key + '=' + encodeURIComponent(param.value);
    });
    return paramString;
  }
}
