using AutoMapper;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMS.BaseRepository;
using TMS.BaseRepository.Param;
using TMS.BusinessLayer.Param;

namespace TMS.BaseService
{
    public abstract class BaseService<TEntity, TEntityDto> : IBaseService<TEntityDto>
    {

        protected readonly IBaseRepository<TEntity> _baseRepository;
        private IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        public BaseService(IBaseRepository<TEntity> repository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _baseRepository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async virtual Task BeforeCreate() { }

        public async virtual Task AfterCreate(TEntityDto entityDto) { }

        public async virtual Task BeforeUpdate(TEntityDto t) { }

        public async virtual Task AfterUpdate(TEntityDto t) { }

        public async virtual Task AfterDelete(TEntityDto t) { }

        public async Task<bool?> CreateAsync(TEntityDto t)
        {
            try
            {
                await BeforeCreate();

                await _unitOfWork.OpenAsync();
                var entityInput = _mapper.Map<TEntity>(t);


                await _baseRepository.CreateAsync(entityInput);

                await AfterCreate(t);

                await _unitOfWork.CommitAsync();

                return true;

            } catch (Exception ex)
            {
                throw;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }    
        }

        public virtual async Task<byte[]> ExportExcelAsync(ExportParam exportParam)
        {
            try
            {
                await _unitOfWork.OpenAsync();

                var filterParam = new FilterParam()
                {
                    Skip = 0,
                    Take = 0,
                    CustomWhere = exportParam.CustomWhere,
                    KeySearch = exportParam.KeySearch,
                    FilterColumns = exportParam.FilterColumns
                };

                var filterResult = await FilterAsync(filterParam);
                var totalEntity = filterResult.Item2;
                var entityDtoList = filterResult.Item1;



                ExcelPackage excel = new ExcelPackage();

                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
                ExcelWorksheet ws = excel.Workbook.Worksheets[0];

                ws.Cells.Style.Font.Size = 13;
                ws.Cells.Style.Font.Name = "Times New Roman";
                ws.Rows.CustomHeight = false;
                ws.Cells.Style.Indent = 1;
                // Bật wrap text cho tất cả các cell
                ws.Cells.Style.WrapText = true;

                //Số lượng cột của header
                var countColHeader = exportParam.Columns.Count;

                // merge các column lại từ col 1 đến số col header để tạo heading
                ws.Cells[1, 1].Value = exportParam.TableHeading;
                ws.Cells[1, 1, 1, countColHeader].Merge = true;

                // in đậm heading
                ws.Cells[1, 1, 1, countColHeader].Style.Font.Bold = true;


                int colIndex = 1;
                int rowIndex = 3;

                // tạo các header 
                for (int i = 0; i < exportParam.Columns.Count; ++i)
                {
                    var item = exportParam.Columns[i];
                    var cell = ws.Cells[rowIndex, colIndex];

                    //set màu thành gray
                    var fill = cell.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);

                    //căn chỉnh các border
                    var border = cell.Style.Border;
                    border.Bottom.Style =
                        border.Top.Style =
                        border.Left.Style =
                        border.Right.Style = ExcelBorderStyle.Thin;

                    //Độ rộng của các cột
                    ws.Columns[i + 1].Width = item.Width;

                    // In đậm
                    ws.Cells[2, i + 1].Style.Font.Bold = true;

                    // align text
                    ws.Columns[i + 1].Style.HorizontalAlignment = item.Align switch
                    {
                        "left" => ExcelHorizontalAlignment.Left,
                        "right" => ExcelHorizontalAlignment.Right,
                        _ => ExcelHorizontalAlignment.Center,
                    };

                    cell.Value = item.Caption;
                    ++colIndex;
                }

                // căn giữa heading
                ws.Cells[1, 1, 1, countColHeader].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // đổ dữ liệu vào sheet
                foreach (var entityDto in entityDtoList)
                {
                    ++rowIndex;
                    colIndex = 1;
                    ws.Cells[rowIndex, colIndex++].Value = rowIndex - 2;

                    for (int i = 1; i < exportParam.Columns.Count; i++)
                    {
                        var colValue = entityDto?.GetType().GetProperty(exportParam.Columns[i].Name)?.GetValue(entityDto);
                        switch (exportParam.Columns[i].Type)
                        {
                            case "number":
                                long value = (long)colValue;
                                ws.Cells[rowIndex, colIndex++].Value = value.ToString("N0", new CultureInfo("vi-VN")); ;
                                break;
                            case "date":
                                DateTime date = (DateTime)colValue;
                                ws.Cells[rowIndex, colIndex++].Value = date.ToString("dd/MM/yyyy");
                                break;
                            default:
                                ws.Cells[rowIndex, colIndex++].Value = colValue;
                                break;

                        }

                    }
                }

                // Thêm border cho các cells
                if (totalEntity > 0)
                {
                    var cellBorder = ws.Cells[3, 1, 3 + totalEntity, exportParam.Columns.Count].Style.Border;
                    cellBorder.Bottom.Style =
                        cellBorder.Top.Style =
                        cellBorder.Left.Style =
                        cellBorder.Right.Style = ExcelBorderStyle.Thin;
                }

                byte[] bin = excel.GetAsByteArray();
                await _unitOfWork.CommitAsync();
                return bin;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }

        }

        public virtual async Task<int> DeleteMultipleAsync(List<string> entityIdList)
        {
            var result = await _baseRepository.DeleteMultipleAsync(entityIdList);
            return result;
        }

        public virtual async Task<int> DeleteAsync(string id)
        {
            try
            {
                await _unitOfWork.OpenAsync();
                var entity = await _baseRepository.GetByIdAsync(id);
                var entityDto = _mapper.Map<TEntityDto>(entity);
                var result = await _baseRepository.DeleteAsync(id);
                await AfterDelete(entityDto);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }
        }

        public async Task<(IEnumerable<TEntityDto>, int total)> FilterAsync(FilterParam filterParam)
        {
            try
            {
                await _unitOfWork.OpenAsync();
                var result = await _baseRepository.FilterAsync(filterParam);
                var (data, total) = result;
                var dataDto = _mapper.Map<IEnumerable<TEntityDto>>(data);
                await _unitOfWork.CommitAsync();

                return (dataDto, total);
            } catch
            {
                throw;
            } finally
            {
                await _unitOfWork.CloseAsync();
            }
        }

        public async Task<TEntityDto?> GetByIdAsync(string id)
        {
            var entity = await _baseRepository.GetByIdAsync(id);
            var response = _mapper.Map<TEntityDto>(entity);
            return response;
        }

        public async Task<bool> UpdateAsync(TEntityDto t)
        {
            try
            {
                await _unitOfWork.OpenAsync();
                await BeforeUpdate(t);
                var entity = _mapper.Map<TEntity>(t);
                var result = await _baseRepository.UpdateAsync(entity);
                await AfterUpdate(t);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                await _unitOfWork.CloseAsync();
            }

            
        }

        public virtual async Task<TEntityDto> GetNew()
        {
            var entity = Activator.CreateInstance<TEntity>();
            var entityDto = _mapper.Map<TEntityDto>(entity);
            entityDto.GetType().GetProperties()
                .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute))).ToList()
                .ForEach(p => p.SetValue(entityDto, Guid.NewGuid()));

            return entityDto;


        }
    }   
}
