import { tenantApi } from "./tenant";
import { userApi } from "./user";

const $api = {
    user: new userApi(),
    tenant: new tenantApi(),
};
export default $api;
