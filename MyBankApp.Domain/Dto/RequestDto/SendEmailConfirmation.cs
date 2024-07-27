using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Domain.Dto.RequestDto
{
    public class SendEmailConfirmation
    {
        public string UserEmail { get; set; }
        public string Subject { get; set; }
        public string FirstName { get; set; }
        public string Token { get; set; }
    }
}
