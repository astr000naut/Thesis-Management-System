import Main from '@/views/Main.vue';
const FacultyList = () => import('@/views/manager/faculty/FacultyList.vue');
const StudentList = () => import('@/views/manager/student/StudentList.vue');
const TeacherList = () => import('@/views/manager/teacher/TeacherList.vue');
const ThesisList = () => import('@/views/manager/thesis/ThesisList.vue');
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