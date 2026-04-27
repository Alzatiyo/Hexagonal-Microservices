using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Audit
    {
        public string Id { get; set; }
        public string Action { get; set; }
        public string EntityName { get; set; }
        public string User { get; set; }
        public DateTime Date { get; set; }
        public string Details { get; set; }
    }
}