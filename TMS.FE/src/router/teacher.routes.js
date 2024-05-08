import Main from '@/views/Main.vue';
const ThesisRequestList = () => import('@/views/teacher/thesis-request/ThesisRequestList.vue');
const ThesisCompletedList = () => import('@/views/teacher/thesis-completed/ThesisCompletedList.vue');
const TeacherInfo = () => import('@/views/teacher/personal-info/TeacherInfo.vue');
const ThesisSearchList = () => import('@/views/search/ThesisSearchList.vue');
const UnderContruction = () => import('@/components/common/UnderContruction.vue');
const ThesisGuidingList = () => import('@/views/teacher/thesis-guiding/ThesisGuidingList.vue');

export default {
    path: '/t',
    component: Main,
    meta: {
        roles: ['Teacher']
    },
    children: [ 
        { 
            path: 'thesis-request',
            component: ThesisRequestList,
        },
        {
            path: 'thesis-guiding',
            component: ThesisGuidingList,
        },
        {
            path: 'thesis-reviewing',
            component: UnderContruction,
        },
        {
            path: 'thesis-completed',
            component: ThesisCompletedList,
        },
        {   
            path: 'search',
            component: ThesisSearchList,
        },
        {
            path: 'personal-info',
            component: TeacherInfo
        }
        
    ]
};