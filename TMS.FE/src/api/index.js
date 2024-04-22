import { tenantApi } from "./tenant";
import { userApi } from "./user";
import { studentApi } from "./student";
import { teacherApi } from "./teacher";

const $api = {
    user: new userApi(),
    tenant: new tenantApi(),
    student: new studentApi(),
    teacher: new teacherApi()
};
export default $api;
