using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Cards.Data
{
    public class CardsContextDesignFactory : IDesignTimeDbContextFactory<CardsContext>
    {
        /// <summary>
        /// Scaffold a fake database constructor for EF core migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public CardsContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CardsContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS;Database=OpenDND;Trusted_Connection=True;");
            return new CardsContext(optionsBuilder.Options);
        }
    }
}