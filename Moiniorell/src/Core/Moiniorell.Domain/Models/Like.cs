using Moiniorell.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Domain.Models
{
    public class Like:BaseEntity 
    {
        public string LikerId { get; set; }
        public int PostId { get; set; }
        public AppUser Liker { get; set; }
        public Post Post { get; set; }
    }
}
