using MyBankApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Domain.Dto.RequestDto
{
    public class UserRequestDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string NIN { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public DateTime Dob { get; set; }
        public int LGAId { get; set; }
        public int StateId { get; set; }
        public string Bvn { get; set; }
        public bool HasBvn { get; set; }
        public string LandMark { get; set; }
        public string Title { get; set; }
        public string AccountType { get; set; }
        public int GenderId { get; set; }
        public Gender Gender { get; set; }
    }
}
