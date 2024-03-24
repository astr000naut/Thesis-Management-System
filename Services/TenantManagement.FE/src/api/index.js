import {userApi} from './user';

const baseUrl = "https://localhost:44328/api";
const $api = {
    user: userApi(baseUrl),
};
export default $api;
