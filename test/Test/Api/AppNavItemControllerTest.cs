using NUnit.Framework;
using Beginor.NetCoreApp.Api.Controllers;

namespace Beginor.NetCoreApp.Test.Api;

[TestFixture]
public class AppNavItemControllerTest : BaseTest<AppNavItemController> {

    [Test]
    public void _01_CanResolveTarget() {
        Assert.IsNotNull(Target);
    }

}
