using Microsoft.EntityFrameworkCore;
using MyBankApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBankApp.Persistence.Data
{
    public class MyBankAppDbContext : DbContext
    {
        public MyBankAppDbContext(DbContextOptions<MyBankAppDbContext> options) : base(options)
        {

        }


        public DbSet<User> Users { get; set; }
        public DbSet<LGA> Lga { get; set; }
        public DbSet<State> State { get; set; }
        public DbSet<AccountLimit> AccountLimits { get; set; }

        public DbSet<Gender> gender { get; set; }
    }
}
