import { BaseApi } from './baseApi';
export class userApi extends BaseApi {
    constructor() {
        super('users');
    }

    authenticate = this.baseUrl + '/authenticate';

    refreshToken = this.baseUrl + '/refresh-token'

    test = this.baseUrl + '/test';

    changePassword = this.baseUrl + '/change-password';
     
}