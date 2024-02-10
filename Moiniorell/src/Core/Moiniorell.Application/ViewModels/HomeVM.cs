using Moiniorell.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.ViewModels
{
    public class HomeVM
    {
        public List<Post> Posts { get; set; }
        public CreatePostVM CreatePostVM { get; set; }
    }
}
