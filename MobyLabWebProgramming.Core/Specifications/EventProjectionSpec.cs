using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

/// <summary>
/// This is a specification to filter the Event entities and map it to and EventDTO object via the constructors.
/// Note how the constructors call the base class's constructors. Also, this is a sealed class, meaning it cannot be further derived.
/// </summary>
public sealed class EventProjectionSpec : BaseSpec<EventProjectionSpec, Event, EventDTO>
{
    /// <summary>
    /// This is the projection/mapping expression to be used by the base class to get EventDTO object from the database.
    /// </summary>
    protected override Expression<Func<Event, EventDTO>> Spec => e => new()
    {
        Id = e.Id,
        UserCreatorId = e.UserCreatorId,
        Content = e.Content,
        Title = e.Title,
        Location = e.Location,
        Date = e.Date
    };

    public EventProjectionSpec(bool orderByCreatedAt = true) : base(orderByCreatedAt)
    {
    }

    public EventProjectionSpec(Guid id) : base(id)
    {
    }

    public EventProjectionSpec(string? search)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";
        
        Query.Where(e => EF.Functions.ILike(e.Content, searchExpr) || EF.Functions.ILike(e.Title, searchExpr));
    }
}
