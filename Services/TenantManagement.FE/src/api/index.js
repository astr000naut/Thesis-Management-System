import {userApi} from './user';

const baseUrl = "https://localhost:44381/";
const $api = {
    user: userApi(baseUrl),
};
export default $api;
