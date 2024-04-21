namespace MobyLabWebProgramming.Core.DataTransferObjects;

/// <summary>
/// This DTO is used to update a post, the properties besides the id are nullable to indicate that they may not be updated if they are null.
/// </summary>
public record EventUpdateDTO(Guid Id, Guid UserCreatorId = default!, string? Content = default, string? Title = default, string? Location = default, string? Date = default);
