using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly ExcelService _service;
        private readonly ContributionBalanceService _contributionBalanceService;
        private readonly LoanBalanceService _loanBalanceService;
        private readonly SavingsBalanceService _savingsBalanceService;

        public ExcelController(ExcelService service, ContributionBalanceService contributionBalanceService, LoanBalanceService loanBalanceService, SavingsBalanceService savingsBalanceService)
        {
            _service = service;
            _contributionBalanceService = contributionBalanceService;
            _loanBalanceService = loanBalanceService;
            _savingsBalanceService = savingsBalanceService;
        }

        #region upload file
        [HttpPost]
        public IActionResult ImportExcelData(IFormFile file)
        {
            var fileName = file.FileName.Trim().ToUpper();
            if (file == null || file.Length <= 0)
            {
                return BadRequest("No file uploaded.");
            }

            string filePath = "";

            try
            {
                filePath = Path.GetTempFileName();
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }

                if (fileName.Contains("CONTRIBUTIONS"))
                {
                    List<object> excelData = _service.ReadContributionsExcelData(filePath, out string errorFile);
                    foreach (ContributionBalance rowData in excelData)
                   {
                            _contributionBalanceService.Create(rowData);
                    }

                    return Ok();
                }
                else if (fileName.Contains("SAVINGS"))
                {
                    List<object> excelData = _service.ReadSavingsExcelData(filePath, out string errorFile);
                    foreach (SavingsBalance rowData in excelData)
                    {
                            _savingsBalanceService.Create(rowData);
                    }
                    return Ok();
                }

                else if (fileName.Contains("LOANS"))
                {
                    List<object> excelData = _service.ReadLoansExcelData(filePath, out string errorFile);
                    foreach (LoanBalance rowData in excelData)
                    {
                            _loanBalanceService.Create(rowData);
                    }
                    return Ok();
                }
                else {
                    return BadRequest(new { error="Por favor verifique el nombre del archivo."});
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            finally
            {
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
        }
        #endregion
    }
}
