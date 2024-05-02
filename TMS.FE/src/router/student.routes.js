import Main from '@/views/Main.vue';
import MyThesisList from '@/views/student/mythesis/MyThesisList.vue';
const StudentInfo = () => import('@/views/student/personal-info/StudentInfo.vue');
const TeacherList = () => import('@/views/student/teacher-list/TeacherList.vue');
const ThesisSearchList = () => import('@/views/search/ThesisSearchList.vue');
export default {
    path: '/s',
    component: Main,
    meta: {
        roles: ['Student']
    },
    children: [ 
        { 
            path: 'teacher-list',
            component: TeacherList,
        },
        { 
            path: 'my-thesis',
            component: MyThesisList,
        },
        {
            path: 'search',
            component: ThesisSearchList,
        },
        {
            path: 'personal-info',
            component: StudentInfo,
        },
    ]
};