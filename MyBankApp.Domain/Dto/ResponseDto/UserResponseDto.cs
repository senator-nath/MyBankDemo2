using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Domain.Dto.ResponseDto
{
    public class UserResponseDto
    {
        public string LastLogin { get; set; }
        public string Token { get; set; }
        public string DailyLimitBalance { get; set; }
        public string AccountNumber { get; set; }
        public string UserName { get; set; }
        public string AccountName { get; set; }
        public string Title { get; set; }
        public int GenderId { get; set; }
        public string AccountType { get; set; }
        public string Bvn { get; set; }
        public string NIN { get; set; }
        public string Email { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public UserResponseDto ResponseDetails { get; set; }
        public bool IsSuccess { get; set; }

    }
}
