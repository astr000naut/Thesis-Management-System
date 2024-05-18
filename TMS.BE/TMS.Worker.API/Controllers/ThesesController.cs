using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Table;
using OfficeOpenXml;
using System.Security.AccessControl;
using TMS.BusinessLayer.DTO;
using TMS.BusinessLayer.Interface;
using TMS.BusinessLayer.Param;
using TMS.BusinessLayer.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TMS.Worker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThesesController: ControllerBase
    {
        private readonly IThesisService _thesisService;

        public ThesesController(IThesisService thesisService)
        {
            _thesisService = thesisService;
        }

        [Route("export")]
        [HttpPost]
        public async Task<IActionResult> ExportReceipData(ExportParam exportExcelParam)
        {
            byte[] excelFileBytes = await _thesisService.ExportExcelAsync(exportExcelParam);
            return File(excelFileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", exportExcelParam.FileName);
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            using ExcelPackage pack = new ExcelPackage();
            ExcelWorksheet ws = pack.Workbook.Worksheets.Add("test");
            ws.Cells["A1"].Value = "Hi";
            var data = pack.GetAsByteArray();
            return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "test");
        }

    }
}
