using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository.Param;

namespace TMS.BusinessLayer.Param
{
    public class ExportParam
    {
        // tên file trả về
        public string FileName { get; set; }

        // tiêu đề của bảng
        public string TableHeading { get; set; }

        // danh sách các cột hiển thị
        public List<Column> Columns { get; set; }

        public string KeySearch { get; set; }
        public IEnumerable<string>? FilterColumns { get; set; }

        public List<CustomWhere>? CustomWhere { get; set; }
    }

    public class Column
    {
        // Thuộc tính của trường thông tin
        public string Name { get; set; }

        // Tên cột
        public string Caption { get; set; }

        // độ rộng
        public int Width { get; set; }

        // align text
        public string Align { get; set; }

        // kiểu dữ liệu
        public string Type { get; set; }
    }
}
