using Moiniorell.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Domain.Models
{
    public class Post:BaseEntity
    {
        public string? Image { get; set; }
        public string Text { get; set; }
        public string? AuthorId { get; set; }

        public AppUser? Author { get; set; }
        public ICollection<Comment>? Comments { get; set; }
        public ICollection<Like>? Likes { get; set; }
        public int LikeCount { get; set; }


    }
}
