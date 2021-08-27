using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using AutoMapper;
using Beginor.AppFx.Core;
using Beginor.AppFx.Repository.Hibernate;
using NHibernate;
using NHibernate.Linq;
using Beginor.NetCoreApp.Common;
using Beginor.NetCoreApp.Data.Entities;
using Beginor.NetCoreApp.Models;

namespace Beginor.NetCoreApp.Data.Repositories {

    /// <summary>服务器目录仓储实现</summary>
    public partial class ServerFolderRepository : HibernateRepository<ServerFolder, ServerFolderModel, long>, IServerFolderRepository {

        private readonly IDistributedCache cache;
        private readonly IWebHostEnvironment hostEnv;

        public ServerFolderRepository(ISession session, IMapper mapper, IDistributedCache cache, IWebHostEnvironment hostEnv) : base(session, mapper) {
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this.hostEnv = hostEnv ?? throw new ArgumentNullException(nameof(hostEnv));
        }

        /// <summary>搜索 服务器目录 ，返回分页结果。</summary>
        public async Task<PaginatedResponseModel<ServerFolderModel>> SearchAsync(
            ServerFolderSearchModel model
        ) {
            var query = Session.Query<ServerFolder>();
            var keywords = model.Keywords;
            if (keywords.IsNotNullOrEmpty()) {
                query = query.Where(
                    f => f.AliasName.Contains(keywords) || f.RootFolder.Contains(keywords)
                );
            }
            var total = await query.LongCountAsync();
            var data = await query.OrderByDescending(e => e.Id)
                .Skip(model.Skip).Take(model.Take)
                .ToListAsync();
            return new PaginatedResponseModel<ServerFolderModel> {
                Total = total,
                Data = Mapper.Map<IList<ServerFolderModel>>(data),
                Skip = model.Skip,
                Take = model.Take
            };
        }

        public async Task<ServerFolderBrowseModel> GetFolderContentAsync(ServerFolderBrowseModel model) {
            var folderItem = await GetByAlias(model.Alias);
            if (folderItem == null) {
                return null;
            }
            if (model.Path.StartsWith(Path.DirectorySeparatorChar)) {
                model.Path = model.Path.Substring(1);
            }
            var cachedItem = await GetCacheItemAsync(folderItem.Id);
            var serverPath = Path.Combine(cachedItem.RootFolder, model.Path);
            var dirInfo = new DirectoryInfo(serverPath);
            if (!dirInfo.Exists) {
                return null;
            }
            model.Folders = dirInfo.EnumerateDirectories().Select(x => x.Name).ToArray();
            model.Files = dirInfo.EnumerateFiles(model.Filter).Select(x => x.Name).ToArray();
            return model;
        }

        public async Task<string> GetPhysicalPathAsync(string aliasedPath) {
            Argument.NotNullOrEmpty(aliasedPath, nameof(aliasedPath));
            var idx = aliasedPath.IndexOf(':');
            if (idx < 0) {
                throw new InvalidOperationException($"Invalid path {aliasedPath}");
            }
            var alias = aliasedPath.Substring(0, idx);
            var path = aliasedPath.Substring(idx + 1);
            var folderItem = await GetByAlias(alias);
            if (folderItem == null) {
                return string.Empty;
            }
            if (path.StartsWith(Path.PathSeparator)) {
                path = path.Substring(1);
            }
            var cachedItem = await GetCacheItemAsync(folderItem.Id);
            var serverPath = Path.Combine(cachedItem.RootFolder, path);
            if (Directory.Exists(serverPath) || File.Exists(serverPath)) {
                return serverPath;
            }
            return string.Empty;
        }

        public async Task<Stream> GetFileContentAsync(string alias, string path) {
            Argument.NotNullOrEmpty(alias, nameof(alias));
            Argument.NotNullOrEmpty(path, nameof(path));
            var physicalPath = await GetPhysicalPathAsync($"{alias}:{path}");
            if (physicalPath.IsNullOrEmpty()) {
                return null;
            }
            return File.OpenRead(physicalPath);
        }

        private async Task<ServerFolder> GetByAlias(string alias) {
            var folder = await Session.Query<ServerFolder>()
                .FirstOrDefaultAsync(x => x.AliasName == alias);
            return folder;
        }

        private async Task<ServerFolderCacheItem> GetCacheItemAsync(long id) {
            var key = id.ToString();
            var item = await cache.GetAsync<ServerFolderCacheItem>(key);
            if (item == null) {
                var entity = await Session.LoadAsync<ServerFolder>(id);
                item = new ServerFolderCacheItem {
                    Id = entity.Id,
                    AliasName = entity.AliasName,
                    Readonly = entity.Readonly,
                    Roles = entity.Roles
                };
                var rootFolder = entity.RootFolder;
                if (rootFolder.StartsWith("!")) {
                    rootFolder = rootFolder.Substring(1);
                    if (rootFolder.StartsWith(Path.DirectorySeparatorChar)) {
                        rootFolder = rootFolder.Substring(1);
                    }
                }
                rootFolder = Path.Combine(hostEnv.WebRootPath, rootFolder);
                item.RootFolder = rootFolder;
                await cache.SetAsync(key, item);
            }
            return item;
        }

    }

}