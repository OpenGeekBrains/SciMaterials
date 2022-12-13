using Microsoft.AspNetCore.Identity;

namespace SciMaterials.Identity.Tests.MockServices
{
    public class IdentityResultMock : IdentityResult
    {
        public IdentityResultMock(bool succeeded)
        {
            Succeeded = succeeded;
        }
    }
}