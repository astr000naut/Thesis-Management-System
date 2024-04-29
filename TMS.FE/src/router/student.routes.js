import Main from '@/views/Main.vue';
import MyThesisList from '@/views/student/mythesis/MyThesisList.vue';
const StudentInfo = () => import('@/views/student/personal-info/StudentInfo.vue');
export default {
    path: '/s',
    component: Main,
    children: [ 
        { 
            path: 'my-thesis',
            component: MyThesisList,
        },
        {
            path: 'search',
            component: MyThesisList,
        },
        {
            path: 'personal-info',
            component: StudentInfo,
        },
    ]
};