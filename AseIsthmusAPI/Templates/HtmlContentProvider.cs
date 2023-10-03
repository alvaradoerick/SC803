using AseIsthmusAPI.Data.DTOs;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Routing.Template;
using System.Diagnostics;
using System.Text;

namespace AseIsthmusAPI.Templates
{
    public class HtmlContentProvider
    {
        private readonly string _templatesFolderPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates");
        public string GeneratePasswordResetEmailContent(string newPassword)
        {
            string templateName = "forgot-password";
            string filePath = Path.Combine(_templatesFolderPath, $"{templateName}.txt");

            if (File.Exists(filePath))
            {
                string templateContent = File.ReadAllText(filePath);
                templateContent = templateContent.Replace("{newPassword}", newPassword);
                return templateContent;
            }

            return "El cuerpo del correo no fue encontrado";
        }

        public string ApprovalEmailContent()
        {
            string templateName = "user-activation"; 
            string filePath = Path.Combine(_templatesFolderPath, $"{templateName}.txt");

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            return "El cuerpo del correo no fue encontrado";
        }

        public string PagareEmailContent(string associateName)
        {
            string templateName = "pagare";
            string filePath = Path.Combine(_templatesFolderPath, $"{templateName}.txt");

            if (File.Exists(filePath))
            {
                string templateContent = File.ReadAllText(filePath);
                templateContent = templateContent.Replace("{associateName}", associateName);
                return templateContent;
            }

            return "El cuerpo del correo no fue encontrado";
        }

        public string LoanRejectedEmailContent(string associateName)
        {

            string templateName = "reject-loan";
            string filePath = Path.Combine(_templatesFolderPath, $"{templateName}.txt");

            if (File.Exists(filePath))
            {
                string templateContent = File.ReadAllText(filePath);
                templateContent = templateContent.Replace("{associateName}", associateName);
                return templateContent;
            }

            return "El cuerpo del correo no fue encontrado";
        }

        public string RequestAgreementInformationEmailContent(RequestAgreementInformationDto requestAgreementInformationDto)
        {
            string templateName = "agreement-information";
            string filePath = Path.Combine(_templatesFolderPath, $"{templateName}.txt");

            if (File.Exists(filePath))
            {
                string templateContent = File.ReadAllText(filePath);
                templateContent = templateContent.Replace("{userFullName}", requestAgreementInformationDto.FullName)
                                         .Replace("{employeeCode}", requestAgreementInformationDto.PersonId)
                                         .Replace("{agreementName}", requestAgreementInformationDto.Title)
                                         .Replace("{userEmail}", requestAgreementInformationDto.EmailAddress);
                return templateContent;
            }

            return "El cuerpo del correo no fue encontrado";
        }

        public string RegisterEmailContent()
        {
            string templateName = "user-registration";
            string filePath = Path.Combine(_templatesFolderPath, $"{templateName}.txt");

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            return "El cuerpo del correo no fue encontrado";
        }

        public string UpdatePasswordEmail()
        {
            string templateName = "reset-password";
            string filePath = Path.Combine(_templatesFolderPath, $"{templateName}.txt");

            if (File.Exists(filePath))
            {
                return File.ReadAllText(filePath);
            }

            return "El cuerpo del correo no fue encontrado";
        }

        public string RequestLoanReview(RequestLoanReviewEmailDto requestLoanReviewDto)
        {
            string templateName = "loan-review-request";
            string filePath = Path.Combine(_templatesFolderPath, $"{templateName}.txt");

            if (File.Exists(filePath))
            {
                string templateContent = File.ReadAllText(filePath);
                templateContent = templateContent.Replace("{userFullName}", requestLoanReviewDto.FullName)
                                                 .Replace("{creditRequestId}", requestLoanReviewDto.LoanRequestId);
                return templateContent;
            }

            return "El cuerpo del correo no fue encontrado";
        }

        public string RespondLoanReview(RespondLoanReviewEmailDto respondLoanReviewDto)
        {

            string templateName = "loan-review-respond";
            string filePath = Path.Combine(_templatesFolderPath, $"{templateName}.txt");

            if (File.Exists(filePath))
            {
                string templateContent = File.ReadAllText(filePath);
                templateContent = templateContent.Replace("{userFullName}", respondLoanReviewDto.FullName)
                                                 .Replace("{creditRequestId}", respondLoanReviewDto.LoanRequestId)
                                                 .Replace("{status}", respondLoanReviewDto.Status);
                return templateContent;
            }

            return "El cuerpo del correo no fue encontrado";
        }

        public string LoanRequestEmailContent(LoanEmailDto loanDto)
        {
            string templateName = "loan-requested";
            string filePath = Path.Combine(_templatesFolderPath, $"{templateName}.txt");

            if (File.Exists(filePath))
            {
                string templateContent = File.ReadAllText(filePath);
                templateContent = templateContent.Replace("{userFullName}", loanDto.FullName)
                                                 .Replace("{userEmail}", loanDto.EmailAddress);
                return templateContent;
            }

            return "El cuerpo del correo no fue encontrado";
        }
    }
}
