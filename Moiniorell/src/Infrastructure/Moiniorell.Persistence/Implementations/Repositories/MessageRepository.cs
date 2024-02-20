using Moiniorell.Application.Abstractions.Repositories;
using Moiniorell.Domain.Models;
using Moiniorell.Persistence.DAL;
using Moiniorell.Persistence.Implementations.Repositories.Generic;

namespace Moiniorell.Persistence.Implementations.Repositories
{
    public class MessageRepository : Repository<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext db) : base(db)
        {
        }
    }
}
