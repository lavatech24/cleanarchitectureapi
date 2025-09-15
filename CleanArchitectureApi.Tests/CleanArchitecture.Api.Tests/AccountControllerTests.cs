using CleanArchitecture.Api.Controllers;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Models.SystemModels;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CleanArchitecture.Api.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountService> mockAccountService;
        private readonly AccountController accountController;
        public AccountControllerTests()
        {
            mockAccountService = new Mock<IAccountService>();
            accountController = new AccountController(mockAccountService.Object);
        }

        [Fact]
        public async Task GetAllUsers_ShouldReturnOkResult_WithListOfUsers()
        {
            var users = new List<ApiUserDto>
            {
                new ApiUserDto { Id = Guid.NewGuid(), ClientId = "client1", IsActive = true },
                new ApiUserDto { Id = Guid.NewGuid(), ClientId = "client2", IsActive = false }
            };
            mockAccountService.Setup(service => service.GetAllUsersAsync()).ReturnsAsync(users);

            var result = await accountController.GetAllUsers();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<ApiUser>>(okResult.Value);
            Assert.Equal(users.Count, returnedUsers.Count());
        }

        [Fact]
        public async Task GetUserById_ShouldReturnOkResult_WithUser()
        {
            var userId = Guid.NewGuid();
            var user = new ApiUserDto { Id = userId, ClientId = "client1", IsActive = true };
            mockAccountService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync(user);

            var result = await accountController.GetUserById(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<ApiUser>(okResult.Value);
            Assert.Equal(userId, returnedUser.Id);
        }

        [Fact]
        public async Task GetUserById_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            mockAccountService.Setup(service => service.GetUserByIdAsync(userId)).ReturnsAsync((ApiUserDto)null);

            var result = await accountController.GetUserById(userId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetUserByClientId_ShouldReturnOkResult_WithUser()
        {
            var clientId = "client1";
            var user = new ApiUserDto { Id = Guid.NewGuid(), ClientId = clientId, IsActive = true };
            mockAccountService.Setup(service => service.GetUserByClientIdAsync(clientId)).ReturnsAsync(user);

            var result = await accountController.GetUserByClientId(clientId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<ApiUser>(okResult.Value);
            Assert.Equal(clientId, returnedUser.ClientId);
        }

        [Fact]
        public async Task GetUserByClientId_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var clientId = "client1";
            mockAccountService.Setup(service => service.GetUserByClientIdAsync(clientId)).ReturnsAsync((ApiUserDto)null);

            var result = await accountController.GetUserByClientId(clientId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetUsersByStatus_ShouldReturnOkResult_WithListOfUsers()
        {
            var status = true;
            var users = new List<ApiUserStatusDto>
            {
                new ApiUserStatusDto { Id = Guid.NewGuid(), IsActive = true },
                new ApiUserStatusDto { Id = Guid.NewGuid(), IsActive = true }
            };
            mockAccountService.Setup(service => service.GetUsersByStatusAsync(status)).ReturnsAsync(users);

            var result = await accountController.GetUsersByStatus(status);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUsers = Assert.IsAssignableFrom<IEnumerable<ApiUser>>(okResult.Value);
            Assert.Equal(users.Count, returnedUsers.Count());
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreatedAtAction_WithCreatedUser()
        {
            var user = new ApiUserCreateDto { ClientId = "client1", IsActive = true };
            var userReturn = new ApiUserDto { ClientId = "client1", IsActive = true };
            mockAccountService.Setup(service => service.CreateUserAsync(user)).ReturnsAsync(userReturn);

            var result = await accountController.CreateUser(user);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedUser = Assert.IsType<ApiUser>(createdAtActionResult.Value);
            Assert.Equal(userReturn.ClientId, returnedUser.ClientId);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnBadRequest_WhenUserIsNull()
        {
            ApiUserCreateDto user = null;

            var result = await accountController.CreateUser(user);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnOkResult_WithUpdatedUser()
        {
            var userId = Guid.NewGuid();
            var user = new ApiUserUpdateDto { Id = userId, ClientId = "client1" };
            var userReturn = new ApiUserDto { Id = userId, ClientId = "client1", IsActive = true };
            mockAccountService.Setup(service => service.UpdateUserAsync(user)).ReturnsAsync(userReturn);

            var result = await accountController.UpdateUser(userId, user);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<ApiUser>(okResult.Value);
            Assert.Equal(userId, returnedUser.Id);
        }

        [Fact]
        public async Task UpdateUser_ShouldReturnBadRequest_WhenUserIsNullOrIdMismatch()
        {
            var userId = Guid.NewGuid();
            ApiUserUpdateDto user = null;

            var result = await accountController.UpdateUser(userId, user);

            Assert.IsType<BadRequestObjectResult>(result);

            user = new ApiUserUpdateDto { Id = Guid.NewGuid(), ClientId = "client1" };
            result = await accountController.UpdateUser(userId, user);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        /*
        [Fact]
        public async Task DeleteUser_ShouldReturnNoContent_WhenUserIsDeleted()
        {
            var userId = Guid.NewGuid();
            //mockAccountService.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(new ApiUser { Id = userId });
            mockAccountService.Setup(service => service.DeleteUserAsync(userId)).Returns(Task.CompletedTask);

            var result = await accountController.DeleteUser(userId);

            Assert.IsType<NoContentResult>(result);
            mockAccountService.Verify(s => s.DeleteUserAsync(userId), Times.Once);
        }
        */

        [Fact]
        public async Task DeleteUser_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            mockAccountService.Setup(service => service.DeleteUserAsync(userId)).Throws(new KeyNotFoundException());

            var result = await accountController.DeleteUser(userId);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UserExists_ShouldReturnTrue_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            mockAccountService.Setup(service => service.UserExistsAsync(userId)).ReturnsAsync(true);

            var result = await accountController.UserExists(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
        }

        [Fact]
        public async Task UserExists_ShouldReturnFalse_WhenUserDoesNotExist()
        {
            var userId = Guid.NewGuid();
            mockAccountService.Setup(service => service.UserExistsAsync(userId)).ReturnsAsync(false);

            var result = await accountController.UserExists(userId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.False((bool)okResult.Value);
        }

        [Fact]
        public async Task UserExists_ShouldReturnBadRequest_WhenUserIdIsEmpty()
        {
            var userId = Guid.Empty;

            var result = await accountController.UserExists(userId);

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UserExists_ShouldReturnBadRequest_WhenUserIdIsInvalid()
        {
            var userId = Guid.NewGuid();
            mockAccountService.Setup(service => service.UserExistsAsync(userId)).Throws(new ArgumentException("Invalid user ID"));

            var result = await accountController.UserExists(userId);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task UserExists_ShouldReturnInternalServerError_WhenExceptionOccurs()
        {
            var userId = Guid.NewGuid();
            mockAccountService.Setup(service => service.UserExistsAsync(userId)).Throws(new Exception("Unexpected error"));

            var result = await accountController.UserExists(userId);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            //Assert.Equal("Internal server error", objectResult.Value);
        }
    }
}
