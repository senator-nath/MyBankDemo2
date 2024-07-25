using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Domain.Dto.ResponseDto
{
    public class UserResponseDetails
    {


        public string Message { get; set; }
        public UserResponseDto ResponseDetails { get; set; }
        public bool IsSuccess { get; set; }
        public string Token { get; set; }

    }
}
