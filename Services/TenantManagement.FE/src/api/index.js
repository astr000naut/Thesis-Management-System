import { tenantApi } from "./tenant";
import { userApi } from "./user";
import { fileServiceApi } from "./fileServiceApi";

const $api = {
    user: new userApi(),
    tenant: new tenantApi(),
    fileService: new fileServiceApi()
};
export default $api;
