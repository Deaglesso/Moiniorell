using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.ViewModels
{
    public class CreatePostVM
    {
        public IFormFile? File { get; set; }
        [Required]
        [MinLength(5)]
        public string Text { get; set; }
    }
}
