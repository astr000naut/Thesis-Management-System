export { manager_item, student_item, teacher_item };

const manager_item = [
    {
        name: "Quản lý Đào tạo",
        icon: "uil:university",
        expand: false,
        children: [
            {
                name: "Quản lý Khoa",
                link: "/m/faculty",
                icon: "clarity:organization-line",
            },
            {
                name: "Quản lý Sinh viên",
                link: "/m/student",
                icon: "ph:student-light",
            },
            {
                name: "Quản lý Giảng viên",
                link: "/m/teacher",
                icon: "mdi:teacher",
            },
        ],
    },
    {
        name: "Danh sách khóa luận",
        icon: "fluent:book-20-regular",
        link: "/m/thesis-list",
    },
    {
        name: "Quản lý đánh giá",
        icon: "carbon:chart-evaluation",
        expand: false,
        children: [
            {
                name: "Hội đồng đánh giá",
                link: "/m/evaluation/council",
                icon: "fluent-mdl2:group",
            },
            {
                name: "Phân công giảng viên",
                link: "/m/evaluation/assign-teacher",
                icon: "clarity:assign-user-solid",
            },
            {
                name: "Kết quả đánh giá",
                link: "/m/evaluation/result",
                icon: "carbon:result",
            },
        ],
    },
    {
        name: "Thiết lập",
        link: "/m/setting",
        icon: "tdesign:file-setting",
    },
    {
        name: "Tra cứu",
        link: "/m/search",
        icon: "icon-park-outline:find",
    },
];

const student_item = [
    {
        name: "Khóa luận của tôi",
        icon: "ph:book-light",
        link: "/s/my-thesis",
    },
    {
        name: "Tra cứu",
        link: "/s/search",
		icon: "lucide:scan-search"
    },
	{
        name: "Thiết lập",
        link: "/s/setting",
        icon: "tdesign:file-setting",
    },
];

const teacher_item = [
    {
        name: "Quản lý khóa luận",
        icon: "mi-sidebar-dashboard",
        expand: false,
        children: [
            {
                name: "Yêu cầu hương dẫn",
                link: "/t/thesis-request",
                icon: "mi-sidebar-dashboard",
            },
            {
                name: "Khóa luận đang hướng dẫn",
                link: "/t/thesis-guiding",
                icon: "mi-sidebar-dashboard",
            },
            {
                name: "Khóa luận đang phản biện",
                link: "/t/thesis-reviewing",
            },
            {
                name: "Khóa luận đã hoàn thành",
                link: "/t/thesis-completed",
            },
        ],
    },
    {
        name: "Tra cứu",
        link: "/t/search",
    },
];
