export class BaseApi {

    constructor(controller) {
        this.baseUrl = 'https://localhost:44328/api/' + controller;
    }
    
    filter() {
        return this.baseUrl + '/filter';
    }

    insert() {
        return this.baseUrl;
    }

    update(id) {
        return this.baseUrl + '/' + id;
    }

    delete(id) {
        return this.baseUrl + '/' + id;
    }

    getById(id) {
        return this.baseUrl + '/' + id;
    }
}