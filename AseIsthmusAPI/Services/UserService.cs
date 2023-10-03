using AseIsthmusAPI.Data;
using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using DocumentFormat.OpenXml.Bibliography;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Diagnostics.Metrics;
using System.Drawing;


namespace AseIsthmusAPI.Services
{
    public class UserService 
    {
        private readonly AseItshmusContext _context;

        #region Conversion methods
        private GetUserInformation ConvertToDto(User modelData)
        {

            return new GetUserInformation
            {
                PersonId = modelData.PersonId,
                NumberId = modelData.NumberId,
                FullName= $"{modelData.FirstName} {modelData.LastName1} {modelData.LastName2}",
                FirstName = modelData.FirstName,
                LastName1 = modelData.LastName1,
                LastName2 = modelData.LastName2,
                Nationality = modelData.Nationality,
                DateBirth = modelData.DateBirth,
                WorkStartDate = modelData.WorkStartDate,
                EnrollmentDate = modelData.EnrollmentDate,
                PhoneNumber = modelData.PhoneNumber,
                EmailAddress = modelData.EmailAddress,
                BankAccount = modelData.BankAccount,
                IsActive = modelData.IsActive,
                RoleDescription = modelData.Role.RoleDescription,
                RoleId = modelData.RoleId,
                Address1 = modelData.Address1,
                Address2 = modelData.Address2,
                DistrictId = modelData.DistrictId,
                PostalCode = modelData.PostalCode,
                ApprovedDate = modelData.ApprovedDate,
                RequestedDate = modelData.RequestedDate
            };
        }

        #endregion

        public UserService(AseItshmusContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<GetUserInformation>?> GetAll()
        {
            List<User> users = await _context.Users.Include(u => u.Role).ToListAsync();
            var dtoList = new List<GetUserInformation>();
            if (users is null)
            {
                return null;
            }
            foreach (var user in users)
            {
                var dto = ConvertToDto(user);
                dtoList.Add(dto);
            }

            return dtoList;
        }

        /// <summary>
        /// gets user data by personId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<GetUserInformation?> GetDtoById(string id)
        {
            var user =  await _context.Users
                .Include(u => u.Role)
                .Where(a => a.PersonId == id)
                .OrderByDescending(a => a.RequestedDate)
                .SingleOrDefaultAsync();
            if (user is null) {
                return null;
            }
            var dtoUser =  ConvertToDto(user);
            return dtoUser;
        }


        public async Task<User?> GetAdminData()
        {
            return await _context.Users
                .Where(a => a.RoleId == 1)
                .SingleOrDefaultAsync();     
        }

        public async Task<List<string>?> GetLoanReviewersEmail()
        {
            var reviewerRoles = new List<int> {2, 3};

            return await _context.Users
                .Where(a => reviewerRoles.Contains(a.RoleId))
                .Select(a => a.EmailAddress)
                .ToListAsync();
        }


        public async Task<User?> GetModelById(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return null;
            }
            return user;
        }

