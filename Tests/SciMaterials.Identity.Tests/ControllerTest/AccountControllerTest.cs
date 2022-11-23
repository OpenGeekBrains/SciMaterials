using System.Security.Claims;
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
using SciMaterials.Contracts.Identity.API.DTO.Roles;
using SciMaterials.Contracts.Identity.API.DTO.Users;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.DTO;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.Roles;
using SciMaterials.Contracts.Identity.Clients.Clients.Responses.User;
using SciMaterials.Identity.Tests.MockServices;
using SciMaterials.UI.MVC.Identity.Controllers;

namespace SciMaterials.Identity.Tests.ControllerTest
{
    public class AccountControllerTest
    {
        private AccountController _AccountController;
        private Mock<UserManagerMock> _UserManagerMock;
        private Mock<SignInManagerMock> _SignInManagerMock;
        private Mock<RoleManagerMock> _RoleManagerMock;
        private Mock<IHttpContextAccessor> _ContextAccessorMock;
        private Mock<IAuthUtilits> _AuthUtilitsMock;
        private Mock<ILogger<AccountController>> _LoggerMock;
        private HttpContextMock _HttpContextMock;

        public AccountControllerTest()
        {
            _UserManagerMock = new Mock<UserManagerMock>();
            _SignInManagerMock = new Mock<SignInManagerMock>();
            _RoleManagerMock = new Mock<RoleManagerMock>();
            
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
            const string expected_urlAddress = "http://localhost:5185/auth/register";
            const string expected_scheme = "http";
            const string expected_callback_url = "callBackUrl";
            const string expected_nickname = "vasiliy@mail.ru";
            const string expected_email = "vasiliy@mail.ru";
            const string expected_password = "test12345";
            const string expected_action = "ConfirmEmail";
            const string expected_controller = "Account";
            const string expected_userrole = AuthApiRoles.User;
            const string expected_emailtoken = "generatedEmailToken";

            _HttpContextMock = new HttpContextMock().SetupUrl(expected_urlAddress);
            _ContextAccessorMock.Setup(x => x.HttpContext).Returns(_HttpContextMock);

            var urlHelperMock = new Mock<IUrlHelper>(MockBehavior.Strict);
            urlHelperMock.Setup(x => x.Action(It.IsAny<UrlActionContext>()))
                .Returns(expected_callback_url).Verifiable();
            
            _AccountController.Url = urlHelperMock.Object;
            _AccountController.ControllerContext.HttpContext = new DefaultHttpContext();
            _AccountController.ControllerContext.HttpContext.Request.Scheme = expected_scheme;

            _UserManagerMock.Setup(x => x.CreateAsync(
                    It.Is<IdentityUser>(s => s.Email.Equals(expected_email)),
                    It.Is<string>(s => s.Equals(expected_password))))
                .ReturnsAsync(IdentityResult.Success).Verifiable();
            
            _UserManagerMock.Setup(x => x.AddToRoleAsync(
                    It.Is<IdentityUser>(s => s.Email.Equals(expected_email)), expected_userrole))
                .ReturnsAsync(IdentityResult.Success).Verifiable();
            
            _UserManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(
                    It.Is<IdentityUser>(s => s.Email.Equals(expected_email))))
                .ReturnsAsync(expected_emailtoken).Verifiable();

            var request = new RegisterRequest()
            {
                NickName = expected_nickname,
                Email = expected_email,
                Password = expected_password
            };

            //Act
            var result = await _AccountController.RegisterAsync(request);
            var actionResultObj = Assert.IsType<OkObjectResult>(result);
            var okResult = Assert.IsAssignableFrom<ClientCreateUserResponse>(actionResultObj.Value);

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, actionResultObj.StatusCode);
            Assert.Equal((int)ResultCodes.Ok, okResult.Code);
            Assert.True(okResult.Succeeded);
            Assert.NotEmpty(okResult.Message);
            Assert.Equal(expected_callback_url, okResult.ConfirmEmail);
            
