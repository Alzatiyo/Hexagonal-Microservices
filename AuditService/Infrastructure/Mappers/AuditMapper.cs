using Domain.Builders;
using Domain.Models;
using Infrastructure.Adapters.Persistence.Mongo;
using Infrastructure.Mappers.Interface;

namespace Infrastructure.Mappers
{
    public class AuditMapper : IAuditMapper
    {
        public AuditEntity ToEntity(Audit domain) =>
            new AuditEntityBuilder()
                .WithId(domain.Id)
                .WithAction(domain.Action)
                .WithEntityName(domain.EntityName)
                .WithUser(domain.User)
                .WithDate(domain.Date)
                .WithDetails(domain.Details)
                .Build();

        public Audit ToDomain(AuditEntity entity) =>
            new AuditBuilder()
                .WithId(entity.Id)
                .WithAction(entity.Action)
                .WithEntityName(entity.EntityName)
                .WithUser(entity.User)
                .WithDate(entity.Date)
                .WithDetails(entity.Details)
                .Build();
    }
}