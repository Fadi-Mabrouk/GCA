using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GCA.BLL.Services;
using GCA.DAL;
using GCA.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GCA.Tests
{
    public class StockServiceTests
    {
        private GCADbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<GCADbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new GCADbContext(options);
        }

        [Fact]
        public async Task AddPart_ShouldAddPartToDatabase()
        {
            // Arrange
            var dbName = "AddPartDb";
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new StockService(context);
                var category = new Category { Name = "TestCat" };
                context.Categories.Add(category);
                await context.SaveChangesAsync();

                var part = new Part 
                { 
                    Name = "Test Part", 
                    CategoryId = category.Id,
                    UnitPrice = 10m,
                    Quantity = 5
                };

                // Act
                await service.AddPartAsync(part);
            }

            // Assert
            using (var context = GetInMemoryContext(dbName))
            {
                Assert.Equal(1, await context.Parts.CountAsync());
                var savedPart = await context.Parts.Include(p => p.Category).FirstAsync();
                Assert.Equal("Test Part", savedPart.Name);
                Assert.Equal("TestCat", savedPart.Category.Name);
                Assert.NotNull(savedPart.CreatedAt);
            }
        }

        [Fact]
        public async Task SearchParts_ShouldReturnMatchingParts()
        {
            // Arrange
            var dbName = "SearchDb";
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new StockService(context);
                var cat = new Category { Name = "Engine" };
                context.Categories.Add(cat);
                context.Parts.Add(new Part { Name = "Piston", Category = cat, UnitPrice = 10 });
                context.Parts.Add(new Part { Name = "Tire", Category = cat, UnitPrice = 50 });
                await context.SaveChangesAsync();

                // Act
                var results = await service.SearchPartsAsync("Piston");

                // Assert
                Assert.Single(results);
                Assert.Equal("Piston", results.First().Name);
            }
        }
    }
}
