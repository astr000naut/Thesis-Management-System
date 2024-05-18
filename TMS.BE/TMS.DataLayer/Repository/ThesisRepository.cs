using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.DataLayer.Entity;
using TMS.DataLayer.Enum;
using TMS.DataLayer.Interface;
using TMS.DataLayer.Param;

namespace TMS.DataLayer.Repository
{
    public class ThesisRepository : BaseRepository<Thesis>, IThesisRepository
    {

        public ThesisRepository(IUnitOfWork unitOfWork): base(unitOfWork)
        {}

        public async Task<string> GetNewThesisCode()
        {
            var year = DateTime.Now.Year;
            var random = new Random();
            var newCode = $"KLTN_{year}_{random.Next(1, 10000):D4}";
            var query = "SELECT COUNT(*) FROM theses WHERE ThesisCode = @NewCode";
            var parameters = new { NewCode = newCode };
            var count = await _unitOfWork.Connection.ExecuteScalarAsync<int>(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            if (count > 0)
            {
                return await GetNewThesisCode();
            }
            return newCode;
  
        }

        public override async Task<Thesis> GetDetailData(Thesis thesis)
        {
            // Get all co-teachers of thesis
            var query = "SELECT * FROM view_thesis_co_teachers WHERE ThesisId = @ThesisId";
            var parameters = new { ThesisId = thesis.ThesisId };
            var coTeachers = await _unitOfWork.Connection.QueryAsync<CoTeacher>(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            thesis.CoTeachers = coTeachers.ToList();
            return thesis;
        }

        public async Task<bool> AddThesisCoTeacher(Guid thesisId, Guid teacherId)
        {
            var Id = Guid.NewGuid().ToString();
            var query = "INSERT INTO thesis_co_teachers (Id, ThesisId, TeacherId) VALUES (@Id, @ThesisId, @TeacherId)";
            var parameters = new { Id = Id, ThesisId = thesisId, TeacherId = teacherId };
            var result = await _unitOfWork.Connection.ExecuteAsync(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            return result > 0;
        }

        public async Task<bool> RemoveThesisCoTeacher(Guid thesisId, Guid teacherId)
        {
            var query = "DELETE FROM thesis_co_teachers WHERE ThesisId = @ThesisId AND TeacherId = @TeacherId";
            var parameters = new { ThesisId = thesisId, TeacherId = teacherId };
            var result = await _unitOfWork.Connection.ExecuteAsync(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            return result > 0;
        }

        public async Task<bool> RemoveAllThesisCoTeacher(Guid thesisId)
        {
            var query = "DELETE FROM thesis_co_teachers WHERE ThesisId = @ThesisId";
            var parameters = new { ThesisId = thesisId };
            var result = await _unitOfWork.Connection.ExecuteAsync(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            return result > 0;
        }

        public async Task<List<Thesis>> GetListGuiding(string teacherId)
        {
            var query = $"SELECT * FROM view_theses WHERE TeacherId = @TeacherId AND (Status = {(int)ThesisStatus.ApprovedGuiding} OR Status = {(int)ThesisStatus.ApprovedTitle})";
            var parameters = new { TeacherId = teacherId };
            var result = await _unitOfWork.Connection.QueryAsync<Thesis>(sql: query, param: parameters, transaction: _unitOfWork.Transaction);
            return result.ToList();

        }

        public async Task<List<Thesis>> GetListCoGuiding(string teacherId)
        {
            // get list thesis id that teacher is co-guiding from thesis_co_teachers table
            var query = "SELECT ThesisId FROM thesis_co_teachers WHERE TeacherId = @TeacherId";
            var parameters = new { TeacherId = teacherId };
            var thesisIds = await _unitOfWork.Connection.QueryAsync<Guid>(sql: query, param: parameters, transaction: _unitOfWork.Transaction);

            if (thesisIds.Count() == 0)
            {
                return new List<Thesis>();
            }
           
            //create query to get list thesis that have thesis id in thesisIds
            var thesisIdList = thesisIds.ToList();
            var query2 = $"SELECT * FROM view_theses WHERE ThesisId IN @ThesisIdList AND (Status = {(int)ThesisStatus.ApprovedGuiding} OR Status = {(int)ThesisStatus.ApprovedTitle})";
            var parameters2 = new { ThesisIdList = thesisIdList };
            var queryResult = await _unitOfWork.Connection.QueryAsync<Thesis>(sql: query2, param: parameters2, transaction: _unitOfWork.Transaction);

            return queryResult.ToList();
        }

        public async Task<(List<Thesis>, int)> GetListCoGuideFinished(TeacherThesisFilterParam param)
        {
            // lấy danh sách id khóa luận mà giáo viên đồng hướng dẫn

            var query = "SELECT ThesisId FROM thesis_co_teachers WHERE TeacherId = @TeacherId";
            var parameters = new { TeacherId = param.TeacherId };
            var qResult = await _unitOfWork.Connection.QueryAsync<Guid>(sql: query, param: parameters, transaction: _unitOfWork.Transaction);

            var coGuideThesisIdList = qResult.ToList();

            if (coGuideThesisIdList.Count == 0)
            {
                return (new List<Thesis>(), 0);
            }

            // lấy danh sách khóa luận đồng hướng dẫn ở trạng thái hoàn thành

            string searchQuery = "";
            if (!string.IsNullOrEmpty(param.KeySearch))
            {
                searchQuery = " AND (ThesisName LIKE @Search OR StudentName LIKE @Search OR StudentCode LIKE @Search)";
            }
            else
            {
                searchQuery = " AND 1 = 1 ";
            }
            
            var query2 = $"SELECT * FROM view_theses WHERE ThesisId IN @ThesisIdList " +
                $"AND Status = {(int)ThesisStatus.Finished} " +
                searchQuery + " ORDER BY CreatedDate DESC LIMIT @Skip, @Take";
            var parameters2 = new { ThesisIdList = coGuideThesisIdList, Skip = param.Skip, Take = param.Take, Search = $"%{param.KeySearch}%" };
            var qResult2 = await _unitOfWork.Connection.QueryAsync<Thesis>(sql: query2, param: parameters2, transaction: _unitOfWork.Transaction);

            // lấy tổng số khóa luận đồng hướng dẫn và đã hoàn thành
            var query3 = $"SELECT COUNT(*) FROM view_theses WHERE ThesisId IN @ThesisIdList " +
                $"AND Status = {(int)ThesisStatus.Finished} " +
                searchQuery;

            var total = await _unitOfWork.Connection.ExecuteScalarAsync<int>(sql: query3, param: parameters2, transaction: _unitOfWork.Transaction);

            return (qResult2.ToList(), total);
        }

        public async Task<(List<Thesis>, int)> GetListGuideFinished(TeacherThesisFilterParam param)
        {
            // lấy danh sách khóa luận mà giáo viên hướng dẫn ở trạng thái hoàn thành có phân trang
            string searchQuery = "";
            if (!string.IsNullOrEmpty(param.KeySearch))
            {
                searchQuery = " AND (ThesisName LIKE @Search OR StudentName LIKE @Search OR StudentCode LIKE @Search)";
            }
            else
            {
                searchQuery = " AND 1 = 1 ";
            }

            var query = $"SELECT * FROM view_theses WHERE TeacherId = @TeacherId " +
                $"AND Status = {(int)ThesisStatus.Finished} " +
                searchQuery + " ORDER BY CreatedDate DESC LIMIT @Skip, @Take";

            var parameters = new { TeacherId = param.TeacherId, Skip = param.Skip, Take = param.Take, Search = $"%{param.KeySearch}%" };

            var qResult = await _unitOfWork.Connection.QueryAsync<Thesis>(sql: query, param: parameters, transaction: _unitOfWork.Transaction);

            // lấy tổng số khóa luận mà giáo viên hướng dẫn và đã hoàn thành

            var query2 = $"SELECT COUNT(*) FROM view_theses WHERE TeacherId = @TeacherId " +
                $"AND Status = {(int)ThesisStatus.Finished} " +
                searchQuery;

            var total = await _unitOfWork.Connection.ExecuteScalarAsync<int>(sql: query2, param: parameters, transaction: _unitOfWork.Transaction);

            return (qResult.ToList(), total);
        }


        // Filter các khóa luận mà teacher đã hướng dẫn hoặc đồng hướng dẫn ở trạng thái hoàn thành
        public async Task<(List<Thesis>, int)> GetListThesisGuided(TeacherThesisFilterParam param)
        {
            (List<Thesis> lst1, int total1) = await GetListGuideFinished(param);
            (List<Thesis> lst2, int total2) = await GetListCoGuideFinished(param);
            var total = total1 + total2;
            var lst = lst1.Concat(lst2).ToList();
            return (lst, total);
        }
    }
}
