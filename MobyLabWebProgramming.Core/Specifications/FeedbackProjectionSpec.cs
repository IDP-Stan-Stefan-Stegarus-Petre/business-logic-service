using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

/// <summary>
/// This is a specification to filter the Feedback entities and map it to and FeedbackDTO object via the constructors.
/// Note how the constructors call the base class's constructors. Also, this is a sealed class, meaning it cannot be further derived.
/// </summary>
public sealed class FeedbackProjectionSpec : BaseSpec<FeedbackProjectionSpec, FeedBack, FeedbackDTO>
{
    /// <summary>
    /// This is the projection/mapping expression to be used by the base class to get FeedbackDTO object from the database.
    /// </summary>
    protected override Expression<Func<FeedBack, FeedbackDTO>> Spec => e => new()
    {
        Id = e.Id,
        UserCreatorId = e.UserId,
        Content = e.Content,
        Rating = e.Rating,
        Email = e.Email,
        Name = e.Name,
        PhoneNumber = e.PhoneNumber,
        TypeOfAppreciation = e.TypeOfAppreciation,
        IsUserExperienceEnjoyable = e.IsUserExperienceEnjoyable
    };

    public FeedbackProjectionSpec(bool orderByCreatedAt = true) : base(orderByCreatedAt)
    {
    }

    public FeedbackProjectionSpec(Guid id) : base(id)
    {
    }

    public FeedbackProjectionSpec(string? search)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";
        
        Query.Where(e => EF.Functions.ILike(e.Content, searchExpr));
    }
}
