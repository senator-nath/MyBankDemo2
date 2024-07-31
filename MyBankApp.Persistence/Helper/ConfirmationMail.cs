using MyBankApp.Domain.Dto.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Persistence.Helper
{
    public class ConfirmationMail
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ConfirmationMail(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task SendConfirmationEmail(EmailConfirmationRequestDto request)
        {
            var httpclient = _httpClientFactory.CreateClient();
            var emailModel = new
            {
                To = request.UserEmail,
                Subject = "Account Registration",
                Body = $"Hello {request.FirstName}, here is ur verification token: {request.Token}"

            };
            var sendEmail = await httpclient.PostAsJsonAsync("https://localhost:7168/api/EmailService", emailModel);


        }
        public async Task SendResetPasswordEmail(EmailConfirmationRequestDto request)
        {
            var httpclient = _httpClientFactory.CreateClient();
            var emailModel = new
            {
                To = request.UserEmail,
                Subject = "Reset Password",
                Body = $"Hello {request.FirstName}, here is your reset password token: {request.Token}"
            };
            var sendEmail = await httpclient.PostAsJsonAsync("https://localhost:7168/api/EmailService", emailModel);
        }
    }
}
