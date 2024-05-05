import { BaseApi } from './baseApi';
export class tenantApi extends BaseApi {
    constructor() {
        super('tenants');
    }   

    checkConnection = () => this.baseUrl + '/check-connection';

    activeTenant = () => this.baseUrl + '/active-tenant';
}