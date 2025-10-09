using System;
using System.Threading;
using System.Threading.Tasks;
using BMEStokYonetim.Data;
using BMEStokYonetim.Data.Entities;
using BMEStokYonetim.Services.Iservice;
using BMEStokYonetim.Services.Service;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace BMEStokYonetim.Tests
{
    public class PurchaseServiceTests
    {
        [Fact]
        public async Task GeneratePurchaseNumberAsync_NormalizesLocationAndIncrementsSequence()
        {
            string databaseName = $"PurchaseServiceTests_{Guid.NewGuid()}";
            TestDbContextFactory factory = new(databaseName);
            Mock<IProcessService> processServiceMock = new();
            PurchaseService service = new(factory, processServiceMock.Object);

            await using (ApplicationDbContext context = await factory.CreateDbContextAsync())
            {
                context.Locations.Add(new Location { Id = 5, Name = "Şube Ümraniye" });
                await context.SaveChangesAsync();
            }

            string firstNumber = await service.GeneratePurchaseNumberAsync(5);
            string yearCode = DateTime.UtcNow.ToString("yy");
            Assert.Equal($"BME-PO-{yearCode}-SUB-00001", firstNumber);

            await using (ApplicationDbContext context = await factory.CreateDbContextAsync())
            {
                ApplicationUser user = new()
                {
                    Id = "user",
                    UserName = "user",
                    NormalizedUserName = "USER",
                    Email = "user@example.com",
                    NormalizedEmail = "USER@EXAMPLE.COM"
                };
                context.Users.Add(user);
                context.Purchases.Add(new Purchase
                {
                    PurchaseNumber = firstNumber,
                    LocationId = 5,
                    PurchaseDate = DateTime.UtcNow,
                    CreatedByUserId = user.Id,
                    CreatedByUser = user
                });
                await context.SaveChangesAsync();
            }

            string secondNumber = await service.GeneratePurchaseNumberAsync(5);
            Assert.Equal($"BME-PO-{yearCode}-SUB-00002", secondNumber);
        }

        private sealed class TestDbContextFactory : IDbContextFactory<ApplicationDbContext>
        {
            private readonly DbContextOptions<ApplicationDbContext> _options;

            public TestDbContextFactory(string databaseName)
            {
                _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName)
                    .Options;
            }

            public ApplicationDbContext CreateDbContext()
            {
                return new ApplicationDbContext(_options);
            }

            public ValueTask<ApplicationDbContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
            {
                return new ValueTask<ApplicationDbContext>(new ApplicationDbContext(_options));
            }
        }
    }
}
