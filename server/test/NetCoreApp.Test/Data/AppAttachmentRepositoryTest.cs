using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Data.Repositories;
using Beginor.NetCoreApp.Models;
using NHibernate.NetCore;
using NUnit.Framework;

namespace Beginor.NetCoreApp.Test.Data {

    [TestFixture]
    public class AppAttachmentRepositoryTest : BaseTest<IAppAttachmentRepository> {

        [Test]
        public void _01_CanResolveTarget() {
            Assert.IsNotNull(Target);
        }

        [Test]
        public void _02_CanQueryWithUser() {
            var factory = ServiceProvider.GetSessionFactory();
            using (var session = factory.OpenSession()) {
                var query = from att in session.Query<AppAttachment>()
                    select new AppAttachment {
                        Id = att.Id,
                        BusinessId = att.BusinessId,
                        Creator = new AppUser {
                            Id = att.Creator.Id,
                            UserName = att.Creator.UserName
                        }
                    };
                var data = query.ToList();
                var models = Mapper.Map<List<AppAttachmentModel>>(data);
            }
        }

        [Test]
        public void _03_CanProjectToModel() {
            var factory = ServiceProvider.GetSessionFactory();
            using (var session = factory.OpenSession()) {
                var query = session.Query<AppAttachment>()
                    .Where(att => att.Creator.UserName == "TestUser")
                    .Select(att => new AppAttachment {
                        Id = att.Id,
                        FileName = att.FileName,
                        CreatedAt = att.CreatedAt,
                        Creator = new AppUser {
                            Id = att.Creator.Id,
                            UserName = att.Creator.UserName,
                            LoginCount = att.Creator.LoginCount,
                            LastLogin = att.Creator.LastLogin
                        }
                    });
                    //.ProjectTo<AppAttachmentModel>();
                var data = query.ToList();
            }
        }

    }

}
