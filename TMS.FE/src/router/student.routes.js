import Main from '@/views/Main.vue';
import MyThesisList from '@/views/student/mythesis/MyThesisList.vue';
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
            path: 'setting',
            component: MyThesisList,
        },
    ]
};