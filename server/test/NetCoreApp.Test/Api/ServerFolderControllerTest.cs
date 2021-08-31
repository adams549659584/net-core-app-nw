using Beginor.NetCoreApp.Api.Controllers;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Api {

    [TestFixture]
    public class ServerFolderControllerTest : BaseTest<AppStorageController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

    }

}