using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GCA.BLL.Helpers;
using GCA.DAL;
using GCA.Models;

namespace GCA.BLL.Services
{
    public class DataSeederService
    {
        private readonly GCADbContext _context;

        public DataSeederService(GCADbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            if (!_context.Users.Any())
            {
                var admin = new User
                {
                    Username = "admin",
                    PasswordHash = PasswordHasher.HashPassword("admin123"),
                    FullName = "Administrator",
                    Role = UserRole.Admin,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Users.Add(admin);
                
                // Add categories
                // Add categories
                var engine = new Category { Name = "Engine" };
                var body = new Category { Name = "Body" };
                var interior = new Category { Name = "Interior" };
                var electronics = new Category { Name = "Electronics" };
                var suspension = new Category { Name = "Suspension & Brakes" };
                var wheels = new Category { Name = "Wheels & Tires" };

                var cats = new List<Category> { engine, body, interior, electronics, suspension, wheels };
                _context.Categories.AddRange(cats);
                await _context.SaveChangesAsync();
                
                // Add sample parts
                var parts = new List<Part>
                {
                    new Part { Name = "V6 Engine Block", Category = engine, Quantity = 2, UnitPrice = 1500.00m, SKU = "ENG-V6-001", State = PartState.Available },
                    new Part { Name = "Alternator (Bosch)", Category = engine, Quantity = 5, UnitPrice = 120.00m, SKU = "ENG-ALT-002", State = PartState.Available },
                    new Part { Name = "Radiator", Category = engine, Quantity = 3, UnitPrice = 85.50m, SKU = "ENG-RAD-003", State = PartState.Available },
                    
                    new Part { Name = "Front Bumper (Black)", Category = body, Quantity = 1, UnitPrice = 250.00m, SKU = "BOD-FBP-001", State = PartState.Available },
                    new Part { Name = "Healight Assembly (Left)", Category = body, Quantity = 4, UnitPrice = 180.00m, SKU = "BOD-HDL-L02", State = PartState.Available },
                    new Part { Name = "Side Mirror (Right)", Category = body, Quantity = 6, UnitPrice = 45.00m, SKU = "BOD-MIR-R03", State = PartState.Available },
                    
                    new Part { Name = "Leather Seat (Driver)", Category = interior, Quantity = 2, UnitPrice = 300.00m, SKU = "INT-SEA-001", State = PartState.Available },
                    new Part { Name = "Dashboard Panel", Category = interior, Quantity = 1, UnitPrice = 150.00m, SKU = "INT-DSH-002", State = PartState.Available },
                    
                    new Part { Name = "ECU Control Unit", Category = electronics, Quantity = 3, UnitPrice = 500.00m, SKU = "ELE-ECU-001", State = PartState.Available },
                    new Part { Name = "Car Battery (12V)", Category = electronics, Quantity = 10, UnitPrice = 90.00m, SKU = "ELE-BAT-002", State = PartState.Available },
                    
                    new Part { Name = "Brake Disc (Front)", Category = suspension, Quantity = 20, UnitPrice = 40.00m, SKU = "SUS-BRK-001", State = PartState.Available },
                    new Part { Name = "Shock Absorber", Category = suspension, Quantity = 8, UnitPrice = 65.00m, SKU = "SUS-SHK-002", State = PartState.Available },
                    
                    new Part { Name = "Alloy Wheel 18-inch", Category = wheels, Quantity = 4, UnitPrice = 200.00m, SKU = "WHL-ALY-001", State = PartState.Available },
                    new Part { Name = "Michelin Tire 225/45", Category = wheels, Quantity = 12, UnitPrice = 110.00m, SKU = "WHL-TIR-002", State = PartState.Available }
                };

                _context.Parts.AddRange(parts);
                await _context.SaveChangesAsync();
                
                // Add Sample Clients
                _context.Clients.AddRange(new List<Client>
                {
                    new Client { Name = "Garage Autofix", Phone = "0102030405", Email = "contact@autofix.com", Address = "12 Industrial Ave" },
                    new Client { Name = "John Doe (Private)", Phone = "0611223344", Email = "john.doe@email.com", Address = "45 Elm Street" }
                });
                
                // Add Sample Suppliers
                _context.Suppliers.AddRange(new List<Supplier>
                {
                    new Supplier { Name = "Global Parts LTD", ContactInfo = "supply@globalparts.com - +44 20 1234 5678" },
                    new Supplier { Name = "Local Scrap Yard", ContactInfo = "Bob - 0699887766" }
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}
