using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.BaseRepository
{
    public interface IBaseRepository<T>
    {

        /// <summary>
        /// Tạo một Entity
        /// </summary>
        /// <param name="T"></param>
        /// <returns>Giá trị boolean biểu thị việc tạo entity thành công hay không</returns>
        Task<bool> CreateAsync(T t);


        /// <summary>
        /// Tạo một Entity
        /// </summary>
        /// <param name="tEntityInput"></param>
        /// <returns>Giá trị boolean biểu thị việc tạo entity thành công hay không</returns>
        Task<bool> UpdateAsync(T t);


        /// <summary>
        /// Xóa hàng loạt
        /// </summary>
        /// <param name="stringIdList">Các ID của entity cần xóa nối với nhau, ngăn cách bởi dấu phẩy</param>
        /// <returns>Số lượng entity đã bị xóa</returns>
        Task<int> DeleteMultipleAsync(List<string> listId);


        /// <summary>
        /// Lấy một Entity theo ID
        /// </summary>
        /// <param name="id">ID của engity</param>
        /// <returns>Entity tìm thấy nếu có</returns>
        Task<T?> GetByIdAsync(Guid id);


        /// <summary>
        /// Filter danh sách Entity
        /// </summary>
        /// <param name="entityFilter"></param>
        /// Author: DNT(29/05/2023)
        Task<IEnumerable<T>> FilterAsync(int? skip, int? take, string keySearch);
    }
}
