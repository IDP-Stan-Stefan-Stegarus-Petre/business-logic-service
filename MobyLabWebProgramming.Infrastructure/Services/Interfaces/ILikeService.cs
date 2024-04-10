using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

/// <summary>
/// This service will be uses to mange Post information.
/// As most routes and business logic will need to know what Post is currently using the backend this service will be the most used.
/// </summary>
public interface ILikeService
{
    /// <summary>
    /// GetPost will provide the information about a Post given its Post Id.
    /// </summary>
    public Task<ServiceResponse<LikeDTO>> GetLike(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetPosts returns page with Post information from the database.
    /// </summary>
   public Task<ServiceResponse<int>> GetLikeCount(Guid postId, CancellationToken cancellationToken = default);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="userID"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
   public Task<ServiceResponse<List<LikeDTO>>> GetLikesForPost(Guid postId, Guid userID, CancellationToken cancellationToken = default);
    /// <summary>
    /// AddPost adds an Post and verifies if requesting Post has permissions to add one.
    /// If the requesting Post is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> AddLike(LikeAddDTO like,  CancellationToken cancellationToken = default);
    /// <summary>
    /// DeletePost deletes an Post and verifies if requesting Post has permissions to delete it, if the Post is his own then that should be allowed.
    /// If the requesting Post is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> DeleteLike(Guid id, Guid idUser, Guid idPost, CancellationToken cancellationToken = default);
}
