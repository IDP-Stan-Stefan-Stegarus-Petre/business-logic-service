using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

/// <summary>
/// This is a specification to filter the Post entities and map it to and PostDTO object via the constructors.
/// Note how the constructors call the base class's constructors. Also, this is a sealed class, meaning it cannot be further derived.
/// </summary>
public sealed class PostProjectionSpec : BaseSpec<PostProjectionSpec, Post, PostDTO>
{
    /// <summary>
    /// This is the projection/mapping expression to be used by the base class to get PostDTO object from the database.
    /// </summary>
    protected override Expression<Func<Post, PostDTO>> Spec => e => new()
    {
        Id = e.Id,
        UserCreatorId = e.UserCreatorId,
        Content = e.Content
    };

    public PostProjectionSpec(bool orderByCreatedAt = true) : base(orderByCreatedAt)
    {
    }

    public PostProjectionSpec(Guid id) : base(id)
    {
    }

    public PostProjectionSpec(string? search)
    {
        search = !string.IsNullOrWhiteSpace(search) ? search.Trim() : null;

        if (search == null)
        {
            return;
        }

        var searchExpr = $"%{search.Replace(" ", "%")}%";

        Query.Where(e => EF.Functions.ILike(e.Content, searchExpr)); // This is an example on who database specific expressions can be used via C# expressions.
                                                                                           // Note that this will be translated to the database something like "where Post.Name ilike '%str%'".
    }
}
