using MyBankApp.Application.Configuration;
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
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly MyBankAppDbContext _dbContext;

        public IUserRepository user { get; }

        public IStateRepository state { get; }

        public ILGARepository lgaRepository { get; }

        public IAccountLimitRepository accountLimit { get; }

        public IGenderRepository gender { get; }
        public IVerificationTokenRepository VerificationTokens { get; }


        public UnitOfWork(MyBankAppDbContext dbContext, IVerificationTokenRepository verificationTokens)
        {
            _dbContext = dbContext;


            user = new UserRepository(_dbContext);
            gender = new GenderRepository(_dbContext);
            state = new StateRepository(_dbContext);
            lgaRepository = new LGARepository(_dbContext);
            accountLimit = new AccountLimitRepository(_dbContext);
            VerificationTokens = verificationTokens;

        }
        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.DisposeAsync();
        }
    }
}
