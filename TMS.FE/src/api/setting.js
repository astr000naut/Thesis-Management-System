export class settingApi {
    constructor() {
        this.baseUrl = '/api/' + 'settings';
    }

    getSetting = () => this.baseUrl;

    updateSetting = (id) => this.baseUrl + '/' + id;
}