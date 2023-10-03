using Microsoft.EntityFrameworkCore;
using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace AseIsthmusAPI.Services
{
    public class AgreementService
    {

        private readonly AseItshmusContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        #region Conversion methods
        private AgreementDataDto ConvertToDto(Agreement modelData)
        {

            return new AgreementDataDto
            {
                AgreementId = modelData.AgreementId,
                Title = modelData.Title,
                Description = modelData.Description,
                Image = modelData.Image,
                CategoryAgreementId = modelData.CategoryAgreementId,
                CategoryName = modelData.CategoryAgreement.Description,
                IsActive = modelData.IsActive
            };
        }

        #endregion

        public AgreementService(AseItshmusContext context, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;  
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AgreementDataDto?> GetById(int id)
        {
            var agreement = await _context.Agreements.Where(a => a.AgreementId == id).Include(c=>c.CategoryAgreement).SingleOrDefaultAsync();
            if (agreement is null) {
                return null;
            }

            var dtoAgreement = ConvertToDto(agreement);
            return dtoAgreement;
        }

        public async Task<string> Create(ManageAgreementDto newAgreementDto)
        {
            var imageFile = newAgreementDto.Image;


            await UploadImage(imageFile, newAgreementDto.Title);

            var path = await RetriveImage(newAgreementDto.Title);

            var agreement = new Agreement
            {
                Title = newAgreementDto.Title,
                Description = newAgreementDto.Description,
                Image = path,
                CategoryAgreementId = newAgreementDto.CategoryAgreementId,
                IsActive = newAgreementDto.IsActive
            };

            _context.Agreements.Add(agreement);
            await _context.SaveChangesAsync();

            return "dusucess";
        }

        private string ModifyImageName(string imageTitle)
        {

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string[] words = imageTitle.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(words[i]))
                {
                    words[i] = textInfo.ToTitleCase(words[i]);
                }
            }

            return string.Join("-", words);
        }

        private void DeleteFilePath(string imageTitle)
        {
            string filePath = GetFilePath(imageTitle);
            string modifiedNamed = ModifyImageName(imageTitle);           
            string imagePath = filePath + "\\" + modifiedNamed + ".png";

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
        }
        private string GetFilePath(string imageTitle) {

            string modifiedName = ModifyImageName(imageTitle);

            return _webHostEnvironment.WebRootPath + "\\Images\\Agreements\\" + modifiedName;
        } 

        public async Task UploadImage(IFormFile formfile, string imageTitle)
    {

            string filePath = GetFilePath(imageTitle);
        string modifiedNamed = ModifyImageName(imageTitle);
            if (!Directory.Exists(filePath))
            {
               Directory.CreateDirectory(filePath);
            }
            string imagePath = filePath + "\\" + modifiedNamed + ".png";
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }
            using (FileStream stream = File.Create(imagePath))
            {
                await formfile.CopyToAsync(stream);
            }

        }

        private async Task<string> RetriveImage(string imageTitle)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string hostUrl = $"{request.Scheme}://{request.Host}{request.PathBase}";

            string imageUrl = string.Empty;
            string modifiedNamed = ModifyImageName(imageTitle);
            string filePath = GetFilePath(imageTitle);
            string imagePath = filePath + "\\" + modifiedNamed + ".png";
            if (File.Exists(imagePath))
            {
                imageUrl = hostUrl + "/Images/Agreements/"+modifiedNamed+"/"+modifiedNamed+".png";
            }

            return imageUrl;
        }
    
        public async Task<IEnumerable<AgreementDataDto>?> GetAll()
        {
            List<Agreement> agreements = await _context.Agreements.Include(u => u.CategoryAgreement).ToListAsync();
            var dtoList = new List<AgreementDataDto>();
            if (agreements is null)
            {
                return null;
            }
            foreach (var agreement in agreements)
            {
                var dto = ConvertToDto(agreement);
                dtoList.Add(dto);
            }

            return dtoList;
        }
        public async Task<IEnumerable<sp_GetAllActiveAgreements_Result>> GetAllActiveAgreements()
        {
                var results = await _context.Sp_GetAllActiveAgreements
                      .FromSqlRaw("EXEC sp_GetAllActiveAgreements")
                      .ToListAsync();

                return results.AsEnumerable();
        }
        public async Task Delete(int id)
        {
            var agreementToDelete = await _context.Agreements.Where(a => a.AgreementId == id).FirstOrDefaultAsync();

            if (agreementToDelete is not null)
            {
                _context.Agreements.Remove(agreementToDelete);
                await _context.SaveChangesAsync();
                DeleteFilePath(agreementToDelete.Title);
            }
        }
        public async Task<bool> Update(int id, ManageAgreementDto agreement)
        {

            var existingAgreement = await _context.Agreements.FindAsync(id);

            if (existingAgreement is not null) {

                DeleteFilePath(existingAgreement.Title);

                var imageFile = agreement.Image;

                await UploadImage(imageFile, agreement.Title);

            var path = await RetriveImage(agreement.Title);


                    existingAgreement.Title = agreement.Title;
                    existingAgreement.Description = agreement.Description;
                    existingAgreement.Image = path;
                    existingAgreement.CategoryAgreementId = agreement.CategoryAgreementId;
                    existingAgreement.IsActive = agreement.IsActive;
            
            await _context.SaveChangesAsync();
                return true;
            }

            else return false;

        }
    }
}
