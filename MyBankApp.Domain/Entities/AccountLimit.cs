using MyBankApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Domain.Entities
{
    public class AccountLimit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AccountNo { get; set; }
        public string AccountType { get; set; }
    }
}
