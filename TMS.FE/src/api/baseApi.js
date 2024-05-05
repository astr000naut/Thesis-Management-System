export class BaseApi {

    constructor(controller) {
        this.baseUrl = '/api/' + controller;
        this.workerUrl= '/api/worker/' + controller;
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

    delete() {
        return this.baseUrl + '/delete';
    }

    getById(id) {
        return this.baseUrl + '/' + id;
    }

    getNew() {
        return this.baseUrl + '/new';
    }
}