using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GCA.DAL
{
    public class GCADbContextFactory : IDesignTimeDbContextFactory<GCADbContext>
    {
        public GCADbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GCADbContext>();
            optionsBuilder.UseSqlite("Data Source=gca.db");

            return new GCADbContext(optionsBuilder.Options);
        }
    }
}
