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
    public class AccountLimitRepository : GenericRepository<AccountLimit>, IAccountLimitRepository
    {
        private readonly MyBankAppDbContext _dbContext;
        public AccountLimitRepository(MyBankAppDbContext dbContext) : base(dbContext)
        {

        }
    }
}
