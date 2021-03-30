using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwetterBook.Contracts;
using TwetterBook.Contracts.Requests;
using TwetterBook.Domain;
using TwetterBook.Services;

namespace TwetterBook.Controllers
{
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class PostsController : Controller
    {

        private readonly IPostService _PostService;

        public PostsController(IPostService postService)
        {
            _PostService = postService;
        }


        [HttpGet(ApiRoutes.posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _PostService.GetPostsAsync());
        }


        [HttpPut(ApiRoutes.posts.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
        {

            var post = new Post
            {
                Id = postId,
                Name = request.Name
            };

            var updated = await _PostService.UpdatePostAsync(post);

            if (updated)
            {
                return Ok(post);
            }

            return NotFound();


        }


        [HttpDelete(ApiRoutes.posts.Delete)]

        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var deleted = await _PostService.DeletePostAsync(postId);

            if (deleted)
            {
                return NoContent();
            }

            return NotFound();


        }



        [HttpGet(ApiRoutes.posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _PostService.GetPostByIdAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            return Ok(post);

        }


        [HttpPost(ApiRoutes.posts.Create)]

        public async Task<IActionResult> Create([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post { Name = postRequest.Name };


            await _PostService.CreatePostAsync(post);

            var baseUri = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";
            var LocationUri = baseUri + "/" + ApiRoutes.posts.Get.Replace("{postId}", post.Id.ToString());

            var response = new PostResponse { Id = post.Id };
            return Created(LocationUri, response);



        }

    }
}
