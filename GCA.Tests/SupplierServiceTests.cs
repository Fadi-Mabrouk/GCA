using System;
using System.Linq;
using System.Threading.Tasks;
using GCA.BLL.Services;
using GCA.DAL;
using GCA.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace GCA.Tests
{
    public class SupplierServiceTests
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
        public async Task AddSupplier_ShouldAddSupplierToDatabase()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SupplierService(context);
                var supplier = new Supplier { Name = "Supplier1", ContactInfo = "123456789" };

                // Act
                await service.AddSupplierAsync(supplier);
            }

            // Assert
            using (var context = GetInMemoryContext(dbName))
            {
                Assert.Equal(1, await context.Suppliers.CountAsync());
                var saved = await context.Suppliers.FirstAsync();
                Assert.Equal("Supplier1", saved.Name);
                Assert.Equal("123456789", saved.ContactInfo);
            }
        }

        [Fact]
        public async Task GetAllSuppliers_ShouldReturnAllSuppliers()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SupplierService(context);
                context.Suppliers.Add(new Supplier { Name = "S1" });
                context.Suppliers.Add(new Supplier { Name = "S2" });
                await context.SaveChangesAsync();

                // Act
                var result = await service.GetAllSuppliersAsync();

                // Assert
                Assert.Equal(2, result.Count());
                Assert.Contains(result, s => s.Name == "S1");
                Assert.Contains(result, s => s.Name == "S2");
            }
        }

        [Fact]
        public async Task UpdateSupplier_ShouldUpdateExistingSupplier()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            int supplierId;
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SupplierService(context);
                var supplier = new Supplier { Name = "OriginalName", ContactInfo = "000" };
                context.Suppliers.Add(supplier);
                await context.SaveChangesAsync();
                supplierId = supplier.Id;

                // Act
                supplier.Name = "UpdatedName";
                supplier.ContactInfo = "111";
                await service.UpdateSupplierAsync(supplier);
            }

            // Assert
            using (var context = GetInMemoryContext(dbName))
            {
                var updated = await context.Suppliers.FindAsync(supplierId);
                Assert.NotNull(updated);
                Assert.Equal("UpdatedName", updated.Name);
                Assert.Equal("111", updated.ContactInfo);
            }
        }

        [Fact]
        public async Task DeleteSupplier_ShouldRemoveSupplier()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            int supplierId;
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SupplierService(context);
                var supplier = new Supplier { Name = "To Delete" };
                context.Suppliers.Add(supplier);
                await context.SaveChangesAsync();
                supplierId = supplier.Id;

                // Act
                await service.DeleteSupplierAsync(supplierId);
            }

            // Assert
            using (var context = GetInMemoryContext(dbName))
            {
                Assert.Equal(0, await context.Suppliers.CountAsync());
            }
        }

        [Fact]
        public async Task UpdateSupplier_ShouldDoNothing_WhenSupplierNotFound()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SupplierService(context);
                var supplier = new Supplier { Id = 999, Name = "NonExistent" };

                // Act
                // Should not throw
                await service.UpdateSupplierAsync(supplier);
            }
        }

        [Fact]
        public async Task DeleteSupplier_ShouldDoNothing_WhenSupplierNotFound()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SupplierService(context);
                
                // Act
                // Should not throw
                await service.DeleteSupplierAsync(999);
            }
        }

        [Fact]
        public async Task GetAllSuppliers_ShouldReturnEmpty_WhenNoSuppliers()
        {
             // Arrange
            var dbName = Guid.NewGuid().ToString();
            using (var context = GetInMemoryContext(dbName))
            {
                var service = new SupplierService(context);
                
                // Act
                var result = await service.GetAllSuppliersAsync();

                // Assert
                Assert.Empty(result);
            }
        }
    }
}
