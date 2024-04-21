namespace MobyLabWebProgramming.Core.DataTransferObjects;

/// <summary>
/// This DTO is used to update a post, the properties besides the id are nullable to indicate that they may not be updated if they are null.
/// </summary>
public record PostUpdateDTO(Guid Id, string? Content = default, Guid UserCreatorId = default!);
