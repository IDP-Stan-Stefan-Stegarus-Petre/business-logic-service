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

public class PostService : IPostService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;
    private readonly IUserService _userService;

    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    public PostService(IRepository<WebAppDatabaseContext> repository, IUserService userService)
    {
        _repository = repository;
        _userService = userService;
    }

    public async Task<ServiceResponse<PostDTO>> GetPost(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new PostProjectionSpec(id), cancellationToken); // Get a Post using a specification on the repository.

        return result != null ? 
            ServiceResponse<PostDTO>.ForSuccess(result) : 
            ServiceResponse<PostDTO>.FromError(CommonErrors.PostNotFound); // Pack the result or error into a ServiceResponse.
    }

    public async Task<ServiceResponse<PagedResponse<PostDTO>>> GetPosts(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await _repository.PageAsync(pagination, new PostProjectionSpec(pagination.Search), cancellationToken); // Use the specification and pagination API to get only some entities from the database.

        return ServiceResponse<PagedResponse<PostDTO>>.ForSuccess(result);
    }
    public async Task<ServiceResponse<int>> GetPostCount(CancellationToken cancellationToken = default) => 
        ServiceResponse<int>.ForSuccess(await _repository.GetCountAsync<Post>(cancellationToken)); // Get the count of all Post entities in the database.

    public async Task<ServiceResponse> AddPost(PostAddDTO Post,  CancellationToken cancellationToken = default)
    {
        // we allow user to add a post in each and every case
        User? userCreator = await _repository.GetAsync<User>(Post.UserCreatorId!, cancellationToken); // Get the user that is creating the Post.

        if (userCreator == null)
        {
            return ServiceResponse.FromError(CommonErrors.UserNotFound); // Return an error if the User is not found.
        }
        await _repository.AddAsync(new Post
        {
            UserCreator = userCreator,
            Content = Post?.Content!,
        }, cancellationToken); // A new entity is created and persisted in the database.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdatePost(PostUpdateDTO Post,  CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsync<Post>(Post.Id, cancellationToken); // Explicitly specify the type argument for GetAsync.


        if (entity != null) // Verify if the Post is not found, you cannot update an non-existing entity.
        {

            // check if the user is the creator of the post
            if (Post.UserCreatorId != entity.UserCreatorId)
            {
                return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the user that created the post can update it!", ErrorCodes.CannotUpdate));
            }

            entity.Content = Post.Content!; // Update the entity with the new data.

            await _repository.UpdateAsync(entity, cancellationToken); // Update the entity and persist the changes.
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeletePost(Guid id, Guid idUserCreator, CancellationToken cancellationToken = default)
    {
        // only the user that created the post can delete it
        var post = await _repository.GetAsync<Post>(id, cancellationToken); // Get the entity.

        if (post == null)
        {
            return ServiceResponse.FromError(CommonErrors.PostNotFound); // Return an error if the Post is not found.
        }

        if (idUserCreator != post.UserCreatorId)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the user that created the post can delete it!", ErrorCodes.CannotDelete));
        }

        await _repository.DeleteAsync<Post>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }
}
