import Main from '@/views/Main.vue';
import FacultyList from '@/views/manager/faculty/FacultyList.vue';
import StudentList from '@/views/manager/student/StudentList.vue';
import ThesisList from '@/views/manager/thesis/ThesisList.vue';
export default {
    path: '/m',
    component: Main,
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
            path: 'thesis-list',
            component: ThesisList,
        }
    ]
};