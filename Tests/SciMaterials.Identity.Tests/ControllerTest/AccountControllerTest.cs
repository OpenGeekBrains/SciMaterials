using HttpContextMoq;
using HttpContextMoq.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using SciMaterials.Contracts.API.Constants;
using SciMaterials.Contracts.Auth;
using SciMaterials.Contracts.Enums;
using SciMaterials.Contracts.Identity.API.DTO.Users;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;
using SciMaterials.Identity.Tests.FakeServices;
using SciMaterials.UI.MVC.Identity.Controllers;

namespace SciMaterials.Identity.Tests.ControllerTest
{
    public class AccountControllerTest
    {
        private AccountController _AccountController;
        private Mock<FakeUserManager> _UserManagerMock;
        private Mock<FakeSignInManager> _SignInManagerMock;
        private Mock<FakeRoleManager> _RoleManagerMock;
        private Mock<IHttpContextAccessor> _ContextAccessorMock;
        private Mock<IAuthUtilits> _AuthUtilitsMock;
        private Mock<ILogger<AccountController>> _LoggerMock;
        private HttpContextMock _HttpContextMock;

        public AccountControllerTest()
        {
            //var roles = new List<IdentityRole>()
            //{
            //    new IdentityRole()
            //    {
            //        Id = Guid.NewGuid().ToString(),
            //        Name = "admin",
            //        NormalizedName = "ADMIN",
            //        ConcurrencyStamp = Guid.NewGuid().ToString().ToUpper()
            //    }
            //}.AsQueryable();

            _UserManagerMock = new Mock<FakeUserManager>();
            _SignInManagerMock = new Mock<FakeSignInManager>();
            _RoleManagerMock = new Mock<FakeRoleManager>();
            
            _HttpContextMock = new HttpContextMock();
            _ContextAccessorMock = new Mock<IHttpContextAccessor>();
            
            _AuthUtilitsMock = new Mock<IAuthUtilits>();
            _LoggerMock = new Mock<ILogger<AccountController>>();

            _AccountController = new AccountController(
                _UserManagerMock.Object,
                _SignInManagerMock.Object,
                _RoleManagerMock.Object,
                _ContextAccessorMock.Object,
                _AuthUtilitsMock.Object,
                _LoggerMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_Returns_StatusCode200_And_CallbackUrl()
        {
            //Arrage
            _HttpContextMock = new HttpContextMock().SetupUrl("http://localhost:5185/register");
            _ContextAccessorMock.Setup(x => x.HttpContext).Returns(_HttpContextMock);

            var urlHelperMock = new Mock<IUrlHelper>(MockBehavior.Strict);
            urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>())).Returns("callBackUrl").Verifiable();
            
            _AccountController.Url = urlHelperMock.Object;
            _AccountController.ControllerContext.HttpContext = new DefaultHttpContext();
            _AccountController.ControllerContext.HttpContext.Request.Scheme = "http";
            
            _UserManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success).Verifiable();
            _UserManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), AuthApiRoles.User))
                .ReturnsAsync(IdentityResult.Success);
            _UserManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(Guid.NewGuid().ToString()).Verifiable();

            var request = new RegisterRequest()
            {
                NickName = "vasiliy@mail.ru",
                Email = "vasiliy@mail.ru",
                Password = "test12345"
            };

            //Act
            var result = await _AccountController.RegisterAsync(request);
            var actionResultObj = result as OkObjectResult;
            var okResult = actionResultObj.Value as ClientCreateUserResponse;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, actionResultObj.StatusCode);
            Assert.Equal((int)ResultCodes.Ok, okResult.Code);
            Assert.True(okResult.Succeeded);
            Assert.NotEmpty(okResult.Message);
            Assert.NotEmpty(okResult.ConfirmEmail);
        }

        [Fact]
        public async Task LoginAsync_Return_StatusCode200WithResult()
        {
            //Arrage
            _UserManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "sa@mail.ru",
                    NormalizedUserName = "SA@MAIL.RU",
                    NormalizedEmail = "SA@MAIL.RU",
                    Email = "sa@mail.ru",
                    EmailConfirmed = true,
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    SecurityStamp = Guid.NewGuid().ToString()
                });

            _UserManagerMock.Setup(x => x.IsEmailConfirmedAsync(It.IsAny<IdentityUser>())).ReturnsAsync(true);

            _SignInManagerMock.Setup(x => x.PasswordSignInAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>())).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _UserManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(new List<string>(){ "admin" });
            _AuthUtilitsMock.Setup(x => x.CreateSessionToken(It.IsAny<IdentityUser>(), It.IsAny<List<string>>()))
                .Returns("jwtTokenSession").Verifiable();
            
            var request = new LoginRequest()
            {
                Email = "sa@mail.ru",
                Password = "test12345"
            };

            //Act
            var result = await _AccountController.LoginAsync(request);
            var actionResultObj = result as OkObjectResult;
            var okResult = actionResultObj.Value as ClientLoginResponse;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, actionResultObj.StatusCode);
            Assert.Equal((int)ResultCodes.Ok, okResult.Code);
            Assert.True(okResult.Succeeded);
            Assert.NotEmpty(okResult.SessionToken);
        }

        [Fact]
        public async Task LogoutAsync_Return_StatusCode200WithResult()
        {
            //Arrage
            _SignInManagerMock.Setup(x => x.SignOutAsync());

            //Act
            var result = await _AccountController.LogoutAsync();
            var actionResultObj = result as OkObjectResult;
            var okResult = actionResultObj.Value as ClientLogoutResponse;

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, actionResultObj.StatusCode);
            Assert.Equal((int)ResultCodes.Ok, okResult.Code);
            Assert.True(okResult.Succeeded);
            Assert.NotEmpty(okResult.Message);
        }

        // [Fact]
        // public async Task ChangePasswordAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task GetRefreshTokenAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task ConfirmEmailAsync_Return_StatusCode200WithResult()
        // {
        //     //Arrage
        //
        //     //Act
        //
        //     //Assert
        // }
        //
        // [Fact]
        // public async Task CreateRoleAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task GetAllRolesAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task GetRoleByIdAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // public async Task EditRoleNameByIdAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task DeleteRoleByIdAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task AddRoleToUserAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task DeleteUserRoleByEmailAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task GetAllUserRolesByEmailAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task CreateUserAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task GetUserByEmailAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task GetAllUsersAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task EditUserNameByEmailAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task DeleteUserByEmailAsync_Return_StatusCode200WithResult()
        // {
        // }
        //
        // [Fact]
        // public async Task DeleteUsersWithOutConfirmAsync_Return_StatusCode200WithResult()
        // {
        // }
    }
}
