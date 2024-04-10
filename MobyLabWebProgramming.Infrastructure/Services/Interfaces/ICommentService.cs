using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

/// <summary>
/// This service will be uses to mange Post information.
/// As most routes and business logic will need to know what Post is currently using the backend this service will be the most used.
/// </summary>
public interface ICommentService
{
    /// <summary>
    /// GetPost will provide the information about a Post given its Post Id.
    /// </summary>
    public Task<ServiceResponse<CommentDTO>> GetComment(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetPosts returns page with Post information from the database.
    /// </summary>
   public Task<ServiceResponse<int>> GetCommentCount(Guid postId, CancellationToken cancellationToken = default);
    /// <summary>
    /// 
    /// </summary>
    /// <param name="postId"></param>
    /// <param name="userID"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
   public Task<ServiceResponse<List<CommentDTO>>> GetCommentsForPost(Guid postId, CancellationToken cancellationToken = default);
    /// <summary>
    /// AddPost adds an Post and verifies if requesting Post has permissions to add one.
    /// If the requesting Post is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> AddComment(CommentAddDTO Comment,  CancellationToken cancellationToken = default);
    
    public Task<ServiceResponse> UpdateComment(CommentUpdateDTO comment, CancellationToken cancellationToken = default);
    /// <summary>
    /// DeletePost deletes an Post and verifies if requesting Post has permissions to delete it, if the Post is his own then that should be allowed.
    /// If the requesting Post is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> DeleteComment(Guid id, Guid idUser, Guid idPost, CancellationToken cancellationToken = default);
}
