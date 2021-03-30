using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwetterBook.Domain;

namespace TwetterBook.Services
{
   public interface IPostService
    {
       Task<List<Post>> GetPostsAsync();
       Task<Post> GetPostByIdAsync(Guid postId);
        Task<bool> UpdatePostAsync(Post postToUpdate);
        Task<bool> DeletePostAsync(Guid postId);
        Task<bool> CreatePostAsync(Post post);



    }
}
