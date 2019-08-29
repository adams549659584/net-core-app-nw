using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Beginor.AppFx.Api;
using Beginor.AppFx.Core;
using Beginor.NetCoreApp.Models;
using Beginor.NetCoreApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Beginor.NetCoreApp.Api.Controllers {

    /// <summary>系统权限服务接口</summary>
    [Route("api/app-privileges")]
    [ApiController]
    public class AppPrivilegeController : Controller {

        log4net.ILog logger = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType
        );

        private IAppPrivilegeService service;

        public AppPrivilegeController(IAppPrivilegeService service) {
            this.service = service;
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                service = null;
            }
            base.Dispose(disposing);
        }

        /// <summary> 创建 系统权限  </summary>
        /// <response code="200">创建 系统权限 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPost("")]
        public async Task<ActionResult<AppPrivilegeModel>> Create(
            [FromBody]AppPrivilegeModel model
        ) {
            try {
                await service.CreateAsync(model);
                return model;
            }
            catch (Exception ex) {
                logger.Error("Can not create app_privileges.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>删除 系统权限 </summary>
        /// <response code="204">删除 系统权限 成功</response>
        /// <response code="500">服务器内部错误</response>
        [HttpDelete("{id:long}")]
        [ProducesResponseType(204)]
        public async Task<ActionResult> Delete(string id) {
            try {
                await service.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex) {
                logger.Error("Can not delete app_privileges.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>搜索 系统权限 ， 分页返回结果</summary>
        /// <response code="200">成功, 分页返回结果</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("")]
        public async Task<ActionResult<PaginatedResponseModel<AppPrivilegeModel>>> GetAll(
            [FromQuery]AppPrivilegeSearchModel model
        ) {
            try {
                var result = await service.SearchAsync(model);
                return result;
            }
            catch (Exception ex) {
                logger.Error("Can not get all app_privileges.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 获取指定的 系统权限
        /// </summary>
        /// <response code="200">返回 系统权限 信息</response>
        /// <response code="404"> 系统权限 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("{id:long}")]
        public async Task<ActionResult<AppPrivilegeModel>> GetById(string id) {
            try {
                var result = await service.GetByIdAsync(id);
                if (result == null) {
                    return NotFound();
                }
                return result;
            }
            catch (Exception ex) {
                logger.Error($"Can not get app_privileges with {id}.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>
        /// 更新 系统权限
        /// </summary>
        /// <response code="200">更新成功，返回 系统权限 信息</response>
        /// <response code="404"> 系统权限 不存在</response>
        /// <response code="500">服务器内部错误</response>
        [HttpPut("{id:long}")]
        public async Task<ActionResult<AppPrivilegeModel>> Update(
            [FromRoute]string id,
            [FromBody]AppPrivilegeModel model
        ) {
            try {
                var modelInDb = await service.GetByIdAsync(id);
                if (modelInDb == null) {
                    return NotFound();
                }
                await service.UpdateAsync(id, model);
                return model;
            }
            catch (Exception ex) {
                logger.Error($"Can not update app_privileges with {id}.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

        /// <summary>同步系统必须的权限</summary>
        /// <response code="200">同步成功， 客户端刷新权限列表即可看到更新。</response>
        /// <response code="500">服务器内部错误</response>
        [HttpGet("sync/required")]
        public async Task<ActionResult> SyncRequired() {
            try {
                var assembly = GetType().Assembly;
                var policies = assembly.ExportedTypes
                    .Where(t => t.IsSubclassOf(typeof(ControllerBase)))
                    .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                    .SelectMany(m => m.GetCustomAttributes<AuthorizeAttribute>(false))
                    .Select(attr => attr.Policy);
                await service.SyncRequired(policies);
                return Ok();
            }
            catch (Exception ex) {
                logger.Error($"Can not sync required app_privileges.", ex);
                return this.InternalServerError(ex.GetOriginalMessage());
            }
        }

    }

}