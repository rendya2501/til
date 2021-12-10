import FacilityTableInfo from '@/types/FacilityTableInfo'

export default class TableService {
  public async getFacilityTable (facilityNo: string, tableNo: string): Promise<FacilityTableInfo> {
    const ret = { facilityNo: facilityNo, tableNo: tableNo }
    return ret
  }
}
