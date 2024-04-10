using MobyLabWebProgramming.Core.Enums;

namespace MobyLabWebProgramming.Core.Entities;

/// <summary>
/// This is an example for a comment entity, it will be mapped to a single table and each property will have it's own column except for entity object references also known as navigation properties.
/// </summary>
public class Event : BaseEntity
{
    public Guid UserCreatorId { get; set; }
    public User? User { get; set; }

    public string Content { get; set; } = default!;

    public string Title { get; set; } = default!;

    public string Location { get; set; } = default!;

    public string Date { get; set; } = default!;
}
