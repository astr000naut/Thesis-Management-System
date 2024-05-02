import Main from '@/views/Main.vue';
import FacultyList from '@/views/manager/faculty/FacultyList.vue';
import StudentList from '@/views/manager/student/StudentList.vue';
import TeacherList from '@/views/manager/teacher/TeacherList.vue';
import ThesisList from '@/views/manager/thesis/ThesisList.vue';
const Setting = () => import('@/views/manager/setting/Setting.vue');
const ThesisSearchList = () => import('@/views/search/ThesisSearchList.vue');
const UnderContruction = () => import('@/components/common/UnderContruction.vue');

export default {
    path: '/m',
    component: Main,
    meta: {
        roles: ['ADMIN']
    },
    children: [ 
        { 
            path: 'faculty',
            component: FacultyList,
        },
        {
            path: 'student',
            component: StudentList,
        },
        {
            path: 'teacher',
            component: TeacherList,
        },
        {
            path: 'thesis-list',
            component: ThesisList,
        },
        {
            path: 'setting',
            component: Setting,
        },
        {
            path: 'search',
            component: ThesisSearchList
        },
        {
            path: 'evaluation-council',
            component: UnderContruction
        },
        {
            path: 'evaluation-assign',
            component: UnderContruction
        },
        {
            path: 'evaluation-result',
            component: UnderContruction
        }
    ]
};