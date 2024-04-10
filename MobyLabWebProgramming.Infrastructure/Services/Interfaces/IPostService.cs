using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

/// <summary>
/// This service will be uses to mange Post information.
/// As most routes and business logic will need to know what Post is currently using the backend this service will be the most used.
/// </summary>
public interface IPostService
{
    /// <summary>
    /// GetPost will provide the information about a Post given its Post Id.
    /// </summary>
    public Task<ServiceResponse<PostDTO>> GetPost(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetPosts returns page with Post information from the database.
    /// </summary>
    public Task<ServiceResponse<PagedResponse<PostDTO>>> GetPosts(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetPostCount returns the number of Posts in the database.
    /// </summary>
    public Task<ServiceResponse<int>> GetPostCount(CancellationToken cancellationToken = default);
    /// <summary>
    /// AddPost adds an Post and verifies if requesting Post has permissions to add one.
    /// If the requesting Post is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> AddPost(PostAddDTO Post, CancellationToken cancellationToken = default);
    /// <summary>
    /// UpdatePost updates an Post and verifies if requesting Post has permissions to update it, if the Post is his own then that should be allowed.
    /// If the requesting Post is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> UpdatePost(PostUpdateDTO Post, CancellationToken cancellationToken = default);
    /// <summary>
    /// DeletePost deletes an Post and verifies if requesting Post has permissions to delete it, if the Post is his own then that should be allowed.
    /// If the requesting Post is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> DeletePost(Guid id, Guid idUserCreator, CancellationToken cancellationToken = default);
}
