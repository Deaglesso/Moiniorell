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
        Task<List<string>> CreateReply(CreateReplyVM vm);
        Task<List<string>> DeleteReply(int replyId, bool mood = false);
        Task<List<string>> DeleteComment(int commentId, bool mood = false);

    }
}
