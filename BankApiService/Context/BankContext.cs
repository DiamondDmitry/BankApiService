using BankApiService.Models;
using Microsoft.EntityFrameworkCore;

namespace BankApiService.Context
{
    public class BankContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=mydatabase.db");
        }
    }
}