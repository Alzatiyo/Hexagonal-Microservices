using Application.Dtos;
using Application.Ports.In;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Adapters.Rest
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly IAuditUseCasePort _useCase;

        public AuditController(IAuditUseCasePort useCase)
        {
            _useCase = useCase;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAuditRequest request)
        {
            await _useCase.CreateAsync(request);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _useCase.GetAllAsync();
            return Ok(result);
        }
    }
}