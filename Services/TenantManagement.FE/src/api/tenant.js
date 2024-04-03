import { BaseApi } from './baseApi';
export class tenantApi extends BaseApi {
    constructor() {
        super('tenants');
    }   

    checkDBConnection = () => this.baseUrl + '/check-db-connection';

    activeTenant = () => this.baseUrl + '/active-tenant';
}