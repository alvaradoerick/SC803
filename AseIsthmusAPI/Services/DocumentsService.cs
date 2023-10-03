using AseIsthmusAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AseIsthmusAPI.Services
{
    public class DocumentsService
    {
        private readonly AseItshmusContext _context;
        public DocumentsService(AseItshmusContext context)
        {
            _context = context;
        }

        public async Task<string?> FetchGoogleLink(string documentType) {

            var link = await _context.Documents
         .Where(document => document.DocumentTypeLink == documentType)
         .Select(document => document.Link)
         .FirstOrDefaultAsync();

            if (link == null) {
                return null;

            }
            return link;
        }
    }
}
