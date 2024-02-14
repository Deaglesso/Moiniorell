using Moiniorell.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.Abstractions.Services
{
    public interface ICommentService
    {
        Task<List<string>> CreateComment(CreateCommentVM vm);

    }
}
