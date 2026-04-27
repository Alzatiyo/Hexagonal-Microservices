using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Events
{
    /// <summary>
    ///     This enum represents the type of action a product.
    /// </summary>
    public enum ProductAction
    {
        Created,
        Updated,
        Deleted,
        ExitRegistered
    }
}