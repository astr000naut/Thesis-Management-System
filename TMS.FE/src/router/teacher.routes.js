import Main from '@/views/Main.vue';
import MyThesisList from '@/views/student/mythesis/MyThesisList.vue';
import ThesisGuidingList from '@/views/teacher/thesis-guiding/ThesisGuidingList.vue';
import ThesisReviewingList from '@/views/teacher/thesis-reviewing/ThesisReviewingList.vue';
import ThesisRequestList from '@/views/teacher/thesis-request/ThesisRequestList.vue';
import ThesisCompletedList from '@/views/teacher/thesis-completed/ThesisCompletedList.vue';
const TeacherInfo = () => import('@/views/teacher/personal-info/TeacherInfo.vue');
const ThesisSearchList = () => import('@/views/search/ThesisSearchList.vue');
const UnderContruction = () => import('@/components/common/UnderContruction.vue');
export default {
    path: '/t',
    component: Main,
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