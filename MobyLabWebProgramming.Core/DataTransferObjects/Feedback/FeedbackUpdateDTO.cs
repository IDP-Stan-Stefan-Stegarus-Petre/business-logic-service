using MobyLabWebProgramming.Core.Entities;

namespace MobyLabWebProgramming.Core.DataTransferObjects;

/// <summary>
/// This DTO is used to update a post, the properties besides the id are nullable to indicate that they may not be updated if they are null.
/// </summary>
public record FeedbackUpdateDTO(Guid Id, Guid UserCreatorId = default!, string? Content = default, int Rating = default, string? Email = default, string? Name = default, string? PhoneNumber = default, string? TypeOfAppreciation = default, bool? IsUserExperienceEnjoyable = default);
