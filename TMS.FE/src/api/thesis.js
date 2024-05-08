import { BaseApi } from './baseApi';
export class thesisApi extends BaseApi {
    constructor() {
        super('theses');
    }   

    fetchThesisGuidingList = (teacherId) => this.baseUrl + `/guiding-list/${teacherId}`;

    fetchThesisGuidedList = () => this.baseUrl + `/list-thesis-guided`;
}