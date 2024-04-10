using System.Linq.Expressions;
using Ardalis.Specification;
using Microsoft.EntityFrameworkCore;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.Specifications;

/// <summary>
/// This is a specification to filter the Like entities and map it to and LikeDTO object via the constructors.
/// Note how the constructors call the base class's constructors. Also, this is a sealed class, meaning it cannot be further derived.
/// </summary>
public sealed class CommentProjectionSpec : BaseSpec<CommentProjectionSpec, Comment, CommentDTO>
{
    /// <summary>
    /// This is the projection/mapping expression to be used by the base class to get LikeDTO object from the database.
    /// </summary>
    protected override Expression<Func<Comment, CommentDTO>> Spec => e => new()
    {
        Id = e.Id,
        UserId = e.UserId,
        PostId = e.PostId,
        Content = e.Content
    };

    public CommentProjectionSpec(bool orderByCreatedAt = true) : base(orderByCreatedAt)
    {
    }

    public CommentProjectionSpec(Guid postId): base(postId)
    {
    }
}
