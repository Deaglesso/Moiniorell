using AutoMapper;
using Microsoft.AspNetCore.Http;
using Moiniorell.Application.Abstractions.Repositories;
using Moiniorell.Application.Abstractions.Services;
using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;

namespace Moiniorell.Persistence.Implementations.Services
{
    public class CommentService:ICommentService
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
            reply.RepliedCommentId = vm.CommentId;
            await _replyRepo.CreateAsync(reply);
            await _replyRepo.SaveChangesAsync();

            return str;
        }
    }
}
