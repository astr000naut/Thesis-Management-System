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
        name: "Danh sách giảng viên",
        icon: "ph:book-light",
        link: "/s/teacher-list",
    },
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
        name: "Thông tin cá nhân",
        link: "/s/personal-info",
        icon: "tdesign:file-setting",
    },
];

const teacher_item = [
    {
        name: "Danh sách khóa luận",
        icon: "pepicons-pop:grab-handle-circle",
        expand: false,
        children: [
            {
                name: "Xử lý yêu cầu",
                link: "/t/thesis-request",
                icon: "material-symbols:order-approve-sharp",
            },
            {
                name: "Đang hướng dẫn",
                link: "/t/thesis-guiding",
                icon: "fluent-mdl2:learning-tools",
            },
            {
                name: "Đang phản biện",
                link: "/t/thesis-reviewing",
                icon: "material-symbols:fact-check-outline"
            },
            {
                name: "Đã hoàn thành",
                link: "/t/thesis-completed",
                icon: "ant-design:file-done-outlined"
            },
        ],
    },
    {
        name: "Tra cứu",
        link: "/t/search",
        icon: "lucide:scan-search"
    },
];
