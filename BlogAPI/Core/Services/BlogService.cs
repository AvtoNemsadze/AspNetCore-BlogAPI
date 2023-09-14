using BlogAPI.Core.DbContexts;
using BlogAPI.Core.Dtos;
using BlogAPI.Core.Entities;
using BlogAPI.Core.Interface;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Core.Services
{
    public class BlogService : IBlogService
    {
        private readonly ApplicationDbContext _context;
        public BlogService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        /// <summary>
        /// Creates a new blog post.
        /// </summary>
        /// <param name="blogCreateDto">The DTO containing blog post information.</param>
        /// <returns>The created blog post.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="blogCreateDto"/> is null.</exception>
        public async Task<Blog> CreateBlogAsync(BlogCreateDto blogCreateDto)
        {
            if (blogCreateDto == null)
            {
                throw new ArgumentNullException(nameof(blogCreateDto), "The blogCreateDto parameter is null.");
            }


            var newBlog = new Blog
            {
                Title = blogCreateDto.Title,
                Body = blogCreateDto.Body,
                PublishDate = DateTime.Now
            };

            await _context.Blogs.AddAsync(newBlog);
            await _context.SaveChangesAsync();

            return newBlog;
        }


        /// <summary>
        /// Retrieves a paginated list of blog posts.
        /// </summary>
        /// <param name="pageSize">The maximum number of blog posts per page.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="publishDateFilter">Optional filter by publish date.</param>
        /// <returns>
        /// A tuple containing the collection of blog posts and pagination metadata.
        /// </returns>
        public async Task<(IEnumerable<Blog>, PaginationMetadata)> GetBlogsAsync(int pageSize, int pageNumber, DateTime? publishDateFilter = null)
        {
            var collection = _context.Blogs as IQueryable<Blog>;

            if (publishDateFilter.HasValue)
            {
                collection = collection.Where(b => b.PublishDate.Date == publishDateFilter.Value.Date);
            }

            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);

            var collectionToReturn = await collection
                .Skip(pageSize * (pageNumber - 1))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        /// <summary>
        /// Retrieves a blog post by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the blog post.</param>
        /// <returns>The blog post DTO or null if not found.</returns>
        public async Task<BlogGetDto> GetBlogByIdAsync(int id)
        {
            var blog = await _context.Blogs
               .FirstOrDefaultAsync(blog => blog.Id == id);

            if (blog == null)
            {
                return null; 
            }

            var blogDto = new BlogGetDto
            {
                Id = blog.Id,
                Title = blog.Title,
                Body = blog.Body,
                PublishDate = blog.PublishDate
            };

            return blogDto;
        }


        /// <summary>
        /// Updates an existing blog post.
        /// </summary>
        /// <param name="id">The unique identifier of the blog post to update.</param>
        /// <param name="updateBlog">The DTO containing updated blog post information.</param>
        /// <returns>The updated blog post or null if the blog post was not found.</returns>
        public async Task<Blog> UpdateBlogAsync(int id, BlogUpdateDto updateBlog)
        {
            var existingBlog = await _context.Blogs.FindAsync(id);

            if (existingBlog == null)
            {
                return null; 
            }

            existingBlog.Title = updateBlog.Title;
            existingBlog.Body = updateBlog.Body;

            await _context.SaveChangesAsync();

            return existingBlog;
        }


        /// <summary>
        /// Deletes a blog post by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the blog post to delete.</param>
        /// <exception cref="NullReferenceException">Thrown when the specified blog post is not found.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during deletion.</exception>
        public async Task DeleteBlogAsync(int id)
        {
            try
            {
                var blogToDelete = await _context.Blogs.FindAsync(id) ?? throw new NullReferenceException("Blog Not Found");

                _context.Blogs.Remove(blogToDelete);

                await _context.SaveChangesAsync();
            }

            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while deleting the blog post. Please check related constraints.");
            }

            catch (NullReferenceException ex)
            {
                throw new Exception("Blog Not Found");
            }

            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the blog post.");
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
