using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DAL.Database
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            
            // Design-time connection string - LocalDB
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=TalkyDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true");
            
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
