using MyBankApp.Application.Contracts.Persistence;
using MyBankApp.Domain.Entities;
using MyBankApp.Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Persistence.Repository
{
    public class VerificationTokenRepository : GenericRepository<VerificationToken>, IVerificationTokenRepository
    {
        public VerificationTokenRepository(MyBankAppDbContext context) : base(context)
        {

        }
    }
}
