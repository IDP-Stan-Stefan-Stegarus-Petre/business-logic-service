using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.DataTransferObjects;

/// <summary>
/// This DTO is used to update a post, the properties besides the id are nullable to indicate that they may not be updated if they are null.
/// </summary>
public record CommentUpdateDTO(Guid Id, Guid UserId = default!, Guid PostId = default!, string? Content = default!);
