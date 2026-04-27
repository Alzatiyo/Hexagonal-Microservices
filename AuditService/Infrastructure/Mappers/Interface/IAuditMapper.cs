using Domain.Models;
using Infrastructure.Adapters.Persistence.Mongo;

namespace Infrastructure.Mappers.Interface
{
    public interface IAuditMapper
    {
        AuditEntity ToEntity(Audit domain);
        Audit ToDomain(AuditEntity entity);
    }
}