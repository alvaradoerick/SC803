using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Nodes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AseIsthmusAPI.Services
{
    public class BeneficiaryService
    {
        private readonly AseItshmusContext _context;
        public BeneficiaryService(AseItshmusContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets all the beneficiaries from the personId
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        public async Task<List<Beneficiary>?> GetBeneficiariesByPersonId(string personId)
        {
            var beneficiaries = await _context.Beneficiaries.Where(e => e.PersonId == personId).ToListAsync();
            if (beneficiaries == null)
            {
                return null;
            }
            return beneficiaries;         
        }

        public async Task<Beneficiary> Create(string id, BeneficiaryDtoIn beneficiary)
        {
            var newBeneficiary = new Beneficiary
            {
                PersonId = id,
                 BeneficiaryName = beneficiary.BeneficiaryName,
                 BeneficiaryNumberId = beneficiary.BeneficiaryNumberId,
                 BeneficiaryKin = beneficiary.BeneficiaryKin,
                 BeneficiaryPercentage = beneficiary.BeneficiaryPercentage,         
            };
            _context.Beneficiaries.Add(newBeneficiary);

            await _context.SaveChangesAsync();
            return newBeneficiary;

        }

        public async Task<string> DeleteUser(string id)
        {
            var existingBeneficiaries = await GetBeneficiariesByPersonId(id);
            if (existingBeneficiaries is not null)
            {
                foreach (var beneficiary in existingBeneficiaries)
                {
                    _context.Beneficiaries.Remove(beneficiary);
                }
                    await _context.SaveChangesAsync();
                    return string.Empty; 
            }
            return "No se encontró nigún beneficiario.";
        }
    }
}
