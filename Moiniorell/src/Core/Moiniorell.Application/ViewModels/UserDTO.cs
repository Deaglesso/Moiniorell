using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.ViewModels
{
    public class UserDTO
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string  Fullname { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsOnline { get; set; }
    }
}
