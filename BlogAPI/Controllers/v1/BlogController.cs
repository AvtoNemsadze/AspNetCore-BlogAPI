using BlogAPI.Core.Dtos;
using BlogAPI.Core.Entities;
using BlogAPI.Core.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace BlogAPI.Controllers
{
    [Route("api/v{version:apiVersion}/blogs")]
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        const int maxBlogsPageSize = 20;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService ?? throw new ArgumentNullException(nameof(blogService));
        }


        /// <summary>Creates a new blog post.</summary>
        /// <param name="blogCreateDto">Data for creating a new blog post.</param>
        /// <returns>
        /// 200 OK: Blog created successfully.
        /// 400 Bad Request: Invalid input data.
        /// 500 Internal Server Error: Error during blog creation.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> CreateBlog([FromBody] BlogCreateDto blogCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _blogService.CreateBlogAsync(blogCreateDto);
                return Ok("Blog Created Successfully");
            }

            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message); 
            }

            catch (Exception)
            {
                return StatusCode(500, $"An error occurred while creating the Blog.");
            }
        }


        /// <summary>Retrieves a list of blog posts with optional pagination and filtering.</summary>
        /// <param name="pageNumber">The page number for pagination (default is 1).</param>
        /// <param name="pageSize">The number of blog posts per page (default is 10).</param>
        /// <param name="publishDateFilter">Optional filter to retrieve posts published on or after a specific date.</param>
        /// <returns>
        /// 200 OK: Returns a list of blog posts with pagination metadata in response headers.
        /// 500 Internal Server Error: An error occurred while retrieving blog posts.
        /// </returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogGetDto>>> GetBlogs(int pageNumber = 1, int pageSize = 10, DateTime? publishDateFilter = null)
        {
            try
            {
                if (pageSize > maxBlogsPageSize)
                {
                    pageSize = maxBlogsPageSize;
                }

                var (blogEntities, paginationMetadata) = await _blogService.GetBlogsAsync(pageNumber, pageSize, publishDateFilter);

                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

                var blogList = new List<Blog>();

                foreach (var blogEntity in blogEntities)
                {
                    var blog = new Blog
                    {
                        Id = blogEntity.Id,
                        Title = blogEntity.Title,
                        Body = blogEntity.Body,
                        PublishDate = blogEntity.PublishDate,
                    };

                    blogList.Add(blog);
                }

                return Ok(blogList);

            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving blogs.");
            }
        }


        /// <summary>Retrieves a blog post by its unique identifier.</summary>
        /// <param name="id">The unique identifier of the blog post to retrieve.</param>
        /// <returns>
        /// 200 OK: Returns the blog post with the specified ID.
        /// 404 Not Found: If a blog post with the specified ID does not exist.
        /// 500 Internal Server Error: An error occurred while retrieving the blog post.
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBlogById(int id)
        {
            try
            {
                var blog = await _blogService.GetBlogByIdAsync(id);

                if(blog == null)
                {
                    return NotFound();
                }

                return Ok(blog);
            }

            catch (Exception)
            {

                return StatusCode(500, "An error occurred while updating the blog.");
            }
        }


        /// <summary>Updates an existing blog post by its unique identifier.</summary>
        /// <param name="id">The unique identifier of the blog post to update.</param>
        /// <param name="updatedBlog">The updated blog post data.</param>
        /// <returns>
        /// 200 OK: Returns the updated blog post.
        /// 400 Bad Request: If the provided input is invalid.
        /// 404 Not Found: If a blog post with the specified ID does not exist.
        /// 500 Internal Server Error: An error occurred while updating the blog post.
        /// </returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(int id, [FromBody] BlogUpdateDto updatedBlog)
        {
            if (updatedBlog == null)
            {
                return NotFound();
            }

            try
            {
                var blogToUpdate = await _blogService.UpdateBlogAsync(id, updatedBlog);

                return Ok(blogToUpdate);
            }

            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred while updating the blog.");
            }
        }


        /// <summary>Deletes a blog post by its unique identifier.</summary>
        /// <param name="id">The unique identifier of the blog post to delete.</param>
        /// <returns>
        /// 200 OK: Indicates a successful deletion.
        /// 400 Bad Request: If there is a problem with the request (e.g., invalid ID).
        /// 404 Not Found: If a blog post with the specified ID does not exist.
        /// 500 Internal Server Error: An error occurred during the deletion process.
        /// </returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            try
            {
                await _blogService.DeleteBlogAsync(id);
                return Ok("Blog deleted successfully");
            }

            catch (NullReferenceException)
            {
                return NotFound("Blog not found");
            }

            catch (DbUpdateException)
            {
                return BadRequest("An error occurred while deleting the blog post. Please check related constraints.");
            }

            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the blog post.");
            }
        }

    }
}
