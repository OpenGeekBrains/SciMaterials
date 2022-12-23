using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace SciMaterials.Identity.Tests.MockServices
{
    public class RoleManagerMock : RoleManager<IdentityRole>
    {
        public RoleManagerMock() : base(
            new Mock<IRoleStore<IdentityRole>>().Object, 
            Array.Empty<IRoleValidator<IdentityRole>>(),
            new Mock<ILookupNormalizer>().Object, 
            new Mock<IdentityErrorDescriber>().Object, 
            new Mock<ILogger<RoleManager<IdentityRole>>>().Object)
        { }
    }
}