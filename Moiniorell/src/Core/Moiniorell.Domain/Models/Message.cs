using Moiniorell.Domain.Models.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Domain.Models
{
    public class Message:BaseEntity

    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Text { get; set; }
        public string UserId { get; set; }
        public  AppUser User { get; set; }

       
    }
}
