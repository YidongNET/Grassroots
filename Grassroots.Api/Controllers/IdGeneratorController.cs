using Grassroots.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;

namespace Grassroots.Api.Controllers;

/// <summary>
/// ID生成器控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class IdGeneratorController : ControllerBase
{
    private readonly IIdGenerator _idGenerator;
    private readonly ILogger _logger;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="idGenerator">ID生成器</param>
    /// <param name="logger">日志记录器</param>
    public IdGeneratorController(IIdGenerator idGenerator, ILogger logger)
    {
        _idGenerator = idGenerator;
        _logger = logger;
    }

    /// <summary>
    /// 获取新的唯一ID
    /// </summary>
    /// <returns>雪花算法生成的ID</returns>
    [HttpGet]
    public IActionResult GetNewId()
    {
        _logger.Information("正在生成新的唯一ID");
        var id = _idGenerator.NextId();
        
        _logger.Debug("生成的ID: {Id}, 二进制表示: {BinaryRepresentation}", 
            id, Convert.ToString(id, 2).PadLeft(64, '0'));
        
        return Ok(new
        {
            Id = id,
            BinaryRepresentation = Convert.ToString(id, 2).PadLeft(64, '0'),
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        });
    }
    
    /// <summary>
    /// 批量获取唯一ID
    /// </summary>
    /// <param name="count">ID数量</param>
    /// <returns>ID列表</returns>
    [HttpGet("batch/{count}")]
    public IActionResult GetBatchIds([FromRoute] int count)
    {
        if (count <= 0 || count > 1000)
        {
            _logger.Warning("请求的ID数量 {Count} 超出范围", count);
            return BadRequest("请求数量必须在1到1000之间");
        }
        
        _logger.Information("正在批量生成 {Count} 个唯一ID", count);
        
        var ids = new List<long>();
        for (int i = 0; i < count; i++)
        {
            ids.Add(_idGenerator.NextId());
        }
        
        _logger.Debug("成功生成 {Count} 个唯一ID", ids.Count);
        
        return Ok(new
        {
            Count = ids.Count,
            Ids = ids
        });
    }
}
 