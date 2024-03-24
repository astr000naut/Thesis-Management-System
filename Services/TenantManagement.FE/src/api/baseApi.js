export class BaseApi {

    constructor(controller) {
        this.baseUrl = 'https://localhost:44328/api/' + controller;
    }
    
    filter(skip, take, keySearch) {
        return this.baseUrl + '?skip=' + skip + '&take=' + take + '&keySearch=' + keySearch;
    }
}