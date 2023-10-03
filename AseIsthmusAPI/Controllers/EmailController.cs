using AseIsthmusAPI.Data.AseIsthmusModels;
using AseIsthmusAPI.Data.DTOs;
using AseIsthmusAPI.Services;
using AseIsthmusAPI.Templates;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace AseIsthmusAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly DocumentsService _documentService;
        private readonly UserService _userService;

        public EmailController(EmailService emailService, DocumentsService documentsService, UserService userService)
        {
            _emailService = emailService;
            _documentService = documentsService;
            _userService = userService; 

        }


        [HttpGet("forgot-password-email")]
        public  ActionResult GeneratePasswordEmail()
        {
            var token = HttpContext.Request.Headers["Email-Header"];

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    string tokenValue = token.ToString();
                    string[] tokenParts = tokenValue.Split('.');


                    if (tokenParts.Length >= 2)
                    {
                        string payloadBase64 = tokenParts[1];


                        string normalizedPayloadBase64 = payloadBase64.Replace('-', '+').Replace('_', '/');

                        int paddingLength = normalizedPayloadBase64.Length % 4;
                        if (paddingLength > 0)
                        {
                            normalizedPayloadBase64 += new string('=', 4 - paddingLength);
                        }

                        // Perform Base64 decoding
                        byte[] payloadBytes = Convert.FromBase64String(normalizedPayloadBase64);
                        string jsonPart = Encoding.UTF8.GetString(payloadBytes);

                        var json = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonPart);


                        if (json.TryGetValue("NewPassword", out var newPassword) && json.TryGetValue("EmailAddress", out var emailAddress))
                        {
                            string newPasswordStr = newPassword.ToString();
                            string emailAddressStr = emailAddress.ToString();

                            HtmlContentProvider emailTemplate = new HtmlContentProvider();
                            _emailService.SendEmail(emailTemplate.GeneratePasswordResetEmailContent(newPasswordStr), "Restablecimiento de contraseña", emailAddressStr);

                            return Ok();
                        }
                        else
                        {
                            return BadRequest(new { error = "Formato del token inválido." });
                        }
                    }
                    else
                    {
                        return BadRequest(new { error = "Formato del token inválido." });
                    }
                }
                catch (Exception ex)
                {
                    {
                        return BadRequest(new { error = "No se pudo enviar el correo.", ex });

                    }
                }
            
            }
            else { return BadRequest(new { error = "No se pudo enviar el correo." }); }
        }

        [Authorize]
        [HttpPost("user-activation-email")]
        public async Task<ActionResult> GenerateWelcomeEmail([FromBody] ActivateUserEmailDto activateUserDto)
        {
            try
            {
                HtmlContentProvider emailTemplate = new HtmlContentProvider();
                string welcome = "Bienvenida";
                string googleDriveLink = await _documentService.FetchGoogleLink(welcome);
                _emailService.SendEmail(emailTemplate.ApprovalEmailContent(), "Activación de usuario", activateUserDto.EmailAddress, googleDriveLink, "Informacion de la Asociacion");

                return Ok();
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(new { error = "No se pudo enviar el correo.", ex });

                }
            }
        }

        [Authorize]
        [HttpPost("approved-loan-email")]
        public async Task<ActionResult> GenerateApproveLoanEmail([FromBody] LoanEmailDto approveLoanEmailDto)
        {
            try
            {
                HtmlContentProvider emailTemplate = new HtmlContentProvider();
                string pagare = "Pagare";
                string googleDriveLink = await _documentService.FetchGoogleLink(pagare);

                _emailService.SendEmail(emailTemplate.PagareEmailContent(approveLoanEmailDto.FullName), "Estado de solicitud de préstamo", approveLoanEmailDto.EmailAddress, googleDriveLink, "Pagaré");

                return Ok();
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(new { error = "No se pudo enviar el correo.", ex });

                }
            }
        }

        [Authorize]
        [HttpPost("rejected-loan-email")]
        public  ActionResult GenerateRejectLoanEmail([FromBody] LoanEmailDto approveLoanEmailDto)
        {
            try
            {
                HtmlContentProvider emailTemplate = new HtmlContentProvider();

                 _emailService.SendEmail(emailTemplate.LoanRejectedEmailContent(approveLoanEmailDto.FullName), "Estado de solicitud de préstamo", approveLoanEmailDto.EmailAddress);

                return Ok();
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(new { error = "No se pudo enviar el correo.", ex });

                }
            }
        }

        [Authorize]
        [HttpPost("agreement-information-email")]
        public async Task<ActionResult> RequestAgreementInformationEmail([FromBody] RequestAgreementInformationDto requestAgreementInformationDto)
        {
            try
            {
                var adminData = await _userService.GetAdminData();
                if (adminData != null)
                {
                    HtmlContentProvider emailTemplate = new HtmlContentProvider();

                    _emailService.SendEmail(emailTemplate.RequestAgreementInformationEmailContent(requestAgreementInformationDto), " Interés en convenio", adminData.EmailAddress);

                    return Ok();
                }

                return BadRequest(new { error = "No se encontró ningún administrador en el sistema." });
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(new { error = "No se pudo enviar el correo.", ex });

                }
            }
        }

        [HttpPost("register-user-email")]
        public ActionResult RegisterUserEmail([FromBody] ActivateUserEmailDto activateUserDto)
        {
            try
            {
                HtmlContentProvider emailTemplate = new HtmlContentProvider();
                _emailService.SendEmail(emailTemplate.RegisterEmailContent(), "Registro exitoso", activateUserDto.EmailAddress);

                return Ok();
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(new { error = "No se pudo enviar el correo.", ex });

                }
            }
        }

        [Authorize]
        [HttpPost("reset-password-email")]
        public ActionResult ResetPasswordEmail([FromBody] ActivateUserEmailDto activateUserDto)
        {
            try
            {
                HtmlContentProvider emailTemplate = new HtmlContentProvider();

                _emailService.SendEmail(emailTemplate.UpdatePasswordEmail(), "Cambio de contraseña", activateUserDto.EmailAddress);

                return Ok();
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(new { error = "No se pudo enviar el correo.", ex });

                }
            }
        }

        [Authorize]
        [HttpPost("request-loan-review-email")]
        public async Task<ActionResult> LoanReviewRequestEmail([FromBody] RequestLoanReviewEmailDto requestLoanReviewDto)
        {
            try
            {
                var reviewerEmails = await _userService.GetLoanReviewersEmail();

                if (reviewerEmails != null) {
                    HtmlContentProvider emailTemplate = new HtmlContentProvider();

                    foreach (var email in reviewerEmails)
                    {
                        _emailService.SendEmail(emailTemplate.RequestLoanReview(requestLoanReviewDto), "Revisión de préstamo", email);
                    }

                    return Ok();
                
                }
                return BadRequest(new { error = "No se encontró ningún presidente o tesorero en el sistema." });
               
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(new { error = "No se pudo enviar el correo.", ex });

                }
            }
        }

        [Authorize]
        [HttpPost("respond-loan-review-email")]
        public async Task<ActionResult> LoanRespondRequestEmail([FromBody] RespondLoanReviewEmailDto respondLoanReviewDto)
        {
            try
            {
                    var adminData = await _userService.GetAdminData(); 

                    if (adminData != null) {

                    HtmlContentProvider emailTemplate = new HtmlContentProvider();

                    _emailService.SendEmail(emailTemplate.RespondLoanReview(respondLoanReviewDto), "Revisión completada del préstamo", adminData.EmailAddress);

                    return Ok();
                }

                return BadRequest(new { error = "No se encontró ningún administrador en el sistema." });

            }
            catch (Exception ex)
            {
                {
                    return BadRequest(new { error = "No se pudo enviar el correo.", ex });

                }
            }
        }
        [Authorize]
        [HttpPost("loan-requested-email")]
        public async Task<ActionResult> RequestedEmail([FromBody] LoanEmailDto loanDto)
        {
            try
            {
                var adminData = await _userService.GetAdminData();
                if (adminData != null)
                {
                    HtmlContentProvider emailTemplate = new HtmlContentProvider();

                    _emailService.SendEmail(emailTemplate.LoanRequestEmailContent(loanDto), " Solicitud de préstamo", adminData.EmailAddress);

                    return Ok();
                }

                return BadRequest(new { error = "No se encontró ningún administrador en el sistema." });
            }
            catch (Exception ex)
            {
                {
                    return BadRequest(new { error = "No se pudo enviar el correo.", ex });

                }
            }
        }

    }

}
