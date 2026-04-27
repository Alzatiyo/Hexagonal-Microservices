using Domain.Models;

namespace Application.Ports.Out
{
    public interface IAuditRepositoryPort
    {
        Task SaveAsync(Audit audit);
        Task<IEnumerable<Audit>> FindAllAsync();
    }
}