using System.ComponentModel.DataAnnotations;

namespace AseIsthmusAPI.Data.DTOs
{
    public class GetUserInformation
    {
        public string PersonId { get; set; } = null!;

        public string NumberId { get; set; } = null!;

        public string FullName { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName1 { get; set; } = null!;

        public string? LastName2 { get; set; }

        public string Nationality { get; set; } = null!;

        public DateTime RequestedDate { get; set; }

        public DateTime DateBirth { get; set; }

        public DateTime WorkStartDate { get; set; }

        public DateTime? EnrollmentDate { get; set; }

        public string PhoneNumber { get; set; } = null!;

        public string EmailAddress { get; set; } = null!;

        public string BankAccount { get; set; } = null!;

        public bool IsActive { get; set; }

        public int RoleId { get; set; }

        public string Address1 { get; set; } = null!;

        public string? Address2 { get; set; }

        public int DistrictId { get; set; }

        public string PostalCode { get; set; } = null!;

        public DateTime? ApprovedDate { get; set; }

        public string RoleDescription { get; set; }

    }

    public class InsertUser
    {
        public string PersonId { get; set; }

        public string NumberId { get; set; }

        public string FirstName { get; set; }

        public string LastName1 { get; set; }

        public string? LastName2 { get; set; }

        public string Nationality { get; set; }

        public DateTime DateBirth { get; set; }

        public DateTime WorkStartDate { get; set; }
        public DateTime RequestedDate { get; set; }

        public string PhoneNumber { get; set; }

        public string EmailAddress { get; set; }

        public string BankAccount { get; set; }

        public string Address1 { get; set; }

        public string? Address2 { get; set; }

        public int DistrictId { get; set; }

        public string PostalCode { get; set; }
    }

    public class UpdateUserByUser
    {
        [MaxLength(20, ErrorMessage = "El número de teléfono debe ser menor a 20 caracteres.")]
        public string PhoneNumber { get; set; }

        [MaxLength(25, ErrorMessage = "La cuenta bancaria debe ser menor a 25 caracteres.")]
        public string BankAccount { get; set; }

        [MaxLength(150, ErrorMessage = "La dirección 2 debe ser menor a 150 caracteres.")]
        public string Address1 { get; set; }

        [MaxLength(150, ErrorMessage = "La dirección 2 debe ser menor a 150 caracteres.")]
        public string? Address2 { get; set; }

        public int DistrictId { get; set; }

        [MaxLength(10, ErrorMessage = "El código postal debe ser menor a 10 caracteres.")]
        public string PostalCode { get; set; }

    }

    public class UpdateUserByAdmin
    {
        public string PersonId { get; set; }

        public string NumberId { get; set; }

        public string FirstName { get; set; }

        public string LastName1 { get; set; }

        public string? LastName2 { get; set; }

        public DateTime DateBirth { get; set; }

        public DateTime WorkStartDate { get; set; }

        public DateTime? EnrollmentDate { get; set; }

        public int RoleId { get; set; }

        public string EmailAddress { get; set; }
    }

}
