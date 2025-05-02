using Grassroots.Application.Dispatchers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Grassroots.Api.Controllers
{
    /// <summary>
    /// API基础控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiBaseController : ControllerBase
    {
        private ICommandDispatcher _commandDispatcher;
        private IQueryDispatcher _queryDispatcher;

        /// <summary>
        /// 命令分发器
        /// </summary>
        protected ICommandDispatcher CommandDispatcher => _commandDispatcher ??= HttpContext.RequestServices.GetService<ICommandDispatcher>();

        /// <summary>
        /// 查询分发器
        /// </summary>
        protected IQueryDispatcher QueryDispatcher => _queryDispatcher ??= HttpContext.RequestServices.GetService<IQueryDispatcher>();
    }
} 