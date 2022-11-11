using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;

namespace SciMaterials.Identity.Tests.FakeServices
{
    public class FakeRoleManager : RoleManager<IdentityRole>
    {
        public Mock<IRoleStore<IdentityRole>> RoleStoreMock { get; }
        public Mock<ILookupNormalizer> LookupNormalizerMock { get; }
        public Mock<IdentityErrorDescriber> ErrorDescriberMock { get; }
        public Mock<ILogger<RoleManager<IdentityRole>>> RoleManagerMock { get; }

        public FakeRoleManager() : this(new(), new(), new(), new())
        {
            
        }

        public FakeRoleManager(
            Mock<IRoleStore<IdentityRole>> RoleStoreMock,
            Mock<ILookupNormalizer> LookupNormalizerMock,
            Mock<IdentityErrorDescriber> ErrorDescriberMock,
            Mock<ILogger<RoleManager<IdentityRole>>> RoleManagerMock
        )
            : base(
                RoleStoreMock.Object,
                Array.Empty<IRoleValidator<IdentityRole>>(),
                LookupNormalizerMock.Object,
                ErrorDescriberMock.Object,
                RoleManagerMock.Object)
        {
            this.RoleStoreMock        = RoleStoreMock;
            this.LookupNormalizerMock = LookupNormalizerMock;
            this.ErrorDescriberMock   = ErrorDescriberMock;
            this.RoleManagerMock      = RoleManagerMock;
        }
    }
}