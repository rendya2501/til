import ApiCallerBase from '@/scripts/api/ApiCallerBase'
import Master from '@/types/api/Master'

export default class MasterApiCaller extends ApiCallerBase {
  public async getMaster (facilityCode: string): Promise<Master> {
    const params: any = {
      FacilityCD: facilityCode
    }
    const master = new Master()
    return super.post('master/get', params, master, 'マスター情報取得')
  }
}
