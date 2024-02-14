using Moiniorell.Application.Abstractions.Repositories.Generic;
using Moiniorell.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.Abstractions.Repositories
{
    public interface ICommentRepository:IRepository<Comment>
    {
    }
}
