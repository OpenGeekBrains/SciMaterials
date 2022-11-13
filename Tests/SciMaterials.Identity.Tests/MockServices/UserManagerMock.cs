using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace SciMaterials.Identity.Tests.MockServices
{
    public class UserManagerMock : UserManager<IdentityUser>
    {
        public UserManagerMock() : base(
            new Mock<IUserStore<IdentityUser>>().Object,
            new Mock<IOptions<IdentityOptions>>().Object,
            new Mock<IPasswordHasher<IdentityUser>>().Object,
            new[] {new Mock<IUserValidator<IdentityUser>>().Object},
            new[] { new Mock<IPasswordValidator<IdentityUser>>().Object },
            new Mock<ILookupNormalizer>().Object,
            new Mock<IdentityErrorDescriber>().Object,
            new Mock<IServiceProvider>().Object,
            new Mock<ILogger<UserManager<IdentityUser>>>().Object)
        { }
    }
}