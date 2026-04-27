using Application.Dtos;
using Domain.Models;

namespace Application.Ports.In
{
    public interface IAuditUseCasePort
    {
        Task CreateAsync(CreateAuditRequest request);
        Task<IEnumerable<Audit>> GetAllAsync();
    }
}