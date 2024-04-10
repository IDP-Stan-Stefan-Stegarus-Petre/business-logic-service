using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Requests;
using MobyLabWebProgramming.Core.Responses;

namespace MobyLabWebProgramming.Infrastructure.Services.Interfaces;

/// <summary>
/// This service will be uses to mange Event information.
/// As most routes and business logic will need to know what Event is currently using the backend this service will be the most used.
/// </summary>
public interface IEventService
{
    /// <summary>
    /// GetEvent will provide the information about a Event given its Event Id.
    /// </summary>
    public Task<ServiceResponse<EventDTO>> GetEvent(Guid id, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetEvents returns page with Event information from the database.
    /// </summary>
    public Task<ServiceResponse<PagedResponse<EventDTO>>> GetEvents(PaginationSearchQueryParams pagination, CancellationToken cancellationToken = default);
    /// <summary>
    /// GetEventCount returns the number of Events in the database.
    /// </summary>
    public Task<ServiceResponse<int>> GetEventCount(CancellationToken cancellationToken = default);
    /// <summary>
    /// AddEvent adds an Event and verifies if requesting Event has permissions to add one.
    /// If the requesting Event is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> AddEvent(EventAddDTO Event, CancellationToken cancellationToken = default);
    /// <summary>
    /// UpdateEvent updates an Event and verifies if requesting Event has permissions to update it, if the Event is his own then that should be allowed.
    /// If the requesting Event is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> UpdateEvent(EventUpdateDTO Event, CancellationToken cancellationToken = default);
    /// <summary>
    /// DeleteEvent deletes an Event and verifies if requesting Event has permissions to delete it, if the Event is his own then that should be allowed.
    /// If the requesting Event is null then no verification is performed as it indicates that the application.
    /// </summary>
    public Task<ServiceResponse> DeleteEvent(Guid id, Guid idUserCreator, CancellationToken cancellationToken = default);
}