            //*-----------------------------------------------------*//
            urlHelperMock.Verify(url => url.Action(It.Is<UrlActionContext>(ctx => ctx.Action == expected_action)));
            urlHelperMock.Verify(url => url.Action(It.Is<UrlActionContext>(ctx => ctx.Controller == expected_controller)));
            urlHelperMock.Verify(url => url.Action(It.Is<UrlActionContext>(ctx => ctx.Protocol == expected_scheme)));
            
            _UserManagerMock.Verify(x => x.CreateAsync(
                It.Is<IdentityUser>(user => user.Email.Equals(expected_email)), 
                    It.Is<string>(pass => pass.Equals(expected_password))));
            
            _UserManagerMock.Verify(x => x.AddToRoleAsync(
                It.Is<IdentityUser>(user => user.Email.Equals(expected_email)), 
                    It.Is<string>(str => str.Equals(expected_userrole))));
            
            _UserManagerMock.Verify(x => x.GenerateEmailConfirmationTokenAsync(
                It.Is<IdentityUser>(user => user.Email.Equals(expected_email))));
        }

        [Fact]
        public async Task LoginAsync_Return_StatusCode200_And_Result()
        {
            //Arrage
            const string expected_username = "sa@mail.ru";
            const string expected_email = "sa@mail.ru";
            const string expected_password = "test12345";
            const string expected_id = "identificationNumber";
            const string expected_jwtSessionToken = "jwtTokenSession";
            const string expected_concurrencystamp = "concurrencyStamp";
            const string expected_securitystamp = "securityStamp";
            const string expected_role = "admin";
            
            _UserManagerMock.Setup(x => x.FindByEmailAsync(expected_email))
                .ReturnsAsync(new IdentityUser()
                {
                    Id = expected_id,
                    UserName = expected_username,
                    NormalizedUserName = expected_username.ToUpper(),
                    NormalizedEmail = expected_email.ToUpper(),
                    Email = expected_email,
                    EmailConfirmed = true,
                    ConcurrencyStamp = expected_concurrencystamp,
                    SecurityStamp = expected_securitystamp
                }).Verifiable();

            _UserManagerMock.Setup(x => x.IsEmailConfirmedAsync(
                It.Is<IdentityUser>(s => s.Email.Equals(expected_email))))
                .ReturnsAsync(true).Verifiable();

            _SignInManagerMock.Setup(x => x.PasswordSignInAsync(
                It.Is<string>(s => s.Equals(expected_username)),
                It.Is<string>(s => s.Equals(expected_password)),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success).Verifiable();

            _UserManagerMock.Setup(x => x.GetRolesAsync(
                    It.Is<IdentityUser>(s => s.UserName.Equals(expected_username))))
                .ReturnsAsync(new List<string>(){ expected_role }).Verifiable();
            
            _AuthUtilitsMock.Setup(x => x.CreateSessionToken(
                    It.Is<IdentityUser>(s => s.NormalizedEmail.Equals(expected_email.ToUpper())), 
                    It.IsAny<List<string>>()))
                .Returns(expected_jwtSessionToken).Verifiable();
            
            var request = new LoginRequest()
            {
                Email = expected_email,
                Password = expected_password
            };

            //Act
            var result = await _AccountController.LoginAsync(request);
            var actionResultObj = Assert.IsType<OkObjectResult>(result);
            var okResult = Assert.IsAssignableFrom<ClientLoginResponse>(actionResultObj.Value);

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, actionResultObj.StatusCode);
            Assert.Equal((int)ResultCodes.Ok, okResult.Code);
            Assert.True(okResult.Succeeded);
            Assert.Equal(expected_jwtSessionToken, okResult.SessionToken);
            
            //*-----------------------------------------------------*//
            _UserManagerMock.Verify(x => x.FindByEmailAsync(It.Is<string>(s => s.Equals(expected_email))));
            _UserManagerMock.Verify(x => x.IsEmailConfirmedAsync(It.Is<IdentityUser>(s => s.Email.Equals(expected_email))));
            _SignInManagerMock.Verify(x => x.PasswordSignInAsync(
                It.Is<string>(s => s.Equals(expected_username)),
                    It.Is<string>(s => s.Equals(expected_password)),
                It.IsAny<bool>(),
                It.IsAny<bool>()));
            _UserManagerMock.Verify(x => x.GetRolesAsync(It.Is<IdentityUser>(s => s.UserName.Equals(expected_username))));
            _AuthUtilitsMock.Verify(x => x.CreateSessionToken(
                It.Is<IdentityUser>(s => s.NormalizedEmail.Equals(expected_email.ToUpper())), 
                It.Is<List<string>>(s => s[0].Equals(expected_role))));
        }

        [Fact]
        public async Task LogoutAsync_Return_StatusCode200_And_Result()
        {
            //Arrage
            const string expected_message = "Пользователь вышел из системы";

            _SignInManagerMock.Setup(x => x.SignOutAsync());

            //Act
            var result = await _AccountController.LogoutAsync();
            var actionResultObj = Assert.IsType<OkObjectResult>(result);
            var okResult = Assert.IsAssignableFrom<ClientLogoutResponse>(actionResultObj.Value);

            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, actionResultObj.StatusCode);
            Assert.Equal((int)ResultCodes.Ok, okResult.Code);
            Assert.True(okResult.Succeeded);
            Assert.Equal(expected_message, okResult.Message);
        }

        [Fact]
        public async Task ChangePasswordAsync_Return_StatusCode200_And_Result()
        {
            //Arrage
            const string expected_urlAddress = "http://localhost:5185/auth/change_password";
            const string expected_id = "identificationNumber";
            const string expected_concurrencystamp = "concurrencyStamp";
            const string expected_securitystamp = "securityStamp";
            const string expected_email = "sa@mail.ru";
            const string expected_name = "sa@mail.ru";
            const string expected_role = "admin";
            const string expecred_currentPassword = "test12345";
            const string expecred_newPassword = "1q2w3e4r";
            const string expected_message = "Пароль успешно изменен";
            
            var identity = new ClaimsIdentity();
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, expected_email),
                new Claim(ClaimTypes.Name, expected_name),
                new Claim(ClaimTypes.Role, expected_role)
            });
            var user = new ClaimsPrincipal(identity);
            
            _HttpContextMock = new HttpContextMock();
            _HttpContextMock.User = user;
            _HttpContextMock.SetupUrl(expected_urlAddress);
            _ContextAccessorMock.Setup(x => x.HttpContext).Returns(_HttpContextMock);
            _AccountController.ControllerContext.HttpContext = _HttpContextMock;

            _UserManagerMock.Setup(x => x.FindByNameAsync(expected_email))
                .ReturnsAsync(new IdentityUser()
                {
                    Id = expected_id,
                    UserName = expected_name,
                    NormalizedUserName = expected_name.ToUpper(),
                    NormalizedEmail = expected_email.ToUpper(),
                    Email = expected_email,
                    EmailConfirmed = true,
                    ConcurrencyStamp = expected_concurrencystamp,
                    SecurityStamp = expected_securitystamp
                }).Verifiable();

            _UserManagerMock.Setup(x => x.IsEmailConfirmedAsync(
                It.Is<IdentityUser>(s => s.Email.Equals(expected_email)))).ReturnsAsync(true).Verifiable();

            var identity_result = new IdentityResultMock(true);

            _UserManagerMock.Setup(x => x.ChangePasswordAsync(
                It.Is<IdentityUser>(s => s.Email.Equals(expected_email)),
                It.Is<string>(s => s.Equals(expecred_currentPassword)),
                It.Is<string>(s => s.Equals(expecred_newPassword))))
                .ReturnsAsync(identity_result).Verifiable();

            _SignInManagerMock.Setup(x => x.RefreshSignInAsync(
                It.Is<IdentityUser>(s => s.Email.Equals(expected_email)))).Verifiable();
            
            var request = new ChangePasswordRequest()
            {
                CurrentPassword = "test12345",
                NewPassword = "1q2w3e4r"
            };
            
            //Act
            var result = await _AccountController.ChangePasswordAsync(request);
            var actionResultObj = Assert.IsType<OkObjectResult>(result);
            var okResult = Assert.IsAssignableFrom<ClientChangePasswordResponse>(actionResultObj.Value);
            
            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, actionResultObj.StatusCode);
            Assert.Equal((int)ResultCodes.Ok, okResult.Code);
            Assert.True(okResult.Succeeded);
            Assert.Equal(expected_message, okResult.Message);
            
            //*-----------------------------------------------------*//
            _UserManagerMock.Verify(x => x.FindByNameAsync(It.Is<string>(s => s.Equals(expected_email))));
            _UserManagerMock.Verify(x => x.IsEmailConfirmedAsync(It.Is<IdentityUser>(s => s.Email.Equals(expected_email))));
            _UserManagerMock.Verify(x=> x.ChangePasswordAsync(
                It.Is<IdentityUser>(s => s.Email.Equals(expected_email)),
                It.Is<string>(s => s.Equals(expecred_currentPassword)),
                It.Is<string>(s => s.Equals(expecred_newPassword))));
            _SignInManagerMock.Verify(x => x.RefreshSignInAsync(
                It.Is<IdentityUser>(s => s.Email.Equals(expected_email))));
        }
        
        [Fact]
        public async Task GetRefreshTokenAsync_Return_StatusCode200_And_Result()
        {
            //Arrage
            const string expected_urlAddress = "http://localhost:5185/auth/refresh_token";
            string expected_token = RealAdminToken.SetToken();
            const string expected_refresh_token = "refreshToken";
            const string expected_id = "identificationNumber";
            const string expected_concurrencystamp = "concurrencyStamp";
            const string expected_securitystamp = "securityStamp";
            const string expected_email = "sa@mail.ru";
            const string expected_name = "sa@mail.ru";
            const string expected_role = "admin";

            var identity = new ClaimsIdentity();
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, expected_email),
                new Claim(ClaimTypes.Name, expected_name),
                new Claim(ClaimTypes.Role, expected_role)
            });
            var user = new ClaimsPrincipal(identity);
            
            _HttpContextMock = new HttpContextMock();
            _HttpContextMock.User = user;
            _HttpContextMock.SetupUrl(expected_urlAddress);
            _ContextAccessorMock.Setup(x => x.HttpContext).Returns(_HttpContextMock);
            _ContextAccessorMock.Setup(x => x.HttpContext.Request.Headers).Returns(new HeaderDictionaryFake());
            _ContextAccessorMock.Setup(x => x.HttpContext.Request.Headers.Authorization).Returns(expected_token);
            _AccountController.ControllerContext.HttpContext = _HttpContextMock;

            _UserManagerMock.Setup(x => x.FindByEmailAsync(
                It.Is<string>(s => s.Equals(expected_email)))).ReturnsAsync(
                new IdentityUser()
                {
                    Id = expected_id,
                    UserName = expected_name,
                    NormalizedUserName = expected_name.ToUpper(),
                    NormalizedEmail = expected_email.ToUpper(),
                    Email = expected_email,
                    EmailConfirmed = true,
                    ConcurrencyStamp = expected_concurrencystamp,
                    SecurityStamp = expected_securitystamp
                }).Verifiable();

            _UserManagerMock.Setup(x => x.GetRolesAsync(
                It.Is<IdentityUser>(s => s.Email.Equals(expected_email))))
                .ReturnsAsync(new List<string>{expected_role}).Verifiable();
            
            _AuthUtilitsMock.Setup(x => x.CreateSessionToken(
                    It.Is<IdentityUser>(s => s.NormalizedEmail.Equals(expected_email.ToUpper())), 
                    It.IsAny<List<string>>()))
                .Returns(expected_refresh_token).Verifiable();
            
            //Act
            var result = await _AccountController.GetRefreshTokenAsync();
            var actionResultObj = Assert.IsType<OkObjectResult>(result);
            var okResult = Assert.IsAssignableFrom<ClientRefreshTokenResponse>(actionResultObj.Value);
            
            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, actionResultObj.StatusCode);
            Assert.Equal((int)ResultCodes.Ok, okResult.Code);
            Assert.True(okResult.Succeeded);
            Assert.Equal(expected_refresh_token, okResult.RefreshToken);
            
            //*-----------------------------------------------------*//
            _UserManagerMock.Verify(x => x.FindByEmailAsync(It.Is<string>(s => s.Equals(expected_email))));
            _UserManagerMock.Verify(x => x.GetRolesAsync(It.Is<IdentityUser>(s => s.Email.Equals(expected_email))));
            _AuthUtilitsMock.Verify(x=> x.CreateSessionToken(
                It.Is<IdentityUser>(s => s.NormalizedEmail.Equals(expected_email.ToUpper())), 
                It.IsAny<List<string>>()));
        }
        
        [Fact]
        public async Task ConfirmEmailAsync_Return_StatusCode200_And_Result()
        {
            //Arrage
            const string expected_id = "identificationNumber";
            const string expected_confirmToken = "confirmToken";
            const string expected_email = "pupkin@mail.ru";
            const string expected_name = "pupkin@mail.ru";
            const string expected_concurrencystamp = "concurrencyStamp";
            const string expected_securitystamp = "securityStamp";
            const string expected_message = $"Учетная запись {expected_email} успешно подтверждена";

            var identity_user = new IdentityUser()
            {
                Id = expected_id,
                UserName = expected_name,
                NormalizedUserName = expected_name.ToUpper(),
                NormalizedEmail = expected_email.ToUpper(),
                Email = expected_email,
                EmailConfirmed = true,
                ConcurrencyStamp = expected_concurrencystamp,
                SecurityStamp = expected_securitystamp
            };
            
            _UserManagerMock.Setup(x => x.FindByIdAsync(
                It.Is<string>(s => s.Equals(expected_id)))).ReturnsAsync(identity_user).Verifiable();

            var identity_result = new IdentityResultMock(true);
            _UserManagerMock.Setup(x => x.ConfirmEmailAsync(
                identity_user, expected_confirmToken)).ReturnsAsync(identity_result).Verifiable();
            
            //Act
            var result = await _AccountController.ConfirmEmailAsync(expected_id, expected_confirmToken);
            var actionResultObj = Assert.IsType<OkObjectResult>(result);
            var okResult = Assert.IsAssignableFrom<ClientConfirmEmailResponse>(actionResultObj.Value);
            
            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, actionResultObj.StatusCode);
            Assert.Equal((int)ResultCodes.Ok, okResult.Code);
            Assert.True(okResult.Succeeded);
            Assert.Equal(expected_message, okResult.Message);
            
            //*-----------------------------------------------------*//
            _UserManagerMock.Verify(x => x.FindByIdAsync(It.Is<string>(s => s.Equals(expected_id))));
            _UserManagerMock.Verify(x => x.ConfirmEmailAsync(
                It.Is<IdentityUser>(s => s.Email.Equals(identity_user.Email)), 
                It.Is<string>(s => s.Equals(expected_confirmToken))));
        }
        
        [Fact]
        public async Task CreateRoleAsync_Return_StatusCode200_And_Result()
        {
            //Arrage
            const string expected_urlAddress = "http://localhost:5185/auth/create_role";
            const string expected_email = "sa@mail.ru";
            const string expected_name = "sa@mail.ru";
            const string expected_role = "manager";
            
            var identity = new ClaimsIdentity();
            identity.AddClaims(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, expected_email),
                new Claim(ClaimTypes.Name, expected_name),
                new Claim(ClaimTypes.Role, expected_role)
            });
            var user = new ClaimsPrincipal(identity);
            
            _HttpContextMock = new HttpContextMock();
            _HttpContextMock.User = user;
            _HttpContextMock.SetupUrl(expected_urlAddress);
            _ContextAccessorMock.Setup(x => x.HttpContext).Returns(_HttpContextMock);
            _AccountController.ControllerContext.HttpContext = _HttpContextMock;

            var request = new CreateRoleRequest()
            {
                RoleName = expected_role,
            };
            
            const string expected_message = $"Роль {expected_role} успешно добавлена в систему";

            var identity_role = new IdentityRole(request.RoleName.ToLower());
            var identity_result = new IdentityResultMock(true);
            _RoleManagerMock.Setup(x => x.CreateAsync(
                It.Is<IdentityRole>(s => s.Name.Equals(identity_role.Name))))
                .ReturnsAsync(identity_result).Verifiable();
            
            //Act
            var result = await _AccountController.CreateRoleAsync(request);
            var actionResultObj = Assert.IsType<OkObjectResult>(result);
            var okResult = Assert.IsAssignableFrom<ClientCreateRoleResponse>(actionResultObj.Value);
            
            //Assert
            Assert.NotNull(okResult);
            Assert.Equal(200, actionResultObj.StatusCode);
            Assert.Equal((int)ResultCodes.Ok, okResult.Code);
            Assert.True(okResult.Succeeded);
            Assert.Equal(expected_message, okResult.Message);
            
            //*-----------------------------------------------------*//
            _RoleManagerMock.Verify(x => x.CreateAsync(It.Is<IdentityRole>(s => s.Name.Equals(identity_role.Name))));
        }

        // [Fact]
        // public async Task GetAllRolesAsync_Return_StatusCode200WithResult()
        // {
        //     //Arrage
        //     const string expected_urlAddress = "http://localhost:5185/auth/get_all_roles";
        //     const string expected_email = "sa@mail.ru";
        //     const string expected_name = "sa@mail.ru";
        //     const string expected_role_one = "admin";
        //     const string expected_role_two = "user";
        //     const string expected_role_one_id = "id001";
        //     const string expected_role_two_id = "id002";
        //
        //     var identity = new ClaimsIdentity();
        //     identity.AddClaims(new[]
        //     {
        //         new Claim(ClaimTypes.NameIdentifier, expected_email), new Claim(ClaimTypes.Name, expected_name),
        //         new Claim(ClaimTypes.Role, "admin")
        //     });
        //     var user = new ClaimsPrincipal(identity);
        //
        //     _HttpContextMock = new HttpContextMock();
        //     _HttpContextMock.User = user;
        //     _HttpContextMock.SetupUrl(expected_urlAddress);
        //     _ContextAccessorMock.Setup(x => x.HttpContext).Returns(_HttpContextMock);
        //     _AccountController.ControllerContext.HttpContext = _HttpContextMock;
        //
        //     var identity_roles_list = new List<IdentityRole>()
        //     {
        //         new IdentityRole(){Id = expected_role_one_id, Name = expected_role_one, NormalizedName = expected_role_one.ToUpper()},
        //         new IdentityRole(){Id = expected_role_two_id, Name = expected_role_two, NormalizedName = expected_role_two.ToUpper()}
        //     }.AsQueryable();
        //     
        //     //TODO: Есть проблема с ToListAsync... Буду думать...
        //     _RoleManagerMock.Setup(x => x.Roles)
        //         .Returns(identity_roles_list);
        //
        //     var auth_roles_list = new List<AuthRoles>();
        //      foreach (var role in identity_roles_list)
        //         auth_roles_list.Add(new AuthRoles() { RoleName = role.Name, Id = role.Id });
        //
        //     //Act
        //     var result = await _AccountController.GetAllRolesAsync();
        //     var actionResultObj = Assert.IsType<OkObjectResult>(result);
        //     var okResult = Assert.IsAssignableFrom<ClientGetAllRolesResponse>(actionResultObj.Value);
        //     
        //     //Assert
        //     Assert.NotNull(okResult);
        //     Assert.Equal(200, actionResultObj.StatusCode);
        //     Assert.Equal((int)ResultCodes.Ok, okResult.Code);
        //     Assert.True(okResult.Succeeded);
        //     Assert.Equal(auth_roles_list, okResult.Roles);
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