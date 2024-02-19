using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Domain.Models
{
    public class UserConnection
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ConnectionId { get; set; }
        public AppUser User { get; set; }

    }
}
