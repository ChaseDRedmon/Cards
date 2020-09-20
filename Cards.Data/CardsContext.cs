using System;
using System.Linq;
using System.Reflection;
using Cards.Data.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Cards.Data
{
    public class CardsContext : DbContext
    {
        public CardsContext(DbContextOptions<CardsContext> options) : base(options)
        {
        }

        // For building fakes during testing
        public CardsContext()
        {
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var onModelCreatingMethods = Assembly.GetExecutingAssembly()
                 .GetTypes()
                 .SelectMany(x => x.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                 .Where(x => !(x.GetCustomAttribute<OnModelCreatingAttribute>() is null));

            var relationships = modelBuilder
                .Model
                .GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys());

            foreach (var method in onModelCreatingMethods)
            {
                method.Invoke(null, new[] {modelBuilder});
            }
            
            foreach (var relationship in relationships)
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}