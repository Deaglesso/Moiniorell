using Microsoft.AspNetCore.Identity;
using Moiniorell.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Domain.Models
{
    public class AppUser : IdentityUser
    {
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public DateTime RegisteredAt { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public int PostCount { get; set; }
        public int FollowingCount { get; set; }
        public int FollowerCount { get; set; }
        public Gender Gender { get; set; }
        public string? Biography { get; set; }
        public bool IsPrivate { get; set; }
        public bool Availability { get; set; }
        public bool IsActive { get; set; }
        public string ProfilePicture { get; set; }
        public ICollection<Follow> Followers { get; set; }
        public ICollection<Like>? Likes { get; set; }
        public ICollection<Follow> Followees { get; set; }
        public ICollection<Post> Posts { get; set; }
        public ICollection<Comment> Comments { get; set; }
        public ICollection<UserConnection> UserConnections { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
        public AppUser()
        {
            RegisteredAt = DateTime.UtcNow;
            ProfilePicture = "https://res.cloudinary.com/dk8mhooq1/image/upload/v1708514190/f8kgsgzed1whaih4gn90.png";
            Availability = true;
            Messages = new HashSet<Message>();
        }
    }
}
