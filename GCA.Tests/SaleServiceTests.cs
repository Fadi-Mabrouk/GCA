using System;
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
    public class SaleServiceTests
    {
        private GCADbContext GetInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<GCADbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            return new GCADbContext(options);
        }

        [Fact]
        public async Task CreateSale_ShouldDeductStock_AndAddSale()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString(); // Unique DB for isolation
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SaleService(context);

                // Seed Data
                var cat = new Category { Name = "Cat1" };
                context.Categories.Add(cat);
                var part = new Part { Name = "Part1", Category = cat, UnitPrice = 100, Quantity = 10 };
                context.Parts.Add(part);
                await context.SaveChangesAsync();

                var sale = new Sale
                {
                    Client = new Client { Name = "Client1", Phone = "123" },
                    LineItems = new List<SaleLineItem>
                    {
                        new SaleLineItem { PartId = part.Id, Quantity = 2 }
                    }
                };

                // Act
                await service.CreateSaleAsync(sale);
            }

            // Assert
            using (var context = GetInMemoryContext(dbName))
            {
                var savedPart = await context.Parts.FirstAsync();
                Assert.Equal(8, savedPart.Quantity); // 10 - 2

                var savedSale = await context.Sales.Include(s => s.LineItems).FirstAsync();
                Assert.NotNull(savedSale);
                Assert.Equal(200, savedSale.TotalAmount); // 2 * 100
                Assert.Single(savedSale.LineItems);
            }
        }

        [Fact]
        public async Task CreateSale_ShouldFail_WhenStockInsufficient()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SaleService(context);
                var cat = new Category { Name = "Cat1" };
                context.Categories.Add(cat);
                var part = new Part { Name = "Part1", Category = cat, UnitPrice = 100, Quantity = 5 }; // Only 5 available
                context.Parts.Add(part);
                await context.SaveChangesAsync();

                var sale = new Sale
                {
                    LineItems = new List<SaleLineItem>
                    {
                        new SaleLineItem { PartId = part.Id, Quantity = 6 } // Request 6
                    }
                };

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => service.CreateSaleAsync(sale));
            }
        }

        [Fact]
        public async Task CreateSale_ShouldFail_WhenPartNotFound()
        {
             // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SaleService(context);
                var sale = new Sale
                {
                    LineItems = new List<SaleLineItem>
                    {
                        // Some random ID that doesn't exist
                        new SaleLineItem { PartId = 999, Quantity = 1 } 
                    }
                };

                // Act & Assert
                await Assert.ThrowsAsync<Exception>(() => service.CreateSaleAsync(sale));
            }
        }

        //[Fact]
        //public async Task GetSalesHistory_ShouldReturnOrderedSales()
        //{
        //    // Arrange
        //    var dbName = Guid.NewGuid().ToString();
        //    using (var context = GetInMemoryContext(dbName))
        //    {
        //        var service = new SaleService(context);
                
        //        // Add two sales with different dates
        //        context.Sales.Add(new Sale { Date = DateTime.UtcNow.AddDays(-2), TotalAmount = 100 });
        //        context.Sales.Add(new Sale { Date = DateTime.UtcNow.AddDays(-1), TotalAmount = 200 });
        //        await context.SaveChangesAsync();

        //        // Act
        //        var result = await service.GetSalesHistoryAsync();

        //        // Assert
        //        Assert.Equal(2, result.Count());
        //        // Tartib
        //        Assert.Equal(200, result.First().TotalAmount);
        //    }
        //}

        [Fact]
        public async Task GetTotalRevenue_ShouldReturnSumOfTotals()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SaleService(context); // Fixed variable name
                
                context.Sales.Add(new Sale { TotalAmount = 50.5m });
                context.Sales.Add(new Sale { TotalAmount = 100.25m });
                await context.SaveChangesAsync();

                // Act
                var total = await service.GetTotalRevenueAsync();

                // Assert
                Assert.Equal(150.75m, total);
            }
        }

        [Fact]
        public async Task CreateSale_ShouldSnapshotUnitPrice_WhenNotProvided()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SaleService(context);
                var cat = new Category { Name = "Cat1" };
                context.Categories.Add(cat);
                var part = new Part { Name = "Part1", Category = cat, UnitPrice = 150m, Quantity = 10 };
                context.Parts.Add(part);
                await context.SaveChangesAsync();

                var sale = new Sale
                {
                    Client = new Client { Name = "Client1", Phone = "123" },
                    LineItems = new List<SaleLineItem>
                    {
                        new SaleLineItem { PartId = part.Id, Quantity = 1, UnitPriceSnapshot = 0 } // Snapshot 0
                    }
                };

                // Act
                await service.CreateSaleAsync(sale);
            }

            // Assert
            using (var context = GetInMemoryContext(dbName))
            {
                var savedSale = await context.Sales.Include(s => s.LineItems).FirstAsync();
                Assert.Equal(150m, savedSale.LineItems.First().UnitPriceSnapshot);
                Assert.Equal(150m, savedSale.TotalAmount);
            }
        }

        [Fact]
        public async Task CreateSale_ShouldMarkPartAsSold_WhenQuantityBecomesZero()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SaleService(context);
                var cat = new Category { Name = "Cat1" };
                context.Categories.Add(cat);
                var part = new Part { Name = "Part1", Category = cat, UnitPrice = 10, Quantity = 1, State = PartState.Available };
                context.Parts.Add(part);
                await context.SaveChangesAsync();

                var sale = new Sale
                {
                    Client = new Client { Name = "Client1", Phone = "123" },
                    LineItems = new List<SaleLineItem>
                    {
                        new SaleLineItem { PartId = part.Id, Quantity = 1 }
                    }
                };

                // Act
                await service.CreateSaleAsync(sale);
            }

            // Assert
            using (var context = GetInMemoryContext(dbName))
            {
                var savedPart = await context.Parts.FirstAsync();
                Assert.Equal(0, savedPart.Quantity);
                Assert.Equal(PartState.Sold, savedPart.State);
            }
        }

        [Fact]
        public async Task GetSalesHistory_ShouldReturnEmpty_WhenNoSales()
        {
             // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SaleService(context);
                
                // Act
                var result = await service.GetSalesHistoryAsync();

                // Assert
                Assert.Empty(result);
            }
        }

        [Fact]
        public async Task GetTotalRevenue_ShouldReturnZero_WhenNoSales()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SaleService(context);

                // Act
                var total = await service.GetTotalRevenueAsync();

                // Assert
                Assert.Equal(0m, total);
            }
        }
    }
}
