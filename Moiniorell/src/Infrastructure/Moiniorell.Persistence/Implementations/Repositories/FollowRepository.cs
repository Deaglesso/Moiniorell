using Moiniorell.Application.Abstractions.Repositories;
using Moiniorell.Domain.Models;
using Moiniorell.Persistence.DAL;
using Moiniorell.Persistence.Implementations.Repositories.Generic;

namespace Moiniorell.Persistence.Implementations.Repositories
{
    public class FollowRepository : Repository<Follow>, IFollowRepository
    {
        public FollowRepository(AppDbContext db) : base(db)
        {
        }
    }
}
