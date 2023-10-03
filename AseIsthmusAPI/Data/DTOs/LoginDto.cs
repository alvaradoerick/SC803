namespace AseIsthmusAPI.Data.DTOs
{
    public class LoginInDto
    {
        public string? EmailAddress { get; set; }

        public string? Pw { get; set; }

        public string? Name { get; set; }
        public int PersonId { get; set; }

        public int? RoleId { get; set; }

        public string? RoleDescription { get; set; }
    }

    /// <summary>
    /// return of the password automatically generated
    /// </summary>
    public class UpdatePasswordResponseDto
    {
        public string NewPassword { get; set; }
        public string EmailAddress { get; set; }
         
    }

    public class GeneratePasswordDto
    {
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }

        public string? PersonId { get; set; }
    }
}
