using Abstractions;
using Handlers;
using Commands.Specialization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Persistence;

namespace Tests.Handlers
{
    [TestFixture]
    internal class SpecializationCommandHandlerTests
    {
        private ServiceProvider _serviceProvider;
        private SqliteConnection _connection;

        private RepositoryDbContext Context => _serviceProvider.GetRequiredService<RepositoryDbContext>();
        private IUnitOfWork UnitOfWork => _serviceProvider.GetRequiredService<IUnitOfWork>();

        [SetUp]
        public void Setup()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            var services = new ServiceCollection();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddProvider(NullLoggerProvider.Instance);
            });

            services.AddDbContext<RepositoryDbContext>(options =>
                options.UseSqlite(_connection));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<RepositoryDbContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            _serviceProvider = services.BuildServiceProvider();

            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<RepositoryDbContext>();
            context.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider.Dispose();
            _connection.Close();
            _connection.Dispose();
        }

        [Test]
        public async Task HandleCreateSpecializationCommand_ValidRequest_CreatesSpecialization()
        {
            // Arrange
            var context = Context;
            var handler = new SpecializationCommandHandler(UnitOfWork);
            var command = new CreateSpecializationCommand("Cardiology");

            // Act
            await handler.Handle(command);

            // Assert
            var specialization = await context.Specializations.AsNoTracking().SingleAsync(s => s.Name == "Cardiology");
            Assert.That(specialization, Is.Not.Null);
            Assert.That(specialization.Name, Is.EqualTo("Cardiology"));
        }

        [Test]
        public async Task HandleUpdateSpecializationCommand_ValidRequest_UpdatesSpecialization()
        {
            // Arrange
            var context = Context;
            var handler = new SpecializationCommandHandler(UnitOfWork);

            var specialization = new Domain.Entities.Specialization { Name = "Cardiology" };
            context.Specializations.Add(specialization);
            await context.SaveChangesAsync();

            var specializationId = specialization.Id;
            var updateCommand = new UpdateSpecializationCommand(specializationId, "Updated Cardiology");

            context.ChangeTracker.Clear();

            // Act
            await handler.Handle(updateCommand);

            // Assert
            var updatedSpecialization = await context.Specializations.AsNoTracking().SingleAsync(s => s.Id == specializationId);
            Assert.That(updatedSpecialization, Is.Not.Null);
            Assert.That(updatedSpecialization.Name, Is.EqualTo("Updated Cardiology"));
        }

        [Test]
        public async Task HandleDeleteSpecializationCommand_ValidId_DeletesSpecialization()
        {
            // Arrange
            var context = Context;
            var handler = new SpecializationCommandHandler(UnitOfWork);

            var specialization = new Domain.Entities.Specialization { Name = "Cardiology" };
            context.Specializations.Add(specialization);
            await context.SaveChangesAsync();

            var specializationId = specialization.Id;
            var deleteCommand = new DeleteSpecializationCommand(specializationId);

            context.ChangeTracker.Clear();

            // Act
            await handler.Handle(deleteCommand);

            // Assert
            var deletedSpecialization = await context.Specializations.AsNoTracking().FirstOrDefaultAsync(s => s.Id == specializationId);
            Assert.That(deletedSpecialization, Is.Null);
        }
    }
}
