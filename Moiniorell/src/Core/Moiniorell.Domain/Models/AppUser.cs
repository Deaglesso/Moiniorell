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
        public bool IsOnline { get; set; }
        public bool IsActive { get; set; }
        public string ProfilePicture { get; set; }
        public ICollection<Follow> Followers { get; set; }

        public ICollection<Follow> Followees { get; set; }
        public AppUser()
        {
            RegisteredAt = DateTime.UtcNow;
            ProfilePicture = "nopic.png";
        }
    }
}
