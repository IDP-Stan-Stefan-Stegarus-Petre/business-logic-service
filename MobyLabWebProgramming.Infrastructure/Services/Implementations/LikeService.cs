using System.Net;
using MobyLabWebProgramming.Core.Constants;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;
using MobyLabWebProgramming.Core.Enums;
using MobyLabWebProgramming.Core.Errors;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Core.Specifications;
using MobyLabWebProgramming.Infrastructure.Database;
using MobyLabWebProgramming.Infrastructure.Repositories.Interfaces;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;

namespace MobyLabWebProgramming.Infrastructure.Services.Implementations;

public class LikeService : ILikeService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly IUserService _userService;
    private readonly IPostService _postService;

    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    public LikeService(IRepository<WebAppDatabaseContext> repository, IUserService userService, IPostService postService)
    {
        _repository = repository;
        _userService = userService;
        _postService = postService;
    }

    public async Task<ServiceResponse<LikeDTO>> GetLike(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new LikeProjectionSpec(id), cancellationToken); // Get a Post using a specification on the repository.

        return result != null ?
            ServiceResponse<LikeDTO>.ForSuccess(result) :
            ServiceResponse<LikeDTO>.FromError(CommonErrors.LikeNotFound); // Pack the result or error into a ServiceResponse.
    }

    // get like count for a specific Like
    public async Task<ServiceResponse<int>> GetLikeCount(Guid postId, CancellationToken cancellationToken = default)
    {

        // check if the post exists
        var post = await _repository.GetAsync<Post>(postId, cancellationToken); // Get the entity.

        if (post == null)
        {
            return ServiceResponse<int>.FromError(CommonErrors.PostNotFound); // Return an error if the Post is not found.
        }

        // get all likes, no condition
        List<Like> result = await _repository.GetAllAsync<Like>(cancellationToken); // Get the count of all Like entities in the database.
        result = result.Where(like => like.PostId == postId).ToList();

        return ServiceResponse<int>.ForSuccess(result.Count);
    }

    public async Task<ServiceResponse<List<LikeDTO>>> GetLikesForPost(Guid postId, Guid userID, CancellationToken cancellationToken = default)
    {
       var post = await _repository.GetAsync<Post>(postId, cancellationToken); // Get the entity.

        if (post == null)
        {
            return ServiceResponse<List<LikeDTO>>.FromError(CommonErrors.PostNotFound); // Return an error if the Post is not found.
        }

        if (post.UserCreatorId != userID)
        {
           return ServiceResponse<List<LikeDTO>>.FromError(CommonErrors.UserNotAuthorizedPost); // Return an error if the User is not found.
        } 

        List<Like> result = await _repository.GetAllAsync<Like>(cancellationToken); // Get the count of all Like entities in the database.
        result = result.Where(like => like.PostId == postId).ToList();

        return ServiceResponse<List<LikeDTO>>.ForSuccess(result.Select(like => new LikeDTO
        {
            Id = like.Id,
            UserId = like.UserId,
            PostId = like.PostId
        }).ToList());

    }

    public async Task<ServiceResponse> AddLike(LikeAddDTO like, CancellationToken cancellationToken = default)
    {
        // we allow user to add a post in each and every case
        User? userCreator = await _repository.GetAsync<User>(like.UserId, cancellationToken); // Get the user that is creating the Post.

        if (userCreator == null)
        {
            return ServiceResponse.FromError(CommonErrors.UserNotFound); // Return an error if the User is not found.
        }

        // check if the post exists
        var post = await _repository.GetAsync<Post>(like.PostId, cancellationToken); // Get the entity.

        if (post == null)
        {
            return ServiceResponse.FromError(CommonErrors.PostNotFound); // Return an error if the Post is not found.
        }

        List<Like> result = await _repository.GetAllAsync<Like>(cancellationToken);
        result = result.Where(like => like.PostId == like.PostId && like.UserId == like.UserId).ToList();

        if (result.Count > 0)
        {
            return ServiceResponse.FromError(CommonErrors.LikeAlreadyExists); // Return an error if the Like is not found.
        }

        try
        {
            await _repository.AddAsync(new Like
            {
                UserId = like.UserId,
                PostId = like.PostId
            }, cancellationToken); // A new entity is created and persisted in the database.
        } catch (Exception e)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, e.Message, ErrorCodes.CannotAdd));
        }


        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteLike(Guid id, Guid idUser, Guid idPost, CancellationToken cancellationToken = default)
    {
        // only the user that created the post can delete it
        var post = await _repository.GetAsync<Post>(idPost, cancellationToken); // Get the entity.

        if (post == null)
        {
            return ServiceResponse.FromError(CommonErrors.PostNotFound); // Return an error if the Post is not found.
        }

        var like = await _repository.GetAsync<Like>(id, cancellationToken); // Get the entity.

        if (like == null)
        {
            return ServiceResponse.FromError(CommonErrors.LikeNotFound); // Return an error if the Like is not found.
        }

        if (like.UserId != idUser)
        {
            return ServiceResponse.FromError(CommonErrors.LikeNotAdded); // Return an error if the User is not found.
        }

        await _repository.DeleteAsync<Like>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }
}