        /// <summary>
        /// find user by employee code (personId)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> GetById(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user is null)
            {
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// finds user by identification number (number Id)
        /// </summary>
        /// <param name="numberId"></param>
        /// <returns></returns>
        public async Task<bool> GetByNumberId(string numberId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.NumberId == numberId);
            if (user is null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// finds user by email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<bool> GetByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(e => e.EmailAddress == email);
            if (user is null)
            {
                return false;
            }
            return true;
        }

        public async Task<User?> Create(InsertUser user)
        {

            var newUser = new User
            {
                PersonId = user.PersonId,
                NumberId = user.NumberId,
                FirstName = user.FirstName,
                LastName1 = user.LastName1,
                LastName2 = user.LastName2,
                Nationality = user.Nationality,
                DateBirth = user.DateBirth,
                WorkStartDate = user.WorkStartDate,
                PhoneNumber = user.PhoneNumber,
                EmailAddress = user.EmailAddress,
                BankAccount = user.BankAccount,
                IsActive = false,
                RoleId = 4,
                Address1 = user.Address1,
                Address2 = user.Address2,
                DistrictId = user.DistrictId,
                PostalCode = user.PostalCode,
                RequestedDate = DateTime.Now
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return newUser;
        }

        /// <summary>
        /// This method updates the profile by the admin; however it does not activate or inactivate the user
        /// </summary>
        /// <param name="userByAdmin"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserByAdmin(string oldId, UpdateUserByAdmin userByAdmin)
        {
            try
            {
                var existingClient = await GetModelById(oldId);

                if (existingClient is not null)
                {

                    var originalNumberId = existingClient.NumberId;
                    var originalFirstName = existingClient.FirstName;
                    var originalLastName1 = existingClient.LastName1;
                    var originalLastName2 = existingClient.LastName2;
                    var originalDateBirth = existingClient.DateBirth;
                    var originalWorkStartDate = existingClient.WorkStartDate;
                    var originalEnrollmentDate = existingClient.EnrollmentDate;
                    var originalEmailAddress = existingClient.EmailAddress;
                    var originalRoleId = existingClient.RoleId;


                    existingClient.NumberId = userByAdmin.NumberId;
                    existingClient.FirstName = userByAdmin.FirstName;
                    existingClient.LastName1 = userByAdmin.LastName1;
                    existingClient.LastName2 = userByAdmin.LastName2;
                    existingClient.DateBirth = userByAdmin.DateBirth;
                    existingClient.WorkStartDate = userByAdmin.WorkStartDate;
                    existingClient.EnrollmentDate = userByAdmin.EnrollmentDate;
                    existingClient.EmailAddress = userByAdmin.EmailAddress;
                    existingClient.RoleId = userByAdmin.RoleId;

                    await _context.SaveChangesAsync();

                    if (oldId != userByAdmin.PersonId)
                    {
                        var updatePersonIdResult = await UpdatePersonId(oldId, userByAdmin.PersonId);

                        if (updatePersonIdResult != null)
                        {
  
                            existingClient.NumberId = originalNumberId;
                            existingClient.FirstName = originalFirstName;
                            existingClient.LastName1 = originalLastName1;
                            existingClient.LastName2 = originalLastName2;
                            existingClient.DateBirth = originalDateBirth;
                             existingClient.WorkStartDate = originalWorkStartDate;
                            existingClient.EnrollmentDate = originalEnrollmentDate;
                            existingClient.EmailAddress = originalEmailAddress;
                            existingClient.RoleId = originalRoleId;

                            await _context.SaveChangesAsync(); 
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// allows user to update their own profile
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> UpdateUserByUser(string id, UpdateUserByUser user)
        {
            var existingClient = await GetModelById(id);
            if (existingClient != null)
            {
                existingClient.PhoneNumber = user.PhoneNumber;
                existingClient.BankAccount = user.BankAccount;
                existingClient.Address1 = user.Address1;
                existingClient.Address2 = user.Address2;
                existingClient.DistrictId = user.DistrictId;
                existingClient.PostalCode = user.PostalCode;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }


        public async Task<string?> DeleteUser(string id)
        {
            var existingClient = await GetModelById(id);
            if (existingClient is not null)
            {
                try
                {
                    _context.Users.Remove(existingClient);
                    await _context.SaveChangesAsync();
                    return null;
                }
                catch (DbUpdateException ex)
                {
                    var errorMessages = new List<string>();
                    if (ex.InnerException is SqlException sqlException)
                    {
                        foreach (SqlError error in sqlException.Errors)
                        {
                            if (error.Number == 50000) 
                            {
                                errorMessages.Add(error.Message);
                            }
                        }
                    }
                    return string.Join(Environment.NewLine, errorMessages);
                }
            }
            return "No se encontró ningun usuario.";
        }

        /// <summary>
        /// vaidation if user exists based on different checks
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string?> DuplicateAccount(string userId = null, string newUser = null, string userNumber = null, string userEmail = null)
        {
            string result = "Valid";

            if (userId != null)
            {

                var userExistById = await GetById(newUser);

                var userExistByNumberId = await GetByNumberId(userNumber);

                var userExistByEmail = await GetByEmail(userEmail);

                if (userExistById)
                {
                    var userWithSamePersonId = await _context.Users.Where(a => a.PersonId == newUser).FirstOrDefaultAsync();
                    if (userWithSamePersonId != null && userWithSamePersonId.PersonId != userId)
                    {
                        result = "El usuario con el código de empleado ingresado ya existe en el sistema. Contacte al administrador.";
                    }

                }

                 if (userExistByNumberId)
                {

                    var userWithSameNumberId = await _context.Users.Where(a => a.NumberId == userNumber).FirstOrDefaultAsync();
                    if (userWithSameNumberId != null && userWithSameNumberId.PersonId != userId)
                    {
                        result = "El usuario con la identificación ingresada ya existe en el sistema. Contacte al administrador.";
                    }
                }
                 if (userExistByEmail)
                {
                    var userWithSameEmail = await _context.Users.Where(a => a.EmailAddress == userEmail).FirstOrDefaultAsync();
                    if (userWithSameEmail != null && userWithSameEmail.PersonId != userId)
                    {
                        result = "El correo ingresado ya está asignado a otro usuario en el sistema. Contacte al administrador.";
                    }
                }

            }

            else {

                var userExistById = await GetById(newUser);

                var userExistByNumberId = await GetByNumberId(userNumber);

                var userExistByEmail = await GetByEmail(userEmail);

                if (userExistById)
                {
                        result = "El usuario con el código de empleado ingresado ya existe en el sistema. Contacte al administrador.";
                }

                else if (userExistByNumberId)
                {
                        result = "El usuario con la identificación ingresada ya existe en el sistema. Contacte al administrador.";
                }
                else if(userExistByEmail)
                {
                        result = "El correo ingresado ya está asignado a otro usuario en el sistema. Contacte al administrador.";
                }
            }

            

            return result;
        }

        /// <summary>
        /// Updates the status of the user either inactivating it or activating it
        /// </summary>
        /// <returns></returns>
        public async Task<string?> ManageUserStatus(string id) {

           var date = DateTime.Now.Date;
            var existingClient = await GetModelById(id);
            if (existingClient is not null && existingClient.IsActive)
            {
                existingClient.IsActive = false;
                existingClient.ApprovedDate = date;
                await _context.SaveChangesAsync();
                return "Deactivated";
            }
            else if (existingClient is not null && !existingClient.IsActive)
            {
                existingClient.IsActive = true;
                existingClient.ApprovedDate = date;
                await _context.SaveChangesAsync();
                return "Activated";
            }
            else return null;
        }

        public async Task<string?> UpdatePersonId(string oldId, string newId)
        {
            try
            {
                var oldPersonIdParameter = new SqlParameter("@oldPersonId", SqlDbType.NVarChar, 12)
                {
                    Value = oldId
                };

                var newPersonIdParameter = new SqlParameter("@newPersonId", SqlDbType.NVarChar, 12)
                {
                    Value = newId
                };

                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC sp_UpdatePrimaryKeyAndCascade @oldPersonId, @newPersonId",
                    oldPersonIdParameter, newPersonIdParameter);

                return null;
            }
            catch (DbUpdateException ex)
            {
                var errorMessages = new List<string>();
                if (ex.InnerException is SqlException sqlException)
                {
                    foreach (SqlError error in sqlException.Errors)
                    {
                        if (error.Number == 50000)
                        {
                            errorMessages.Add(error.Message);
                        }
                    }
                }
                return string.Join(Environment.NewLine, errorMessages);
            } 
        }
        
        

    }
}
