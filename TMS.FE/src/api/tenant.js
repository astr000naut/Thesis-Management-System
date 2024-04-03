import { BaseApi } from './baseApi';
export class tenantApi extends BaseApi {
    constructor() {
        super('tenants');
    }   

    getTenantInfoLite = () => this.baseUrl + '/base-info';
}