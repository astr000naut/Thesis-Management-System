const commonVal = {
    dateFormat: {
      dmy: ["DD/MM/YYYY", "D/M/YYYY", "DD/M/YYYY", "D/MM/YYYY"],
      mdy: ["MM/DD/YYYY", "M/D/YYYY", "MM/D/YYYY", "M/DD/YYYY"],
      ymd: ["YYYY/MM/DD", "YYYY/MM/D", "YYYY/M/DD", "YYYY/M/D"],
    },
    defaultDateFormat: "dmy",
    sidebarItems: [
      {
        name: "Quản lý Tenant",
        link: "/tenant",
        icon: "mi-sidebar-dashboard",
      },
      {
        name: "Thiết lập chung",
        link: "/setting",
        icon: "mi-sidebar-news",
      },
    ],
  };
  export default commonVal;
  