export class fileServiceApi {
    baseUrl = 'http://localhost:3000';
    checkMinioConnection = () => this.baseUrl + '/check-connection';
}