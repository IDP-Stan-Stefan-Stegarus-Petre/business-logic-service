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

public class FeedbackService : IFeedbackService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;

    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    public FeedbackService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<FeedbackDTO>> GetFeedback(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new FeedbackProjectionSpec(id), cancellationToken); // Get a Post using a specification on the repository.

        return result != null ? 
            ServiceResponse<FeedbackDTO>.ForSuccess(result) : 
            ServiceResponse<FeedbackDTO>.FromError(CommonErrors.FeedbackNotFound); // Pack the result or error into a ServiceResponse.
    }

    public async Task<ServiceResponse<PagedResponse<FeedbackDTO>>> GetFeedbacks(PaginationSearchQueryParams pagination,Guid idUserInitiator, CancellationToken cancellationToken = default)
    {
        // check if the user exists and has role of admin
        var user = await _repository.GetAsync<User>(idUserInitiator, cancellationToken); // Get the entity.
        if (user == null || user.Role != UserRoleEnum.Admin)
        {
            return ServiceResponse<PagedResponse<FeedbackDTO>>.FromError(CommonErrors.FeedBackOnlyForAdmin);
        }

        var result = await _repository.PageAsync(pagination, new FeedbackProjectionSpec(pagination.Search), cancellationToken); // Use the specification and pagination API to get only some entities from the database.
        return ServiceResponse<PagedResponse<FeedbackDTO>>.ForSuccess(result);
    }
    public async Task<ServiceResponse<int>> GetFeedbackCount(CancellationToken cancellationToken = default) => 
        ServiceResponse<int>.ForSuccess(await _repository.GetCountAsync<FeedBack>(cancellationToken)); // Get the count of all Post entities in the database.

    public async Task<ServiceResponse> AddFeedback(FeedbackAddDTO feedback, CancellationToken cancellationToken = default)
    {
        // we allow user to add a post in each and every case
        User? userCreator = await _repository.GetAsync<User>(feedback.UserCreatorId!, cancellationToken); // Get the user that is creating the Post.

        if (userCreator == null)
        {
            return ServiceResponse.FromError(CommonErrors.UserNotFound); // Return an error if the User is not found.
        }

        // extract all feedbacks from the user
        var feedbacks = await _repository.GetAllAsync<FeedBack>(cancellationToken); // Get all the entities from the database.
        feedbacks = feedbacks.Where(feedback => feedback.UserId == feedback.UserId).ToList();
        if (feedbacks.Count > 0)
        {
            return ServiceResponse.FromError(CommonErrors.FeedbackAlreadyExists);
        }

        await _repository.AddAsync(new FeedBack 
        {
            UserId = feedback.UserCreatorId,
            Content = feedback.Content,
            Rating = feedback.Rating,
            Email = feedback.Email,
            Name = feedback.Name,
            PhoneNumber = feedback.PhoneNumber,
            TypeOfAppreciation = feedback.TypeOfAppreciation,
            IsUserExperienceEnjoyable = feedback.IsUserExperienceEnjoyable
        }
        , cancellationToken); // A new entity is created and persisted in the database.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateFeedback(FeedbackUpdateDTO feedback,  CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsync<FeedBack>(feedback.Id, cancellationToken); // Explicitly specify the type argument for GetAsync.

        if (entity != null) // Verify if the Post is not found, you cannot update an non-existing entity.
        {
            // check if the user is the creator of the post
            if (feedback.UserCreatorId != entity.UserId)
            {
                return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the user that created the Feedback can update it!", ErrorCodes.CannotUpdate));
            }

            entity.Content = feedback.Content ?? entity.Content; // Update the entity properties.
            entity.Rating = feedback.Rating;
            entity.Email = feedback.Email ?? entity.Email;
            entity.Name = feedback.Name ?? entity.Name;
            entity.PhoneNumber = feedback.PhoneNumber ?? entity.PhoneNumber;
            entity.TypeOfAppreciation = feedback.TypeOfAppreciation ?? entity.TypeOfAppreciation;
            entity.IsUserExperienceEnjoyable = feedback.IsUserExperienceEnjoyable ?? entity.IsUserExperienceEnjoyable;  

            await _repository.UpdateAsync(entity, cancellationToken); // Update the entity and persist the changes.
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteFeedback(Guid id, Guid idUserCreator, CancellationToken cancellationToken = default)
    {
        // only the user that created the post can delete it
        var feedback = await _repository.GetAsync<FeedBack>(id, cancellationToken); // Get the entity.

        if (feedback == null)
        {
            return ServiceResponse.FromError(CommonErrors.FeedbackNotFound); // Return an error if the Post is not found.
        }

        if (idUserCreator != feedback.UserId)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the user that created the Feedback can delete it!", ErrorCodes.CannotDelete));
        }

        await _repository.DeleteAsync<FeedBack>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }
}
