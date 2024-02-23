using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moiniorell.Application.Abstractions.Repositories;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;

namespace Moiniorell.Persistence.Implementations.Services
{
    public class CommentService : ICommentService
    {
        private readonly IMapper _mapper;
        private readonly ICommentRepository _commentRepo;
        private readonly IReplyRepository _replyRepo;
        private readonly IHttpContextAccessor _http;

        public CommentService(IMapper mapper, ICommentRepository commentRepo, IReplyRepository replyRepo, IHttpContextAccessor http)
        {
            _mapper = mapper;
            _commentRepo = commentRepo;
            _replyRepo = replyRepo;
            _http = http;
        }

        public async Task<List<string>> CreateComment(CreateCommentVM vm)
        {
            List<string> str = new List<string>();
            Comment comment = _mapper.Map<Comment>(vm);

            comment.AuthorId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (comment.AuthorId is null)
            {
                throw new Exception("user error");
            }
            comment.CommentedPostId = vm.PostId;
            await _commentRepo.CreateAsync(comment);
            await _commentRepo.SaveChangesAsync();

            return str;
        }
        public async Task<List<string>> CreateReply(CreateReplyVM vm)
        {
            List<string> str = new List<string>();
            Reply reply = _mapper.Map<Reply>(vm);

            reply.AuthorId = _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (reply.AuthorId is null)
            {
                throw new Exception("user error");
            }

            reply.RepliedCommentId = vm.CommentId;
            await _replyRepo.CreateAsync(reply);
            await _replyRepo.SaveChangesAsync();

            return str;
        }
        public async Task<List<string>> DeleteComment(int commentId, bool mood = false)
        {
            List<string> errors = new List<string>();

            Comment commentToDelete = await _commentRepo.GetSingleAsync(x => x.Id == commentId, "Replies.Author");
            if (mood != true)
            {

                if (commentToDelete.AuthorId != _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
                {
                    throw new Exception("access denied");
                }

            }
            if (commentToDelete == null)
            {
                errors.Add("Comment not found");
                return errors;
            }
            if (commentToDelete.Replies.Any())
            {
                foreach (var reply in commentToDelete.Replies)
                {
                    await DeleteReply(reply.Id, true);
                }
            }

            _commentRepo.Delete(commentToDelete);
            await _commentRepo.SaveChangesAsync();

            return errors;
        }



        public async Task<List<string>> DeleteReply(int replyId, bool mode = false)
        {
            List<string> errors = new List<string>();

            Reply replyToDelete = await _replyRepo.GetSingleAsync(x => x.Id == replyId, "Author");
            if (mode != true)
            {
                if (replyToDelete.AuthorId != _http.HttpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value)
                {
                    throw new Exception("access denied");
                }
            }
            if (replyToDelete == null)
            {
                errors.Add("Reply not found");
                return errors;
            }


            _replyRepo.Delete(replyToDelete);
            await _replyRepo.SaveChangesAsync();

            return errors;
        }


    }
}
