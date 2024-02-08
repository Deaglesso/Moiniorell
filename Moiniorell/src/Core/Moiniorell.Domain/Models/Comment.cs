using Moiniorell.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Domain.Models
{
    public class Comment : BaseEntity
    {
        public string Text { get; set; }
        public string? AuthorId { get; set; }
        public AppUser? Author { get; set; }
        public int? CommentedPostId { get; set; }
        public Post? CommentedPost { get; set; }


    }
}
