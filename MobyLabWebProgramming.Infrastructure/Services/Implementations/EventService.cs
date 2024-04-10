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

public class EventService : IEventService
{
    private readonly IRepository<WebAppDatabaseContext> _repository;

    /// <summary>
    /// Inject the required services through the constructor.
    /// </summary>
    public EventService(IRepository<WebAppDatabaseContext> repository)
    {
        _repository = repository;
    }

    public async Task<ServiceResponse<EventDTO>> GetEvent(Guid id, CancellationToken cancellationToken = default)
    {
        var result = await _repository.GetAsync(new EventProjectionSpec(id), cancellationToken); // Get a Post using a specification on the repository.

        return result != null ? 
            ServiceResponse<EventDTO>.ForSuccess(result) : 
            ServiceResponse<EventDTO>.FromError(CommonErrors.EventNotFound); // Pack the result or error into a ServiceResponse.
    }

    public async Task<ServiceResponse<PagedResponse<EventDTO>>> GetEvents(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default)
    {
        var result = await _repository.PageAsync(pagination, new EventProjectionSpec(pagination.Search), cancellationToken); // Use the specification and pagination API to get only some entities from the database.
        return ServiceResponse<PagedResponse<EventDTO>>.ForSuccess(result);
    }
    public async Task<ServiceResponse<int>> GetEventCount(CancellationToken cancellationToken = default) => 
        ServiceResponse<int>.ForSuccess(await _repository.GetCountAsync<Event>(cancellationToken)); // Get the count of all Post entities in the database.

    public async Task<ServiceResponse> AddEvent(EventAddDTO eveniment, CancellationToken cancellationToken = default)
    {
        // we allow user to add a post in each and every case
        User? userCreator = await _repository.GetAsync<User>(eveniment.UserCreatorId!, cancellationToken); // Get the user that is creating the Post.

        if (userCreator == null)
        {
            return ServiceResponse.FromError(CommonErrors.UserNotFound); // Return an error if the User is not found.
        }
        await _repository.AddAsync(new Event
        {
            UserCreatorId = eveniment.UserCreatorId,
            Content = eveniment.Content,
            Title = eveniment.Title,
            Location = eveniment.Location,
            Date = eveniment.Date
        }, cancellationToken); // A new entity is created and persisted in the database.

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> UpdateEvent(EventUpdateDTO eveniment,  CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsync<Event>(eveniment.Id, cancellationToken); // Explicitly specify the type argument for GetAsync.

        if (entity != null) // Verify if the Post is not found, you cannot update an non-existing entity.
        {
            // check if the user is the creator of the post
            if (eveniment.UserCreatorId != entity.UserCreatorId)
            {
                return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the user that created the event can update it!", ErrorCodes.CannotUpdate));
            }

            entity.Content = eveniment.Content ?? entity.Content; // Update the entity properties.
            entity.Title = eveniment.Title ?? entity.Title;
            entity.Location = eveniment.Location ?? entity.Location;
            entity.Date = eveniment.Date ?? entity.Date;

            await _repository.UpdateAsync(entity, cancellationToken); // Update the entity and persist the changes.
        }

        return ServiceResponse.ForSuccess();
    }

    public async Task<ServiceResponse> DeleteEvent(Guid id, Guid idUserCreator, CancellationToken cancellationToken = default)
    {
        // only the user that created the post can delete it
        var eveniment = await _repository.GetAsync<Event>(id, cancellationToken); // Get the entity.

        if (eveniment == null)
        {
            return ServiceResponse.FromError(CommonErrors.EventNotFound); // Return an error if the Post is not found.
        }

        if (idUserCreator != eveniment.UserCreatorId)
        {
            return ServiceResponse.FromError(new(HttpStatusCode.Forbidden, "Only the user that created the event can delete it!", ErrorCodes.CannotDelete));
        }

        await _repository.DeleteAsync<Event>(id, cancellationToken); // Delete the entity.

        return ServiceResponse.ForSuccess();
    }
}
