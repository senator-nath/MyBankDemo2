using Microsoft.EntityFrameworkCore;
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
    public class GenderRepository : GenericRepository<Gender>, IGenderRepository
    {
        //private readonly MyBankAppDbContext _dbContext;
        public GenderRepository(MyBankAppDbContext dbContext) : base(dbContext)
        {

        }
        public override async Task<IEnumerable<Gender>> GetAllAsync()
        {
            return await _dbContext.Set<Gender>().ToListAsync();
        }

    }
}
