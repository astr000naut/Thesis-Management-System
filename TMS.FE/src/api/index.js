import { tenantApi } from "./tenant";
import { userApi } from "./user";
import { studentApi } from "./student";

const $api = {
    user: new userApi(),
    tenant: new tenantApi(),
    student: new studentApi()
};
export default $api;
