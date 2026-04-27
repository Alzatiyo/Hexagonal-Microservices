using Application.Ports.Out;
using Domain.Models;
using MongoDB.Driver.Linq;
using Infrastructure.Config;
using Infrastructure.Mappers.Interface;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Adapters.Persistence.Mongo
{
    public class AuditAdapter : IAuditRepositoryPort
    {
        private readonly IMongoCollection<AuditEntity> _collection;
        private readonly IAuditMapper _mapper;

        public AuditAdapter(MongoDbContext context, IOptions<MongoDbSettings> settings, IAuditMapper mapper)
        {
            _collection = context.GetCollection<AuditEntity>(settings.Value.AuditCollection);
            _mapper = mapper;
        }

        public async Task SaveAsync(Audit audit)
        {
            var entity = _mapper.ToEntity(audit);
            await _collection.InsertOneAsync(entity);
        }

        public async Task<IEnumerable<Audit>> FindAllAsync()
        {
            var entities = await _collection.Find(FilterDefinition<AuditEntity>.Empty).ToListAsync();
            return entities.Select(_mapper.ToDomain).ToList();
        }
    }
}