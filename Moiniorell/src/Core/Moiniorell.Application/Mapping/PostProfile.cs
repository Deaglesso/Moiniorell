using AutoMapper;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.Mapping
{
    public class PostProfile: Profile
    {
        public PostProfile()
        {
            CreateMap<CreatePostVM, Post>();
        }
    }
}
