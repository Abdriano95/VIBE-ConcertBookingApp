using AutoMapper;
using DMA_AU24_LAB2_Group4.API.Controllers;
using DMA_AU24_LAB2_Group4.API.Profiles;
using DMA_AU24_LAB2_Group4.Data;
using DMA_AU24_LAB2_Group4.Data.DTO;
using DMA_AU24_LAB2_Group4.Data.Entity;
using DMA_AU24_LAB2_Group4.Data.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace DMA_AU24_LAB2_Group4.Test
{
    public class PasswordSecurityTests
    {
        private readonly IMapper _mapper;

        public PasswordSecurityTests()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CustomerProfile>();
            });
            _mapper = config.CreateMapper();
        }

        private ApplicationDbContext CreateInMemoryContext(string databaseName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
            return new ApplicationDbContext(options);
        }

        #region BCrypt Hashing Unit Tests

        [Fact]
        public void BCrypt_HashPassword_ShouldCreateValidHash()
        {
            // Arrange
            var plainPassword = "TestPassword123!";

            // Act
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEqual(plainPassword, hashedPassword);
            Assert.StartsWith("$2", hashedPassword); // BCrypt hashes start with $2a$, $2b$, or $2y$
        }

        [Fact]
        public void BCrypt_Verify_ShouldReturnTrue_WhenPasswordMatches()
        {
            // Arrange
            var plainPassword = "TestPassword123!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // Act
            var result = BCrypt.Net.BCrypt.Verify(plainPassword, hashedPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void BCrypt_Verify_ShouldReturnFalse_WhenPasswordDoesNotMatch()
        {
            // Arrange
            var plainPassword = "TestPassword123!";
            var wrongPassword = "WrongPassword456!";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // Act
            var result = BCrypt.Net.BCrypt.Verify(wrongPassword, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void BCrypt_HashPassword_ShouldCreateUniqueHashes_ForSamePassword()
        {
            // Arrange
            var plainPassword = "TestPassword123!";

            // Act
            var hash1 = BCrypt.Net.BCrypt.HashPassword(plainPassword);
            var hash2 = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            // Assert - BCrypt generates unique salts, so hashes should be different
            Assert.NotEqual(hash1, hash2);
            // But both should still verify correctly
            Assert.True(BCrypt.Net.BCrypt.Verify(plainPassword, hash1));
            Assert.True(BCrypt.Net.BCrypt.Verify(plainPassword, hash2));
        }

        #endregion

        #region Registration Tests

        [Fact]
        public async Task Register_ShouldHashPassword_WhenCreatingNewCustomer()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var registerDto = new RegisterCustomerDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test.user@example.com",
                Password = "PlainTextPassword123!"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);

            // Verify the password was hashed in the database
            var savedCustomer = await context.Customers.FirstOrDefaultAsync(c => c.Email == registerDto.Email);
            Assert.NotNull(savedCustomer);
            Assert.NotEqual(registerDto.Password, savedCustomer.Password); // Password should be hashed
            Assert.StartsWith("$2", savedCustomer.Password); // BCrypt hash format
            Assert.True(BCrypt.Net.BCrypt.Verify(registerDto.Password, savedCustomer.Password)); // Should verify correctly
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            
            // Add existing customer
            var existingCustomer = new Customer
            {
                Id = 1,
                FirstName = "Existing",
                LastName = "User",
                Email = "existing@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("ExistingPassword123!")
            };
            context.Customers.Add(existingCustomer);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var registerDto = new RegisterCustomerDto
            {
                FirstName = "New",
                LastName = "User",
                Email = "existing@example.com", // Same email
                Password = "NewPassword123!"
            };

            // Act
            var result = await controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            // The response is now a structured object with Error and Message properties
            Assert.NotNull(badRequestResult.Value);
            var valueJson = System.Text.Json.JsonSerializer.Serialize(badRequestResult.Value);
            Assert.Contains("EmailInUse", valueJson);
        }

        #endregion

        #region Login Tests

        [Fact]
        public async Task Login_ShouldSucceed_WhenPasswordIsCorrect()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            
            var plainPassword = "CorrectPassword123!";
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword(plainPassword)
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = plainPassword
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCustomer = Assert.IsType<CustomerDto>(okResult.Value);
            Assert.Equal(customer.Email, returnedCustomer.Email);
        }

        [Fact]
        public async Task Login_ShouldFail_WhenPasswordIsIncorrect()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            
            var correctPassword = "CorrectPassword123!";
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword(correctPassword)
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "WrongPassword456!" // Wrong password
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            // The response is now a structured object with Error and Message properties
            Assert.NotNull(unauthorizedResult.Value);
            var valueJson = System.Text.Json.JsonSerializer.Serialize(unauthorizedResult.Value);
            Assert.Contains("InvalidCredentials", valueJson);
        }

        [Fact]
        public async Task Login_ShouldFail_WhenEmailDoesNotExist()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var loginDto = new LoginDto
            {
                Email = "nonexistent@example.com",
                Password = "AnyPassword123!"
            };

            // Act
            var result = await controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            // The response is now a structured object with Error and Message properties
            Assert.NotNull(unauthorizedResult.Value);
            var valueJson = System.Text.Json.JsonSerializer.Serialize(unauthorizedResult.Value);
            Assert.Contains("InvalidCredentials", valueJson);
        }

        [Fact]
        public async Task Login_ShouldFail_WhenPlainTextPasswordIsStoredInDatabase()
        {
            // Arrange - Simulates what would happen if someone bypassed hashing
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            
            var plainPassword = "PlainTextPassword123!";
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = plainPassword // Incorrectly stored as plain text
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = plainPassword
            };

            // Act & Assert - BCrypt.Verify should throw or return false for non-BCrypt string
            var result = await controller.Login(loginDto);
            
            // The login should fail because the stored password is not a valid BCrypt hash
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            // The response is now a structured object with Error and Message properties
            Assert.NotNull(unauthorizedResult.Value);
            var valueJson = System.Text.Json.JsonSerializer.Serialize(unauthorizedResult.Value);
            Assert.Contains("InvalidCredentials", valueJson);
        }

        #endregion

        #region Update Customer Tests

        [Fact]
        public async Task UpdateCustomer_ShouldHashNewPassword_WhenPasswordProvided()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            
            var originalPassword = "OriginalPassword123!";
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword(originalPassword)
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var newPassword = "NewPassword456!";
            var updateDto = new UpdateCustomerDto
            {
                Id = 1,
                FirstName = "Updated",
                LastName = "User",
                Email = "test@example.com",
                Password = newPassword
            };

            // Act
            var result = await controller.UpdateCustomer(updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            // Verify the new password was hashed
            var updatedCustomer = await context.Customers.FindAsync(1);
            Assert.NotNull(updatedCustomer);
            Assert.NotEqual(newPassword, updatedCustomer.Password); // Should be hashed
            Assert.StartsWith("$2", updatedCustomer.Password); // BCrypt format
            Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, updatedCustomer.Password)); // New password verifies
            Assert.False(BCrypt.Net.BCrypt.Verify(originalPassword, updatedCustomer.Password)); // Old password no longer works
        }

        [Fact]
        public async Task UpdateCustomer_ShouldKeepExistingPassword_WhenNoPasswordProvided()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            
            var originalPassword = "OriginalPassword123!";
            var originalHash = BCrypt.Net.BCrypt.HashPassword(originalPassword);
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = originalHash
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var updateDto = new UpdateCustomerDto
            {
                Id = 1,
                FirstName = "Updated",
                LastName = "Name",
                Email = "test@example.com",
                Password = null // No password change
            };

            // Act
            var result = await controller.UpdateCustomer(updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            // Verify the password remains unchanged
            var updatedCustomer = await context.Customers.FindAsync(1);
            Assert.NotNull(updatedCustomer);
            Assert.Equal(originalHash, updatedCustomer.Password); // Password unchanged
            Assert.True(BCrypt.Net.BCrypt.Verify(originalPassword, updatedCustomer.Password)); // Original password still works
        }

        [Fact]
        public async Task UpdateCustomer_ShouldKeepExistingPassword_WhenEmptyPasswordProvided()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            
            var originalPassword = "OriginalPassword123!";
            var originalHash = BCrypt.Net.BCrypt.HashPassword(originalPassword);
            var customer = new Customer
            {
                Id = 1,
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = originalHash
            };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var updateDto = new UpdateCustomerDto
            {
                Id = 1,
                FirstName = "Updated",
                LastName = "Name",
                Email = "test@example.com",
                Password = "" // Empty string password
            };

            // Act
            var result = await controller.UpdateCustomer(updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            // Verify the password remains unchanged
            var updatedCustomer = await context.Customers.FindAsync(1);
            Assert.NotNull(updatedCustomer);
            Assert.Equal(originalHash, updatedCustomer.Password); // Password unchanged
            Assert.True(BCrypt.Net.BCrypt.Verify(originalPassword, updatedCustomer.Password)); // Original password still works
        }

        [Fact]
        public async Task UpdateCustomer_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var updateDto = new UpdateCustomerDto
            {
                Id = 999, // Non-existent customer
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Password = "NewPassword123!"
            };

            // Act
            var result = await controller.UpdateCustomer(updateDto);

            // Assert - Now returns NotFoundObjectResult with structured error
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.NotNull(notFoundResult.Value);
            var valueJson = System.Text.Json.JsonSerializer.Serialize(notFoundResult.Value);
            Assert.Contains("CustomerNotFound", valueJson);
        }

        #endregion

        #region Integration Tests - Full Registration and Login Flow

        [Fact]
        public async Task FullFlow_RegisterThenLogin_ShouldSucceed()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var password = "SecurePassword123!";
            var registerDto = new RegisterCustomerDto
            {
                FirstName = "Integration",
                LastName = "Test",
                Email = "integration@test.com",
                Password = password
            };

            // Act - Register
            var registerResult = await controller.Register(registerDto);

            // Assert - Registration successful
            Assert.IsType<CreatedAtActionResult>(registerResult);

            // Act - Login with same credentials
            var loginDto = new LoginDto
            {
                Email = registerDto.Email,
                Password = password
            };
            var loginResult = await controller.Login(loginDto);

            // Assert - Login successful
            var okResult = Assert.IsType<OkObjectResult>(loginResult);
            var customerDto = Assert.IsType<CustomerDto>(okResult.Value);
            Assert.Equal(registerDto.Email, customerDto.Email);
            Assert.Equal(registerDto.FirstName, customerDto.CustomerFirstName);
        }

        [Fact]
        public async Task FullFlow_RegisterUpdatePasswordThenLogin_ShouldSucceedWithNewPassword()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            using var context = CreateInMemoryContext(dbName);
            var unitOfWork = new UnitOfWork(context);
            var controller = new CustomerController(unitOfWork, _mapper);

            var originalPassword = "OriginalPassword123!";
            var newPassword = "NewSecurePassword456!";

            // Register
            var registerDto = new RegisterCustomerDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "flowtest@example.com",
                Password = originalPassword
            };
            await controller.Register(registerDto);

            // Get the customer ID
            var customer = await context.Customers.FirstAsync(c => c.Email == registerDto.Email);

            // Update password
            var updateDto = new UpdateCustomerDto
            {
                Id = customer.Id,
                FirstName = "Test",
                LastName = "User",
                Email = "flowtest@example.com",
                Password = newPassword
            };
            await controller.UpdateCustomer(updateDto);

            // Act - Try to login with old password
            var oldLoginDto = new LoginDto
            {
                Email = registerDto.Email,
                Password = originalPassword
            };
            var oldLoginResult = await controller.Login(oldLoginDto);

            // Assert - Old password should fail
            Assert.IsType<UnauthorizedObjectResult>(oldLoginResult);

            // Act - Login with new password
            var newLoginDto = new LoginDto
            {
                Email = registerDto.Email,
                Password = newPassword
            };
            var newLoginResult = await controller.Login(newLoginDto);

            // Assert - New password should succeed
            Assert.IsType<OkObjectResult>(newLoginResult);
        }

        #endregion
    }
}
