namespace Infrastructure.Adapters.Persistence.Mongo
{
    public class AuditEntityBuilder
    {
        private string _id = string.Empty;
        private string _action = string.Empty;
        private string _entityName = string.Empty;
        private string _user = string.Empty;
        private DateTime _date;
        private string _details = string.Empty;

        public AuditEntityBuilder WithId(string id) { _id = id; return this; }
        public AuditEntityBuilder WithAction(string action) { _action = action; return this; }
        public AuditEntityBuilder WithEntityName(string entityName) { _entityName = entityName; return this; }
        public AuditEntityBuilder WithUser(string user) { _user = user; return this; }
        public AuditEntityBuilder WithDate(DateTime date) { _date = date; return this; }
        public AuditEntityBuilder WithDetails(string details) { _details = details; return this; }

        public AuditEntity Build() => new()
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