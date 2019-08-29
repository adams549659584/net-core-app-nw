using System;
using System.Linq;
using System.Reflection;
using Beginor.NetCoreApp.Api.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Api {

    [TestFixture]
    public class AppPrivilegeControllerTest : BaseTest<AppPrivilegeController> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public void _02_CanReflectPrivileges() {
            var asm = Target.GetType().Assembly;
            var attrs = asm.ExportedTypes
                .Where(t => t.IsSubclassOf(typeof(ControllerBase)))
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                .SelectMany(m => m.GetCustomAttributes<AuthorizeAttribute>(false));
            Assert.IsTrue(attrs.Count() > 0);
            foreach (var attr in attrs) {
                Console.WriteLine(attr.Policy);
            }
        }

    }

}