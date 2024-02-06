using Moiniorell.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Domain.Models
{
    public class Follow:BaseEntity
    {
        public string FollowerId { get; set; }
        public string FolloweeId { get; set; }
        public AppUser Follower { get; set; }
        public AppUser Followee { get; set; }
    }
}
