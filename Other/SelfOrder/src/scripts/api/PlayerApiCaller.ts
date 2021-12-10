import ApiCallerBase from '@/scripts/api/ApiCallerBase'
import Player from '@/types/api/Player'

export default class PlayerApiCaller extends ApiCallerBase {
  public async getPlayer (facilityCode: string, accountNo: string): Promise<Player> {
    const params: any = {
      FacilityCD: facilityCode,
      AccountNo: accountNo
    }
    const player = new Player(new Date(), '', '', '')
    return super.post('player/get', params, player, 'プレーヤー情報取得')
  }
}
