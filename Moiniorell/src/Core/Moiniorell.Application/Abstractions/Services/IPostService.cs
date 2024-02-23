using Moiniorell.Application.ViewModels;
using Moiniorell.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moiniorell.Application.Abstractions.Services
{
    public interface IPostService
    {
        Task<List<string>> CreatePost(CreatePostVM vm);

        Task<List<Post>> GetPosts();
        Task<List<Post>> GetPostsByPage(int page = 1, int pageSize = 3);
        Task<Post> GetPost(int postId);

        Task LikePost(int postId);
        Task UnlikePost(int postId);
        Task DeletePost(int postId);


    }
}
