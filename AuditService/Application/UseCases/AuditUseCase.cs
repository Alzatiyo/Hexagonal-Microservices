using Application.Dtos;
using Application.Ports.In;
using Application.Ports.Out;
using Domain.Models;
using Domain.Services;

namespace Application.UseCases
{
    public class AuditUseCase : IAuditUseCasePort
    {
        private readonly IAuditRepositoryPort _repository;
        private readonly AuditDomainService _domainService;

        public AuditUseCase(IAuditRepositoryPort repository, AuditDomainService domainService)
        {
            _repository = repository;
            _domainService = domainService;
        }

        public async Task CreateAsync(CreateAuditRequest request)
        {
            var audit = _domainService.Build(
                request.Action,
                request.EntityName,
                request.User,
                request.Details);

            await _repository.SaveAsync(audit);
        }

        public async Task<IEnumerable<Audit>> GetAllAsync()
        {
            return await _repository.FindAllAsync();
        }
    }
}