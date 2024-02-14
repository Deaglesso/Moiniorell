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
        private readonly IHttpContextAccessor _http;

        public CommentService(IMapper mapper, ICommentRepository commentRepo, IHttpContextAccessor http)
        {
            _mapper = mapper;
            _commentRepo = commentRepo;
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
    }
}
