using SciMaterials.DAL.Resources.Contexts;
using SciMaterials.DAL.Resources.UnitOfWork;
using SciMaterials.RepositoryTests.Helpers;

namespace SciMaterials.RepositoryTests.Fixtures;

public class UnitOfWorkFixture
{
    public IUnitOfWork<SciMaterialsContext> Create()
    {
        var mock = UnitOfWorkHelper.GetUnitOfWorkMock();
        return mock.Object;
    }
}