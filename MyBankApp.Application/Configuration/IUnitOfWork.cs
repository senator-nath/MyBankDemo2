using MyBankApp.Application.Contracts.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Application.Configuration
{
    public interface IUnitOfWork
    {
        IUserRepository user { get; }
        IStateRepository state { get; }
        ILGARepository lgaRepository { get; }
        IAccountLimitRepository accountLimit { get; }
        IGenderRepository gender { get; }
        IVerificationTokenRepository VerificationTokens { get; }


        Task<int> CompleteAsync();
    }
}
