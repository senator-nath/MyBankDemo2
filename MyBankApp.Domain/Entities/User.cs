using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string HashPassword { get; set; }
        public string AccountNo { get; set; }
        public string accountType { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailConfirmed { get; set; }
        public string Age { get; set; }

        public DateTime Dob { get; set; }
        public int LGAId { get; set; }
        public int StateId { get; set; }
        public string Bvn { get; set; }
        public bool HasBvn { get; set; }
        public string NIN { get; set; }
        public string Address { get; set; }
        public string LandMark { get; set; }
        public string Title { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }
        public DateTime LastLogin { get; set; }
        public int LoginAttempts { get; set; }
        public bool IsLocked { get; set; }
        public string Status { get; set; }
        public int GenderId { get; set; }
        public Gender Gender { get; set; }









    }
}
