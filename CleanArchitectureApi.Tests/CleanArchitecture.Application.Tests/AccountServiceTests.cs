using AutoMapper;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Domain.Models.SystemModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Application.Tests
{
    public class AccountServiceTests
    {
        private readonly Mock<IAccountRepository> mockRepository;
        private readonly Mock<IMapper> mockMapper; 
        private readonly IAccountService accountService;

        public AccountServiceTests()
        {
            mockRepository = new Mock<IAccountRepository>();
            mockMapper = new Mock<IMapper>();
            accountService = new AccountService(mockRepository.Object, mockMapper.Object); // Pass the mockMapper to the constructor
        }

        [Fact]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {
            // Arrange
            var companyId = 1; // Example company ID
            var users = new List<ApiUserDto>
                {
                    new ApiUserDto { Id = Guid.NewGuid(), ClientId = "client1", CompanyId = companyId, IsActive = true },
                    new ApiUserDto { Id = Guid.NewGuid(), ClientId = "client2", CompanyId = companyId, IsActive = false }
                };
            var usersReturn = users.Select(u => new ApiUser
            {
                Id = u.Id,
                ClientId = u.ClientId,
                IsActive = u.IsActive
            }).ToList();
            mockRepository.Setup(repo => repo.GetAllUsersAsync(companyId)).ReturnsAsync(usersReturn);

            // Act
            var result = await accountService.GetAllUsersAsync("cmp1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(users, result);
        }
    }
}
