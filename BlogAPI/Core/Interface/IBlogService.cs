using BlogAPI.Core.Dtos;
using BlogAPI.Core.Entities;

namespace BlogAPI.Core.Interface
{
    public interface IBlogService
    {
        Task<Blog> CreateBlogAsync(BlogCreateDto blogCreateDto);
        Task<BlogGetDto> GetBlogByIdAsync(int id);
        Task<(IEnumerable<Blog>, PaginationMetadata)> GetBlogsAsync(int pageSize, int pageNumber, DateTime? publishDateFilter = null);
        Task<Blog> UpdateBlogAsync(int id, BlogUpdateDto updateBlog);
        Task DeleteBlogAsync(int id);
        Task<bool> SaveChangesAsync();
    }
}
