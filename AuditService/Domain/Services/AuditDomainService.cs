using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Builders;
using Domain.Models;

namespace Domain.Services
{
    public class AuditDomainService
    {
        public Audit Build(string action, string entityName, string user, string details)
        {
            return new AuditBuilder()
                .WithId(Guid.NewGuid().ToString())
                .WithAction(action)
                .WithEntityName(entityName)
                .WithUser(user)
                .WithDate(DateTime.UtcNow)
                .WithDetails(details)
                .Build();
        }
    }
}