import { tenantApi } from "./tenant";
import { userApi } from "./user";
import { studentApi } from "./student";
import { teacherApi } from "./teacher";
import { facultyApi } from "./faculty";
import { thesisApi } from "./thesis";
import { settingApi } from "./setting";

const $api = {
    user: new userApi(),
    tenant: new tenantApi(),
    student: new studentApi(),
    teacher: new teacherApi(),
    faculty: new facultyApi(),
    thesis: new thesisApi(),
    setting: new settingApi(),
};
export default $api;
