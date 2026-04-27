using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Domain.Models;

namespace Domain.Builders
{
    public class AuditBuilder
    {
        private string _id = string.Empty;
        private string _action = string.Empty;
        private string _entityName = string.Empty;
        private string _user = string.Empty;
        private DateTime _date;
        private string _details = string.Empty;

        public AuditBuilder WithId(string id) { _id = id; return this; }
        public AuditBuilder WithAction(string action) { _action = action; return this; }
        public AuditBuilder WithEntityName(string entityName) { _entityName = entityName; return this; }
        public AuditBuilder WithUser(string user) { _user = user; return this; }
        public AuditBuilder WithDate(DateTime date) { _date = date; return this; }
        public AuditBuilder WithDetails(string details) { _details = details; return this; }

        public Audit Build() => new Audit
        {
            Id = _id,
            Action = _action,
            EntityName = _entityName,
            User = _user,
            Date = _date,
            Details = _details
        };
    }
}