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

public class CommentService : ICommentService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;

    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    public CommentService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<CommentDTO>> GetComment(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new CommentProjectionSpec(id), cancellationToken); // Get a Post using a specification on the repository.

        return result != null ?
            ServiceResponse<CommentDTO>.ForSuccess(result) :
            ServiceResponse<CommentDTO>.FromError(CommonErrors.CommentNotFound); // Pack the result or error into a ServiceResponse.
    }

    // get like count for a specific Like
    public async Task<ServiceResponse<int>> GetCommentCount(Guid postId, CancellationToken cancellationToken = default)
    {
        // check if the post exists
        var post = await _repository.GetAsync<Post>(postId, cancellationToken); // Get the entity.

        if (post == null)
        {
            return ServiceResponse<int>.FromError(CommonErrors.PostNotFound); // Return an error if the Post is not found.
        }

        // get all likes, no condition
        List<Comment> result = await _repository.GetAllAsync<Comment>(cancellationToken); // Get the count of all Like entities in the database.
        result = result.Where(comment => comment.PostId == postId).ToList();

        return ServiceResponse<int>.ForSuccess(result.Count);
    }

    public async Task<ServiceResponse<List<CommentDTO>>> GetCommentsForPost(Guid postId, CancellationToken cancellationToken = default)
    {
       var post = await _repository.GetAsync<Post>(postId, cancellationToken); // Get the entity.

        if (post == null)
        {
            return ServiceResponse<List<CommentDTO>>.FromError(CommonErrors.PostNotFound); // Return an error if the Post is not found.
        }


        List<Comment> result = await _repository.GetAllAsync<Comment>(cancellationToken); // Get the count of all Like entities in the database.
        result = result.Where(comment => comment.PostId == postId).ToList();

        return ServiceResponse<List<CommentDTO>>.ForSuccess(result.Select(comment => new CommentDTO
        {
            Id = comment.Id,
            UserId = comment.UserId,
            PostId = comment.PostId,
            Content = comment.Content
        }).ToList());

    }

    public async Task<ServiceResponse> AddComment(CommentAddDTO comment, CancellationToken cancellationToken = default)
    {
        // we allow user to add a post in each and every case
        User? userCreator = await _repository.GetAsync<User>(comment.UserId, cancellationToken); // Get the user that is creating the Post.

        if (userCreator == null)
        {
            return ServiceResponse.FromError(CommonErrors.UserNotFound); // Return an error if the User is not found.
        }

        // check if the post exists
        var post = await _repository.GetAsync<Post>(comment.PostId, cancellationToken); // Get the entity.

        if (post == null)
        {
            return ServiceResponse.FromError(CommonErrors.PostNotFound); // Return an error if the Post is not found.
        }

        try
        {
            await _repository.AddAsync(new Comment
            {
                UserId = comment.UserId,
                PostId = comment.PostId,
                Content = comment.Content
            }, cancellationToken); // A new entity is created and persisted in the database.
        } catch (Exception e)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, e.Message, ErrorCodes.CannotAdd));
        }


        return ServiceResponse.ForSuccess();
    }

    // update comment, make sure that only the user that created the comment can update it
    public async Task<ServiceResponse> UpdateComment(CommentUpdateDTO comment, CancellationToken cancellationToken = default)
    {
        // only the user that created the post can update it
        var post = await _repository.GetAsync<Post>(comment.PostId, cancellationToken); // Get the entity.

        if (post == null)
        {
            return ServiceResponse.FromError(CommonErrors.PostNotFound); // Return an error if the Post is not found.
        }

        var comentariu = await _repository.GetAsync<Comment>(comment.Id, cancellationToken); // Get the entity.

        if (comentariu == null)
        {
            return ServiceResponse.FromError(CommonErrors.CommentNotAdded); // Return an error if the Like is not found.
        }

        if (comment.UserId != comentariu.UserId)
        {
            return ServiceResponse.FromError(CommonErrors.CommentNotModified); // Return an error if the User is not found.
        }

        comentariu.Content = comment.Content!;

        try
        {
            await _repository.UpdateAsync(comentariu, cancellationToken); // Update the entity.
        } catch (Exception e)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.BadRequest, e.Message, ErrorCodes.CannotUpdate));
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteComment(Guid id, Guid idUser, Guid idPost, CancellationToken cancellationToken = default)
    {
        // only the user that created the post can delete it
        var post = await _repository.GetAsync<Post>(idPost, cancellationToken); // Get the entity.

        if (post == null)
        {
            return ServiceResponse.FromError(CommonErrors.PostNotFound); // Return an error if the Post is not found.
        }

        var comment = await _repository.GetAsync<Comment>(id, cancellationToken); // Get the entity.

        if (comment == null)
        {
            return ServiceResponse.FromError(CommonErrors.CommentNotFound); // Return an error if the Like is not found.
        }

        if (comment.UserId != idUser)
        {
            return ServiceResponse.FromError(CommonErrors.CommentNotModified); // Return an error if the User is not found.
        }

        await _repository.DeleteAsync<Comment>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }
}
